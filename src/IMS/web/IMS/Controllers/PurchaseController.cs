﻿using IMS.BusinessModel.Dto;
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
    public class PurchaseController : AllBaseController
    {

        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ISupplierService _supplierService;
        private readonly IPurchaseService _purchaseService;
        private readonly IAccountService _accountService;

        public PurchaseController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _categoryService = new CategoryService(session);
            _productService = new ProductService(session);
            _brandService = new BrandService(session);
            _supplierService = new SupplierService(session);
            _purchaseService = new PurchaseService(session);
            _accountService = new AccountService(session);
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

            var supplierList = _supplierService.LoadAllActiveSuppliers();
            ViewBag.SupplierList = supplierList.Select(x => new SelectListItem
            {
                Text = x.Item2,
                Value = x.Item1.ToString()
            }).ToList();

            return View();
        }        

        [HttpPost]
        public async Task<ActionResult> Create(PurchaseDetailsModel[] model, long supplierId, decimal grandTotal)
        {

            if (model != null && supplierId > 0 && grandTotal > -1)
            {
                try
                {
                    model = model.Where(x => x.IsDeleted == false && x.ProductId > 0).ToArray();
                    if (model.Length == 0)
                    {
                        ViewResponse("You make mistake during purchase creation", ResponseTypes.Danger);
                        return RedirectToAction("Create");
                    }
                    var userId = User.Identity.GetUserId<long>();
                    await _purchaseService.AddAsync(model, grandTotal, supplierId, userId);
                    ViewResponse("Successfully purchase completed!", ResponseTypes.Success);

                }
                catch (Exception ex)
                {
                    ViewResponse("Something went wrong", ResponseTypes.Danger);
                    _logger.Error(ex.Message, ex);
                }
            }
            else
            {
                ViewResponse("You make mistake during purchase creation", ResponseTypes.Danger);
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

        public JsonResult GetPurchases()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _purchaseService.LoadAllPurchases(model.SearchText, model.Length, model.Start, model.SortColumn,
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
                                _supplierService.GetNameById(record.SupplierId),
                                _accountService.GetUserName(record.CreateBy),
                                record.PurchaseDate.ToString(),                                
                                record.GrandTotalPrice.ToString(),
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
            var model = await _purchaseService.GetPurchaseDetailsAsync(id);
            return View(model);
        }

        #endregion
    }
}