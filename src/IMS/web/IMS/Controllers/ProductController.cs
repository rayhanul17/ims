using IMS.BusinessModel.Entity;
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
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Controllers
{
    [Authorize]
    public class ProductController : AllBaseController
    {
        #region Initialization
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        private readonly IUserService _userService;
        private readonly IImageService _imageService;

        public ProductController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();
            _productService = new ProductService(session);
            _categoryService = new CategoryService(session);
            _brandService = new BrandService(session);
            _userService = new UserService(session);
            _imageService = new ImageService();
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
            var model = new ProductAddModel();

            #region Dropdown box loading

            var enumList = Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
            ViewBag.StatusList = new SelectList(enumList, "Key", "Value", Status.Active);

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

            #endregion

            _logger.Info("Product Creation Page");
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken()]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Create(ProductAddModel model, HttpPostedFileBase image)
        {

            try
            {
                ValidateProductAddModel(model);
                if (ModelState.IsValid)
                {
                    string path = Server.MapPath("~/UploadedFiles/");                  

                    model.Image = await _imageService.SaveImage(image, path);

                    await _productService.AddAsync(model, User.Identity.GetUserId<long>());
                    ViewResponse("Successfully added a new Product.", ResponseTypes.Success);
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
            return RedirectToAction("Index", "Product");
        }

        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(long id)
        {
            if (id == 0)
                return View();
            try
            {
                #region Dropdown box loading
                var selectList = Enum.GetValues(typeof(Status))
                       .Cast<Status>()
                       .Where(e => e != Status.Delete).ToDictionary(key => (int)key);
                ViewBag.StatusList = new SelectList(selectList, "Key", "Value");

                var categoryList = _categoryService.LoadAllCategories();
                ViewBag.CategoryList = categoryList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                var brandList = _brandService.LoadAllBrands();
                ViewBag.BrandList = brandList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();
                #endregion

                var model = await _productService.GetByIdAsync(id);
                string path = Server.MapPath("~/UploadedFiles/");
                model.Image = "/UploadedFiles/" + model.Image;
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
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(ProductEditModel model, HttpPostedFileBase image)
        {
            try
            {
                ValidateProductEditModel(model);
                if (ModelState.IsValid)
                {
                    string path = Server.MapPath("~/UploadedFiles/");

                    model.Image = await _imageService.SaveImage(image, path);

                    var userId = User.Identity.GetUserId<long>();
                    await _productService.UpdateAsync(model, userId);

                    ViewResponse("Successfully updated!", ResponseTypes.Success);
                }
                else
                {
                    ViewResponse("Fillup form properly", ResponseTypes.Danger);
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

            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Delete(long id)
        {
            try
            {
                var userId = User.Identity.GetUserId<long>();
                await _productService.RemoveByIdAsync(id, userId);
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
            return RedirectToAction("Index", "Product");
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetRank(long productId)
        {
            try
            {
                var products = await _productService.GetProductRankAsync(productId);

                return Json(products);
            }
            catch(CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);                
            }
            catch(Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);  
            }
            return default(JsonResult);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> UpdateRank(long id, long targetRank)
        {
            try
            {
                await _productService.UpdateRankAsync(id, targetRank);
                return RedirectToAction("Index", "Product");
            }
            catch(CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Danger);
            }
            catch(Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Danger);
                _logger.Error(ex.Message, ex);
            }
            return RedirectToAction("Index");
        }
        #endregion

        #region Ajax Call
        [AllowAnonymous]
        public JsonResult GetProducts()
        {
            try
            {
                //string path = Server.MapPath("~/UploadedFiles/");
                string path = "\\UploadedFiles\\";
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _productService.LoadAllProducts(model.SearchText, model.Length, model.Start, model.SortColumn,
                    model.SortDirection);
                

                return Json(new
                {
                    draw = Request["draw"],
                    recordsTotal = data.total,
                    recordsFiltered = data.totalDisplay,
                    data = (from record in data.records
                            select new string[]
                            {                                
                                record.Name,
                                record.Category,
                                record.Brand,
                                record.Description,
                                record.Status.ToString(),                                
                                record.DiscountPrice.ToString(),
                                record.SellingPrice.ToString(),
                                record.InStockQuantity.ToString(),
                                _imageService.GetImage(path, record.Image),
                                record.Id.ToString()
                            }
                        ).ToArray()
                });
            }
            catch (Exception ex)
            {
                ViewResponse("Image saving failed", ResponseTypes.Warning);
                _logger.Error(ex.Message, ex);
            }

            return default(JsonResult);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetProductBuyingPrice(long productId)
        {
            var product = await _productService.GetByIdAsync(productId);
            return Json(new { UnitPrice = product.BuyingPrice});
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> GetProductSellingPriceAndQuantiy(long productId)
        {
            var product = await _productService.GetPriceAndQuantityByIdAsync(productId);
            var data = Json(new { UnitPrice = product.SellingPrice, Quantity = product.InStockQuantity });
            return data;
        }
        #endregion

        #region Helper Function
        private void ValidateProductAddModel(ProductAddModel model)
        {            
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length<3 || model.Name.Length >100)
            {
                ModelState.AddModelError("Name", "Name Length Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }

        private void ValidateProductEditModel(ProductEditModel model)
        {
            if(model.Id == 0)
            {
                ModelState.AddModelError("Id", "Object id not found");
            }
            if(model.CreateBy == 0)
            {
                ModelState.AddModelError("CreateBy", "CreateBy id not found");
            }
            if (model.CreationDate.GetType() == typeof(DateTime).GetType())
            {
                ModelState.AddModelError("CreationDate", "CreationDate invalid");
            }
            if (model.Name.IsNullOrWhiteSpace() || model.Name.Length < 3 || model.Name.Length > 100)
            {
                ModelState.AddModelError("Name", "Name Invalid");
            }
            if (model.Rank == 0)
            {
                ModelState.AddModelError("Rank", "Rank Invalid");
            }
            if (!(model.Status == Status.Active || model.Status == Status.Inactive))
            {
                ModelState.AddModelError("Status", "Status must active or inactive");
            }
        }
        #endregion
    }
}