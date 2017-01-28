using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autofac;

namespace Metering.Station.Core
{
    public class CoreModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AirQualityIntervalBroker>().As<IAirQualityIntervalBroker>().SingleInstance();
            builder.RegisterType<AirQualityMeter>().As<IAirQualityMeter>().SingleInstance();
        }
    }
}
