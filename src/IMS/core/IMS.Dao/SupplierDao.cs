using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public interface ISupplierDao : IBaseDao<Supplier, long>
    {

    }

    public class SupplierDao : BaseDao<Supplier, long>, ISupplierDao
    {
        #region Initialization
        public SupplierDao(ISession session) : base(session)
        {

        }
        #endregion
    }
}
