using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
using IMS.Models;
using IMS.Services;
using IMS.Services.SessionFactories;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Controllers
{

    public class CategoryController : AllBaseController
    {
        #region Initialization
        private readonly ICategoryService _categoryService;

        public CategoryController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _categoryService = new CategoryService(session);

        }

        #endregion

        #region Index
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Operational Function
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
                    ViewResponse("Successfully added a new category.", ResponseTypes.Success);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return RedirectToAction("Index", "Category");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            var model = await _categoryService.GetByIdAsync(id);

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CategoryEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _categoryService.UpdateAsync(model, userId);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return RedirectToAction("Index", "Category");
        }
        
        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _categoryService.RemoveByIdAsync(id, userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

            }
            return RedirectToAction("Index", "Category");
        }
        #endregion

        #region Ajax Call
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
                                record.Description,
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
        #endregion
    }
}