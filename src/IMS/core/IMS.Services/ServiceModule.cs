using Autofac;
using IMS.DataAccess.Repositories;
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

            builder.RegisterType<CategoryService>().AsSelf()
                .InstancePerLifetimeScope();

            builder.RegisterType<CategoryRepository>().As<ICategoryRepository>()
                .InstancePerLifetimeScope();

            base.Load(builder);
        }
    }
}
