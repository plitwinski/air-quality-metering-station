using System.Threading.Tasks;

namespace Metering.Station.Core
{
    internal class AirQualityIntervalBroker : IAirQualityIntervalBroker
    {
        public Task PauseToStabilizeReadings()
        {
            return Task.Delay(10000);
        }

        public Task PauseToGatherReadings()
        {
            return Task.Delay(10000);
        }
    }
}
