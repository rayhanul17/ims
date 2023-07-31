using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
using IMS.Services;
using IMS.Services.SessionFactories;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
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
            model.Roles = model.Roles.Where(x => x.IsChecked).ToList();
            var userName = UserManager.FindById(model.UserId).UserName;

            var res = Roles.RoleExists("sa");
            var r = await UserManager.AddToRoleAsync(model.UserId, "SA");
            //Roles.RemoveUserFromRole(userName, "sa");

            return View();
        }
    }
}