using IMS.BusinessModel.Dto;
using IMS.BusinessRules;
using IMS.BusinessRules.Exceptions;
using IMS.Services;
using IMS.Services.SessionFactories;
using System;
using System.Globalization;
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
        public ActionResult LP()
        {
            var model = new LoseProfitReportDto();
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> LP(string dateRange)
        {
            var model = new LoseProfitReportDto();
            try
            {
                if (string.IsNullOrWhiteSpace(dateRange))
                {
                    throw new CustomException("Empty DateTime Range Found");
                }
                var dates = dateRange.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);

                var startDate = DateTime.Parse(dates[0]).ToUniversalTime().ToString();
                var endDate = DateTime.Parse(dates[1]).ToUniversalTime().ToString();
                model = await _reportService.GetLoseProfitReport(startDate, endDate);

                return View(model);
            }
            catch(CustomException ex)
            {
                ViewResponse(ex.Message, Models.ResponseTypes.Warning);
                return View(model);
            }


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