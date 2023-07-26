using IMS.BusinessModel.Dto;
using IMS.Dao;
using NHibernate;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace IMS.Services
{
    #region Interface
    public interface IReportService
    {
        Task<DashboardDto> GetDashboardDataAsync();
        Task<IEnumerable<MyModel>> ExecuteRawQueryAsync();
    }
    #endregion

    public class ReportService : IReportService
    {
        #region Initializtion
        private readonly IReportDao _reportDao;

        public ReportService(ISession session)
        {
            _reportDao = new ReportDao(session);
        }

        public async Task<IEnumerable<MyModel>> ExecuteRawQueryAsync()
        {
            var result = await _reportDao.ExecuteQueryAsync<MyModel>("SELECT SupplierId, PurchaseDate, GrandTotalPrice FROM Purchase");
            return result;
        }

        #endregion
        public async Task<DashboardDto> GetDashboardDataAsync()
        {
            var dashboard = new DashboardDto();
            dashboard.ToatlPurchasePrice = await _reportDao.GetSingleColumnTatal("GrandTotalPrice", "Purchase");
            dashboard.TotalSalePrice = await _reportDao.GetSingleColumnTatal("GrandTotalPrice", "Sale");
            dashboard.TotalActiveCustomer = await _reportDao.GetTotalCount("Id", "Customer", "Status = 1");
            dashboard.TotalInActiveCustomer = await _reportDao.GetTotalCount("Id", "Customer", "Status = 0");
            dashboard.TotalActiveSupplier = await _reportDao.GetTotalCount("Id", "Supplier", "Status = 1");
            dashboard.TotalInActiveSupplier = await _reportDao.GetTotalCount("Id", "Supplier", "Status = 0");

            return dashboard;
        }
    }
}
