using IMS.BusinessModel.Dto;
using IMS.BusinessModel.ViewModel;
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
    public class ReportController : AllBaseController
    {
        #region Initialization
        private readonly IReportService _reportService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly ISupplierService _supplierService;
        private readonly ICustomerService _customerService;

        public ReportController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _reportService = new ReportService(session);
            _categoryService = new CategoryService(session);
            _brandService = new BrandService(session);
            _supplierService = new SupplierService(session);
            _customerService = new CustomerService(session);
        }

        #endregion

        #region Operational Function
        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public ActionResult BS()
        {
            try
            {
                var categoryList = _categoryService.LoadAllActiveCategories();
                ViewBag.CategoryList = categoryList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                var brandList = _brandService.LoadAllActiveBrands();
                ViewBag.BrandList = brandList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                var supplierList = _supplierService.LoadAllActiveSuppliers();
                ViewBag.SupplierList = supplierList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                var CustomerList = _customerService.LoadAllActiveCustomers();
                ViewBag.CustomerList = CustomerList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                ViewResponse("Failed to load metadata", ResponseTypes.Danger);
            }
            var model = new BuyingSellingReportDto();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> BS(BuyingSellingReportDto model)
        {
            try
            {
                var categoryList = _categoryService.LoadAllActiveCategories();
                ViewBag.CategoryList = categoryList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                var brandList = _brandService.LoadAllActiveBrands();
                ViewBag.BrandList = brandList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                var supplierList = _supplierService.LoadAllActiveSuppliers();
                ViewBag.SupplierList = supplierList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                var CustomerList = _customerService.LoadAllActiveCustomers();
                ViewBag.CustomerList = CustomerList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                model = await _reportService.GetBuyingSellingReport(model);
                return View(model);

            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);
            }
            catch(Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }

            return RedirectToAction("BS");
        }

        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public ActionResult LP()
        {
            var model = new LoseProfitReportDto();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> LP(LoseProfitReportDto model)
        {

            try
            {
                if (string.IsNullOrWhiteSpace(model.DateRange))
                {
                    throw new CustomException("Empty DateTime Range Found");
                }

                model = await _reportService.GetLoseProfitReport(model);

                return View(model);
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, Models.ResponseTypes.Warning);
                return View(model);
            }


        }

        [HttpGet]
        public async Task<ActionResult> Dashboard()
        {
            try
            {               
                var model = await _reportService.GetDashboardDataAsync();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                var model = new DashboardDto();

                return View(model);
            }
        }

        #endregion
    }
}