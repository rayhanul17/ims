using Autofac;
using log4net;
using log4net.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
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

        public ActionResult Index()
        {
            _logger.Info("Log from home/index");
            _serviceLogger.Info("From Service Logger");
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