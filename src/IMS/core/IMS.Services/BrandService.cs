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
    public interface IBrandService
    {
        #region Opperational Function
        Task AddAsync(BrandAddViewModel model, long userId);
        Task UpdateAsync(BrandEditViewModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        #endregion

        #region Single Instance Loading Function
        Task<BrandEditViewModel> GetByIdAsync(long id);
        #endregion

        #region List Loading Function
        IList<(long, string)> LoadAllBrands();
        IList<(long, string)> LoadAllActiveBrands();
        Task<(int total, int totalDisplay, IList<BrandDto> records)> LoadAllBrands(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

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
        public async Task AddAsync(BrandAddViewModel model, long userId)
        {
            try
            {
                var count = _brandDao.GetCount(x => x.Name == model.Name);

                if (count > 0)
                {
                    throw new CustomException("Found another Brand with this name");
                }

                var brand = new Brand()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Status = (int)model.Status,
                    Rank = await _brandDao.GetMaxRank(typeof(Brand).Name) + 1,
                    CreateBy = userId,
                    CreationDate = _timeService.Now,
                };

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _brandDao.AddAsync(brand);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();                        
                        throw ex;
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }
        }            

        public async Task UpdateAsync(BrandEditViewModel model, long userId)
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

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _brandDao.EditAsync(brand);
                        transaction.Commit();                        
                    }
                    catch(Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }            
        }

        public async Task RemoveByIdAsync(long id, long userId)
        {
            try
            {
                var brand = await _brandDao.GetByIdAsync(id);
                if (brand == null)
                {
                    throw new CustomException("No object found with this id");
                }
                brand.Status = (int)Status.Delete;
                brand.ModifyBy = userId;
                brand.ModificationDate = _timeService.Now;

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _brandDao.EditAsync(brand);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;            
            }
        }
        #endregion

        #region Single Instance Loading
        public async Task<BrandEditViewModel> GetByIdAsync(long id)
        {
            try
            {
                var brand = await _brandDao.GetByIdAsync(id);
                if (brand == null)
                {
                    throw new CustomException("No object found with this id");
                }
                return new BrandEditViewModel
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
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }
        }
        #endregion

        #region List Loading Function
        public async Task<(int total, int totalDisplay, IList<BrandDto> records)> LoadAllBrands(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Brand, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) || x.Description.Contains(searchBy);
                }

                var result = _brandDao.GetDynamic(filter, null, start, length, sortBy, sortDir);

                List<BrandDto> categories = new List<BrandDto>();
                foreach (Brand brand in result.data)
                {
                    categories.Add(
                        new BrandDto
                        {
                            Id = brand.Id.ToString(),
                            Name = brand.Name,
                            Description = brand.Description,
                            CreateBy = await _userService.GetUserNameAsync(brand.CreateBy),
                            CreationDate = brand.CreationDate.ToString(),
                            Status = ((Status)brand.Status).ToString(),
                            Rank = brand.Rank.ToString()

                        });
                }

                return (result.total, result.totalDisplay, categories);
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }
        }

        public IList<(long, string)> LoadAllBrands()
        {
            List<(long, string)> brands = new List<(long, string)>();
            try
            {
                var allBrands = _brandDao.Get(x => x.Status != (int)Status.Delete);
                foreach (var brand in allBrands)
                {
                    brands.Add((brand.Id, brand.Name));
                }
            }
            catch(Exception ex) 
            {
                _serviceLogger.Error(ex.Message, ex);
            }

            return brands;
        }

        public IList<(long, string)> LoadAllActiveBrands()
        {
            List<(long, string)> brands = new List<(long, string)>();
            try
            {
                var allBrands = _brandDao.Get(x => x.Status == (int)Status.Active);
                foreach (var brand in allBrands)
                {
                    brands.Add((brand.Id, brand.Name));
                }
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
            }

            return brands;
        }
        #endregion
    }
}
