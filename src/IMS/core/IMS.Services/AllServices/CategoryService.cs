using Autofac;
using IMS.DataAccess.Repositories;
using IMS.Infrastructure.Utility;
using IMS.Models.Dtos.Categories;
using IMS.Models.Entities;
using log4net;
using NHibernate;
using System;
using System.Threading.Tasks;

namespace IMS.AllServices
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISession _session;
        private readonly ILifetimeScope _scope;
        private readonly ITimeService _timeService;
        private readonly ILog _serviceLogger;

        public CategoryService(ILifetimeScope scope, ISession session, ICategoryRepository categoryRepository, ITimeService timeService)
        {
            _timeService = timeService;
            _session = session;
            _scope = scope;
            _categoryRepository = categoryRepository;
            _serviceLogger = _scope.ResolveNamed<ILog>("Service");
        }

        public async Task AddAsync(CategoryAdd categoryadd)
        {
            using (var tx = _session.BeginTransaction())
            {
                try
                {
                    #region Mapping
                    Category category = new Category();
                    category.Name = categoryadd.Name;
                    category.Description = categoryadd.Description;
                    category.Status = (int)categoryadd.Status;
                    #endregion
                    category.CreationDate = _timeService.Now;
                    //Add Real User Id----------
                    category.CreateBy = 10;
                    //--------------------------
                    await _categoryRepository.AddAsync(category);
                    tx.Commit();
                    _serviceLogger.Info("Successfully saved!");
                }
                catch (Exception ex)
                {
                    _serviceLogger.Error(ex);
                    tx.Rollback();
                }
            }
        }

        public async Task<Category> GetByIdAsync(long id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }
    }
}
