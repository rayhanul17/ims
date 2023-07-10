using NHibernate;

namespace IMS.DataAccess.Utilities
{
    public interface IDataSessionFactory
    {
        ISession OpenSession();
    }
}
