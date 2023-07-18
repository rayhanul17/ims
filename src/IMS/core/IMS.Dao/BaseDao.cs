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

namespace IMS.Dao
{
    #region Interfaces
    public interface IBaseDao<TEntity, TKey> where TEntity : class, IEntity<TKey>
    {
        Task AddAsync(TEntity entity);
        Task RemoveByIdAsync(TKey id);
        Task RemoveAsync(TEntity entityToDelete);
        Task EditAsync(TEntity entityToUpdate);
        Task<TEntity> GetByIdAsync(TKey id);
        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter);
        IList<TEntity> GetAll();
        int GetCount(Expression<Func<TEntity, bool>> filter = null);
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

        //For BaseEntity
        (IList<Entity> data, int total, int totalDisplay) LoadAll<Entity>(Expression<Func<Entity, bool>> filter = null,
           string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null) where Entity : BaseEntity<TKey>;
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

        public virtual async Task AddAsync(TEntity entity)
        {
            await _session.SaveAsync(entity);
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
        public (IList<Entity> data, int total, int totalDisplay) LoadAll<Entity>(Expression<Func<Entity, bool>> filter = null,
            string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null) where Entity: BaseEntity<TKey>
        {
            IQueryable<Entity> query = _session.Query<Entity>();

            query = query.Where(x => x.Status != (int)Status.Delete);
            

            var total = query.Count();
            var totalDisplay = query.Count();

            if (filter != null)
            {
                query = query.Where(filter);
                totalDisplay = query.Count();
            }

            //sorting
            switch (sortBy)
            {
                case "Name":
                    query = sortDir == "asc" ? query.OrderBy(c => c.Name) : query.OrderByDescending(c => c.Name);
                    break;
                case "Created By":
                    query = sortDir == "asc" ? query.OrderBy(c => c.CreateBy) : query.OrderByDescending(c => c.CreateBy);
                    break;
                case "Modified By":
                    query = sortDir == "asc" ? query.OrderBy(c => c.ModifyBy) : query.OrderByDescending(c => c.ModifyBy);
                    break;
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
    }
#endregion

}
