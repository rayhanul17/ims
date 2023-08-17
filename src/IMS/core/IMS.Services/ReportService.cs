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
        Task<BuyingSellingReportDto> GetBuyingSellingReport(BuyingSellingReportDto model);
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

            var startDate = DateTime.Parse(dates[0]).ToString();
            var endDate = DateTime.Parse(dates[1]).ToString();

            #region If Timservice provide Utc time
            //var startDate = DateTime.Parse(dates[0]).ToUniversalTime().ToString();
            //var endDate = DateTime.Parse(dates[1]).ToUniversalTime().ToString();
            #endregion

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

        public async Task<BuyingSellingReportDto> GetBuyingSellingReport(BuyingSellingReportDto model)
        {
            try
            {
                var parameterDictionary = new Dictionary<string, object>();
                var dates = model.DateRange.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                var startDate = DateTime.Parse(dates[0]).ToString();
                var endDate = DateTime.Parse(dates[1]).ToString();


                var saleSql = $"SELECT s.SaleDate AS Date, p.Name, sd.Quantity, sd.TotalPrice " +
                                    $"FROM {typeof(Sale).Name} s " +
                                        $"JOIN {typeof(Customer).Name} c ON c.Id = s.CustomerId " +
                                        $"JOIN {typeof(SaleDetails).Name} sd ON s.id = sd.SaleId " +
                                        $"JOIN {typeof(Product).Name} p ON sd.ProductId = p.Id " +
                                        $"WHERE s.SaleDate BETWEEN '{startDate}' AND '{endDate}' ";

                var purchaseSql = $"SELECT pur.PurchaseDate AS Date, p.Name, purd.Quantity, purd.TotalPrice " +
                                $"FROM {typeof(Purchase).Name} pur " +
                                    $"JOIN {typeof(Supplier).Name} s ON s.Id = pur.SupplierId " +
                                    $"JOIN {typeof(PurchaseDetails).Name} purd ON pur.id = purd.PurchaseId " +
                                    $"JOIN {typeof(Product).Name} p ON purd.ProductId = p.Id " +
                                    $"WHERE pur.PurchaseDate BETWEEN '{startDate}' AND '{endDate}' ";

                if (model.CustomerId != null)
                {
                    saleSql += $"AND c.Id = :customerId ";
                    parameterDictionary.Add("customerId", model.CustomerId);
                }
                if (model.SupplierId != null)
                {
                    purchaseSql += $"AND s.Id = :supplierId ";                    
                }
                if (model.ProductId == null)
                {
                    string sq = $"SELECT p.Id WHERE ";
                    if(model.CategoryId != null)
                    {
                        sq += $"CategoryId = :categoryId AND ";
                        parameterDictionary.Add("categoryId", model.CategoryId);
                    }
                    else
                    {
                        sq += "1=1 AND ";
                    }
                    if(model.BrandId != null)
                    {
                        sq += $"BrandId = :brandId";
                        parameterDictionary.Add("brandId", model.BrandId);
                    }
                    else
                    {
                        sq += "1=1";
                    }
                    saleSql += $"AND p.Id IN ({sq})";
                    purchaseSql += $"AND p.Id IN ({sq})";
                }
                else
                {
                    saleSql += $"AND p.Id = :productId";
                    purchaseSql += $"AND p.Id = :productId";
                    parameterDictionary.Add("productId", model.ProductId);
                }

                var saleProductList = await _reportDao.ExecuteParametrizedQueryAsync<ProductListDto>(saleSql, parameterDictionary);
                
                parameterDictionary.Remove("customerId");
                if(model.SupplierId != null)
                {
                    parameterDictionary.Add("supplierId", model.SupplierId);
                }

                var purchaseProductList = await _reportDao.ExecuteParametrizedQueryAsync<ProductListDto>(purchaseSql, parameterDictionary);

                var purchaseAmount = purchaseProductList.Sum(x => x.TotalPrice);
                var saleAmount = saleProductList.Sum(x => x.TotalPrice);
                var buyingSellingReportDto = new BuyingSellingReportDto
                {
                    SaleAmount = saleAmount,
                    PuchaseAmount = purchaseAmount,
                    SaleProductList = saleProductList,
                    PurchaseProductList = purchaseProductList,
                };

                return buyingSellingReportDto;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }            
        }
        #endregion
    }
}
