using log4net;
using NHibernate;

namespace IMS.Services
{
    public abstract class BaseService
    {  
        public BaseService(ISession session)
        {

        }

        protected readonly ILog _serviceLogger = LogManager.GetLogger("ServiceLogger");
    }
}