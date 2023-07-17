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
    public interface ICategoryService
    {
        Task AddAsync(CategoryAddModel model, long userId);
        Task<CategoryEditModel> GetByIdAsync(long id);
        Task UpdateAsync(CategoryEditModel model, long userId);
        #region Load instances
        IList<CategoryDto> LoadAllCategories();
        (int total, int totalDisplay, IList<CategoryDto> records) LoadAllCategories(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryDao _categoryDao;

        public CategoryService(ISession session) : base(session)
        {
            _categoryDao = new CategoryDao(session);
        }

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
                        CreateBy = userId,
                        CreationDate = _timeService.Now,

                    };

                    await _categoryDao.AddAsync(category);
                    transaction.Commit();

                    _serviceLogger.Info("Data Saved!");
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);
                }
            }
        }

        public Task UpdateAsync(CategoryEditModel model, long userId)
        {
            throw new NotImplementedException();
        }

        public (int total, int totalDisplay, IList<CategoryDto> records) LoadAllCategories(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<BaseEntity<long>, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy);
                }
                var result = _categoryDao.LoadAll(filter, null, start, length, sortBy, sortDir);

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

        public async Task<CategoryEditModel> GetByIdAsync(long id)
        {
            var category = await _categoryDao.GetByIdAsync(id);

            return new CategoryEditModel
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
    }

}
