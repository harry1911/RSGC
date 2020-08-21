using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Database;
using GCRS.Base;
using GCRS.Base.APIDatabase;
using GCRS.Base.IRepositories;
using GCRS.Services;

namespace GCRS.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        UserService userRepo;
        PrivilegesService privilege;
        public UsersController(IDB idb):base()
        {
            userRepo = new UserService(idb);
            privilege = new PrivilegesService(idb);

        }
        // GET: Users
        public ActionResult Index()
        {
            return View(userRepo.ListAllUsers());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            User user = new User();
            if (userRepo.GetUserById(id, out user))
                if (user != null)
                    return View(user);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            var priviligesList = GetPrivilegesSelectList();
            ViewBag.PrivilegesID = priviligesList;

            return View();
        }

        private SelectList GetPrivilegesSelectList(object selectedPrivilege = null)
        {
            var privilegesQuery = privilege.GetPrivilegesSelectList();
            return new SelectList(privilegesQuery, "PrivilegesID", "Name", selectedPrivilege);
        }

        // POST: Users/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "UserID,Name,Email,Phone,Pasword,PrivilegesID")] User user)
        {
            if (ModelState.IsValid)
            {
                userRepo.AddUser(user);
                return RedirectToAction("Index");
            }
            var priviligesList = GetPrivilegesSelectList();
            ViewBag.PrivilegesID = priviligesList;

            return View(user);
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            User user = new User();
            if (userRepo.GetUserById(id, out user))
                if (user != null)
                {
                    var priviligesList = user.Privileges != null ? GetPrivilegesSelectList(user.Privileges.PrivilegesID) : GetPrivilegesSelectList();
                    ViewBag.PrivilegesID = priviligesList;

                    return View(user);
                }
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Users/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "UserID,Name,Email,Phone,Pasword,PrivilegesID")] User user)
        {
            if (ModelState.IsValid)
            {
                userRepo.EditUser(user);
                return RedirectToAction("Index");
            }
            var priviligesList = GetPrivilegesSelectList(user.PrivilegesID);
            ViewBag.PrivilegesID = priviligesList;

            return View(user);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            User user = new User();
            if (userRepo.GetUserById(id, out user))
                if (user != null)
                    return View(user);
                else return HttpNotFound();
            else return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            userRepo.DeleteUser(id);
            return RedirectToAction("Index");
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        userRepo.Dispose();
        //        privilege.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
    }
}
