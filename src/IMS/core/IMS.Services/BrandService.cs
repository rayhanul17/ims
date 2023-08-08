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
    public interface IBrandService
    {
        Task AddAsync(BrandAddModel model, long userId);
        Task UpdateAsync(BrandEditModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<BrandEditModel> GetByIdAsync(long id);
        IList<(long, string)> LoadAllBrands();
        IList<(long, string)> LoadAllActiveBrands();
        (int total, int totalDisplay, IList<BrandDto> records) LoadAllBrands(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class BrandService : BaseService, IBrandService
    {
        #region Initializtion
        private readonly IBrandDao _brandDao;

        public BrandService(ISession session) : base(session)
        {
            _brandDao = new BrandDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(BrandAddModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var count = _brandDao.GetCount(x => x.Name == model.Name);
                    if(count > 0)
                    {
                        throw new CustomException("Found another Brand with this name");
                    }

                    var brand = new Brand()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status,
                        Rank = await _brandDao.GetMaxRank("Brand") + 1,
                        CreateBy = userId,
                        CreationDate = _timeService.Now,
                    };

                    await _brandDao.AddAsync(brand);
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

        public async Task UpdateAsync(BrandEditModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var brand = await _brandDao.GetByIdAsync(model.Id);
                    var namecount = _brandDao.GetCount(x => x.Name == model.Name);

                    if (brand == null)
                    {
                        throw new CustomException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new CustomException("Already exist brand with this name");
                    }

                    brand.Name = model.Name;
                    brand.Description = model.Description;
                    brand.Status = (int)model.Status;
                    brand.ModifyBy = userId;
                    brand.ModificationDate = _timeService.Now;

                    await _brandDao.EditAsync(brand);
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
                    //await _brandDao.RemoveByIdAsync(id);
                    var brand = await _brandDao.GetByIdAsync(id);
                    if (brand == null)
                    {
                        throw new CustomException("No object found with this id");
                    }
                    brand.Status = (int)Status.Delete;
                    brand.ModifyBy = userId;
                    brand.ModificationDate = _timeService.Now;
                    await _brandDao.EditAsync(brand);
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex);
                    transaction.Rollback();
                    throw;
                }
            }
        }

        public async Task<BrandEditModel> GetByIdAsync(long id)
        {
            try
            {
                var brand = await _brandDao.GetByIdAsync(id);
                if (brand == null)
                {
                    throw new CustomException("No object found with this id");
                }
                return new BrandEditModel
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description,
                    CreateBy = brand.CreateBy,
                    CreationDate = brand.CreationDate,
                    ModifyBy = brand.ModifyBy,
                    ModificationDate = brand.ModificationDate,
                    Status = (Status)brand.Status,
                    Rank = brand.Rank,
                    VersionNumber = brand.VersionNumber,
                    BusinessId = brand.BusinessId,
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
        public (int total, int totalDisplay, IList<BrandDto> records) LoadAllBrands(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Brand, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) || x.Description.Contains(searchBy);
                }

                var result = _brandDao.LoadAllBrands(filter, null, start, length, sortBy, sortDir);

                List<BrandDto> categories = new List<BrandDto>();
                foreach (Brand brand in result.data)
                {
                    categories.Add(
                        new BrandDto
                        {
                            Id = brand.Id,
                            Name = brand.Name,
                            Description = brand.Description,
                            CreateBy = brand.CreateBy,
                            CreationDate = brand.CreationDate,                            
                            Status = (Status)brand.Status,
                            Rank = brand.Rank,
                            VersionNumber = brand.VersionNumber,
                            BusinessId = brand.BusinessId,
                        });
                }

                return (result.total, result.totalDisplay, categories);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }

        public IList<(long, string)> LoadAllBrands()
        {
            List<(long, string)> brands = new List<(long, string)>();
            var allBrands = _brandDao.GetBrand(x => x.Status != (int)Status.Delete);
            foreach (var brand in allBrands)
            {
                brands.Add((brand.Id, brand.Name));
            }
            return brands;
        }

        public IList<(long, string)> LoadAllActiveBrands()
        {
            List<(long, string)> brands = new List<(long, string)>();
            var allBrands = _brandDao.GetBrand(x => x.Status == (int)Status.Active);
            foreach (var brand in allBrands)
            {
                brands.Add((brand.Id, brand.Name));
            }
            return brands;
        }
        #endregion

    }
}
