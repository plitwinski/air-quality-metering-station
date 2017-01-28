using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Metering.Station.Core;
using Metering.Station.Core.Devices;
using Metering.Station.Test.Unit.Fixtures;
using Metering.Station.Test.Unit.Factory;
using Moq;

namespace Metering.Station.Test.Unit
{
    public class WhenCollectingDataFromMultipleSensors
    {
        private AirQualityMeter _airQualityMeter;
        private AirQualityMeterFixture _fixture;
        private AirQualitySensorFactory sensor1;
        private AirQualitySensorFactory sensor2;

        [OneTimeSetUp]
        public async Task When()
        {
            var broker = new FakeMeasuringInterval();

            int called = 0;
            Action resolveCallback = () =>
            {
                if (++called == 2)
                    broker.Resolve();
            };

            sensor1 = new AirQualitySensorFactory().AddCallbackResult(new AirQualityReading(1, 1))
                                                       .AddCallbackResult(new AirQualityReading(2, 2))
                                                       .AddCallbackResult(new AirQualityReading(3, 3))
                                                       .SetupFinishedCallback(resolveCallback);
            sensor2 = new AirQualitySensorFactory().AddCallbackResult(new AirQualityReading(2, 2))
                                                       .AddCallbackResult(new AirQualityReading(4, 4))
                                                       .AddCallbackResult(new AirQualityReading(6, 6))
                                                       .SetupFinishedCallback(resolveCallback);

            _fixture = new AirQualityMeterFixture().AddSensor(sensor1.Build())
                                                   .AddSensor(sensor2.Build())
                                                   .SetIntervalBroker(broker);

            _airQualityMeter = _fixture.Create();

            await _airQualityMeter.CollectReadingsAsync();
        }

        [Test]
        public void Then_there_is_one_reading_per_sensor()
        {
            Assert.AreEqual(_fixture.Sensors.Count, _airQualityMeter.RecentAirQualityReadings.Count);
        }

        [Test]
        public void Then_avarge_reading_is_calculated_properly_per_sensor()
        {
            Assert.AreEqual(new AirQualityReading(2, 2), _airQualityMeter.RecentAirQualityReadings.ElementAt(0).Value);
            Assert.AreEqual(new AirQualityReading(4, 4), _airQualityMeter.RecentAirQualityReadings.ElementAt(1).Value);
        }

        [Test]
        public void Then_start_was_called_once_per_sensors()
        {
            sensor1.SensorMock.Verify(p => p.StartAsync(It.IsAny<Action<AirQualityReading>>()), Times.Once);
            sensor2.SensorMock.Verify(p => p.StartAsync(It.IsAny<Action<AirQualityReading>>()), Times.Once);
        }

        [Test]
        public void Then_stop_was_called_once_per_sensors()
        {
            sensor1.SensorMock.Verify(p => p.StopAsync(), Times.Once);
            sensor2.SensorMock.Verify(p => p.StopAsync(), Times.Once);
        }
    }
}
