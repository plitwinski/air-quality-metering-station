using System.Threading.Tasks;

namespace Metering.Station.Core
{
    public interface IAirQualityIntervalBroker
    {
        Task PauseToGatherReadings();
        Task PauseToStabilizeReadings();
    }
}