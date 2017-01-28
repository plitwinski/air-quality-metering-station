using System.Linq;
using Autofac;
using Metering.Station.Core;
using Metering.Station.Core.Devices;

namespace Metering.Station.Sds011
{
    public class Sds011Module : Module
    {
        private readonly IMeteringStationSettings _settings;
        public Sds011Module(IMeteringStationSettings settings)
        {
            _settings = settings;
        }

        protected override void Load(ContainerBuilder builder)
        {
            var devices = _settings.DeviceSettings.Where(p => p.DeviceType == DeviceType.Sds011);
            foreach(var device in devices) 
               builder.RegisterType<SdsSensor>().As<IAirQualitySensor>()
                      .WithParameter("portName", device.Port)
                      .WithParameter("deviceName", device.Name)
                      .SingleInstance();
        }
    }
}
