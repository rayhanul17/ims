﻿using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.Dao;
using NHibernate;
using System;
using System.Threading.Tasks;

namespace IMS.Services
{
    public interface ICategoryService
    {
        Task Add(CategoryAddModel model);
    }

    public class CategoryService : BaseService, ICategoryService
    {
        private readonly ICategoryDao _categoryDao;

        public CategoryService(ISession session) : base(session)
        {
            _categoryDao = new CategoryDao(session);
        }

        public async Task Add(CategoryAddModel model)
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
    }

}
