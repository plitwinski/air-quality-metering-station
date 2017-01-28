using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metering.Station.Core.Devices
{
    public interface IAirQualitySensor : IDisposable
    {
        string DeviceName { get; }
        DeviceType DeviceType { get; }
        bool IsRunning { get; }

        Task PauseAsync();
        Task ResumeAsync();
        Task StartAsync(Action<AirQualityReading> callback);
        Task StopAsync();
    }
}
