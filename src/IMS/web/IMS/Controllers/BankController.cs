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
    public class BankController : AllBaseController
    {
        #region Initialization
        private readonly IBankService _bankService;

        public BankController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _bankService = new BankService(session);
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
            try
            {
                var model = new BankAddViewModel();
                var selectList = Enum.GetValues(typeof(Status))
                           .Cast<Status>()
                           .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
                ViewBag.StatusList = new SelectList(selectList, "Key", "Value", (int)Status.Active);
                
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                return View();
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Create(BankAddViewModel model)
        {
            try
            {
                ValidateBankAddModel(model);
                if (ModelState.IsValid)
                {
                    await _bankService.AddAsync(model, User.Identity.GetUserId<long>());
                    ViewResponse("Successfully added a new Bank.", ResponseTypes.Success);
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
                _logger.Error(ex);
            }
            return RedirectToAction("Index", "Bank");
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

                var model = await _bankService.GetByIdAsync(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewResponse("Invalid object id", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index", "Bank");
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(BankEditViewModel model)
        {
            try
            {
                ValidateBankEditModel(model);
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _bankService.UpdateAsync(model, userId);
                    ViewResponse("Bank updated sucessfully", ResponseTypes.Success);
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
                _logger.Error(ex);
            }

            return RedirectToAction("Index", "Bank");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _bankService.RemoveByIdAsync(id, userId);
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                ViewResponse("Something went wrong", ResponseTypes.Danger);
            }
            return RedirectToAction("Index", "Bank");
        }
        #endregion

        #region Ajax Call
        [AllowAnonymous]
        public JsonResult GetBanks()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _bankService.LoadAllBanks(model.SearchText, model.Length, model.Start, model.SortColumn,
                    model.SortDirection);

                return Json(new
                {
                    draw = Request["draw"],
                    recordsTotal = data.total,
                    recordsFiltered = data.totalDisplay,
                    data = (from record in data.records
                            select new string[]
                            {
                                record.Rank,
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
                ViewResponse("Something went wrong", ResponseTypes.Danger);
            }

            return default(JsonResult);
        }
        #endregion

        #region Helper Function
        private void ValidateBankAddModel(BankAddViewModel model)
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

        private void ValidateBankEditModel(BankEditViewModel model)
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
