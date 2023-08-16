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
    public interface IPurchaseService
    {
        #region Opperational Function
        Task AddAsync(IList<PurchaseDetailsViewModel> model, decimal grandTotal, long supplierId, long userId);
        #endregion

        #region Single Instance Loading Function
        Task<PurchaseReportDto> GetPurchaseDetailsAsync(long purchaseId);
        #endregion

        #region List Loading Function
        Task<(int total, int totalDisplay, IList<PurchaseDto> records)> LoadAllPurchases(string searchBy, int length, int start, string sortBy, string sortDir);
        #endregion
    }

    public class PurchaseService : BaseService, IPurchaseService
    {
        #region Initializtion
        private readonly IPurchaseDao _purchaseDao;
        private readonly IProductDao _productDao;
        private readonly ISupplierDao _supplierDao;
        private readonly IPaymentDao _paymentDao;
       
        public PurchaseService(ISession session) : base(session)
        {
            _purchaseDao = new PurchaseDao(session);
            _productDao = new ProductDao(session);
            _supplierDao = new SupplierDao(session);
            _paymentDao = new PaymentDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(IList<PurchaseDetailsViewModel> model, decimal grandTotal, long supplierId, long userId)
        {            
            try
            {
                if (model == null || model.Count < 1 || grandTotal < 0 || supplierId < 1 || userId < 1)
                {
                    throw new CustomException("Invalid purchase request");
                }

                List<PurchaseDetails> purchaseDetails = new List<PurchaseDetails>();
                List<Product> productList = new List<Product>();
                foreach (var item in model)
                {
                    var product = await _productDao.GetByIdAsync(item.ProductId);
                    product.InStockQuantity += item.Quantity;
                    product.BuyingPrice = item.UnitPrice;
                    product.SellingPrice = item.UnitPrice + (item.UnitPrice * product.ProfitMargin / 100) - product.DiscountPrice;

                    productList.Add(product);

                    purchaseDetails.Add(
                        new PurchaseDetails
                        {
                            ProductId = item.ProductId,
                            Quantity = item.Quantity,
                            TotalPrice = item.Total,
                            CreateBy = userId,
                            CreationDate = _timeService.Now,
                            Status = (int)Status.Active
                        }
                    );
                }

                var purchase = new Purchase()
                {
                    SupplierId = supplierId,
                    PurchaseDate = _timeService.Now,
                    GrandTotalPrice = grandTotal,
                    PurchaseDetails = purchaseDetails,
                    CreateBy = userId,
                    CreationDate = _timeService.Now,
                    Rank = await _purchaseDao.GetMaxRank() + 1,
                    Status = (int)Status.Active                     
                };
                purchase.VoucherId = "#P" + purchase.Rank + DateTime.UtcNow.ToString("ddMMyyyy");

                var payment = new Payment
                {
                    VoucherId = purchase.VoucherId,
                    OperationType = (int)OperationType.Purchase,
                    TotalAmount = purchase.GrandTotalPrice,
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

                        var purchaseId = await _purchaseDao.AddAsync(purchase);
                        payment.PurchaseId = purchaseId;

                        var paymentId = await _paymentDao.AddAsync(payment);

                        await _paymentDao.SetPaymentIdToPurchaseTable(paymentId, purchaseId);

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
                throw;
            }            
        }
        #endregion

        #region Single Instance Loading
        public async Task<PurchaseReportDto> GetPurchaseDetailsAsync(long id)
        {
            try
            {
                if(id == 0)
                {
                    throw new CustomException("Invalid id");
                }
                var purchase = await _purchaseDao.GetByIdAsync(id);
                var supplier = await _supplierDao.GetByIdAsync(purchase.SupplierId);
                var purchaseProducts = new List<ProductInformation>();

                foreach (var item in purchase.PurchaseDetails)
                {
                    var product = await _productDao.GetByIdAsync(item.ProductId);
                    var unitPrice = item.TotalPrice / item.Quantity;
                    purchaseProducts.Add(new ProductInformation
                    {
                        ProductName = product.Name,
                        Description = product.Description,
                        UnitPrice = unitPrice,
                        Quantity = item.Quantity,
                        TotalPrice = item.TotalPrice                        
                    });
                }
                var purchaseReport = new PurchaseReportDto
                {
                    SupplierName = supplier.Name,
                    SupplierDescription = supplier.Address,
                    PurchaseDate = purchase.PurchaseDate,
                    GrandTotalPrice = purchase.GrandTotalPrice,
                    Products = purchaseProducts,
                    VoucherId = purchase.VoucherId,
                };

                return purchaseReport;
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

        #region List Loading Function
        public async Task<(int total, int totalDisplay, IList<PurchaseDto> records)> LoadAllPurchases(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Purchase, bool>> filter = null;

                if (!string.IsNullOrWhiteSpace(searchBy))
                {
                    searchBy = searchBy.Trim();
                    filter = x => x.VoucherId.Contains(searchBy);
                }

                var result = _purchaseDao.LoadAllPurchases(filter, null, start, length, sortBy, sortDir);

                List<PurchaseDto> categories = new List<PurchaseDto>();
                foreach (Purchase purchase in result.data)
                {
                    categories.Add(
                        new PurchaseDto
                        {
                            Id = purchase.Id.ToString(),
                            SupplierName = (await _supplierDao.GetByIdAsync(purchase.SupplierId)).Name,
                            CreateBy = await _userService.GetUserNameAsync(purchase.CreateBy),
                            PurchaseDate = purchase.PurchaseDate.ToString(),
                            GrandTotalPrice = purchase.GrandTotalPrice.ToString(),
                            IsPaid = purchase.IsPaid.ToString(),
                            PaymentId = purchase.PaymentId.ToString(),
                            Rank = purchase.Rank.ToString(),
                            VoucherId = purchase.VoucherId
                        });
                }

                return (result.total, result.totalDisplay, categories);
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
