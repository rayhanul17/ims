using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
using IMS.Services;
using IMS.Services.SessionFactories;
using log4net;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Controllers
{
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

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
            var model = new CategoryAddModel();
            _logger.Info("Category Creation Page");
            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CategoryAddModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _categoryService.Add(model);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return View();
        }
    }
}