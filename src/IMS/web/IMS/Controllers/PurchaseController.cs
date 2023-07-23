using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class PurchaseController : AllBaseController
    {
        // GET: Purchase
        public ActionResult Index()
        {
            return RedirectToAction("Create");
        }

        public async Task<ActionResult> Create()
        {
            return View();
        }
    }
}