using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class ReportController : Controller
    {
        // GET: Report
        public ActionResult LP()
        {
            return View();
        }

        public ActionResult Dashboard()
        {
            return View();
        }
    }
}