using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public interface IProductDao : IBaseDao<Product, long>
    {

    }

    public class ProductDao : BaseDao<Product, long>, IProductDao
    {
        #region Initialization
        public ProductDao(ISession session) : base(session)
        {

        }
        #endregion
    }
}
