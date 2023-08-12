﻿using IMS.BusinessModel.ViewModel;
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
    [Authorize]
    public class SaleController : AllBaseController
    {
        #region Initialization
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ICustomerService _customerService;
        private readonly ISaleService _saleService;
        private readonly IPaymentService _paymentService;
        
        public SaleController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _categoryService = new CategoryService(session);
            _productService = new ProductService(session);
            _brandService = new BrandService(session);
            _customerService = new CustomerService(session);
            _saleService = new SaleService(session);
            _paymentService = new PaymentService(session);            
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
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager, Seller")]
        public async Task<ActionResult> Create(SaleDetailsViewModel[] model, long customerId, decimal grandTotal)
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

        [Authorize(Roles = "SA, Manager, Seller")]
        public async Task<ActionResult> Details(long id)
        {
            var model = await _saleService.GetSaleDetailsAsync(id);
            return View(model);
        }

        #endregion

        #region Ajax Call
        [HttpPost]
        [AllowAnonymous]
        public JsonResult UpdateProductList(long categoryId, long brandId)
        {
            var products = _productService.LoadAvailableProducts(categoryId, brandId).Select(x => new { value = x.Id, text = x.Name, qty = x.InStockQuantity, price = x.SellingPrice });
            return Json(products);
        }

        [HttpPost]
        [AllowAnonymous]
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
                                record.CustomerName,
                                record.CreateBy,
                                record.SaleDate,
                                record.GrandTotalPrice,
                                record.Id,
                                record.IsPaid,
                                record.PaymentId,
                            }
                        ).ToArray()
                });
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

            return default(JsonResult);
        }
        #endregion
    }
}