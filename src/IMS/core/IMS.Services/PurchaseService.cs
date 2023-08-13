using IMS.BusinessModel.Dto;
using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.Dao;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IPurchaseService
    {
        Task<long> AddAsync(IList<PurchaseDetailsViewModel> model, decimal grandTotal, long supplierId, long userId);

        Task<(int total, int totalDisplay, IList<PurchaseDto> records)> LoadAllPurchases(string searchBy, int length, int start, string sortBy, string sortDir);
        Task<PurchaseReportDto> GetPurchaseDetailsAsync(long purchaseId);
    }
    #endregion

    public class PurchaseService : BaseService, IPurchaseService
    {
        #region Initializtion
        private readonly IPurchaseDao _purchaseDao;
        private readonly IProductDao _productDao;
        private readonly ISupplierDao _supplierDao;
        private readonly ISupplierService _supplierService;

        public PurchaseService(ISession session) : base(session)
        {
            _purchaseDao = new PurchaseDao(session);
            _productDao = new ProductDao(session);
            _supplierDao = new SupplierDao(session);
            _supplierService = new SupplierService(session);
        }
        #endregion

        #region Operational Function
        public async Task<long> AddAsync(IList<PurchaseDetailsViewModel> model, decimal grandTotal, long supplierId, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    List<PurchaseDetails> purchaseDetails = new List<PurchaseDetails>();
                    foreach (var item in model)
                    {
                        var product = await _productDao.GetByIdAsync(item.ProductId);
                        product.InStockQuantity += item.Quantity;
                        product.ModifyBy = userId;
                        product.ModificationDate = _timeService.Now;
                        product.BuyingPrice = item.UnitPrice;
                        product.SellingPrice = item.UnitPrice + (item.UnitPrice * product.ProfitMargin / 100) - product.DiscountPrice;

                        await _productDao.EditAsync(product);

                        purchaseDetails.Add(
                            new PurchaseDetails
                            {
                                ProductId = item.ProductId,
                                Quantity = item.Quantity,
                                TotalPrice = item.Total,
                            }
                        );
                    }

                    var Purchase = new Purchase()
                    {
                        SupplierId = supplierId,
                        CreateBy = userId,
                        PurchaseDate = _timeService.Now,
                        GrandTotalPrice = grandTotal,
                        PurchaseDetails = purchaseDetails,
                        CreationDate = _timeService.Now,
                        Rank = await _purchaseDao.GetMaxRank(typeof(Purchase).Name) + 1,
                        Status = (int)Status.Active
                    };

                    var id = await _purchaseDao.AddAsync(Purchase);
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

        public async Task<PurchaseReportDto> GetPurchaseDetailsAsync(long id)
        {
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
                    UnitPrice = Math.Round(unitPrice, 2),
                    Quantity = item.Quantity,
                    TotalPrice = Math.Round(item.TotalPrice, 2)
                });
            }
            var purchaseReport = new PurchaseReportDto
            {
                SupplierName = supplier.Name,
                SupplierDescription = supplier.Address,
                PurchaseDate = purchase.PurchaseDate,
                GrandTotalPrice = Math.Round(purchase.GrandTotalPrice, 2),
                Products = purchaseProducts,
                PaymentId = purchase.PaymentId,
                PurchaseId = purchase.Id
            };

            return purchaseReport;
        }
        #endregion

        #region Single Instance Loading
        #endregion     

        #region List Loading Function
        public async Task<(int total, int totalDisplay, IList<PurchaseDto> records)> LoadAllPurchases(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Purchase, bool>> filter = null;
                
                var result = _purchaseDao.LoadAllPurchases(filter, null, start, length, sortBy, sortDir);

                List<PurchaseDto> categories = new List<PurchaseDto>();
                foreach (Purchase purchase in result.data)
                {
                    categories.Add(
                        new PurchaseDto
                        {
                            Id = purchase.Id.ToString(),
                            SupplierName = await _supplierService.GetNameByIdAsync(purchase.SupplierId),
                            CreateBy = await _userService.GetUserNameAsync(purchase.CreateBy),
                            PurchaseDate = purchase.PurchaseDate.ToString(),
                            GrandTotalPrice = purchase.GrandTotalPrice.ToString(),
                            IsPaid = purchase.IsPaid.ToString(),
                            PaymentId = purchase.PaymentId.ToString(),
                            Rank = purchase.Rank.ToString(),
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
