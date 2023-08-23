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
    public interface ICategoryService
    {
        #region Operational Function
        Task AddAsync(CategoryAddViewModel model, long userId);
        Task UpdateAsync(CategoryEditViewModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        #endregion

        #region Single Instance Loading Function
        Task<CategoryEditViewModel> GetByIdAsync(long id);
        #endregion

        #region List Loading Function
        IList<(long, string)> LoadAllCategories();
        IList<(long, string)> LoadAllActiveCategories();
        Task<(int total, int totalDisplay, IList<CategoryDto> records)> LoadAllCategories(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

    public class CategoryService : BaseService, ICategoryService
    {
        #region Initializtion
        private readonly ICategoryDao _categoryDao;
        private readonly IProductDao _productDao;

        public CategoryService(ISession session) : base(session)
        {
            _categoryDao = new CategoryDao(session);
            _productDao = new ProductDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(CategoryAddViewModel model, long userId)
        {
            try
            {
                if (model == null)
                {
                    throw new CustomException("Null model found");
                }
                var count = _categoryDao.GetCount(x => x.Name == model.Name && x.Status != (int)Status.Delete);
                if (count > 0)
                {
                    throw new CustomException("Found another category with this name");
                }

                var category = new Category()
                {
                    Name = model.Name,
                    Description = model.Description,
                    Status = (int)model.Status,
                    Rank = await _categoryDao.GetMaxRank() + 1,
                    CreateBy = userId,
                    CreationDate = _timeService.Now,
                };

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _categoryDao.AddAsync(category);
                        transaction.Commit();
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch(CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {                
                _serviceLogger.Error(ex.Message, ex);
                throw ex;            
            }
        }

        public async Task UpdateAsync(CategoryEditViewModel model, long userId)
        {
            try
            {
                if (model == null)
                {
                    throw new CustomException("Null model found");
                }

                var nameCount = _categoryDao.GetCount(x => x.Name == model.Name && x.Status != (int)Status.Delete);
                var category = await _categoryDao.GetByIdAsync(model.Id);

                if (category == null)
                {
                    throw new CustomException("No record found with this id!");
                }
                if (nameCount > 1)
                {
                    throw new CustomException("Already exist category with this name");
                }

                category.Name = model.Name;
                category.Description = model.Description;
                category.Status = (int)model.Status;
                category.ModifyBy = userId;
                category.ModificationDate = _timeService.Now;

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _categoryDao.EditAsync(category);
                        transaction.Commit();
                    }
                    catch (CustomException ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }                    
                }
            }
            catch(CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {                
                _serviceLogger.Error(ex.Message, ex);
                throw;            
            }
        }

        public async Task RemoveByIdAsync(long id, long userId)
        {
            try
            {
                var count = _productDao.GetCount(x => x.Category.Id == id && x.Status != (int)Status.Delete);
                var category = await _categoryDao.GetByIdAsync(id);

                if (category == null)
                {
                    throw new CustomException("No record found with this id!");
                }
                if (count > 0)
                {
                    throw new CustomException("Found product under this category");
                }

                category.Status = (int)Status.Delete;
                category.ModifyBy = userId;
                category.ModificationDate = _timeService.Now;

                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        await _categoryDao.EditAsync(category);
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
                _serviceLogger.Error(ex);             
                throw ex;            
            }
        }

        #endregion

        #region Single Instance Loading
        public async Task<CategoryEditViewModel> GetByIdAsync(long id)
        {
            try
            {
                var category = await _categoryDao.GetByIdAsync(id);
                if (category == null)
                {
                    throw new CustomException("No object found with this id");
                }
                return new CategoryEditViewModel
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    CreateBy = category.CreateBy,
                    CreationDate = category.CreationDate,
                    ModifyBy = category.ModifyBy,
                    ModificationDate = category.ModificationDate,
                    Status = (Status)category.Status,
                    Rank = category.Rank,
                    VersionNumber = category.VersionNumber,
                    BusinessId = category.BusinessId,
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
        public async Task<(int total, int totalDisplay, IList<CategoryDto> records)> LoadAllCategories(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Category, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) || x.Description.Contains(searchBy);
                }

                var result = _categoryDao.GetDynamic(filter, null, start, length, sortBy, sortDir);

                List<CategoryDto> categories = new List<CategoryDto>();
                foreach (Category category in result.data)
                {
                    categories.Add(
                        new CategoryDto
                        {
                            Id = category.Id.ToString(),
                            Name = category.Name,
                            Description = category.Description,
                            CreateBy = await _userService.GetUserNameAsync(category.CreateBy),
                            CreationDate = category.CreationDate.ToString(),
                            Status = ((Status)category.Status).ToString(),
                            Rank = category.Rank.ToString()
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

        public IList<(long, string)> LoadAllCategories()
        {
            List<(long, string)> categories = new List<(long, string)>();
            var allcategories = _categoryDao.Get(x => x.Status != (int)Status.Delete);
            foreach (var category in allcategories)
            {
                categories.Add((category.Id, category.Name));
            }
            return categories;
        }

        public IList<(long, string)> LoadAllActiveCategories()
        {
            List<(long, string)> categories = new List<(long, string)>();
            var allcategories = _categoryDao.Get(x => x.Status == (int)Status.Active);
            foreach (var category in allcategories)
            {
                categories.Add((category.Id, category.Name));
            }
            return categories;
        }
        #endregion

    }
}
