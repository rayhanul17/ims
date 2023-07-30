using IMS.BusinessModel.Entity;
using NHibernate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using System.Runtime.Remoting.Contexts;
using IMS.BusinessRules.Enum;
using System.Collections;

namespace IMS.Dao
{
    #region Interfaces
    public interface IBaseDao<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        Task<TKey> AddAsync(TEntity entity);
        Task RemoveByIdAsync(TKey id);
        Task RemoveAsync(TEntity entityToDelete);
        Task EditAsync(TEntity entityToUpdate);
        Task<TEntity> GetByIdAsync(TKey id);
        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter);
        IList<TEntity> GetAll();
        int GetCount(Expression<Func<TEntity, bool>> filter = null);
        Task<int> ExecuteUpdateDeleteQuery(string query);
        (IList<TEntity> data, int total, int totalDisplay) GetDynamic(
            Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null, int pageIndex = 1, int pageSize = 10);

        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter, int pageIndex = 1,
            int pageSize = 10);

        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 1, int pageSize = 10);

        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        IList<TEntity> GetDynamic(Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null);

        (IList<TEntity> data, int total, int totalDisplay) LoadAll(Expression<Func<TEntity, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }
    #endregion

    #region Implementations
    public abstract class BaseDao<TEntity, TKey>
        : IBaseDao<TEntity, TKey>
        where TEntity : class, IEntity<TKey>
    {
        protected ISession _session;

        public BaseDao(ISession session)
        {
            _session = session;
        }

        public virtual async Task<TKey> AddAsync(TEntity entity)
        {
            await _session.SaveAsync(entity);
            return entity.Id;
        }

        public virtual async Task<TEntity> GetByIdAsync(TKey id)
        {
            return await _session.GetAsync<TEntity>(id);
        }

        public virtual IList<TEntity> GetAll()
        {
            return _session.Query<TEntity>().ToList();
        }


        public virtual async Task RemoveByIdAsync(TKey id)
        {
            var entityToDelete = await _session.GetAsync<TEntity>(id);
            await RemoveAsync(entityToDelete);
        }

        public virtual async Task RemoveAsync(TEntity entityToDelete)
        {
            await _session.DeleteAsync(entityToDelete);
        }

        public virtual async Task EditAsync(TEntity entityToUpdate)
        {
            await _session.SaveOrUpdateAsync(entityToUpdate);
        }

        public virtual int GetCount(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();
            var count = 0;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            count = query.Count();
            return count;
        }

        public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
        }

        public async Task<int> ExecuteUpdateDeleteQuery(string query)
        {
            var sql = _session.CreateSQLQuery(query);
            int resut = await sql.ExecuteUpdateAsync();

            return resut;
        }
        public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>> filter,
            int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            var result = query.OrderByDescending(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return result;
        }

        public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>> filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                return orderBy(query).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
            else
            {
                return query.OrderByDescending(s => s.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            }
        }

        public virtual (IList<TEntity> data, int total, int totalDisplay) GetDynamic(
            Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null, int pageIndex = 1, int pageSize = 10)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();
            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy).Skip((pageIndex - 1) * pageSize).Take(pageSize);
                return (result.ToList(), total, totalDisplay);
            }
            else
            {
                var result = query.Skip((pageIndex - 1) * pageSize).Take(pageSize);
                return (result.ToList(), total, totalDisplay);
            }
        }

        public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                var result = orderBy(query);
                return result.ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        public virtual IList<TEntity> GetDynamic(Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                var result = query.OrderBy(orderBy);
                return result.ToList();
            }
            else
            {
                return query.ToList();
            }
        }

        //For BaseEntity
        public (IList<TEntity> data, int total, int totalDisplay) LoadAll(Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();        
            
            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }            

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
    }
#endregion

}
