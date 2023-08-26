using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules.Exceptions;
using IMS.Models;
using IMS.Services;
using IMS.Services.SessionFactories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace IMS.Controllers
{
    [Authorize(Roles = "SA, Manager")]
    public class UserController : AllBaseController
    {
        #region Initialization
        private readonly IUserService _userService;
        private ApplicationUserManager _userManager;

        public UserController()
        {
            var session = new MsSqlSessionFactory().OpenSession();
            _userService = new UserService(session);
        }

        public UserController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        #endregion

        #region Index
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        #endregion

        #region Operational Function
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Block(long id)
        {
            try
            {
                await _userService.BlockAsync(id);
            }
            catch (CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);                
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Warning);
                _logger.Error(ex);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(long id)
        {
            try
            {
                var user = UserManager.FindById(id);
                if (user == null)
                {
                    ViewResponse("Wrong id given!", ResponseTypes.Warning);
                    return View();
                }
                var email = user.Email;
                var userName = await _userService.GetUserNameAsync(id);

                var model = new UserEditViewModel
                {
                    Id = id,
                    Email = email,
                    Name = userName,
                };

                return View(model);
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

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> Edit(UserEditViewModel model, long id)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var user = UserManager.FindById(id);
                    user.Email = model.Email;
                    user.UserName = model.Name;

                    await UserManager.UpdateAsync(user);
                    var userId = User.Identity.GetUserId<long>();
                    await _userService.UpdateUserAsync(model.Name, model.Email, model.Id, userId);

                    ViewResponse("Updated successfully", ResponseTypes.Success);
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

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "SA, Manager")]
        public ActionResult ManageUserRole()
        {
            var model = new UserRolesViewModel();
            try
            {                
                var userList = _userService.LoadAllActiveUsers();
                ViewBag.UserList = userList.Select(x => new SelectListItem
                {
                    Text = x.Item2,
                    Value = x.Item1.ToString()
                }).ToList();

                List<Role> roles = new List<Role>()
                   {
                       new Role {Text="SA", Value="SA", IsChecked = false },
                       new Role {Text="Manager", Value="Manager", IsChecked = false },
                       new Role {Text="Seller", Value="Seller", IsChecked = false },

                   };
                model.Roles = roles;
            } 
            catch (Exception ex)
            {
                _logger.Error(ex.Message, ex);
                ViewResponse("Something went wrong", ResponseTypes.Danger);
            }
            
            return View(model);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SA, Manager")]
        public async Task<ActionResult> ManageUserRole(UserRolesViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    //var removeRoles = model.Roles.Where(x => !x.IsChecked).Select(x => x.Text).ToArray();
                    var addRoles = model.Roles.Where(x => x.IsChecked).Select(x => x.Text).ToArray();
                    //var userName = UserManager.FindById(model.UserId).UserName;

                    var userRoles = UserManager.GetRoles(model.UserId).ToArray();

                    var s = await UserManager.RemoveFromRolesAsync(model.UserId, userRoles);
                    var r = await UserManager.AddToRolesAsync(model.UserId, addRoles);
                    ViewResponse("Updated successfully", ResponseTypes.Success);
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
                ViewResponse("Provide Data Properly", ResponseTypes.Warning);
            }

            return RedirectToAction("ManageUserRole");
        }
        #endregion

        #region Ajax call
        public JsonResult GetRoles(long userId)
        {
            var roles = UserManager.GetRoles(userId).ToArray();

            return Json(roles, JsonRequestBehavior.AllowGet);
        }

        public async Task<JsonResult> GetUsers()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = await _userService.LoadAllUsers(model.SearchText, model.Length, model.Start, model.SortColumn,
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
                                record.Name,
                                record.Email,
                                string.Join(", ", UserManager.GetRoles(Convert.ToInt64(record.Id))),
                                record.Status,
                                record.CreateBy,
                                record.CreationDate,
                                record.Id
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
