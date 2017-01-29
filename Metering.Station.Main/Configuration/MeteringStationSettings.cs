using System.Collections.Generic;
using Metering.Station.Core;
using Metering.Station.Core.Devices;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Metering.Station.Main.Configuration
{
    public class MeteringStationSettings : IMeteringStationSettings
    {
        public IEnumerable<IDeviceSettings> DeviceSettings { get; set; }

        protected MeteringStationSettings()
        {

        }

        public static MeteringStationSettings Build()
        {
            var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
                                                    .AddJsonFile("appsettings.json", optional: false);

            var devices = builder.Build().Get<List<DeviceSettings>>( "devices");

            return new MeteringStationSettings
            {
                DeviceSettings = devices
            };
        }
    }

    internal class DeviceSettings : IDeviceSettings
    {
        public string Name { get; set; }

        public string Port { get; set; }

        public DeviceType DeviceType { get; set; }
    }
}
