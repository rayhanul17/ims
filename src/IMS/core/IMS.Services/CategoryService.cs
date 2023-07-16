using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.Dao;
using log4net;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Services
{
    public interface ICategoryService
    {
        Task Add(CategoryAddModel model);
    }

    public class CategoryService : ICategoryService
    {
        private readonly ICategoryDao _categoryDao;
        private readonly ILog _serviceLogger = LogManager.GetLogger("ServiceLogger");

        public CategoryService(ISession session)
        {            
            _categoryDao = new CategoryDao(session);
        }

        public async Task Add(CategoryAddModel model)
        {
            var category = new Category()
            {
                Name = model.Name,
                Description = model.Description,
                Status = (int)model.Status,
            };

            await _categoryDao.AddAsync(category);
            _serviceLogger.Info("Data Saved!");
        }
    }

}
