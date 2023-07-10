using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IMS.DataAccess.Mappings;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace IMS.DataAccess.Utilities
{
    public class MsSqlSessionFactory : IDataSessionFactory
    {
        public ISessionFactory SessionFactory { get; }

        public MsSqlSessionFactory(string connectionString)
        {
            SessionFactory = Fluently
                .Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<MappingReference>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }
}
