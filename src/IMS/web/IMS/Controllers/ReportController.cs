using IMS.BusinessModel.Dto;
using IMS.BusinessRules.Exceptions;
using IMS.Services;
using IMS.Services.SessionFactories;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Controllers
{
    [Authorize]
    public class ReportController : AllBaseController
    {
        #region Initialization
        private readonly IReportService _reportService;

        public ReportController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _reportService = new ReportService(session);
        }

        #endregion

        #region Operational Function
        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public ActionResult LP()
        {
            var model = new LoseProfitReportDto();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> LP(LoseProfitReportDto model)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(model.DateRange))
                {
                    throw new CustomException("Empty DateTime Range Found");
                }

                model = await _reportService.GetLoseProfitReport(model);

                return View(model);
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, Models.ResponseTypes.Warning);
                return View(model);
            }


        }

        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            try
            {               
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

        #endregion
    }
}