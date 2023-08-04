using IMS.BusinessModel.Dto;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IReportService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<LoseProfitReportDto> GetLoseProfitReport(string dateFrom, string dateTo);
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


    public class ReportService : BaseService, IReportService
    {
        #region Initializtion
        private readonly IReportDao _reportDao;

        public ReportService(ISession session) : base(session)
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
            try
            {
                var totalPurchaseSaleAmount = (await _reportDao.ExecuteQueryAsync<TotalPurchaseSaleAmountDto>($"SELECT " +
                    $"SUM(CASE WHEN [OperationType] = 0 THEN Payment.TotalAmount else 0 END) TotalPurchaseAmount, " +
                    $"SUM(CASE WHEN [OperationType] = 0 THEN Payment.PaidAmount else 0 END) TotalPurchasePaidAmount, " +
                    $"SUM(CASE WHEN [OperationType] = 1 THEN Payment.TotalAmount else 0 END) TotalSaleAmount, " +
                    $"SUM(CASE WHEN [OperationType] = 1 THEN Payment.PaidAmount else 0 END) TotalSalePaidAmount " +
                    $"FROM Payment")).FirstOrDefault();

                var totalCustomer = await _reportDao.GetCountAsync("Customer");
                var totalSupplier = await _reportDao.GetCountAsync("Supplier");

                var dashboard = new DashboardDto();
                dashboard.ToatlPurchaseAmount = totalPurchaseSaleAmount.TotalPurchaseAmount;
                dashboard.ToatlPurchasePaidAmount = totalPurchaseSaleAmount.TotalPurchasePaidAmount;
                dashboard.ToatlPurchaseDueAmount = totalPurchaseSaleAmount.TotalPurchaseAmount - totalPurchaseSaleAmount.TotalPurchasePaidAmount;
                dashboard.TotalSaleAmount = totalPurchaseSaleAmount.TotalSaleAmount;
                dashboard.TotalSalePaidAmount = totalPurchaseSaleAmount.TotalSalePaidAmount;
                dashboard.TotalSaleDueAmount = totalPurchaseSaleAmount.TotalSaleAmount - totalPurchaseSaleAmount.TotalSalePaidAmount;
                dashboard.TotalActiveCustomer = totalCustomer.Active;
                dashboard.TotalInActiveCustomer = totalCustomer.Inactive;
                dashboard.TotalActiveSupplier = totalSupplier.Active;
                dashboard.TotalInActiveSupplier = totalSupplier.Inactive;

                return dashboard;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }

        public async Task<LoseProfitReportDto> GetLoseProfitReport(string dateFrom, string dateTo)
        {
            var saleInfo = (await _reportDao.ExecuteQueryAsync<PaymentDetailsDto>($"SELECT " +
                $"COUNT(s.Id) AS Count, " +
                $"SUM(s.GrandTotalPrice) AS TotalAmount, " +
                $"SUM(pm.PaidAmount) AS PaidAmount, " +
                $"SUM(TotalAmount - PaidAmount) AS DueAmount" +
                $" FROM Sale s " +
                $"JOIN Payment pm ON pm.Id = s.PaymentId " +
                $"WHERE s.SaleDate BETWEEN '{dateFrom}' AND '{dateTo}'")).FirstOrDefault();

            var purchaseInfo = (await _reportDao.ExecuteQueryAsync<PaymentDetailsDto>($"SELECT " +
                $"COUNT(p.Id) AS Count, " +
                $"SUM(p.GrandTotalPrice) AS TotalAmount, " +
                $"SUM(pm.PaidAmount) AS PaidAmount, " +
                $"SUM(TotalAmount - PaidAmount) AS DueAmount" +
                $" FROM Purchase p " +
                $"JOIN Payment pm ON pm.Id = p.PaymentId " +
                $"WHERE p.PurchaseDate BETWEEN '{dateFrom}' AND '{dateTo}'")).FirstOrDefault();

            var loseProfitReportDto = new LoseProfitReportDto
            {
                PurchasePaymentDetails = purchaseInfo,
                SalePaymentDetails = saleInfo,
            };


            return loseProfitReportDto;
        }
    }
}
