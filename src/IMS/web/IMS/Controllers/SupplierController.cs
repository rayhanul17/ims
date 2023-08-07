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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Controllers
{
    public class SupplierController : AllBaseController
    {
        #region Initialization
        private readonly ISupplierService _supplierService;
        private readonly IUserService _userService;

        public SupplierController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _supplierService = new SupplierService(session);
            _userService = new UserService(session);
        }

        #endregion

        #region Index
        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
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
            var model = new SupplierAddModel();
            var selectList = Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
            ViewBag.StatusList = new SelectList(selectList, "Key", "Value", (int)Status.Active);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Create(SupplierAddModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _supplierService.AddAsync(model, User.Identity.GetUserId<long>());
                    ViewResponse("Successfully added a new category.", ResponseTypes.Success);
                }
                else
                {
                    ViewResponse("Provide data properly", ResponseTypes.Danger);
                    return View(model);
                }
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
            return RedirectToAction("Index", "Supplier");
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

                var model = await _supplierService.GetByIdAsync(id);
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
            return RedirectToAction("Index", "Supplier");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(SupplierEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _supplierService.UpdateAsync(model, userId);
                    ViewResponse("Updated successfully", ResponseTypes.Success);
                }
                else
                {
                    ViewResponse("Provide data properly", ResponseTypes.Danger);
                }
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

            return RedirectToAction("Index", "Supplier");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _supplierService.RemoveByIdAsync(id, userId);
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
            return RedirectToAction("Index", "Supplier");
        }
        #endregion

        #region Ajax Call
        [HttpPost]
        [AllowAnonymous]
        public JsonResult GetSuppliers()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _supplierService.LoadAllSuppliers(model.SearchText, model.Length, model.Start, model.SortColumn,
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
                                record.Address,
                                record.ContactNumber,
                                record.Email,
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
        private void ValidateBankAddModel(CustomerAddModel model)
        {
            string phone = @"^(?:\+?88)?01[13-9]\d{8}$";
            string mail = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            Regex phone_rgx = new Regex(phone);
            Regex mail_rgx = new Regex(mail);

            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 30)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Address.IsNullOrWhiteSpace() || model.Address.Length < 10 || model.Address.Length > 255)
            {
                ModelState.AddModelError("Address", "Address Invalid");
            }
            if (model.ContactNumber.IsNullOrWhiteSpace() || phone_rgx.IsMatch(model.ContactNumber))
            {
                ModelState.AddModelError("ContactNumber", "Contact Number Invalid");
            }
            if (model.Email.IsNullOrWhiteSpace() || model.Email.Length > 255 || mail_rgx.IsMatch(model.Email))
            {
                ModelState.AddModelError("Email", "Email Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }

        private void ValidateBankEditModel(CustomerEditModel model)
        {
            string phone = @"^(?:\+?88)?01[13-9]\d{8}$";
            string mail = @"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:\.[a-zA-Z0-9-]+)*$";
            Regex phone_rgx = new Regex(phone);
            Regex mail_rgx = new Regex(mail);

            if (model.Id == 0)
            {
                ModelState.AddModelError("Id", "Object id not found");
            }
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 30)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Address.IsNullOrWhiteSpace() || model.Address.Length < 10 || model.Address.Length > 255)
            {
                ModelState.AddModelError("Address", "Address Invalid");
            }
            if (model.ContactNumber.IsNullOrWhiteSpace() || phone_rgx.IsMatch(model.ContactNumber))
            {
                ModelState.AddModelError("ContactNumber", "Contact Number Invalid");
            }
            if (model.Email.IsNullOrWhiteSpace() || model.Email.Length > 255 || mail_rgx.IsMatch(model.Email))
            {
                ModelState.AddModelError("Email", "Email Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }
        #endregion
    }
}