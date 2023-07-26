using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface ISaleDao : IBaseDao<Sale, long>
    {
        IList<Sale> GetSales(Expression<Func<Sale, bool>> filter);
        (IList<Sale> data, int total, int totalDisplay) LoadAllSales(Expression<Func<Sale, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class SaleDao : BaseDao<Sale, long>, ISaleDao
    {
        public SaleDao(ISession session) : base(session)
        {

        }

        public (IList<Sale> data, int total, int totalDisplay) LoadAllSales(Expression<Func<Sale, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Sale> query = _session.Query<Sale>();

            //query = query.Where(x => x.Status != (int)Status.Delete);

            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            switch (sortBy)
            {
                case "Total Amount":
                    query = sortDir == "asc" ? query.OrderBy(c => c.GrandTotalPrice) : query.OrderByDescending(c => c.GrandTotalPrice);
                    break;
                case "Sale Date":
                    query = sortDir == "asc" ? query.OrderBy(c => c.SaleDate) : query.OrderByDescending(c => c.SaleDate);
                    break;                
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }

        public IList<Sale> GetSales(Expression<Func<Sale, bool>> filter)
        {
            IQueryable<Sale> query = _session.Query<Sale>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }
    }

}
