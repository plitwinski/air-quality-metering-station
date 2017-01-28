using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metering.Station.Core;
using Metering.Station.Core.Devices;

namespace Metering.Station.Main.IoC
{
    internal class MeteringStationSettings : IMeteringStationSettings
    {
        private static readonly List<IDeviceSettings> _deviceSettings = new List<IDeviceSettings>()
        {
            new DeviceSettings("MainSDS011", "COM3", DeviceType.Sds011)
        };

        public IEnumerable<IDeviceSettings> DeviceSettings => _deviceSettings;
    }

    internal class DeviceSettings : IDeviceSettings
    {
        public DeviceSettings(string name, string port, DeviceType deviceType)
        {
            Name = name;
            Port = port;
            DeviceType = deviceType;
        }

        public string Name { get; }

        public string Port { get; }

        public DeviceType DeviceType { get; }
    }
}
