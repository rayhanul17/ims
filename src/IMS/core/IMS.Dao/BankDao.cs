using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface IBankDao : IBaseDao<Bank, long>
    {
        IList<Bank> GetBank(Expression<Func<Bank, bool>> filter);
        (IList<Bank> data, int total, int totalDisplay) LoadAllBanks(Expression<Func<Bank, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class BankDao : BaseDao<Bank, long>, IBankDao
    {
        public BankDao(ISession session) : base(session)
        {

        }

        public (IList<Bank> data, int total, int totalDisplay) LoadAllBanks(Expression<Func<Bank, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Bank> query = _session.Query<Bank>();

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
                case "Description":
                    query = sortDir == "asc" ? query.OrderBy(c => c.Description) : query.OrderByDescending(c => c.Description);
                    break;

            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }

        public IList<Bank> GetBank(Expression<Func<Bank, bool>> filter)
        {
            IQueryable<Bank> query = _session.Query<Bank>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }

    }

}
