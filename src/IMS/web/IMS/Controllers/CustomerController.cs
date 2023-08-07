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
    [Authorize]
    public class CustomerController : AllBaseController
    {
        #region Initialization
        private readonly ICustomerService _customerService;
        private readonly IUserService _userService;

        public CustomerController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _customerService = new CustomerService(session);
            _userService = new UserService(session);
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
            var model = new CustomerAddModel();
            var selectList = Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
            ViewBag.StatusList = new SelectList(selectList, "Key", "Value", (int)Status.Active);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Create(CustomerAddModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _customerService.AddAsync(model, User.Identity.GetUserId<long>());
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
            return RedirectToAction("Index", "Customer");
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

                var model = await _customerService.GetByIdAsync(id);
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
            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(CustomerEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _customerService.UpdateAsync(model, userId);
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

            return RedirectToAction("Index", "Customer");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _customerService.RemoveByIdAsync(id, userId);
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
            return RedirectToAction("Index", "Customer");
        }
        #endregion

        #region Ajax Call
        [AllowAnonymous]
        public JsonResult GetCustomers()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _customerService.LoadAllCustomers(model.SearchText, model.Length, model.Start, model.SortColumn,
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
            string pattern = @"^(?:\+?88)?01[13-9]\d{8}$";
            Regex rgx = new Regex(pattern);

            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 100)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Address.IsNullOrWhiteSpace() || model.Address.Length < 10 || model.Address.Length > 100)
            {
                ModelState.AddModelError("Address", "Address Invalid");
            }
            if (model.ContactNumber.IsNullOrWhiteSpace() || rgx.IsMatch(model.ContactNumber))
            {
                ModelState.AddModelError("ContactNumber", "Contact Number Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }

        private void ValidateBankEditModel(CustomerEditModel model)
        {
            string pattern = @"^(?:\+?88)?01[13-9]\d{8}$";
            Regex rgx = new Regex(pattern);

            if (model.Id == 0)
            {
                ModelState.AddModelError("Id", "Object id not found");
            }
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 100)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Address.IsNullOrWhiteSpace() || model.Address.Length < 10 || model.Address.Length > 100)
            {
                ModelState.AddModelError("Address", "Address Invalid");
            }
            if (model.ContactNumber.IsNullOrWhiteSpace() || rgx.IsMatch(model.ContactNumber))
            {
                ModelState.AddModelError("ContacNumber", "Contact Number Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }
        #endregion
    }
}