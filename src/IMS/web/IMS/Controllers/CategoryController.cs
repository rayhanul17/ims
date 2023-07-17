using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
using IMS.Models;
using IMS.Services;
using IMS.Services.SessionFactories;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;

namespace IMS.Controllers
{
   
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
                    await _categoryService.AddAsync(model, User.Identity.GetUserId<long>());
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            var model = await _categoryService.GetByIdAsync(id);
            
            return View(model);
        }
        
        public JsonResult GetCategories()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _categoryService.LoadAllCategories(model.SearchText, model.Length, model.Start, model.SortColumn,
                    model.SortDirection);

                var count = 1;

                return Json(new
                {
                    draw = Request["draw"],
                    recordsTotal = data.total,
                    recordsFiltered = data.totalDisplay,
                    data = (from record in data.records
                            select new string[]
                            {
                                count++.ToString(),
                                record.Name,
                                record.Status.ToString(),
                                record.ModifyBy.ToString(),
                                record.ModificationDate.ToString(),
                                //_accountAdapter.FindById(record.CreateBy).Email,
                                record.CreateBy.ToString(),
                                record.CreationDate.ToString(),
                                record.Id.ToString()
                            }
                        ).ToArray()
                });
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
            }

            return default(JsonResult);
        }
    }
}