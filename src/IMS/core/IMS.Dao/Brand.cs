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
    public interface IBrandDao : IBaseDao<Brand, long>
    {
        (IList<Brand> data, int total, int totalDisplay) LoadAllBrands(Expression<Func<Brand, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class BrandDao : BaseDao<Brand, long>, IBrandDao
    {
        public BrandDao(ISession session) : base(session)
        {

        }

        public (IList<Brand> data, int total, int totalDisplay) LoadAllBrands(Expression<Func<Brand, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Brand> query = _session.Query<Brand>();
            
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
    }
    
}
