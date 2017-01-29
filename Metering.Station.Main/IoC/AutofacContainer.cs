using Autofac;
using Metering.Station.Core;
using Metering.Station.Main.Background;
using Metering.Station.Main.Configuration;
using Metering.Station.Sds011;

namespace Metering.Station.Main.IoC
{
    public static class AutofacContainer
    {
        public static IContainer Build()
        {
            var settings = GetMeteringStationSettings();
            var builder = new ContainerBuilder();
            builder.Register(_ => settings).As<IMeteringStationSettings>().SingleInstance();
            builder.RegisterModule<MainModule>();
            builder.RegisterModule<QuartzModule>();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule(new Sds011Module(settings));
            return builder.Build();
        }

        private static IMeteringStationSettings GetMeteringStationSettings()
        {
            return MeteringStationSettings.Build();
        }
    }
}
