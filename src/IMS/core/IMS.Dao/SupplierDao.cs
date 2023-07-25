using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface ISupplierDao : IBaseDao<Supplier, long>
    {
        Supplier GetById(long id);
        IList<Supplier> GetSuppliers(Expression<Func<Supplier, bool>> filter);
        (IList<Supplier> data, int total, int totalDisplay) LoadAllSuppliers(Expression<Func<Supplier, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class SupplierDao : BaseDao<Supplier, long>, ISupplierDao
    {
        public SupplierDao(ISession session) : base(session)
        {
        }

        public Supplier GetById(long id)
        {
            return _session.Get<Supplier>(id);
        }
        public IList<Supplier> GetSuppliers(Expression<Func<Supplier, bool>> filter)
        {
            IQueryable<Supplier> query = _session.Query<Supplier>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }

        public (IList<Supplier> data, int total, int totalDisplay) LoadAllSuppliers(Expression<Func<Supplier, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Supplier> query = _session.Query<Supplier>();

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
                case "Created By":
                    query = sortDir == "asc" ? query.OrderBy(c => c.CreateBy) : query.OrderByDescending(c => c.CreateBy);
                    break;                    
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
    }

}
