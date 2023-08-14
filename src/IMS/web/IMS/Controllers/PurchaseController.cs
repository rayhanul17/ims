using IMS.BusinessModel.Dto;
using IMS.BusinessModel.ViewModel;
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
    public class PurchaseController : AllBaseController
    {
        #region Initializtion
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IBrandService _brandService;
        private readonly ISupplierService _supplierService;
        private readonly IPurchaseService _purchaseService;
        private readonly IPaymentService _paymentService;

        public PurchaseController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _categoryService = new CategoryService(session);
            _productService = new ProductService(session);
            _brandService = new BrandService(session);
            _supplierService = new SupplierService(session);
            _purchaseService = new PurchaseService(session);
            _paymentService = new PaymentService(session);
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
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                ViewResponse("Failed to load metadata", ResponseTypes.Danger);
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Create(PurchaseDetailsViewModel[] model, long supplierId, decimal grandTotal)
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
                ViewResponse("You make mistake during purchase creation", ResponseTypes.Danger);
            }
            return RedirectToAction("Create");
        }


        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Details(long id)
        {
            var model = new PurchaseReportDto();
            try
            {
                model = await _purchaseService.GetPurchaseDetailsAsync(id);
            }
            catch(Exception ex)
            {
                _logger.Error(ex.Message, ex);
                ViewResponse("Something went wrong", ResponseTypes.Danger);
            }
            return View(model);
        }

        #endregion

        #region Ajax Call
        [HttpPost]
        [AllowAnonymous]
        public JsonResult UpdateProductList(long categoryId, long brandId)
        {
            try
            {
                var products = _productService.LoadActiveProducts(categoryId, brandId).Select(x => new { value = x.Id, text = x.Name, qty = x.InStockQuantity, price = x.SellingPrice });
                return Json(products);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                return default(JsonResult);
            }
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetPurchases()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = await _purchaseService.LoadAllPurchases(model.SearchText, model.Length, model.Start, model.SortColumn,
                    model.SortDirection);               

                return Json(new
                {
                    draw = Request["draw"],
                    recordsTotal = data.total,
                    recordsFiltered = data.totalDisplay,
                    data = (from record in data.records
                            select new string[]
                            {
                                record.Rank,
                                record.SupplierName,
                                record.CreateBy,
                                record.PurchaseDate.ToString(),
                                record.GrandTotalPrice.ToString(),
                                record.Id.ToString(),
                                record.IsPaid.ToString(),
                                record.PaymentId.ToString(),
                            }
                        ).ToArray()
                });
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

            return default(JsonResult);
        }
        #endregion
    }
}
