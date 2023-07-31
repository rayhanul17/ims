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
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Controllers
{
    [Authorize(Roles = "SA")]
    public class BankController : AllBaseController
    {
        #region Initialization
        private readonly IBankService _bankService;
        private readonly IUserService _userService;

        public BankController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _bankService = new BankService(session);
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
            var model = new BankAddModel();
            var selectList = Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
            ViewBag.StatusList = new SelectList(selectList, "Key", "Value", (int)Status.Active);
                       
            _logger.Info("Bank Creation Page");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(BankAddModel model)
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
            catch (Exception ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);
                _logger.Error(ex);
            }
            return RedirectToAction("Index", "Bank");
        }

        [HttpGet]
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
        public async Task<ActionResult> Edit(BankEditModel model)
        {
            try
            {
                ValidateBankEditModel(model);
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _bankService.UpdateAsync(model, userId);
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

            return RedirectToAction("Index", "Bank");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _bankService.RemoveByIdAsync(id, userId);
            }
            catch(CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

            }
            return RedirectToAction("Index", "Bank");
        }
        #endregion

        #region Ajax Call
        public JsonResult GetBanks()
        {
            
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _bankService.LoadAllBanks(model.SearchText, model.Length, model.Start, model.SortColumn,
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
        private void ValidateBankAddModel(BankAddModel model)
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

        private void ValidateBankEditModel(BankEditModel model)
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