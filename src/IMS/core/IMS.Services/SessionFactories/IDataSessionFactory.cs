using NHibernate;

namespace IMS.Services.SessionFactories
{
    public interface IDataSessionFactory
    {
        ISession OpenSession();
    }
}
