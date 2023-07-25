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
    #region Interface
    public interface IPurchaseService
    {
        Task AddAsync(IList<PurchaseDetailsModel> model, decimal grandTotal, long supplierId, long userId);

        (int total, int totalDisplay, IList<PurchaseDto> records) LoadAllPurchases(string searchBy, int length, int start, string sortBy, string sortDir);
    }
    #endregion

    public class PurchaseService : BaseService, IPurchaseService
    {
        #region Initializtion
        private readonly IPurchaseDao _purchaseDao;
        private readonly IProductDao _productDao;

        public PurchaseService(ISession session) : base(session)
        {
            _purchaseDao = new PurchaseDao(session);
            _productDao = new ProductDao(session);
        }
        #endregion

        #region Operational Function
        public async Task AddAsync(IList<PurchaseDetailsModel> model, decimal grandTotal, long supplierId, long userId)
        {
            using (var transaction = _session.BeginTransaction())
            {
                try
                {
                    List<PurchaseDetails> purchaseDetails = new List<PurchaseDetails> ();
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
                        PurchaseDetails = purchaseDetails
                    };

                    await _purchaseDao.AddAsync(Purchase);
                    transaction.Commit();
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
        #endregion

        #region List Loading Function
        public (int total, int totalDisplay, IList<PurchaseDto> records) LoadAllPurchases(string searchBy = null, int length = 10, int start = 1, string sortBy = null, string sortDir = null)
        {
            try
            {
                Expression<Func<Purchase, bool>> filter = null;
                //if (searchBy != null)
                //{
                //    filter = x => x.GrandTotalPrice.Equals(searchBy);
                //}

                var result = _purchaseDao.LoadAllPurchases(filter, null, start, length, sortBy, sortDir);

                List<PurchaseDto> categories = new List<PurchaseDto>();
                foreach (Purchase purchase in result.data)
                {
                    categories.Add(
                        new PurchaseDto
                        {
                            Id = purchase.Id,
                            SupplierId = purchase.SupplierId,
                            CreateBy = purchase.CreateBy,
                            PurchaseDate = purchase.PurchaseDate,
                            GrandTotalPrice = purchase.GrandTotalPrice,
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
