using log4net;
using NHibernate;

namespace IMS.Services
{
    public abstract class BaseService
    {
        protected readonly ISession _session;
        public BaseService(ISession session)
        {
            _session = session;
        }

        protected readonly ILog _serviceLogger = LogManager.GetLogger("ServiceLogger");
    }
}