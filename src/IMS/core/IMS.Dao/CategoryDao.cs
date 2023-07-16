using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public class CategoryDao : BaseDao<Category, long>, ICategoryDao
    {
        public CategoryDao(ISession session) : base(session)
        {

        }
    }

    public interface ICategoryDao : IBaseDao<Category, long>
    {
    }
}
