using System;
using Metering.Station.Main.IoC;
using Autofac;
using Metering.Station.Core;
using Metering.Station.Core.Devices;

namespace Metering.Station.Main
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var autofacContainer = AutofacContainer.Build();
            using (var container = autofacContainer.BeginLifetimeScope())
            {
                var sensor = container.Resolve<IAirQualitySensor>();
                var airQualityMeter = container.Resolve<IAirQualityMeter>();
                Console.WriteLine("Gathering data...");
                airQualityMeter.CollectReadingsAsync().Wait();
                foreach(var reading in airQualityMeter.RecentAirQualityReadings)
                   Console.WriteLine($"Device {reading.Key}: PM 2.5: {reading.Value.PM25} , PM 10: {reading.Value.PM10}");
            }
            Console.ReadLine();
        }
    }
}
