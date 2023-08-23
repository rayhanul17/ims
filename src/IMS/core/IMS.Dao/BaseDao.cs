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
    public interface IBaseDao<TEntity, TKey> where TEntity : BaseEntity<TKey>
    {
        #region Opperationa Function
        Task<TKey> AddAsync(TEntity entity);
        Task EditAsync(TEntity entityToUpdate);
        Task<TEntity> GetByIdAsync(TKey id);
        Task RemoveByIdAsync(TKey id);
        Task RemoveAsync(TEntity entityToDelete);
        int GetCount(Expression<Func<TEntity, bool>> filter = null);
        Task<long> GetMaxRank();
        Task<long> GetMaxRank(string tableName);
        Task<object> GetScallerValueAsync(string sql);
        Task<int> ExecuteUpdateDeleteQuery(string query);
        Task<T> GetRankByIdAsync<T>(long id, string tableName);
        Task<long> RankUpAsync(long rankFrom, long rankTo, string tableName);
        Task<long> RankDownAsync(long rankFrom, long rankTo, string tableName);
        #endregion

        #region List Loading Function
        IList<TEntity> Get(Expression<Func<TEntity, bool>> filter);
        (IList<TEntity> data, int total, int totalDisplay) GetDynamic(Expression<Func<TEntity, bool>> filter = null, string orderBy = null, int pageIndex = 1, int pageSize = 10, string sortBy = null, string sortDir = null);
        #endregion
    }

    public abstract class BaseDao<TEntity, TKey>
        : IBaseDao<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        #region Initialization
        protected ISession _session;

        public BaseDao(ISession session)
        {
            _session = session;
        }
        #endregion

        #region Opperational Function
        public virtual async Task<TKey> AddAsync(TEntity entity)
        {
            await _session.SaveAsync(entity);
            return entity.Id;
        }

        public virtual async Task<object> GetScallerValueAsync(string sql)
        {
            var query = _session.CreateSQLQuery(sql);
            var result = await query.UniqueResultAsync();

            return result;
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
            var sql = $"SELECT MAX([Rank]) Rank FROM {typeof(TEntity).Name} WHERE [Status] != {(int)Status.Delete}";
            var query = _session.CreateSQLQuery(sql);
            var result = Convert.ToInt64(await query.UniqueResultAsync());

            return result;
        }

        public async Task<long> GetMaxRank(string tableName)
        {
            var sql = $"SELECT MAX([Rank]) Rank FROM {tableName} WHERE [Status] != {(int)Status.Delete}";
            var query = _session.CreateSQLQuery(sql);
            var result = Convert.ToInt64(await query.UniqueResultAsync());

            return result;
        }
        #endregion

        #region List Loading Function
        public virtual IList<TEntity> Get(Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = _session.Query<TEntity>();

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.ToList();
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
        #endregion
    }
}
