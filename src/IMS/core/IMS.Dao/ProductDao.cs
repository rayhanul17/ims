﻿using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IMS.Dao
{
    public interface IProductDao : IBaseDao<Product, long>
    {
        (IList<Product> data, int total, int totalDisplay) LoadAllProducts(Expression<Func<Product, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }

    public class ProductDao : BaseDao<Product, long>, IProductDao
    {
        public ProductDao(ISession session) : base(session)
        {

        }

        public (IList<Product> data, int total, int totalDisplay) LoadAllProducts(Expression<Func<Product, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<Product> query = _session.Query<Product>();

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
                case "Category":
                    query = sortDir == "asc" ? query.OrderBy(c => c.Category) : query.OrderByDescending(c => c.Category);
                    break;
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
    }

}