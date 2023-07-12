using Autofac;
using IMS.Models.Entities;
using IMS.Services;
using log4net;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILifetimeScope _scope;
        private readonly ILog _logger;
        private readonly ILog _serviceLogger;       

        public HomeController(ILifetimeScope scope)
        {
            _scope = scope;
            _logger = _scope.Resolve<ILog>();
            _serviceLogger = _scope.ResolveNamed<ILog>("Service");            
        }

       
        public async Task<ActionResult> Index(Category obj)
        {
            
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