using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Metering.Station.Core;
using Metering.Station.Core.Devices;
using Moq;

namespace Metering.Station.Test.Unit.Factory
{
    public class AirQualitySensorFactory
    {
        public Mock<IAirQualitySensor> SensorMock { get; private set; }
        private readonly Queue<AirQualityReading> _callbackResults;
        private string _deviceName;
        private Action callbackFinished;

        public AirQualitySensorFactory()
        {
            _callbackResults = new Queue<AirQualityReading>();
            _deviceName = Guid.NewGuid().ToString();
        }

        public AirQualitySensorFactory AddCallbackResult(AirQualityReading reading)
        {
            _callbackResults.Enqueue(reading);
            return this;
        }

        public AirQualitySensorFactory SetDeviceName(string name)
        {
            _deviceName = name;
            return this;
        }

        public AirQualitySensorFactory SetupFinishedCallback(Action action)
        {
            callbackFinished = action;
            return this;
        }

        public IAirQualitySensor Build()
        {
            SensorMock = new Mock<IAirQualitySensor>();
            SensorMock.Setup(p => p.StartAsync(It.IsAny<Action<AirQualityReading>>()))
                .Callback<Action<AirQualityReading>>(action => {
                    Task.Run(() => {
                        foreach (var result in _callbackResults)
                            action(result);
                        callbackFinished?.Invoke();
                    });
                })
                .Returns(Task.CompletedTask);
            SensorMock.Setup(p => p.DeviceName).Returns(_deviceName);
            return SensorMock.Object;
        }
    }
}
