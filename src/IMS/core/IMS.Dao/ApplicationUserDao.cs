using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
