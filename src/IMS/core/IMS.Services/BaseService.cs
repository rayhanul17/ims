using log4net;
using NHibernate;

namespace IMS.Services
{
    public abstract class BaseService
    {
        protected readonly ISession _session;
        protected readonly ITimeService _timeService;
        protected readonly ILog _serviceLogger = LogManager.GetLogger("ServiceLogger");
        public BaseService(ISession session)
        {
            _session = session;
            _timeService = new TimeService();
        }

    }
}
