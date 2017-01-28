using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metering.Station.Core.Devices;
using Metering.Station.Core;

namespace Metering.Station.Test.Unit.Fixtures
{
    public class AirQualityMeterFixture
    {
        public Queue<IAirQualitySensor> Sensors { get; }
        private IAirQualityIntervalBroker _intervalBroker;

        public AirQualityMeterFixture()
        {
            Sensors = new Queue<IAirQualitySensor>();
        }

        public AirQualityMeterFixture AddSensor(IAirQualitySensor sensor)
        {
            Sensors.Enqueue(sensor);
            return this;
        }

        public AirQualityMeterFixture SetIntervalBroker(IAirQualityIntervalBroker broker)
        {
            _intervalBroker = broker;
            return this;
        }

        public AirQualityMeter Create()
        {
            return new AirQualityMeter(Sensors, _intervalBroker);
        }
    }
}
