using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface IPaymentDao : IBaseDao<Payment, long>
    {
        IList<Payment> GetPayments(Expression<Func<Payment, bool>> filter);
        (IList<Payment> data, int total, int totalDisplay) LoadAllPayments(Expression<Func<Payment, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class PaymentDao : BaseDao<Payment, long>, IPaymentDao
    {
        public PaymentDao(ISession session) : base(session)
        {

        }

        public (IList<Payment> data, int total, int totalDisplay) LoadAllPayments(Expression<Func<Payment, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Payment> query = _session.Query<Payment>();

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
                    query = sortDir == "asc" ? query.OrderBy(c => c.Amount) : query.OrderByDescending(c => c.Amount);
                    break;                             
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }

        public IList<Payment> GetPayments(Expression<Func<Payment, bool>> filter)
        {
            IQueryable<Payment> query = _session.Query<Payment>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }
    }

}
