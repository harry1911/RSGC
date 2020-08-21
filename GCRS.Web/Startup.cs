using Autofac;
using Autofac.Integration.Mvc;
using Database;
using GCRS.Base.APIDatabase;
using GCRS.Web.Controllers;
using Microsoft.Owin;
using Owin;
using System;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;

[assembly: OwinStartup(typeof(GCRS.Web.Startup))]
namespace GCRS.Web
{
    public partial class Startup : System.Web.HttpApplication
    {
        private void Register(IAppBuilder app)
        {
            var builder = new ContainerBuilder();
            builder.RegisterControllers(typeof(Startup).Assembly);
            builder.RegisterType<HomeController>().InstancePerRequest();
            builder.RegisterGeneric(typeof(Repo<>)).As(typeof(IRepo<>)).InstancePerLifetimeScope();
            builder.RegisterType<DB>().As<IDB>().InstancePerLifetimeScope();

            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));

        }
        public void Configuration(IAppBuilder app)
        {
            Register(app);
            ConfigureBundles(BundleTable.Bundles);
            ConfigureFilters(GlobalFilters.Filters);
            ConfigureRoutes(RouteTable.Routes);

            app.Use((ctx, next) =>
            {
                Application_AuthenticateRequest(ctx);
                return next();
            });
        }

        protected void Application_AuthenticateRequest(IOwinContext ctx)
        {
            var authCookie = ctx.Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie);
                string[] roles = authTicket.UserData.Split(new Char[] { ',' });
                var identity = new GenericIdentity(authTicket.Name);
                identity.AddClaims(roles.Select(r => new Claim(ClaimTypes.Role, r)));
                GenericPrincipal userPrincipal =
                                 new GenericPrincipal(identity, roles);
                ctx.Request.User = userPrincipal;
            }
        }
    }
}