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
    public class SaleController : AllBaseController
    {

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICustomerService _customerService;
        private readonly ISaleService _saleService;
        private readonly IPaymentService _paymentService;
        private readonly IUserService _userService;

        public SaleController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _categoryService = new CategoryService(session);
            _productService = new ProductService(session);
            _brandService = new BrandService(session);
            _customerService = new CustomerService(session);
            _saleService = new SaleService(session);
            _paymentService = new PaymentService(session);
            _userService = new UserService(session);
        }
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult Create()
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

            var CustomerList = _customerService.LoadAllActiveCustomers();
            ViewBag.CustomerList = CustomerList.Select(x => new SelectListItem
            {
                Text = x.Item2,
                Value = x.Item1.ToString()
            }).ToList();

            return View();
        }        

        [HttpPost]
        public async Task<ActionResult> Create(SaleDetailsModel[] model, long customerId, decimal grandTotal)
        {

            if (model != null && customerId > 0 && grandTotal > -1)
            {
                try
                {
                    model = model.Where(x => x.IsDeleted == false && x.ProductId > 0).ToArray();
                    if (model.Length == 0)
                    {
                        ViewResponse("You make mistake during Sale creation", ResponseTypes.Danger);
                        return RedirectToAction("Create");
                    }
                    var userId = User.Identity.GetUserId<long>();
                    var id = await _saleService.AddAsync(model, grandTotal, customerId, userId);
                    await _paymentService.AddAsync(id, OperationType.Sale, grandTotal);

                    ViewResponse("Successfully Sale completed!", ResponseTypes.Success);

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
                ViewResponse("You make mistake during Sale creation", ResponseTypes.Danger);
            }
            return RedirectToAction("Create");
        }

        #region JSON
        [HttpPost]
        public JsonResult UpdateProductList(long categoryId, long brandId)
        {
            var products = _productService.LoadAllProducts(categoryId, brandId).Select(x => new { value = x.Id, text = x.Name, qty = x.InStockQuantity, price = x.SellingPrice });
            return Json(products);
        }

        public JsonResult GetSales()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _saleService.LoadAllSales(model.SearchText, model.Length, model.Start, model.SortColumn,
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
                                _customerService.GetNameById(record.CustomerId),
                                _userService.GetUserName(record.CreateBy),
                                record.SaleDate.ToString(),
                                record.GrandTotalPrice.ToString(),
                                record.Id.ToString(),
                                record.IsPaid.ToString(),
                                record.PaymentId.ToString(),
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
            var model = await _saleService.GetSaleDetailsAsync(id);
            return View(model);
        }

        #endregion
    }
}