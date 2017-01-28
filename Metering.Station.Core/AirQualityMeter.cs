using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metering.Station.Core.Devices;

namespace Metering.Station.Core
{
    public class AirQualityMeter : IAirQualityMeter
    {
        private readonly List<IAirQualitySensor> _sensors;
        public bool IsCollectingReadings { get; private set; }
        public IReadOnlyDictionary<string, AirQualityReading> RecentAirQualityReadings { get; private set; }

        private bool IsReadyToReceiveReadings { get; set; }

        private readonly object syncObj = new object();
        private readonly IReadOnlyDictionary<string, Queue<AirQualityReading>> _collectedReadings;
        private readonly int _defaultDelayBetweenReadings;
        private readonly IAirQualityIntervalBroker _intervalManager;

        public AirQualityMeter(IEnumerable<IAirQualitySensor> sensors, IAirQualityIntervalBroker intervalManager)
        {
            if (!sensors.Any())
                throw new ArgumentException("No air quality sensors provided - please pass at least one");
            _intervalManager = intervalManager;
            _sensors = sensors.ToList();
            IsCollectingReadings = false;
            IsReadyToReceiveReadings = false;
            _collectedReadings = _sensors.ToDictionary(k => k.DeviceName, v => new Queue<AirQualityReading>());
            RecentAirQualityReadings = sensors.ToDictionary(k => k.DeviceName, v => AirQualityReading.Empty);
        }

        public async Task CollectReadingsAsync()
        {
            lock (syncObj)
            {
                if (IsCollectingReadings)
                    throw new InvalidOperationException("Collecting data at the moment");
                IsCollectingReadings = true;
            }

            _sensors.ForEach(async p => await p.StartAsync(NewReadingArrived(p.DeviceName)));

            ClearStoredReadings();

            await _intervalManager.PauseToStabilizeReadings(); //wait for data to stabilize
            IsReadyToReceiveReadings = true;
            await _intervalManager.PauseToGatherReadings(); //collect data within timeout
            IsReadyToReceiveReadings = false;
            RecentAirQualityReadings = GetRecentReadings();

            _sensors.ForEach(async p => await p.StopAsync());

            lock (syncObj)
            {
                IsCollectingReadings = false;
            }
        }

        private void ClearStoredReadings()
        {
            foreach (var queue in _collectedReadings.Values)
                queue.Clear();
        }

        private IReadOnlyDictionary<string, AirQualityReading> GetRecentReadings()
        {
            return _collectedReadings.ToDictionary(k => k.Key, v =>
            {
                var avgPM10 = (int)v.Value.Average(p => p.PM10);
                var avgPM25 = (int)v.Value.Average(p => p.PM25);
                return new AirQualityReading(avgPM25, avgPM10);
            });
        }

        private Action<AirQualityReading> NewReadingArrived(string deviceName)
        {
            return (reading) =>
            {
                if (IsReadyToReceiveReadings)
                {
                   _collectedReadings[deviceName].Enqueue(reading);
                }
            };
        }
    }
}
