using Autofac;
using IMS.AllServices;
using IMS.DataAccess.Repositories;
using IMS.Infrastructure.Utility;
using IMS.Services.SessionFactories;

namespace IMS.Services
{
    public class ServiceModule : Module
    {
        private readonly string _connectionString;

        public ServiceModule(string connectionString)
        {
            _connectionString = connectionString;
        }
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MsSqlSessionFactory>().As<IDataSessionFactory>()
               .WithParameter("connectionString", _connectionString)
               .SingleInstance();

            builder.RegisterType<TimeService>().As<ITimeService>()
               .InstancePerLifetimeScope();

            builder.RegisterType<CategoryService>().As<ICategoryService>()
               .InstancePerLifetimeScope();

            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
