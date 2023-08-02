using IMS.BusinessModel.Dto;
using IMS.Dao;
using NHibernate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IReportService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<IEnumerable<MyModel>> ExecuteRawQueryAsync();
    }
    #endregion
    /* 
     ***===
     * SELECT
           SUM(CASE WHEN [Status] = 0 THEN 1 else 0 END) Inactive,
           SUM(CASE WHEN [Status] = 1 THEN 1 else 0 END) Active
           FROM Supplier
     **** Purchase & Sale Amout according to date
     *   select 
	     sum(case when p.OperationType = 1 Then pd.Amount else 0 end) Sale
	    ,sum(case when p.OperationType = 0 Then pd.Amount else 0 end) Purchase
        from PaymentDetails pd
        join Payment p on p.Id = pd.PaymentId
        WHERE PaymentDate BETWEEN '2023-08-02 14:17:39.2373632' AND '2023-08-02 14:18:57.3401938'
     */


    public class ReportService : IReportService
    {
        #region Initializtion
        private readonly IReportDao _reportDao;

        public ReportService(ISession session)
        {
            _reportDao = new ReportDao(session);
        }

        public async Task<IEnumerable<MyModel>> ExecuteRawQueryAsync()
        {
            var result = await _reportDao.ExecuteQueryAsync<MyModel>("SELECT SupplierId, PurchaseDate, GrandTotalPrice FROM Purchase");
            return result;
        }

        #endregion
        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var totalPurchaseSaleAmount = await _reportDao.ExecuteQueryAsync<PurchaseSaleAmountDto>(@"SELECT
                                        OperationType,
                                        SUM(TotalAmount) AS TotalAmount,
                                        SUM(PaidAmount) AS TotalPaidAmount,
                                        SUM(TotalAmount-PaidAmount) AS TotalDueAmount
                                        FROM Payment
                                        GROUP BY OperationType");

            var totalCustomer = await _reportDao.ExecuteQueryAsync<ActiveInactiveDto>(@"SELECT
                                [Status],
                                Count(CASE WHEN [Status] = 0 THEN 'I' WHEN [Status] = 1 THEN 'A' END) AS Count
                                FROM Customer
                                GROUP BY [Status]");

            var totalSuplier = await _reportDao.ExecuteQueryAsync<ActiveInactiveDto>(@"SELECT
                                [Status],
                                Count(CASE WHEN [Status] = 0 THEN 'I' WHEN [Status] = 1 THEN 'A' END) AS Count
                                FROM Supplier
                                GROUP BY [Status]");

            var totalBank = await _reportDao.ExecuteQueryAsync<ActiveInactiveDto>(@"SELECT
                                [Status],
                                Count(CASE WHEN [Status] = 0 THEN 'I' WHEN [Status] = 1 THEN 'A' END) AS Count
                                FROM Bank
                                GROUP BY [Status]");

            var dashboard = new DashboardDto();
            dashboard.ToatlPurchaseAmount = totalPurchaseSaleAmount[0].TotalAmount;
            dashboard.ToatlPurchasePaidAmount = totalPurchaseSaleAmount[0].TotalPaidAmount;
            dashboard.ToatlPurchaseDueAmount = totalPurchaseSaleAmount[0].TotalDueAmount;
            dashboard.TotalSaleAmount = totalPurchaseSaleAmount[1].TotalAmount;
            dashboard.TotalSalePaidAmount = totalPurchaseSaleAmount[1].TotalPaidAmount;
            dashboard.TotalSaleDueAmount = totalPurchaseSaleAmount[1].TotalDueAmount;
            dashboard.TotalActiveCustomer = await _reportDao.GetTotalCount("Id", "Customer", "Status = 1");
            dashboard.TotalInActiveCustomer = await _reportDao.GetTotalCount("Id", "Customer", "Status = 0");
            dashboard.TotalActiveSupplier = await _reportDao.GetTotalCount("Id", "Supplier", "Status = 1");
            dashboard.TotalInActiveSupplier = await _reportDao.GetTotalCount("Id", "Supplier", "Status = 0");


            /*
            dashboard.ToatlPurchasePrice = await _reportDao.GetSingleColumnTatal("GrandTotalPrice", "Purchase");
            dashboard.TotalSalePrice = await _reportDao.GetSingleColumnTatal("GrandTotalPrice", "Sale");
            dashboard.TotalActiveCustomer = await _reportDao.GetTotalCount("Id", "Customer", "Status = 1");
            dashboard.TotalInActiveCustomer = await _reportDao.GetTotalCount("Id", "Customer", "Status = 0");
            dashboard.TotalActiveSupplier = await _reportDao.GetTotalCount("Id", "Supplier", "Status = 1");
            dashboard.TotalInActiveSupplier = await _reportDao.GetTotalCount("Id", "Supplier", "Status = 0");

            dashboard.TotalActiveCustomer = totalCustomer[0] != null? totalCustomer[0].Count : 0;
            dashboard.TotalInActiveCustomer = totalCustomer[1] != null ? totalCustomer[1].Count : 0;
            dashboard.TotalActiveSupplier = totalSuplier[0] != null ? totalSuplier[0].Count : 0;
            dashboard.TotalInActiveSupplier = totalSuplier[1] != null ? totalSuplier[0].Count : 0;
            */
            return dashboard;
        }
    }
}
