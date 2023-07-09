using IMS.Web.Models;
using log4net;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        //private static readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private readonly ILog _logger;
        private ITest _test;
        public HomeController(ILog logger, ITest test)
        {
            _logger = logger;
            _test = test;
        }
        public ActionResult Index()
        {
            _test.Print("Hello");
            _logger.Info("HomeContorller\\Index");
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