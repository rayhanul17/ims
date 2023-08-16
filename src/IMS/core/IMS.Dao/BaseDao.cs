using IMS.BusinessModel.Entity;
using IMS.BusinessRules.Enum;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IMS.Dao
{
    #region Interfaces
    public interface IBaseDao<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        Task<TKey> AddAsync(TEntity entity);
        Task RemoveByIdAsync(TKey id);
        Task RemoveAsync(TEntity entityToDelete);
        Task EditAsync(TEntity entityToUpdate);
        Task<TEntity> GetByIdAsync(TKey id);
        Task<long> GetMaxRank();
        Task<long> GetMaxRank(string tableName);
        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter);
        IList<TEntity> LoadAll();
        int GetCount(Expression<Func<TEntity, bool>> filter = null);
        Task<int> ExecuteUpdateDeleteQuery(string query);
        Task<T> GetRankByIdAsync<T>(long id, string tableName);
        Task<long> RankUpAsync(long rankFrom, long rankTo, string tableName);
        Task<long> RankDownAsync(long rankFrom, long rankTo, string tableName);

        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter, int pageIndex = 1,
            int pageSize = 10);

        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter, Func<IQueryable<TEntity>,
                IOrderedQueryable<TEntity>> orderBy = null, int pageIndex = 1, int pageSize = 10);

        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null);

        IList<TEntity> GetDynamic(Expression<Func<TEntity, bool>> filter = null,
            string orderBy = null);

        (IList<TEntity> data, int total, int totalDisplay) GetDynamic(Expression<Func<TEntity, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
    }
    #endregion

    #region Implementations
    public abstract class BaseDao<TEntity, TKey>
        : IBaseDao<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
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

        public virtual IList<TEntity> LoadAll()
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

        public async Task<T> GetRankByIdAsync<T>(long id, string tableName)
        {
            var sql = $"  SELECT " +
                $"Id, " +
                $"[Rank]," +
                $" (SELECT MAX([Rank]) FROM {tableName}) Count" +
                $" FROM {tableName}" +
                $" Where Id = {id}";

            var query = _session.CreateSQLQuery(sql);
            var result = (await query.SetResultTransformer(Transformers.AliasToBean<T>()).ListAsync<T>()).FirstOrDefault();

            return result;
        }

        public async Task<long> RankDownAsync(long rankFrom, long rankTo, string tableName)
        {
            var sql = $"Update {tableName} " +
                $"SET [Rank] = CASE " +
                $"WHEN [Rank] < {rankFrom} AND [Rank] >= {rankTo} then [Rank]+1 " +
                $"WHEN [Rank] = {rankFrom} THEN {rankTo} " +
                $"ELSE [Rank] END";

            var result = await ExecuteUpdateDeleteQuery(sql);

            return result;
        }

        public async Task<long> RankUpAsync(long rankFrom, long rankTo, string tableName)
        {
            var sql = $"Update {tableName} " +
                $"SET [Rank] = CASE " +
                $"WHEN [Rank] > {rankFrom} AND [Rank] <= {rankTo} then [Rank]-1 " +
                $"WHEN [Rank] = {rankFrom} THEN {rankTo} " +
                $"ELSE [Rank] END";

            var result = await ExecuteUpdateDeleteQuery(sql);

            return result;
        }

        public async Task<long> GetMaxRank()
        {
            var sql = $"SELECT MAX([Rank]) Rank FROM {typeof(TEntity).Name}";
            var query = _session.CreateSQLQuery(sql);
            var result = Convert.ToInt64(await query.UniqueResultAsync());

            return result;
        }

        public async Task<long> GetMaxRank(string tableName)
        {
            var sql = $"SELECT MAX([Rank]) Rank FROM {tableName}";
            var query = _session.CreateSQLQuery(sql);
            var result = Convert.ToInt64(await query.UniqueResultAsync());

            return result;
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
        public (IList<TEntity> data, int total, int totalDisplay) GetDynamic(Expression<Func<TEntity, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();

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
                case "Creation Date":
                    query = sortDir == "asc" ? query.OrderBy(c => c.CreationDate) : query.OrderByDescending(c => c.CreationDate);
                    break;
                case "Rank":
                    query = sortDir == "asc" ? query.OrderBy(c => c.Rank) : query.OrderByDescending(c => c.Rank);
                    break;
            }

            var result = query.Skip(pageIndex).Take(pageSize);

            return (result.ToList(), total, totalDisplay);
        }
    }
    #endregion

}
