using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface IApplicationUserDao : IBaseDao<ApplicationUser, long>
    {
        ApplicationUser GetUserById(long id);
        ApplicationUser GetUser(Expression<Func<ApplicationUser, bool>> filter);
        List<ApplicationUser> GetUsers(Expression<Func<ApplicationUser, bool>> filter);
        (IList<ApplicationUser> data, int total, int totalDisplay) LoadAllUsers(Expression<Func<ApplicationUser, bool>> filter = null,
            string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
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

        public (IList<ApplicationUser> data, int total, int totalDisplay) LoadAllUsers(Expression<Func<ApplicationUser, bool>> filter = null,
            string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<ApplicationUser> query = _session.Query<ApplicationUser>();

            query = query.Where(x => x.Status != (int)Status.Delete);

            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            switch (sortBy)
            {
                case "Name":
                    query = sortDir == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
                    break;
                case "Created Date":
                    query = sortDir == "asc" ? query.OrderBy(c => c.CreationDate) : query.OrderByDescending(c => c.CreationDate);
                    break;
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
    }

}
