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
    public interface ICategoryService
    {
        Task AddAsync(CategoryAddViewModel model, long userId);
        Task UpdateAsync(CategoryEditViewModel model, long userId);
        Task RemoveByIdAsync(long id, long userId);
        Task<CategoryEditViewModel> GetByIdAsync(long id);
        IList<(long, string)> LoadAllCategories();
        IList<(long, string)> LoadAllActiveCategories();
        (int total, int totalDisplay, IList<CategoryDto> records) LoadAllCategories(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

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
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var count = _categoryDao.GetCount(x => x.Name == model.Name);
                    if(count > 0)
                    {
                        throw new CustomException("Found another category with this name");
                    }

                    var category = new Category()
                    {
                        Name = model.Name,
                        Description = model.Description,
                        Status = (int)model.Status,
                        Rank = await _categoryDao.GetMaxRank("Category") + 1,
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

        public async Task UpdateAsync(CategoryEditViewModel model, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    var category = await _categoryDao.GetByIdAsync(model.Id);
                    var namecount = _categoryDao.GetCount(x => x.Name == model.Name);

                    if (category == null)
                    {
                        throw new CustomException("No record found with this id!");
                    }
                    if (namecount > 1)
                    {
                        throw new CustomException("Already exist category with this name");
                    }

                    category.Name = model.Name;
                    category.Description = model.Description;
                    category.Status = (int)model.Status;
                    category.ModifyBy = userId;
                    category.ModificationDate = _timeService.Now;

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
                    var count = _productDao.GetCount(x => x.Category.Id == id && x.Status != (int)Status.Delete);
                    if (count > 0)
                    {
                        throw new CustomException("Found product under this category");
                    }
                    //await _categoryDao.RemoveByIdAsync(id);
                    var category = await _categoryDao.GetByIdAsync(id);
                    category.Status = (int)Status.Delete;
                    category.ModifyBy = userId;
                    category.ModificationDate = _timeService.Now;
                    await _categoryDao.EditAsync(category);
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

                    throw;
                }
            }
        }

        public async Task<CategoryEditViewModel> GetByIdAsync(long id)
        {
            try
            {
                var category = await _categoryDao.GetByIdAsync(id);
                if (category == null)
                {
                    throw new ArgumentNullException(nameof(category));
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
        public (int total, int totalDisplay, IList<CategoryDto> records) LoadAllCategories(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Category, bool>> filter = null;
                if (searchBy != null)
                {
                    filter = x => x.Name.Contains(searchBy) || x.Description.Contains(searchBy);
                }

                var result = _categoryDao.LoadAllCategories(filter, null, start, length, sortBy, sortDir);

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
        
        public IList<(long,string)> LoadAllCategories()
        {
            List<(long, string)> categories = new List<(long, string)> ();
            var allcategories = _categoryDao.GetCategory(x => x.Status != (int)Status.Delete);
            foreach (var category in allcategories)
            {
                categories.Add((category.Id, category.Name));
            }
            return categories;
        }

        public IList<(long, string)> LoadAllActiveCategories()
        {
            List<(long, string)> categories = new List<(long, string)>();
            var allcategories = _categoryDao.GetCategory(x => x.Status == (int)Status.Active);
            foreach (var category in allcategories)
            {
                categories.Add((category.Id, category.Name));
            }
            return categories;
        }
        #endregion

    }
}
