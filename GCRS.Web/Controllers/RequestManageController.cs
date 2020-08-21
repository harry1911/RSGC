using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GCRS.Web.Controllers
{
    public class RequestManageController : Controller
    {
        // GET: RequestManage
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ShowRequest()
        {
            throw new NotImplementedException();
        }

        public ActionResult Cancel()
        {
            throw new NotImplementedException();
        }
        public ActionResult Pending()
        {
            throw new NotImplementedException();
        }
        #region Realtor

        public ActionResult Done()
        {
            throw new NotImplementedException();
        }
        public ActionResult Draft()
        {
            throw new NotImplementedException();
        }
        public ActionResult Accept()
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}