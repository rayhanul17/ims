﻿using Autofac;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMS.Web
{
    public class WebModule : Module
    {
        private static readonly ILog _logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static readonly ILog _serviceLogger = LogManager.GetLogger("Logger2");

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(l => _logger).As<ILog>().SingleInstance();
            builder.Register(l => _serviceLogger).Named<ILog>("Service").SingleInstance();

            base.Load(builder);
        }
    }
}