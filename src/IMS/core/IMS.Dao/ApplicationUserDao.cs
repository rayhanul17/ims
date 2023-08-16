using IMS.BusinessModel.Entity;
using NHibernate;

namespace IMS.Dao
{
    public interface IApplicationUserDao : IBaseDao<ApplicationUser, long>
    {

    }

    public class ApplicationUserDao : BaseDao<ApplicationUser, long>, IApplicationUserDao
    {
        public ApplicationUserDao(ISession session) : base(session)
        {

        }
    }
}
