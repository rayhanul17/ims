using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface ICategoryService
    {
        Task AddAsync(CategoryAddModel model, long userId);
        Task UpdateAsync(CategoryEditModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<CategoryEditModel> GetByIdAsync(long id);
        IList<CategoryDto> LoadAllCategories();
        (int total, int totalDisplay, IList<CategoryDto> records) LoadAllCategories(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class CategoryService : BaseService, ICategoryService
    {
        #region Initializtion
        private readonly ICategoryDao _categoryDao;

        public CategoryService(ISession session) : base(session)
        {
            _categoryDao = new CategoryDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(CategoryAddModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var category = new Category()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status,
                        Rank = model.Rank,
                        CreateBy = userId,
                        CreationDate = _timeService.Now,
                    };

                    await _categoryDao.AddAsync(category);
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

        public async Task UpdateAsync(CategoryEditModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var category = new Category()
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

                    await _categoryDao.EditAsync(category);
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
                    //await _categoryDao.RemoveByIdAsync(id);
                    var category = await _categoryDao.GetByIdAsync(id);
                    category.Status = (int)Status.Delete;
                    category.ModifyBy = userId;
                    category.ModificationDate = _timeService.Now;
                    await _categoryDao.EditAsync(category);
                    transaction.Commit();

                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex);
                    transaction.Rollback();
                }
            }
        }

        public async Task<CategoryEditModel> GetByIdAsync(long id)
        {
            var category = await _categoryDao.GetByIdAsync(id);

            return new CategoryEditModel
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description,
                CreateBy = category.CreateBy,
                CreationDate = (category.CreationDate).ToString(),
                ModifyBy = category.ModifyBy,
                ModificationDate = category.ModificationDate,
                Status = (Status)category.Status,
                Rank = category.Rank,
                VersionNumber = category.VersionNumber,
                BusinessId = category.BusinessId,
            };
        }
        #endregion

        #region Single Instance Loading
        #endregion

        #region List Loading Function
        public (int total, int totalDisplay, IList<CategoryDto> records) LoadAllCategories(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Category, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) && x.Status != (int)Status.Delete;
                }
                var result = _categoryDao.LoadAll<Category>(filter, null, start, length, sortBy, sortDir);

                List<CategoryDto> categories = new List<CategoryDto>();
                foreach (Category category in result.data)
                {
                    categories.Add(
                        new CategoryDto
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
        
        public IList<CategoryDto> LoadAllCategories()
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}
