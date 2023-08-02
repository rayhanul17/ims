﻿using IMS.BusinessModel.Entity;
using IMS.BusinessModel.ViewModel;
using IMS.BusinessRules;
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

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Block(long id)
        {
            try
            {
                await _userService.BlockAsync(id);
            }
            catch(CustomException ex)
            {
                ViewResponse(ex.Message, ResponseTypes.Warning);
                _logger.Error(ex);
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong", ResponseTypes.Warning);
                _logger.Error(ex);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
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
                var userName = _userService.GetUserName(id);

                var model = new UserEditModel
                {
                    Id = id,
                    Email = email,
                    Name = userName,
                };

                return View(model);
            }
            catch (Exception ex)
            {
                ViewResponse("Something went wrong!", ResponseTypes.Warning);
                _logger.Error(ex);
            }
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Edit(UserEditModel model, long id)
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
                }

                catch (Exception ex)
                {
                    ViewResponse("Something went wrong during user update", ResponseTypes.Warning);
                    _logger.Error(ex);
                }
            }
            return View(model);
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
                    //var removeRoles = model.Roles.Where(x => !x.IsChecked).Select(x => x.Text).ToArray();
                    var addRoles = model.Roles.Where(x => x.IsChecked).Select(x => x.Text).ToArray();
                    //var userName = UserManager.FindById(model.UserId).UserName;

                    var userRoles = UserManager.GetRoles(model.UserId).ToArray();
                    
                    var s = await UserManager.RemoveFromRolesAsync(model.UserId, userRoles);                    
                    var r = await UserManager.AddToRolesAsync(model.UserId, addRoles);
                }
                catch(Exception ex)
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

        public JsonResult GetUsers()
        {
            try
            {
                var model = new DataTablesAjaxRequestModel(Request);
                var data = _userService.LoadAllUsers(model.SearchText, model.Length, model.Start, model.SortColumn,
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
                                record.Name,
                                record.Email,
                                string.Join(", ", UserManager.GetRoles(record.Id)),
                                record.Status.ToString(),
                                _userService.GetUserName(record.CreateBy),
                                record.CreationDate.ToString(),
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
    }
}