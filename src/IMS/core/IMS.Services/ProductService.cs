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
        Task<ProductEditModel> GetByIdAsync(long id);
        IList<ProductDto> LoadAllProducts();
        (int total, int totalDisplay, IList<ProductDto> records) LoadAllProducts(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class ProductService : BaseService, IProductService
    {
        #region Initializtion
        private readonly IProductDao _productDao;
        private readonly ICategoryDao _categoryDao;

        public ProductService(ISession session) : base(session)
        {
            _productDao = new ProductDao(session);
            _categoryDao = new CategoryDao(session);
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
                        throw new DuplicateException("Found another Product with this name");
                    }

                    var product = new Product()
                    {
                        Name = model.Name,
                        CategoryId = model.CategoryId,
                        Category = await _categoryDao.GetByIdAsync(model.CategoryId),
                        Description = model.Description,
                        Status = (int)model.Status,
                        ProfitMargin = model.ProfitMargin,
                        DiscountPrice = model.DiscountPrice,
                        Rank = model.Rank,
                        CreateBy = userId,
                        CreationDate = _timeService.Now,
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
                    var objectcount = _productDao.GetCount(x => x.Id == model.Id);
                    var namecount = _productDao.GetCount(x => x.Name == model.Name);

                    if (objectcount < 1)
                    {
                        throw new InvalidOperationException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new DuplicateException("Already exist Product with this name");
                    }

                    var Product = new Product()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status,
                        CreateBy = model.CreateBy,
                        CreationDate = DateTime.Parse(model.CreationDate),
                        ModifyBy = userId,
                        ModificationDate = _timeService.Now
                    };

                    await _productDao.EditAsync(Product);
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
                    var Product = await _productDao.GetByIdAsync(id);
                    Product.Status = (int)Status.Delete;
                    Product.ModifyBy = userId;
                    Product.ModificationDate = _timeService.Now;
                    await _productDao.EditAsync(Product);
                    transaction.Commit();

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
                var Product = await _productDao.GetByIdAsync(id);
                if (Product == null)
                {
                    throw new ArgumentNullException(nameof(Product));
                }
                return new ProductEditModel
                {
                    Id = Product.Id,
                    Name = Product.Name,
                    Description = Product.Description,
                    CreateBy = Product.CreateBy,
                    CreationDate = (Product.CreationDate).ToString(),
                    ModifyBy = Product.ModifyBy,
                    ModificationDate = Product.ModificationDate,
                    Status = (Status)Product.Status,
                    Rank = Product.Rank,
                    VersionNumber = Product.VersionNumber,
                    BusinessId = Product.BusinessId,
                };
            }
            catch(Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
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
                            Description = product.Description,                                                       
                            Status = (Status)product.Status,
                            ProfitMargin = product.ProfitMargin,
                            DiscountPrice = product.DiscountPrice,
                            SellingPrice = product.SellingPrice,
                            Image = product.Image == null ? "ImageNotFound":  "Found"                          
                        });;
                }

                return (result.total, result.totalDisplay, products);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        
        public IList<ProductDto> LoadAllProducts()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
