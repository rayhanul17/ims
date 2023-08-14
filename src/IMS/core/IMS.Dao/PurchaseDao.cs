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
        #region List Loading Function
        (IList<Purchase> data, int total, int totalDisplay) LoadAllPurchases(Expression<Func<Purchase, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
        #endregion
    }

    public class PurchaseDao : BaseDao<Purchase, long>, IPurchaseDao
    {
        #region Initialization
        public PurchaseDao(ISession session) : base(session)
        {

        }
        #endregion

        #region List Loading Function
        public (IList<Purchase> data, int total, int totalDisplay) LoadAllPurchases(Expression<Func<Purchase, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Purchase> query = _session.Query<Purchase>();

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
                case "Purchase Date":
                    query = sortDir == "asc" ? query.OrderBy(c => c.PurchaseDate) : query.OrderByDescending(c => c.PurchaseDate);
                    break;
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
        #endregion
    }
}
