using IMS.BusinessModel.Dto;
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
