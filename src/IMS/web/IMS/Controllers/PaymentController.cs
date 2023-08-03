using IMS.BusinessModel.Dto;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
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
    public class PaymentController : AllBaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly IBankService _bankService;
        private readonly IUserService _userService;

        public PaymentController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _paymentService = new PaymentService(session);
            _bankService = new BankService(session);
            _userService = new UserService(session);
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<ActionResult> Create(long id)
        {

            var paymentMethodList = Enum.GetValues(typeof(PaymentMethod))
                       .Cast<PaymentMethod>().ToDictionary(key => (int)key);
            ViewBag.PaymentMethodList = new SelectList(paymentMethodList, "Key", "Value", PaymentMethod.Bank);

            var bankList = _bankService.LoadAllActiveBanks();
            ViewBag.BankList = bankList.Select(x => new SelectListItem
            {
                Text = x.Item2,
                Value = x.Item1.ToString()
            }).ToList();

            var model = await _paymentService.GetPaymentByIdAsync(id);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        public async Task<ActionResult> Create(PaymentModel model)
        {           

            if (model != null)
            {
                try
                {  
                    await _paymentService.MakePaymentAsync(model);
                    ViewResponse("Successfully Payment completed!", ResponseTypes.Success);

                }
                catch(CustomException ex)
                {
                    ViewResponse(ex.Message, ResponseTypes.Warning);
                }
                catch (Exception ex)
                {
                    ViewResponse("Something went wrong", ResponseTypes.Danger);
                    _logger.Error(ex.Message, ex);
                }
            }
            else
            {
                ViewResponse("You make mistake during Payment creation", ResponseTypes.Danger);
            }
            return RedirectToAction("Index");
        }

        #region JSON       

        public JsonResult GetPayments()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _paymentService.LoadAllPayments(model.SearchText, model.Length, model.Start, model.SortColumn,
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
                                record.OperationType.ToString(),
                                record.TotalAmount.ToString(),
                                record.PaidAmount.ToString(),
                                record.DueAmount.ToString(),
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

        public async Task<ActionResult> Details(long id)
        {
            var model = await _paymentService.GetPaymentDetailsAsync(id);
            return View(model);
        }
        #endregion
    }
}