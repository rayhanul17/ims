using Autofac;
using log4net;

namespace IMS.Web
{
    public class WebModule : Module
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(l => _logger).As<ILog>().SingleInstance();

            base.Load(builder);
        }
    }
}