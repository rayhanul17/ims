using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public interface IBrandDao : IBaseDao<Brand, long>
    {

    }

    public class BrandDao : BaseDao<Brand, long>, IBrandDao
    {
        #region Initialization
        public BrandDao(ISession session) : base(session)
        {

        }
        #endregion
    }
}
