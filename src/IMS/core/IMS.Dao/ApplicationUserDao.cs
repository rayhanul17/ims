using FluentNHibernate.Data;
using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace IMS.Dao
{
    public interface IApplicationUserDao : IBaseDao<ApplicationUser, long>
    {
        ApplicationUser GetUserById(long id);
        ApplicationUser GetUser(Expression<Func<ApplicationUser, bool>> filter);
        List<ApplicationUser> GetUsers(Expression<Func<ApplicationUser, bool>> filter);
    }

    public class ApplicationUserDao : BaseDao<ApplicationUser, long>, IApplicationUserDao
    {
        public ApplicationUserDao(ISession session) : base(session)
        {

        }
        public ApplicationUser GetUserById(long id)
        {
            return _session.Get<ApplicationUser>(id);
        }

        public List<ApplicationUser> GetUsers(Expression<Func<ApplicationUser, bool>> filter)
        {
            IQueryable<ApplicationUser> query = _session.Query<ApplicationUser>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }

        public ApplicationUser GetUser(Expression<Func<ApplicationUser, bool>> filter)
        {
            IQueryable<ApplicationUser> query = _session.Query<ApplicationUser>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.FirstOrDefault();
        }
    }

}
