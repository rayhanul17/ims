using Autofac;
using Autofac.Integration.Mvc;
using IMS.Services;
using IMS.Services.SessionFactories;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace IMS.Web
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

            #region Autofac
            var builder = new ContainerBuilder();

            // Register your MVC controllers. (MvcApplication is the name of
            // the class in Global.asax.)
            builder.RegisterControllers(typeof(MvcApplication).Assembly);

            //Register model binders that require DI.
            builder.RegisterModelBinders(typeof(MvcApplication).Assembly);
            builder.RegisterModelBinderProvider();

            //Register web abstractions like HttpContextBase.
            builder.RegisterModule<AutofacWebTypesModule>();

            //Enable property injection in view pages.
            builder.RegisterSource(new ViewRegistrationSource());

            //Add extra Module class
            builder.RegisterModule(new WebModule());
            builder.RegisterModule(new ServiceModule(connectionString));

            builder.Register(x => new MsSqlSessionFactory(connectionString).OpenSession()).SingleInstance();


            var container = builder.Build();
            DependencyResolver.SetResolver(new AutofacDependencyResolver(container));
            #endregion

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
