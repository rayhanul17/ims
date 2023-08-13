using IMS.BusinessModel.Dto;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Enum;
using IMS.BusinessRules.Exceptions;
using IMS.Models;
using IMS.Services;
using IMS.Services.SessionFactories;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace IMS.Controllers
{
    [Authorize]
    public class PaymentController : AllBaseController
    {
        #region Initialization
        private readonly IPaymentService _paymentService;
        private readonly IBankService _bankService;

        public PaymentController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _paymentService = new PaymentService(session);
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
        [Authorize(Roles = "SA, Manager, Seller")]
        public async Task<ActionResult> Create(long id)
        {
            var model = new PaymentViewModel();
            try
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

                model = await _paymentService.GetPaymentByIdAsync(id);
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

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "SA, Manager, Seller")]
        public async Task<ActionResult> Create(PaymentViewModel model)
        {

            if (model != null)
            {
                try
                {
                    await _paymentService.MakePaymentAsync(model);
                    ViewResponse("Successfully Payment completed!", ResponseTypes.Success);

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
            }
            else
            {
                ViewResponse("You make mistake during Payment creation", ResponseTypes.Danger);
            }
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "SA, Manager, Seller")]
        public async Task<ActionResult> Details(long id)
        {
            var model = new PaymentReportDto();
            try
            {
                model = await _paymentService.GetPaymentDetailsAsync(id);                
            }
            catch(Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }

            return View(model);
        }

        #endregion

        #region Ajax Call       
        [HttpPost]
        [AllowAnonymous]
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
                                record.OperationType,
                                record.TotalAmount,
                                record.PaidAmount,
                                record.DueAmount,
                                record.Id,
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