using IMS.BusinessModel.Dto;
using IMS.BusinessRules;
using IMS.Services;
using IMS.Services.SessionFactories;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class ReportController : AllBaseController
    {
        #region Initialization
        private readonly IReportService _reportService;
        private readonly IUserService _userService;

        public ReportController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _reportService = new ReportService(session);
            _userService = new UserService(session);
        }

        #endregion
        // GET: Report
        [HttpGet]
        public ActionResult LP(LoseProfitReportDto id)
        {
            //if(model == null)
            //{
            //    model = new LoseProfitReportDto();
            //    return View(model);
            //}

            return View(new LoseProfitReportDto());
        }

        [HttpPost]
        public async Task<ActionResult> LP(string DateRange)
        {

            var model = await _reportService.GetLoseProfitReport("2023-08-02 03:21", "2023-08-03 04:26");

            return View(model);


        }

        public async Task<ActionResult> Dashboard()
        {
            try
            {
                await _reportService.ExecuteRawQueryAsync();
                var model = await _reportService.GetDashboardDataAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                var model = new DashboardDto();

                return View(model);
            }
        }
    }
}