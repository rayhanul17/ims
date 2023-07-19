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
    public class SupplierController : AllBaseController
    {
        #region Initialization
        private readonly ISupplierService _supplierService;
        private readonly IAccountService _accountService;

        public SupplierController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _supplierService = new SupplierService(session);
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
            var model = new SupplierAddModel();

            return View(model);
        }

        [HttpPost]
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
            catch (Exception ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);
                _logger.Error(ex);
            }
            return RedirectToAction("Index", "Supplier");
        }

        [HttpGet]
        public async Task<ActionResult> Edit(long id)
        {
            if (id == 0)
                return View();
            try
            {
                var model = await _supplierService.GetByIdAsync(id);
                return View(model);
            }
            catch (Exception ex)
            {
                ViewResponse("Invalid object id", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index", "Supplier");
        }

        [HttpPost]
        public async Task<ActionResult> Edit(SupplierEditModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var userId = User.Identity.GetUserId<long>();
                    await _supplierService.UpdateAsync(model, userId);
                }
                else
                {
                    ViewResponse("Something went wrong", ResponseTypes.Danger);
                }
            }
            catch (Exception ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);
                _logger.Error(ex);
            }

            return RedirectToAction("Index", "Supplier");
        }

        [HttpPost]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _supplierService.RemoveByIdAsync(id, userId);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);

            }
            return RedirectToAction("Index", "Supplier");
        }
        #endregion

        #region Ajax Call
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
    }
}