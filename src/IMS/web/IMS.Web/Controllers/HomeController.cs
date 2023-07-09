using log4net;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILog _logger;

        public HomeController(ILog logger)
        {
            _logger = logger;
        }

        public ActionResult Index()
        {
            _logger.Info("Log from home/index");
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}