using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GCRS.Web.Infrastructure.Abstract;
using GCRS.Web.Infrastructure.Concrete;
using GCRS.Base;
using Database;
using GCRS.Web.ViewModels;
using GCRS.Base.IRepositories;
using GCRS.Services;
using GCRS.Base.APIDatabase;

namespace GCRS.Web.Controllers.DataManager
{
    public class AccountController : Controller
    {
        UserService userService;
        private IAuthProvider authorizationProvider;

        public AccountController(IDB idb)
            : this(new FormsAuthProvider(), idb)
        { }

        public AccountController(IAuthProvider AuthProvider, IDB idb)
            : base()
        {
            authorizationProvider = AuthProvider;
            userService = new UserService(idb);
        }

        //GET: Account/Login
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(LoginViewModel model, string returnUrl)
        {
            if (!Request.IsAuthenticated)
            {
                if (authorizationProvider.Authentificate(model.Name, model.Password, false))
                {
                    return Redirect(returnUrl ?? Url.Action("Index", "Home"));
                }
            }
            return View();
        }

        //GET: Account/LogOff
        public ActionResult LogOff()
        {
            authorizationProvider.LogOff();
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(RegisterViewModel modelo)
        {
            if (ModelState.IsValid)
            {
                if (userService.GetUserByName(modelo.Name) == null)
                {
                    User nuevo_user = new User { PrivilegesID = 4, Name = modelo.Name, Email = modelo.Email, Phone = modelo.Phone, Pasword = modelo.Password };
                    userService.AddUser(nuevo_user);
                    authorizationProvider.Authentificate(modelo.Name, modelo.Password, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("Username", "The username is being used");
                }
            }
            return View();
        }
    }
}