using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate;
using NHibernate.Tool.hbm2ddl;

namespace ConsoleApp1.Models;

public class NHibernateHelper
{
private static ISessionFactory _sessionFactory;
private static ISessionFactory SessionFactory
{
    get
    {
        if (_sessionFactory == null)
        {
            string connectionString = "Data Source = DESKTOP-L0GNHBL\\SQLEXPRESS; Initial Catalog =TestProject; User Id=test; Password=123456";
            _sessionFactory = Fluently.Configure()
                .Database(MsSqlConfiguration.MsSql2012.ConnectionString(connectionString))
                .Mappings(m => m.FluentMappings.AddFromAssemblyOf<Program>())
                .ExposeConfiguration(cfg => new SchemaUpdate(cfg).Execute(false, true))
                .BuildSessionFactory();
        }
        return _sessionFactory;
    }
}

public static ISession GetSession()
{
    return SessionFactory.OpenSession();
}
}