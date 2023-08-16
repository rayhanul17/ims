using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IMS.Services
{

    public interface IReportService
    {
        #region Operational Function
        Task<DashboardDto> GetDashboardDataAsync();
        Task<LoseProfitReportDto> GetLoseProfitReport(LoseProfitReportDto model);
        #endregion
    }

    public class ReportService : BaseService, IReportService
    {
        #region Initializtion
        private readonly IReportDao _reportDao;

        public ReportService(ISession session) : base(session)
        {
            _reportDao = new ReportDao(session);
        }
        #endregion

        #region Operational Function
        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            try
            {
                var totalPurchaseSaleAmount = (await _reportDao.ExecuteQueryAsync<TotalPurchaseSaleAmountDto>($"SELECT " +
                    $"SUM(CASE WHEN [{typeof(OperationType).Name}] = {(int)OperationType.Purchase} THEN Payment.TotalAmount ELSE 0 END) TotalPurchaseAmount, " +
                    $"SUM(CASE WHEN [{typeof(OperationType).Name}] = {(int)OperationType.Purchase} THEN Payment.PaidAmount ELSE 0 END) TotalPurchasePaidAmount, " +
                    $"SUM(CASE WHEN [{typeof(OperationType).Name}] = {(int)OperationType.Sale} THEN Payment.TotalAmount ELSE 0 END) TotalSaleAmount, " +
                    $"SUM(CASE WHEN [{typeof(OperationType).Name}] = {(int)OperationType.Sale} THEN Payment.PaidAmount ELSE 0 END) TotalSalePaidAmount " +
                    $"FROM Payment")).FirstOrDefault();

                var totalCustomer = await _reportDao.GetCountAsync(typeof(Customer).Name);
                var totalSupplier = await _reportDao.GetCountAsync(typeof(Supplier).Name);

                var dashboard = new DashboardDto
                {
                    ToatlPurchaseAmount = totalPurchaseSaleAmount.TotalPurchaseAmount,
                    ToatlPurchasePaidAmount = totalPurchaseSaleAmount.TotalPurchasePaidAmount,
                    ToatlPurchaseDueAmount = totalPurchaseSaleAmount.TotalPurchaseAmount - totalPurchaseSaleAmount.TotalPurchasePaidAmount,
                    TotalSaleAmount = totalPurchaseSaleAmount.TotalSaleAmount,
                    TotalSalePaidAmount = totalPurchaseSaleAmount.TotalSalePaidAmount,
                    TotalSaleDueAmount = totalPurchaseSaleAmount.TotalSaleAmount - totalPurchaseSaleAmount.TotalSalePaidAmount,
                    TotalActiveCustomer = totalCustomer.Active,
                    TotalInActiveCustomer = totalCustomer.Inactive,
                    TotalActiveSupplier = totalSupplier.Active,
                    TotalInActiveSupplier = totalSupplier.Inactive,
                };

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

            #region If Timservice provide Utc time
            //var startDate = DateTime.Parse(dates[0]).ToUniversalTime().ToString();
            //var endDate = DateTime.Parse(dates[1]).ToUniversalTime().ToString();
            #endregion

            var startDate = DateTime.Parse(dates[0]).ToString();
            var endDate = DateTime.Parse(dates[1]).ToString();

            var saleInfo = (await _reportDao.ExecuteQueryAsync<PaymentDetailsDto>($"SELECT " +
                $"COUNT(s.Id) AS Count, " +
                $"SUM(s.GrandTotalPrice) AS TotalAmount, " +
                $"SUM(pm.PaidAmount) AS PaidAmount, " +
                $"SUM(TotalAmount - PaidAmount) AS DueAmount" +
                $" FROM {typeof(Sale).Name} s " +
                $"JOIN {typeof(Payment).Name} pm ON pm.Id = s.PaymentId " +
                $"WHERE s.SaleDate BETWEEN '{startDate}' AND '{endDate}'")).FirstOrDefault();

            var purchaseInfo = (await _reportDao.ExecuteQueryAsync<PaymentDetailsDto>($"SELECT " +
                $"COUNT(p.Id) AS Count, " +
                $"SUM(p.GrandTotalPrice) AS TotalAmount, " +
                $"SUM(pm.PaidAmount) AS PaidAmount, " +
                $"SUM(TotalAmount - PaidAmount) AS DueAmount" +
                $" FROM {typeof(Purchase).Name} p " +
                $"JOIN {typeof(Payment).Name} pm ON pm.Id = p.PaymentId " +
                $"WHERE p.PurchaseDate BETWEEN '{startDate}' AND '{endDate}'")).FirstOrDefault();

            var saleProductList = await _reportDao.ExecuteQueryAsync<ProductListDto>($"SELECT " +
                $"s.SaleDate AS Date, " +
                $"sd.Quantity, " +
                $"sd.TotalPrice, " +
                $"p.Name " +
                $"FROM {typeof(Sale).Name} s " +
                $"JOIN {typeof(SaleDetails).Name} sd ON s.Id = sd.SaleId " +
                $"LEFT JOIN {typeof(Product).Name} p ON p.id = sd.ProductId " +
                $"WHERE s.SaleDate BETWEEN '{startDate}' AND '{endDate}'");

            var purchaseProductList = await _reportDao.ExecuteQueryAsync<ProductListDto>($"SELECT " +
                $"pr.PurchaseDate AS Date, " +
                $"prd.Quantity, " +
                $"prd.TotalPrice, " +
                $"p.Name " +
                $"FROM {typeof(Purchase).Name} pr " +
                $"JOIN {typeof(PurchaseDetails).Name} prd ON pr.Id = prd.PurchaseId " +
                $"LEFT JOIN {typeof(Product).Name} p ON p.id = prd.ProductId " +
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
        #endregion
    }
}
