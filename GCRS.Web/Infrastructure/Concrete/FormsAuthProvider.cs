using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using GCRS.Web.Infrastructure.Abstract;
using Database;


namespace GCRS.Web.Infrastructure.Concrete
{
    public class FormsAuthProvider : IAuthProvider
    {
        public bool Authentificate(string username, string pasword, bool remember)
        {
            var context = new DB();
            var user = context.Users.SingleOrDefault(x => x.Name == username && x.Pasword == pasword);
            bool result = false;
            if (user != null)
            {
                result = true;
                string rol = user.Privileges.Name;
                FormsAuthenticationTicket authTicket = new FormsAuthenticationTicket(
                                                            1,
                                                            user.Name,
                                                            DateTime.Now,
                                                            DateTime.Now.AddMinutes(40),
                                                            false,
                                                            rol,
                                                            "/");
                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,
                                                FormsAuthentication.Encrypt(authTicket));
                System.Web.HttpContext.Current.Response.Cookies.Add(cookie);
            }
            return result;
        }

        public void LogOff()
        {
            FormsAuthentication.SignOut();
        }
    }
}