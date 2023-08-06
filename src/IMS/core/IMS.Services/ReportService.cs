using IMS.BusinessModel.Dto;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IReportService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<LoseProfitReportDto> GetLoseProfitReport(LoseProfitReportDto model);
        Task<IEnumerable<MyModel>> ExecuteRawQueryAsync();
    }
    #endregion
    /* 
     === Active Inactive Count Query ===
     * SELECT
           SUM(CASE WHEN [Status] = 0 THEN 1 else 0 END) Inactive,
           SUM(CASE WHEN [Status] = 1 THEN 1 else 0 END) Active
           FROM Supplier
     === Purchase & Sale Amout according to date ===
     *   select 
	     sum(case when p.OperationType = 1 Then pd.Amount else 0 end) Sale
	    ,sum(case when p.OperationType = 0 Then pd.Amount else 0 end) Purchase
        from PaymentDetails pd
        join Payment p on p.Id = pd.PaymentId
        WHERE PaymentDate BETWEEN '2023-08-02 14:17:39.2373632' AND '2023-08-02 14:18:57.3401938'
    === Sale/Purchase Product Details ===
        SELECT s.Id, s.SaleDate, sd.ProductId, sd.Quantity, sd.TotalPrice, sd.SaleId
	    FROM Sale s
	    JOIN SaleDetails sd ON s.Id = sd.SaleId
	    WHERE s.SaleDate BETWEEN '2023-08-02 03:22' AND '2023-08-02 09:46'
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

        public async Task<LoseProfitReportDto> GetLoseProfitReport(LoseProfitReportDto model)
        {
            var dates = model.DateRange.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

            /* === If Timservice provide Utc time ===
             * var startDate = DateTime.Parse(dates[0]).ToUniversalTime().ToString();
             * var endDate = DateTime.Parse(dates[1]).ToUniversalTime().ToString();
            */

            var startDate = DateTime.Parse(dates[0]).ToString();
            var endDate = DateTime.Parse(dates[1]).ToString();

            var saleInfo = (await _reportDao.ExecuteQueryAsync<PaymentDetailsDto>($"SELECT " +
                $"COUNT(s.Id) AS Count, " +
                $"SUM(s.GrandTotalPrice) AS TotalAmount, " +
                $"SUM(pm.PaidAmount) AS PaidAmount, " +
                $"SUM(TotalAmount - PaidAmount) AS DueAmount" +
                $" FROM Sale s " +
                $"JOIN Payment pm ON pm.Id = s.PaymentId " +
                $"WHERE s.SaleDate BETWEEN '{startDate}' AND '{endDate}'")).FirstOrDefault();

            var purchaseInfo = (await _reportDao.ExecuteQueryAsync<PaymentDetailsDto>($"SELECT " +
                $"COUNT(p.Id) AS Count, " +
                $"SUM(p.GrandTotalPrice) AS TotalAmount, " +
                $"SUM(pm.PaidAmount) AS PaidAmount, " +
                $"SUM(TotalAmount - PaidAmount) AS DueAmount" +
                $" FROM Purchase p " +
                $"JOIN Payment pm ON pm.Id = p.PaymentId " +
                $"WHERE p.PurchaseDate BETWEEN '{startDate}' AND '{endDate}'")).FirstOrDefault();

            var saleProductList = await _reportDao.ExecuteQueryAsync<ProductListDto>($"SELECT " +
                $"s.SaleDate AS Date, " +                
                $"sd.Quantity, " +
                $"sd.TotalPrice, " +
                $"p.Name " +
                $"FROM Sale s " +
                $"JOIN SaleDetails sd ON s.Id = sd.SaleId " +
                $"LEFT JOIN Product p ON p.id = sd.ProductId " +
                $"WHERE s.SaleDate BETWEEN '{startDate}' AND '{endDate}'");

            var purchaseProductList = await _reportDao.ExecuteQueryAsync<ProductListDto>($"SELECT " +
                $"pr.PurchaseDate AS Date, " +
                $"prd.Quantity, " +
                $"prd.TotalPrice, " +
                $"p.Name " +
                $"FROM Purchase pr " +
                $"JOIN PurchaseDetails prd ON pr.Id = prd.PurchaseId " +
                $"LEFT JOIN Product p ON p.id = prd.ProductId " +
                $"WHERE pr.PurchaseDate BETWEEN '{startDate} ' AND ' {endDate}'");

            var loseProfitReportDto = new LoseProfitReportDto
            {
                DateRange = dates[0] + "-" + dates[1], 
                PurchasePaymentDetails = purchaseInfo,
                SalePaymentDetails = saleInfo,
                PurchaseProductList = purchaseProductList,
                SaleProductList = saleProductList
            };


            return loseProfitReportDto;
        }
    }
}
