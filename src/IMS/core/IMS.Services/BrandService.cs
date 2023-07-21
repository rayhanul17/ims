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
        IList<BrandDto> LoadAllBrands();
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
                        throw new DuplicateException("Found another Brand with this name");
                    }

                    var brand = new Brand()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status,
                        Rank = model.Rank,
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
                    var objectcount = _brandDao.GetCount(x => x.Id == model.Id);
                    var namecount = _brandDao.GetCount(x => x.Name == model.Name);

                    if (objectcount < 1)
                    {
                        throw new InvalidOperationException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new DuplicateException("Already exist brand with this name");
                    }

                    var brand = new Brand()
                    {
                        Id = model.Id,
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status,
                        Rank = model.Rank,
                        CreateBy = model.CreateBy,
                        CreationDate = DateTime.Parse(model.CreationDate),
                        ModifyBy = userId,
                        ModificationDate = _timeService.Now
                    };

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
                    throw new ArgumentNullException(nameof(brand));
                }
                return new BrandEditModel
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    Description = brand.Description,
                    CreateBy = brand.CreateBy,
                    CreationDate = (brand.CreationDate).ToString(),
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
                    filter = x => x.Name.Contains(searchBy) && x.Status != (int)Status.Delete;
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
        
        public IList<BrandDto> LoadAllBrands()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
