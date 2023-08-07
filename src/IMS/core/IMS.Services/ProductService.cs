using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IProductService
    {
        Task AddAsync(ProductAddModel model, long userId);
        Task UpdateAsync(ProductEditModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<RankUpdateDto> GetProductRankAsync(long productId);
        Task UpdateRankAsync(long productId, long targetRank);
        Task<ProductEditModel> GetByIdAsync(long id);
        Task<ProductDto> GetPriceAndQuantityByIdAsync(long id);
        IList<ProductDto> LoadActiveProducts(long categoryId, long brandId);
        IList<ProductDto> LoadAvailableProducts(long categoryId, long brandId);
        IList<(long, string)> LoadAllActiveProducts(long categoryId, long productId);
        (int total, int totalDisplay, IList<ProductDto> records) LoadAllProducts(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class ProductService : BaseService, IProductService
    {
        #region Initializtion
        private readonly IProductDao _productDao;
        private readonly ICategoryDao _categoryDao;
        private readonly IBrandDao _brandDao;
        private readonly IImageService _imageService;

        public ProductService(ISession session) : base(session)
        {
            _productDao = new ProductDao(session);
            _categoryDao = new CategoryDao(session);
            _brandDao = new BrandDao(session);
            _imageService = new ImageService();
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(ProductAddModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var count = _productDao.GetCount(x => x.Name == model.Name);
                    if(count > 0)
                    {
                        throw new CustomException("Found another Product with this name");
                    }                    

                    var product = new Product()
                    {
                        Name = model.Name,
                        CategoryId = model.CategoryId,
                        Category = await _categoryDao.GetByIdAsync(model.CategoryId),
                        BrandId = model.BrandId,
                        Brand = await _brandDao.GetByIdAsync(model.BrandId),
                        Description = model.Description,
                        Status = (int)model.Status,
                        ProfitMargin = model.ProfitMargin,
                        DiscountPrice = model.DiscountPrice,
                        Rank = await _productDao.GetMaxRank("Product") + 1,
                        CreateBy = userId,
                        CreationDate = _timeService.Now,
                        Image = model.Image,
                    };

                    await _productDao.AddAsync(product);
                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);

                    throw;
                }
            }
        }

        public async Task UpdateAsync(ProductEditModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var product = await _productDao.GetByIdAsync(model.Id);
                    var namecount = _productDao.GetCount(x => x.Name == model.Name);

                    if (product == null)
                    {
                        throw new CustomException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new CustomException("Already exist Product with this name");
                    }                    
                    
                    product.Name = model.Name;
                    product.CategoryId = model.CategoryId;
                    product.Category = await _categoryDao.GetByIdAsync(model.CategoryId);
                    product.BrandId = model.BrandId;
                    product.Brand = await _brandDao.GetByIdAsync(model.BrandId);
                    product.Description = model.Description;
                    product.Status = (int)model.Status;
                    product.ProfitMargin = model.ProfitMargin;
                    product.DiscountPrice = model.DiscountPrice;
                    product.SellingPrice = product.BuyingPrice + (product.BuyingPrice * model.ProfitMargin / 100) - model.DiscountPrice;

                    if (!string.IsNullOrWhiteSpace(model.Image))
                        product.Image = model.Image;

                    await _productDao.EditAsync(product);
                    transaction.Commit();

                    _serviceLogger.Info("Data Saved!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);

                    throw;
                }
            }
        }
        
        public async Task RemoveByIdAsync(long id, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    //await _productDao.RemoveByIdAsync(id);
                    var product = await _productDao.GetByIdAsync(id);
                    if (product != null)
                    {
                        throw new CustomException("No object found with this id");
                    }
                    if(product.InStockQuantity > 0)
                    {
                        throw new CustomException("Product can't be delete due to quantity level");
                    }
                    product.Status = (int)Status.Delete;
                    product.ModifyBy = userId;
                    product.ModificationDate = _timeService.Now;

                    await _productDao.EditAsync(product);
                    transaction.Commit();

                }
                catch(CustomException ex)
                {
                    throw ex;
                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task<ProductEditModel> GetByIdAsync(long id)
        {
            try
            {
                var product = await _productDao.GetByIdAsync(id);
                if (product == null)
                {
                    throw new CustomException("No object found with this id");
                }
                return new ProductEditModel
                {
                    Id = product.Id,
                    Name = product.Name,
                    CategoryId = product.Category.Id, 
                    BrandId = product.Brand.Id,
                    Description = product.Description,
                    BuyingPrice = product.BuyingPrice,
                    SellingPrice = product.SellingPrice,
                    DiscountPrice = product.DiscountPrice,
                    ProfitMargin = product.ProfitMargin,
                    Image = product.Image,
                    CreateBy = product.CreateBy,
                    CreationDate = product.CreationDate,
                    ModifyBy = product.ModifyBy,
                    ModificationDate = product.ModificationDate,
                    Status = (Status)product.Status,
                    Rank = product.Rank,
                    VersionNumber = product.VersionNumber,
                    BusinessId = product.BusinessId,
                };
            }
            catch(Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task<ProductDto> GetPriceAndQuantityByIdAsync(long id)
        {
            try
            {
                var product = await _productDao.GetByIdAsync(id);
                if (product == null)
                {
                    throw new CustomException("No object found with this id");
                }
                return new ProductDto
                {
                    SellingPrice = product.SellingPrice,
                    InStockQuantity = product.InStockQuantity
                };
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task<RankUpdateDto> GetProductRankAsync(long productId)
        {
            try
            {
                var result = await _productDao.GetRankByIdAsync<RankUpdateDto>(productId, "Product");
                return result;
            }
            catch(Exception ex)
            { 
                _serviceLogger.Error($"{ex.Message}", ex);
                throw new CustomException("Error on getting product rank");
            }
           
        }

        public async Task UpdateRankAsync(long productId, long targetRank)
        {
            try
            {
                using(var transaction = _session.BeginTransaction())
                {
                    var result = await _productDao.GetRankByIdAsync<RankUpdateDto>(productId, "Product");
                    if(result == null)
                    {
                        throw new CustomException("Invalid product id");
                    }
                    else if(targetRank < 1 || targetRank > result.Count)
                    {
                        throw new CustomException("Invalid Rank count");
                    }
                    else if( result.Rank == targetRank)
                    {
                        throw new CustomException("Present rank and target rank are same");
                    }
                    else if(targetRank > result.Rank)
                    {
                        await _productDao.RankUpAsync(result.Rank, targetRank, "Product");
                    }
                    else if(targetRank < result.Rank)
                    {
                        await _productDao.RankDownAsync(result.Rank, targetRank, "Product");
                    }
                    
                    transaction.Commit();
                }                
            }
            catch(Exception ex)
            {
                _serviceLogger.Error($"{ex.Message}", ex);
                throw;
            }
        }
        #endregion

        #region Single Instance Loading
        #endregion

        #region List Loading Function
        public (int total, int totalDisplay, IList<ProductDto> records) LoadAllProducts(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Product, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) && x.Status != (int)Status.Delete;
                }

                var result = _productDao.LoadAllProducts(filter, null, start, length, sortBy, sortDir);

                List<ProductDto> products = new List<ProductDto>();
                foreach (Product product in result.data)
                {
                    products.Add(
                        new ProductDto
                        {
                            Id = product.Id,
                            Name = product.Name,
                            Category = product.Category.Name,
                            Brand = product.Brand.Name,
                            Description = product.Description,                                                       
                            Status = (Status)product.Status,                            
                            DiscountPrice = product.DiscountPrice,
                            SellingPrice = product.SellingPrice,
                            Image = product.Image, 
                            InStockQuantity = product.InStockQuantity,
                            Rank = product.Rank,
                        });
                }

                return (result.total, result.totalDisplay, products);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        
        public IList<ProductDto> LoadActiveProducts(long categoryId, long brandId)
        {            
            var data = _productDao.GetProducts(x => x.Category.Id == categoryId && x.Brand.Id == brandId && x.Status == (int)Status.Active);
            List<ProductDto> products = new List<ProductDto>();
            foreach (Product product in data)
            {
                products.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        InStockQuantity = product.InStockQuantity,
                        SellingPrice = product.SellingPrice,
                    });
            }
            return products;
        }

        public IList<ProductDto> LoadAvailableProducts(long categoryId, long brandId)
        {
            var data = _productDao.GetProducts(x => x.Category.Id == categoryId && x.Brand.Id == brandId && x.Status == (int)Status.Active && x.InStockQuantity > 0);
            List<ProductDto> products = new List<ProductDto>();
            foreach (Product product in data)
            {
                products.Add(
                    new ProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        InStockQuantity = product.InStockQuantity,
                        SellingPrice = product.SellingPrice,
                    });
            }
            return products;
        }

        public IList<(long, string)> LoadAllActiveProducts(long categoyId, long brandId)
        { 
            
            List<(long, string)> products = new List<(long, string)>();
            var allProducts = _productDao.GetProducts(x => x.CategoryId == categoyId && x.Status == (int)Status.Active);
            foreach (var product in allProducts)
            {
                products.Add((product.Id, product.Name));
            }
            return products;
        }
        #endregion

    }
}
