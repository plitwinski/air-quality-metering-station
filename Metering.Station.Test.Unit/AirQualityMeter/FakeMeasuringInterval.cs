using System;
using System.Threading.Tasks;
using Metering.Station.Core;
using System.Threading;

namespace Metering.Station.Test.Unit
{
    public class FakeMeasuringInterval : IAirQualityIntervalBroker
    {
        private readonly AutoResetEvent are = new AutoResetEvent(false);

        public Task PauseToGatherReadings()
        {
            return Task.Run(() => are.WaitOne());
        }

        public Task PauseToStabilizeReadings()
        {
            are.Reset();
            return Task.CompletedTask;
        }

        public void Resolve()
        {
            are.Set();
        }
    }
}
