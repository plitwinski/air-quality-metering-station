using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metering.Station.Core
{
    internal class AirQualityIntervalBroker : IAirQualityIntervalBroker
    {
        public Task PauseToStabilizeReadings()
        {
            return Task.Delay(60000);
        }

        public Task PauseToGatherReadings()
        {
            return Task.Delay(60000);
        }
    }
}
