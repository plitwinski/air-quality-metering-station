using Metering.Station.Core;
using Nancy;

namespace Metering.Station.Api.Modules
{
    public class ReadingsModule : NancyModule
    {
        public ReadingsModule(IAirQualityMeter meter)
            :base("v1/readings")
        {
            Get["/"] = _ => meter.RecentAirQualityReadings;
        }
    }
}
