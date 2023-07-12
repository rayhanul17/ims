using Autofac;
using IMS.Web.Models.Categories;
using log4net;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILifetimeScope _scope;
        private readonly ILog _logger;

        public CategoryController(ILifetimeScope scope)
        {
            _scope = scope;
            _logger = _scope.Resolve<ILog>();
        }
        public ActionResult Index()
        {
            return View();
        }

        // GET: Category/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            var model = _scope.Resolve<CategoryCreateModel>();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoryCreateModel model)
        {
            model.ResolveDependency(_scope);
            string aspUser = User.Identity.GetUserId();
            await model.AddAsync(aspUser);
            return View();
        }


        // GET: Category/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Category/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // GET: Category/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Category/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
