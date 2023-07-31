using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
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
using System.Web.ApplicationServices;
using System.Web.Mvc;
using System.Web.Security;

namespace IMS.Controllers
{
    public class UserController : AllBaseController
    {
        private readonly IUserService _userService;
        private ApplicationUserManager _userManager;

        public UserController()
        {
            var session = new MsSqlSessionFactory(DbConnectionString.ConnectionString).OpenSession();            
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
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ManageUserRole()
        {

            var userList = _userService.LoadAllActiveUsers();
            ViewBag.UserList = userList.Select(x => new SelectListItem
            {
                Text = x.Item2,
                Value = x.Item1.ToString()
            }).ToList();

            List<Role> roles = new List<Role>()
           {
               new Role {Text="SA",Value="SA",IsChecked=false },
               new Role {Text="Manager",Value="Manager",IsChecked=false },
               new Role {Text="Seller",Value="Seller",IsChecked=false },
               
           };
            UserRolesModel model = new UserRolesModel();
            model.Roles = roles;
            
            return View(model);
            
        }
        [HttpPost]
        public async Task<ActionResult> ManageUserRole(UserRolesModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var removeRoles = model.Roles.Where(x => !x.IsChecked).Select(x => x.Text).ToArray();
                    var addRoles = model.Roles.Where(x => x.IsChecked).Select(x => x.Text).ToArray();
                    //var userName = UserManager.FindById(model.UserId).UserName;

                    var s = await UserManager.RemoveFromRolesAsync(model.UserId, removeRoles);
                    var r = await UserManager.AddToRolesAsync(model.UserId, addRoles);
                }
                catch(Exception ex )
                {
                    _logger.Error(ex);
                    ViewResponse("Something went wrong during role management", ResponseTypes.Warning);
                }

                
            }
            else
            {
                ViewResponse("Provide Data Properly", ResponseTypes.Warning);
            }
            return RedirectToAction("ManageUserRole");
        }
    }
}