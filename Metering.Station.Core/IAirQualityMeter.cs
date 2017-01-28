using System.Threading.Tasks;
using System.Collections.Generic;
using Metering.Station.Core.Devices;

namespace Metering.Station.Core
{
    public interface IAirQualityMeter
    {
        bool IsCollectingReadings { get; }
        IReadOnlyDictionary<string, AirQualityReading> RecentAirQualityReadings { get; }
        Task CollectReadingsAsync();
    }
}