using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public interface ICategoryDao : IBaseDao<Category, long>
    {

    }

    public class CategoryDao : BaseDao<Category, long>, ICategoryDao
    {
        #region Initialization
        public CategoryDao(ISession session) : base(session)
        {

        }
        #endregion
    }
}
