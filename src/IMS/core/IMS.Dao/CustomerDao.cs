using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public interface ICustomerDao : IBaseDao<Customer, long>
    {

    }

    public class CustomerDao : BaseDao<Customer, long>, ICustomerDao
    {
        #region Initialization
        public CustomerDao(ISession session) : base(session)
        {

        }
        #endregion     
    }
}
