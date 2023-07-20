using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
using IMS.Models;
using IMS.Services;
using IMS.Services.SessionFactories;
using Microsoft.Ajax.Utilities;
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
        private readonly IAccountService _accountService;

        public CategoryController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _categoryService = new CategoryService(session);
            _accountService = new AccountService(session);
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
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CategoryAddModel model)
        {
            try
            {
                ValidateCategoryAddModel(model);
                if (ModelState.IsValid)
                {
                    await _categoryService.AddAsync(model, User.Identity.GetUserId<long>());
                    ViewResponse("Successfully added a new category.", ResponseTypes.Success);
                }
                else
                {
                    ViewResponse("Provide data properly", ResponseTypes.Danger);
                }
            }
            catch (Exception ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);
                _logger.Error(ex);
            }
            return RedirectToAction("Index", "Category");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            if (id == 0)
                return View();
            try
            {
                var model = await _categoryService.GetByIdAsync(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewResponse("Invalid object id", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index", "Category");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(CategoryEditModel model)
        {
            try
            {
                ValidateCategoryEditModel(model);
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _categoryService.UpdateAsync(model, userId);
                }
                else
                {
                    ViewResponse("Fillup form properly", ResponseTypes.Danger);
                }
            }
            catch (Exception ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);
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
                                _accountService.GetUserName(record.CreateBy),
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

        #region Helper Function
        private void ValidateCategoryAddModel(CategoryAddModel model)
        {            
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length<3 || model.Name.Length >100)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Rank == 0)
            {
                ModelState.AddModelError("Rank", "Rank Invalid");
            }
        }

        private void ValidateCategoryEditModel(CategoryEditModel model)
        {
            if(model.Id == 0)
            {
                ModelState.AddModelError("Id", "Object id not found");
            }
            if(model.CreateBy == 0)
            {
                ModelState.AddModelError("CreateBy", "CreateBy id not found");
            }
            if(model.CreationDate.GetType() != typeof(DateTime))
            {
                ModelState.AddModelError("CreationDate", "CreationDate not found");
            }
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 100)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Rank == 0)
            {
                ModelState.AddModelError("Rank", "Rank Invalid");
            }
        }
        #endregion
    }
}