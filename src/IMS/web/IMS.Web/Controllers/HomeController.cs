using Autofac;
using IMS.Services;
using log4net;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILifetimeScope _scope;
        private readonly ILog _logger;
        private readonly ILog _serviceLogger;
        private readonly CategoryService _categoryService;

        public HomeController(ILifetimeScope scope, CategoryService categoryService)
        {
            _scope = scope;
            _logger = _scope.Resolve<ILog>();
            _serviceLogger = _scope.ResolveNamed<ILog>("Service");
            _categoryService = categoryService;
        }

        public ActionResult Index()
        {
            _logger.Info("Log from home/index");
            _serviceLogger.Info("From Service Logger");
            _categoryService.Add();
            var o = _categoryService.GetByIdAsync(4);
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