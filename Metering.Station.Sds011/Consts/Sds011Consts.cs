using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Metering.Station.Sds011.Consts
{
    internal class Sds011Consts
    {
        public const byte SerialStart = 0xAA;
        public const byte SerialEnd = 0xAB;
        public const byte SendByte = 0xB4;
        public const byte CommandTerminator = 0xFF;
        public const byte SettingMode = 1;
    }
}
