using FluentNHibernate.Data;
using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface ICategoryDao : IBaseDao<Category, long>
    {
        IList<Category> GetCategory(Expression<Func<Category, bool>> filter);
        (IList<Category> data, int total, int totalDisplay) LoadAllCategories(Expression<Func<Category, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class CategoryDao : BaseDao<Category, long>, ICategoryDao
    {
        public CategoryDao(ISession session) : base(session)
        {

        }

        public (IList<Category> data, int total, int totalDisplay) LoadAllCategories(Expression<Func<Category, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Category> query = _session.Query<Category>();
            
            query = query.Where(x => x.Status != (int)Status.Delete);

            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            switch (sortBy)
            {
                case "Name":
                    query = sortDir == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
                    break;
                case "Created By":
                    query = sortDir == "asc" ? query.OrderBy(c => c.CreateBy) : query.OrderByDescending(c => c.CreateBy);
                    break;
                //case "Modified By":
                //    query = sortDir == "asc" ? query.OrderBy(c => c.ModifyBy) : query.OrderByDescending(c => c.ModifyBy);
                //    break;
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }

        public IList<Category> GetCategory(Expression<Func<Category, bool>> filter)
        {
            IQueryable<Category> query = _session.Query<Category>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }
    }
    
}
