using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface IPurchaseDao : IBaseDao<Purchase, long>
    {
        IList<Purchase> GetPurchases(Expression<Func<Purchase, bool>> filter);
        (IList<Purchase> data, int total, int totalDisplay) LoadAllPurchases(Expression<Func<Purchase, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class PurchaseDao : BaseDao<Purchase, long>, IPurchaseDao
    {
        public PurchaseDao(ISession session) : base(session)
        {

        }

        public (IList<Purchase> data, int total, int totalDisplay) LoadAllPurchases(Expression<Func<Purchase, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Purchase> query = _session.Query<Purchase>();

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
                case "Date":
                    query = sortDir == "asc" ? query.OrderBy(c => c.PurchaseDate) : query.OrderByDescending(c => c.PurchaseDate);
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

        public IList<Purchase> GetPurchases(Expression<Func<Purchase, bool>> filter)
        {
            IQueryable<Purchase> query = _session.Query<Purchase>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }
    }

}
