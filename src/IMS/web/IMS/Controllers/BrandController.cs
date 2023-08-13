using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
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
    [Authorize]
    public class BrandController : AllBaseController
    {
        #region Initialization
        private readonly IBrandService _brandService;

        public BrandController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _brandService = new BrandService(session);
        }
        #endregion

        #region Index
        [Authorize(Roles = "SA, Manager, Seller")]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Operational Function
        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public ActionResult Create()
        {
            var model = new BrandAddViewModel();
            try
            {
                var selectList = Enum.GetValues(typeof(Status))
                           .Cast<Status>()
                           .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
                ViewBag.StatusList = new SelectList(selectList, "Key", "Value", (int)Status.Active);
            }
            catch (Exception ex)
            {
                ViewResponse("Failed to load status", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Create(BrandAddViewModel model)
        {
            try
            {
                ValidateBrandAddModel(model);
                if (ModelState.IsValid)
                {
                    await _brandService.AddAsync(model, User.Identity.GetUserId<long>());
                    ViewResponse("Successfully added a new Brand.", ResponseTypes.Success);
                }
                else
                {
                    ViewResponse("Provide data properly", ResponseTypes.Danger);
                }
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);                
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index", "Brand");
        }

        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(long id)
        {
            if (id == 0)
                return View();
            try
            {
                var selectList = Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
                ViewBag.StatusList = new SelectList(selectList, "Key", "Value");

                var model = await _brandService.GetByIdAsync(id);
                return View(model);
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);                
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index", "Brand");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(BrandEditViewModel model)
        {
            try
            {
                ValidateBrandEditModel(model);
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _brandService.UpdateAsync(model, userId);
                    ViewResponse("Updated successfully", ResponseTypes.Success);
                }
                else
                {
                    ViewResponse("Fillup form properly", ResponseTypes.Danger);
                }
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);                
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }

            return RedirectToAction("Index", "Brand");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _brandService.RemoveByIdAsync(id, userId);
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);                
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index", "Brand");
        }
        #endregion

        #region Ajax Call
        [AllowAnonymous]
        public JsonResult GetBrands()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _brandService.LoadAllBrands(model.SearchText, model.Length, model.Start, model.SortColumn,
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
                                record.Status,
                                record.CreateBy,
                                record.CreationDate,
                                record.Id
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
        private void ValidateBrandAddModel(BrandAddViewModel model)
        {
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 30)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }

        private void ValidateBrandEditModel(BrandEditViewModel model)
        {
            if (model.Id == 0)
            {
                ModelState.AddModelError("Id", "Object id not found");
            }
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 30)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }
        #endregion
    }
}