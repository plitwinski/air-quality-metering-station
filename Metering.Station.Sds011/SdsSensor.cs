using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Threading.Tasks;
using System.IO.Ports;
using Metering.Station.Core.Devices;
using Metering.Station.Sds011.Consts;

namespace Metering.Station.Sds011
{
    internal class SdsSensor : IAirQualitySensor
    {
        #region consts

        private const int ReadReadingBufferSize = 10;
        private const int CommandDataMaxLength = 12;
        private const int CommandResponseLength = 12;

        #endregion

        public bool IsRunning { get; private set; }
        public string DeviceName { get; }
        public DeviceType DeviceType => DeviceType.Sds011;

        private bool IsReadingMode { get; set; }

        private readonly SerialPort port;
        private readonly Timer timer;
        private ElapsedEventHandler Timer_Elapsed;

        public SdsSensor(string portName, string deviceName)
        {
            port = new SerialPort(portName, 9600, Parity.None, 8, StopBits.One);
            timer = new Timer(1000);
            IsRunning = false;
            IsReadingMode = true;
            DeviceName = deviceName;
        }

        public async Task StartAsync(Action<AirQualityReading> callback)
        {
            if (port.IsOpen)
                return;
            port.Open();
            await Task.Delay(500);


            Timer_Elapsed = (sender, e) =>
            {
                GetReading(callback);
                HandleCommandResponse();
            };
            timer.Elapsed += Timer_Elapsed;
            await ResumeAsync();
        }

        public async Task StopAsync()
        {
            timer.Elapsed -= Timer_Elapsed;
            await PauseAsync();
            timer.Stop();
            if (port.IsOpen)
                port.Close();
        }

        public async Task PauseAsync()
        {
            if (IsRunning)
            {
                SendCommand(SdsCommand.CreatePauseCommand());
                await Task.Delay(500);
                IsRunning = false;
                timer.Stop();
            }
        }

        public Task ResumeAsync()
        {
            if (!IsRunning)
            {
                SendCommand(SdsCommand.CreateResumeCommand());
                timer.Start();
                IsRunning = true;
            }
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Stop();
            timer?.Dispose();
            if (port?.IsOpen == true)
                port.Close();
            port?.Dispose();
        }

        private void GetReading(Action<AirQualityReading> callback)
        {
            if (IsReadingMode && port.IsOpen)
            {
                var buffer = new byte[ReadReadingBufferSize];
                port.Read(buffer, 0, ReadReadingBufferSize);
                var reading = GetReadings(buffer);
                if (reading != AirQualityReading.Empty && reading != AirQualityReading.Zero)
                {
                    callback(reading);
                }
            }
        }

        private void HandleCommandResponse()
        {
            if (!IsReadingMode)
            {
                IsReadingMode = true;
            }
        }

        private void SendCommand(SdsCommand command)
        {
            if (command.Data.Length > CommandDataMaxLength)
                throw new ArgumentException($"Command {command.CommandType} data length cannot exceed {CommandDataMaxLength} (it's {command.Data.Length})");

            var rawCommandData = new Queue<byte>();
            rawCommandData.Enqueue(Consts.Sds011Consts.SerialStart);
            rawCommandData.Enqueue(Consts.Sds011Consts.SendByte);
            rawCommandData.Enqueue((byte)command.CommandType);
            var alignCommand = Enumerable.Range(0, CommandDataMaxLength).Select((p, index) => command.Data.GetValueOrDefault(index, (byte)0));
            rawCommandData.Enqueue(alignCommand);
            rawCommandData.Enqueue(Sds011Consts.CommandTerminator);
            rawCommandData.Enqueue(Sds011Consts.CommandTerminator);
            rawCommandData.Enqueue(GenerateCommandCrc(rawCommandData));
            rawCommandData.Enqueue(Sds011Consts.SerialEnd);
            var commandBuffer = rawCommandData.ToArray();
            IsReadingMode = false;
            port.Write(commandBuffer, 0, commandBuffer.Length);
        }

        private byte GenerateCommandCrc(IEnumerable<byte> commandData)
        {
            ValidateByte(commandData, 0, Sds011Consts.SerialStart);
            ValidateByte(commandData, 1, Sds011Consts.SendByte);
            var possibleCommandByteValues = Enum.GetValues(typeof(CommandType)).OfType<CommandType>().Select(p => (byte)p).ToArray();
            ValidateByte(commandData, 2, possibleCommandByteValues);
            var checksum = commandData.Skip(2).Select(p => (int)p).Sum();
            return (byte)(checksum % 256);
        }

        private void ValidateByte(IEnumerable<byte> commandData, int index, params byte[] possibleValues)
        {
            if (!possibleValues.Contains(commandData.ElementAt(index)))
                throw new ArgumentException($"Cannot generate command crc - {index} element {commandData.ElementAt(index)} is not equal to {Sds011Consts.SerialStart}");
        }

        private AirQualityReading GetReadings(byte[] buffer)
        {
            if (CheckReadingCrc(buffer))
            {
                int pm2_5 = (buffer[2] | (buffer[3] << 8)) / 10;
                int pm10 = (buffer[4] | (buffer[5] << 8)) / 10;
                return new AirQualityReading(pm2_5, pm10);
            }
            return AirQualityReading.Empty;

        }

        private bool CheckReadingCrc(byte[] rawData)
        {
            var dataValues = rawData.Skip(2).Take(ReadReadingBufferSize - 4).ToArray();
            var crc = dataValues.Aggregate((p1, p2) => (byte)(p1 + p2));
            var checkSum = rawData[ReadReadingBufferSize - 2];
            return crc == checkSum;
        }
    }
}
