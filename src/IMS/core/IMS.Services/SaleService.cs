using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface ISaleService
    {
        Task<long> AddAsync(IList<SaleDetailsModel> model, decimal grandTotal, long CustomerId, long userId);

        (int total, int totalDisplay, IList<SaleDto> records) LoadAllSales(string searchBy, int length, int start, string sortBy, string sortDir);
        Task<SaleReportDto> GetSaleDetailsAsync(long SaleId);
    }
    #endregion

    public class SaleService : BaseService, ISaleService
    {
        #region Initializtion
        private readonly ISaleDao _saleDao;
        private readonly IProductDao _productDao;
        private readonly ICustomerDao _customerDao;

        public SaleService(ISession session) : base(session)
        {
            _saleDao = new SaleDao(session);
            _productDao = new ProductDao(session);
            _customerDao = new CustomerDao(session);
        }
        #endregion

        #region Operational Function
        public async Task<long> AddAsync(IList<SaleDetailsModel> model, decimal grandTotal, long CustomerId, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    List<SaleDetails> saleDetails = new List<SaleDetails>();
                    foreach (var item in model)
                    {
                        var product = await _productDao.GetByIdAsync(item.ProductId);
                        product.InStockQuantity -= item.Quantity;
                        product.ModifyBy = userId;
                        product.ModificationDate = _timeService.Now;
                        product.BuyingPrice = item.UnitPrice;
                        product.SellingPrice = item.UnitPrice + (item.UnitPrice * product.ProfitMargin / 100) - product.DiscountPrice;

                        await _productDao.EditAsync(product);

                        saleDetails.Add(
                            new SaleDetails
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                TotalPrice = item.Total,
                            }
                        );
                    }

                    var sale = new Sale()
                    {
                        CustomerId = CustomerId,
                        CreateBy = userId,
                        SaleDate = _timeService.Now,
                        GrandTotalPrice = grandTotal,
                        SaleDetails = saleDetails
                    };

                    var id = await _saleDao.AddAsync(sale);
                    transaction.Commit();
                    return id;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    _serviceLogger.Error(ex.Message, ex);

                    throw;
                }
            }
        }

        #endregion

        #region Single Instance Loading

        public async Task<SaleReportDto> GetSaleDetailsAsync(long id)
        {
            var sale = await _saleDao.GetByIdAsync(id);
            var customer = _customerDao.GetById(sale.CustomerId);
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
        public (int total, int totalDisplay, IList<SaleDto> records) LoadAllSales(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Sale, bool>> filter = null;
                //if (searchBy != null)
                //{
                //    filter = x => x.GrandTotalPrice.Equals(searchBy);
                //}

                var result = _saleDao.LoadAllSales(filter, null, start, length, sortBy, sortDir);

                List<SaleDto> categories = new List<SaleDto>();
                foreach (Sale Sale in result.data)
                {
                    categories.Add(
                        new SaleDto
                        {
                            Id = Sale.Id,
                            CustomerId = Sale.CustomerId,
                            CreateBy = Sale.CreateBy,
                            SaleDate = Sale.SaleDate,
                            GrandTotalPrice = Math.Round(Sale.GrandTotalPrice, 2),
                            IsPaid = Sale.IsPaid,
                            PaymentId=Sale.PaymentId,
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
