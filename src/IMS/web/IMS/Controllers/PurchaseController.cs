using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
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

        [HttpGet]
        public async Task<ActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(string obj)
        {
            var o = JsonConvert.SerializeObject(obj).Split(new string[] {"Remove", "\\", "\""}, StringSplitOptions.RemoveEmptyEntries);            
            
            return View(obj);
        }
    }
}