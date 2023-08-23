using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Dao
{
    public interface IPaymentDao : IBaseDao<Payment, long>
    {
        #region Opperational Function
        Task SetPaymentIdToPurchaseTable(long paymentId, long purchaseId);
        Task SetPaymentIdToSaleTable(long paymentId, long saleId);
        Task<bool> IsBankExistInAnyPayment(long bankId);
        #endregion

        #region List Loading Function
        (IList<Payment> data, int total, int totalDisplay) LoadAllPayments(Expression<Func<Payment, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
        #endregion
    }

    public class PaymentDao : BaseDao<Payment, long>, IPaymentDao
    {
        #region Initialization
        public PaymentDao(ISession session) : base(session)
        {

        }
        #endregion

        #region Opperational Function
        public async Task SetPaymentIdToPurchaseTable(long paymentId, long purchaseId)
        {
            var query = $"UPDATE Purchase SET PaymentId = {paymentId} WHERE Id = {purchaseId};";

            await ExecuteUpdateDeleteQuery(query);
        }

        public async Task SetPaymentIdToSaleTable(long paymentId, long saleId)
        {
            var query = $"UPDATE Sale SET PaymentId = {paymentId} WHERE Id = {saleId};";

            await ExecuteUpdateDeleteQuery(query);
        }

        public async Task<bool> IsBankExistInAnyPayment(long bankId)
        {
            var query = $"SELECT COUNT(1) FROM {typeof(PaymentDetails).Name} WHERE [BankId] = {bankId}";
            var count = Convert.ToInt32(await GetScallerValueAsync(query));

            return count > 0;
        }
        #endregion

        #region List Loading Function
        public (IList<Payment> data, int total, int totalDisplay) LoadAllPayments(Expression<Func<Payment, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Payment> query = _session.Query<Payment>();

            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            switch (sortBy)
            {
                case "Rank":
                    query = sortDir == "asc" ? query.OrderBy(c => c.Rank) : query.OrderByDescending(c => c.Rank);
                    break;
                case "TotalAmount":
                    query = sortDir == "asc" ? query.OrderBy(c => c.TotalAmount) : query.OrderByDescending(c => c.TotalAmount);
                    break;
                case "PaidAmount":
                    query = sortDir == "asc" ? query.OrderBy(c => c.PaidAmount) : query.OrderByDescending(c => c.PaidAmount);
                    break;
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
        #endregion
    }
}
