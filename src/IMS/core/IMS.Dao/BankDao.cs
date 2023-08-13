using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public interface IBankDao : IBaseDao<Bank, long>
    {

    }

    public class BankDao : BaseDao<Bank, long>, IBankDao
    {
        #region Initialization
        public BankDao(ISession session) : base(session)
        {

        }
        #endregion
    }
}
