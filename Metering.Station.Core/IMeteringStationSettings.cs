using Metering.Station.Core.Devices;
using System.Collections.Generic;

namespace Metering.Station.Core
{
    public interface IMeteringStationSettings
    {
        IEnumerable<IDeviceSettings> DeviceSettings { get; }
    }

    public interface IDeviceSettings
    {
        string Port { get; }
        DeviceType DeviceType { get; }
        string Name { get; }
    }
}
