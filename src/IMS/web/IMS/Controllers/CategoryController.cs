using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
using IMS.Services;
using IMS.Services.SessionFactories;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class CategoryController : Controller
    {        
        private readonly ICategoryService _categoryService;       

        public CategoryController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _categoryService = new CategoryService(session);

        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
        {
            //var model = _scope.Resolve<CategoryCreateModel>();
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoryAddModel model)
        {
            
            return View();
        }
    }
}