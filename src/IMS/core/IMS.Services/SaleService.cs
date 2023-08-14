using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    public interface ISaleService
    {
        #region Opperational Function
        Task AddAsync(IList<SaleDetailsViewModel> model, decimal grandTotal, long CustomerId, long userId);
        #endregion

        #region Single Instance Loading Function
        Task<SaleReportDto> GetSaleDetailsAsync(long SaleId);
        #endregion

        #region List Loading Function
        Task<(int total, int totalDisplay, IList<SaleDto> records)> LoadAllSales(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

    public class SaleService : BaseService, ISaleService
    {
        #region Initializtion
        private readonly ISaleDao _saleDao;
        private readonly IProductDao _productDao;
        private readonly ICustomerDao _customerDao;
        private readonly IPaymentDao _paymentDao;

        public SaleService(ISession session) : base(session)
        {
            _saleDao = new SaleDao(session);
            _productDao = new ProductDao(session);
            _customerDao = new CustomerDao(session);
            _paymentDao = new PaymentDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(IList<SaleDetailsViewModel> model, decimal grandTotal, long CustomerId, long userId)
        {
            try
            {
                List<SaleDetails> saleDetails = new List<SaleDetails>();
                List<Product> productList = new List<Product>();

                foreach (var item in model)
                {
                    var product = await _productDao.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        throw new CustomException("No Product found with id");
                    }
                    else if (product.InStockQuantity < item.Quantity)
                    {
                        throw new CustomException("Selling quantity more than stock quantity");
                    }
                    product.InStockQuantity -= item.Quantity;

                    productList.Add(product);

                    saleDetails.Add(
                        new SaleDetails
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            TotalPrice = product.SellingPrice * item.Quantity,
                            Status = (int)Status.Active,
                            Rank = await _saleDao.GetMaxRank(typeof(SaleDetails).Name) + 1,
                            CreateBy = userId,
                            CreationDate = _timeService.Now,
                        }
                    );
                }

                var sale = new Sale()
                {
                    CustomerId = CustomerId,
                    CreateBy = userId,
                    SaleDate = _timeService.Now,
                    GrandTotalPrice = grandTotal,
                    SaleDetails = saleDetails,
                    CreationDate = _timeService.Now,
                    Rank = await _saleDao.GetMaxRank() + 1,
                    Status = (int)Status.Active
                };
                sale.VoucherId = "#S" + sale.Rank + DateTime.UtcNow.ToString("ddMMyyyy");

                var payment = new Payment
                {
                    VoucherId = sale.VoucherId,
                    OperationType = (int)OperationType.Sale,
                    TotalAmount = sale.GrandTotalPrice,
                    PaidAmount = 0,
                    CreateBy = userId,
                    CreationDate = _timeService.Now,
                    Rank = await _paymentDao.GetMaxRank() + 1,
                    Status = (int)Status.Active
                };
                using (var transaction = _session.BeginTransaction())
                {
                    try
                    {
                        foreach (var product in productList)
                        {
                            await _productDao.EditAsync(product);
                        }
                        var saleId = await _saleDao.AddAsync(sale);
                        
                        payment.SaleId = saleId;
                        
                        var paymentId = await _paymentDao.AddAsync(payment);

                        await _paymentDao.SetPaymentIdToSaleTable(paymentId, saleId);

                        transaction.Commit();                       
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw ex;
                    }
                }
            }
            catch (CustomException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw ex;
            }
        } 
        #endregion

        #region Single Instance Loading Function

        public async Task<SaleReportDto> GetSaleDetailsAsync(long id)
        {
            var sale = await _saleDao.GetByIdAsync(id);
            var customer = await _customerDao.GetByIdAsync(sale.CustomerId);
            var saleProducts = new List<ProductInformation>();

            foreach (var item in sale.SaleDetails)
            {
                var product = await _productDao.GetByIdAsync(item.ProductId);
                var unitPrice = item.TotalPrice / item.Quantity;
                saleProducts.Add(new ProductInformation
                {
                    ProductName = product.Name,
                    Description = product.Description,
                    UnitPrice = Math.Round(unitPrice, 2),
                    Quantity = item.Quantity,
                    TotalPrice = Math.Round(item.TotalPrice, 2)
                });
            }
            var saleReport = new SaleReportDto
            {
                CustomerName = customer.Name,
                CustomerDescription = customer.Address,
                SaleDate = sale.SaleDate,
                GrandTotalPrice = Math.Round(sale.GrandTotalPrice, 2),
                Products = saleProducts,
                PaymentId = sale.PaymentId,
                SaleId = sale.Id
            };

            return saleReport;
        }

        #endregion

        #region List Loading Function
        public async Task<(int total, int totalDisplay, IList<SaleDto> records)> LoadAllSales(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Sale, bool>> filter = null;

                if (!string.IsNullOrWhiteSpace(searchBy))
                {
                    searchBy = searchBy.Trim();
                    filter = x => x.VoucherId.Contains(searchBy);
                }
                var result = _saleDao.LoadAllSales(filter, null, start, length, sortBy, sortDir);

                List<SaleDto> categories = new List<SaleDto>();
                foreach (Sale sale in result.data)
                {
                    categories.Add(
                        new SaleDto
                        {
                            Id = sale.Id.ToString(),
                            CustomerName = (await _customerDao.GetByIdAsync(sale.CustomerId)).Name,
                            CreateBy = await _userService.GetUserNameAsync(sale.CreateBy),
                            SaleDate = sale.SaleDate.ToString(),
                            GrandTotalPrice = sale.GrandTotalPrice.ToString(),
                            IsPaid = sale.IsPaid.ToString(),
                            PaymentId = sale.PaymentId.ToString(),
                            Rank = sale.Rank.ToString(),
                            VoucherId = sale.VoucherId
                        });
                }

                return (result.total, result.totalDisplay, categories);
            }
            catch (Exception ex)
            {
                _serviceLogger.Error(ex.Message, ex);
                throw;
            }
        }
        #endregion

    }
}
