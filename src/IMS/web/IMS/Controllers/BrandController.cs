using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
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
    public class BrandController : AllBaseController
    {
        #region Initialization
        private readonly IBrandService _brandService;
        private readonly IUserService _userService;

        public BrandController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _brandService = new BrandService(session);
            _userService = new UserService(session);
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
            var model = new BrandAddModel();
            var selectList = Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
            ViewBag.StatusList = new SelectList(selectList, "Key", "Value", (int)Status.Active);
                       
            
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BrandAddModel model)
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
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex);
            }
            return RedirectToAction("Index", "Brand");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            if (id == 0)
                return View();
            try
            {
                var selectList = Enum.GetValues(typeof(IMS.BusinessRules.Enum.Status))
                       .Cast<IMS.BusinessRules.Enum.Status>()
                       .Where(e => e != IMS.BusinessRules.Enum.Status.Delete).ToDictionary(key => (int)key);
                ViewBag.StatusList = new SelectList(selectList, "Key", "Value");

                var model = await _brandService.GetByIdAsync(id);
                return View(model);
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);
                _logger.Error(ex.Message, ex);
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index", "Brand");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(BrandEditModel model)
        {
            try
            {
                ValidateBrandEditModel(model);
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _brandService.UpdateAsync(model, userId);
                }
                else
                {
                    ViewResponse("Fillup form properly", ResponseTypes.Danger);
                }
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex);
            }

            return RedirectToAction("Index", "Brand");
        }

        [HttpPost]
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
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex);
            }
            return RedirectToAction("Index", "Brand");
        }
        #endregion

        #region Ajax Call
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
                                record.Status.ToString(),
                                _userService.GetUserName(record.CreateBy),
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
        private void ValidateBrandAddModel(BrandAddModel model)
        {            
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length<3 || model.Name.Length >100)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Rank == 0)
            {
                ModelState.AddModelError("Rank", "Rank Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }

        private void ValidateBrandEditModel(BrandEditModel model)
        {
            if(model.Id == 0)
            {
                ModelState.AddModelError("Id", "Object id not found");
            }
            if(model.CreateBy == 0)
            {
                ModelState.AddModelError("CreateBy", "CreateBy id not found");
            }
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 100)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Rank == 0)
            {
                ModelState.AddModelError("Rank", "Rank Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }
        #endregion
    }
}