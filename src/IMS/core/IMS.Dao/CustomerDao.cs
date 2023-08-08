using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface ICustomerDao : IBaseDao<Customer, long>
    {
        Customer GetById(long id);
        IList<Customer> GetCustomers(Expression<Func<Customer, bool>> filter);
        (IList<Customer> data, int total, int totalDisplay) LoadAllCustomers(Expression<Func<Customer, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class CustomerDao : BaseDao<Customer, long>, ICustomerDao
    {
        public CustomerDao(ISession session) : base(session)
        {
        }
        public Customer GetById(long id)
        {
            return _session.Get<Customer>(id);
        }
        public (IList<Customer> data, int total, int totalDisplay) LoadAllCustomers(Expression<Func<Customer, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Customer> query = _session.Query<Customer>();

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

        public IList<Customer> GetCustomers(Expression<Func<Customer, bool>> filter)
        {
            IQueryable<Customer> query = _session.Query<Customer>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }
    }

}
