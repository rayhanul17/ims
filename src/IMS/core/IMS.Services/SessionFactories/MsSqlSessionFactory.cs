using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using IMS.BusinessRules;
using IMS.Dao.Mappings;
using NHibernate;
using NHibernate.Tool.hbm2ddl;
using System.Data.SqlClient;

namespace IMS.Services.SessionFactories
{
    public interface IDataSessionFactory
    {
        ISession OpenSession();
    }

    public class MsSqlSessionFactory : IDataSessionFactory
    {
        public ISessionFactory SessionFactory { get; }

        public MsSqlSessionFactory()
        {
            SessionFactory = Fluently
                .Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(DbConnectionString.ConnectionString))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<MappingRefference>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildSessionFactory();
        }

        public ISession OpenSession()
        {
            return SessionFactory.OpenSession();
        }
    }


}
