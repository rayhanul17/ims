using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Threading.Tasks;

namespace IMS.Dao
{
    public interface IProductDao : IBaseDao<Product, long>
    {
        #region Opperational Function
        Task<bool> IsProductInPurchaseOrSale(long productId);
        #endregion
    }

    public class ProductDao : BaseDao<Product, long>, IProductDao
    {
        #region Initialization
        public ProductDao(ISession session) : base(session)
        {

        }
        #endregion

        #region Opperational Function
        public async Task<bool> IsProductInPurchaseOrSale(long productId)
        {
            var query = $"SELECT " +
                $"(SELECT COUNT(1) FROM {typeof(SaleDetails).Name} " +
                    $"WHERE ProductId = {productId}) " +
                $"+ " +
                $"(SELECT COUNT(1) FROM {typeof(PurchaseDetails).Name} " +
                    $"WHERE ProductId = {productId})";

            var count = Convert.ToInt32(await GetScallerValueAsync(query));
            bool result = count > 0;

            return result;
        }
        #endregion
    }
}
