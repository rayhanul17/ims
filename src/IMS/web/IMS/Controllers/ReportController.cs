using IMS.BusinessRules;
using IMS.Services;
using IMS.Services.SessionFactories;
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
        public ActionResult LP()
        {
            return View();
        }

        public async Task<ActionResult> Dashboard()
        {
            await _reportService.ExecuteRawQueryAsync();
            var model = await _reportService.GetDashboardDataAsync();
            return View(model);
        }
    }
}