﻿using IMS.BusinessModel.Entity;
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
        (IList<Customer> data, int total, int totalDisplay) LoadAllCustomers(Expression<Func<Customer, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class CustomerDao : BaseDao<Customer, long>, ICustomerDao
    {
        public CustomerDao(ISession session) : base(session)
        {
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
                case "Created By":
                    query = sortDir == "asc" ? query.OrderBy(c => c.CreateBy) : query.OrderByDescending(c => c.CreateBy);
                    break;                    
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
    }

}