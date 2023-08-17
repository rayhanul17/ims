using IMS.BusinessModel.Dto;
using IMS.BusinessRules.Enum;
using NHibernate;
using NHibernate.Transform;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.Dao
{
    public interface IReportDao
    {
        Task<IList<T>> ExecuteQueryAsync<T>(string query);
        Task<IList<T>> ExecuteParametrizedQueryAsync<T>(string rawQuery, Dictionary<string, object> dictionary);
        Task<ActiveInactiveDto> GetCountAsync(string tableName);
        Task<decimal> GetSingleColumnTatal(string columnName, string tableName);
        Task<int> GetTotalCount(string columnName, string tableName, string expression = "1=1");
    }

    public class ReportDao : IReportDao
    {
        private readonly ISession _session;
        public ReportDao(ISession session)
        {
            _session = session;
        }

        public async Task<IList<T>> ExecuteQueryAsync<T>(string rawQuery)
        {
            var query = _session.CreateSQLQuery(rawQuery);
            var result = await query.SetResultTransformer(Transformers.AliasToBean<T>()).ListAsync<T>();

            return result;
        }

        public async Task<IList<T>> ExecuteParametrizedQueryAsync<T>(string rawQuery, Dictionary<string, object> dictionary)
        {
            var query = _session.CreateSQLQuery(rawQuery);
            foreach (var kvp in dictionary)
            {
                query.SetParameter(kvp.Key, kvp.Value);
            }
            var result = await query.SetResultTransformer(Transformers.AliasToBean<T>()).ListAsync<T>();

            return result;
        }
        public async Task<ActiveInactiveDto> GetCountAsync(string tableName)
        {
            var sql = $"SELECT " +
                $"SUM(CASE WHEN [Status] = {(int)Status.Inactive} THEN 1 else 0 END) Inactive, " +
                $"SUM(CASE WHEN [Status] = {(int)Status.Active} THEN 1 else 0 END) Active " +
                $"FROM {tableName}";

            var query = _session.CreateSQLQuery(sql);
            var result = await query.SetResultTransformer(Transformers.AliasToBean<ActiveInactiveDto>()).UniqueResultAsync<ActiveInactiveDto>();

            return result;
        }
        public async Task<decimal> GetSingleColumnTatal(string columnName, string tableName)
        {
            var sql = $"SELECT SUM({columnName}) FROM {tableName}";
            var query = _session.CreateSQLQuery(sql);
            var result = await query.UniqueResultAsync();

            return Convert.ToDecimal(result);
        }

        public async Task<int> GetTotalCount(string columnName, string tableName, string expression = "1=1")
        {
            var sql = $"SELECT COUNT({columnName}) FROM {tableName} WHERE {expression}";
            var query = _session.CreateSQLQuery(sql);
            var result = await query.UniqueResultAsync();

            return Convert.ToInt32(result);
        }
    }

}
