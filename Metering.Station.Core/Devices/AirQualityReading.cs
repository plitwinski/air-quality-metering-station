using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metering.Station.Core.Devices
{
    public struct AirQualityReading
    {
        public int PM25 { get; }
        public int PM10 { get; }

        public AirQualityReading(int pm25, int pm10)
        {
            PM25 = pm25;
            PM10 = pm10;
        }

        public override bool Equals(object obj)
        {
            var item = obj as AirQualityReading?;
            if (item.HasValue)
                return item.Value.PM10 == PM10 && item.Value.PM25 == PM25;
            return false;
        }

        public override int GetHashCode()
        {
            return PM10 ^ PM25;
        }

        public static bool operator ==(AirQualityReading r1, AirQualityReading r2)
        {
            return r1.Equals(r2);
        }

        public static bool operator !=(AirQualityReading r1, AirQualityReading r2)
        {
            return !r1.Equals(r2);
        }

        public static AirQualityReading Empty = new AirQualityReading(-1, -1);
        public static AirQualityReading Zero = new AirQualityReading(0, 0);
    }
}
