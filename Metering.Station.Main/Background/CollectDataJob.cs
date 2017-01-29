using log4net;
using Metering.Station.Core;
using Quartz;
using System;

namespace Metering.Station.Main.Background
{
    public class CollectDataJob : IJob
    {
        private readonly IAirQualityMeter _airQualityMeter;
        private readonly ILog _logger = LogManager.GetLogger(nameof(CollectDataJob));

        public CollectDataJob(IAirQualityMeter airQualityMeter)
        {
            _airQualityMeter = airQualityMeter;
        }

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                _airQualityMeter.CollectReadingsAsync().Wait();
            }
            catch(Exception ex)
            {
                _logger.Error("Cannot collect data", ex);
            }
        }
    }
}
