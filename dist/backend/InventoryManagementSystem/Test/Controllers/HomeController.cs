using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Test.Models;

namespace Test.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (var session = NHibernateHelper.GetSession())
            {
                using (var tx = session.BeginTransaction())
                {

                    var p1 = new ProductModel { Name = "P1", Price = 120.65m };
                    var p2 = new ProductModel { Name = "P2", Price = 120.65m };
                    var c1 = new CategoryModel { Name = "C1"};
                    p1.Category = c1;
                    p2.Category = c1;
                    c1.Products = new List<ProductModel> { p1, p2 };
                    

                    session.Save(c1);
                    tx.Commit();
                }

            }
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