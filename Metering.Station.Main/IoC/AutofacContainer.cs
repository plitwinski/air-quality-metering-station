using Autofac;
using Metering.Station.Core;
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
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule(new Sds011Module(settings));
            return builder.Build();
        }

        private static IMeteringStationSettings GetMeteringStationSettings()
        {
            //TODO read real configs
            return new MeteringStationSettings();
        }
    }
}
