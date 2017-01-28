using Metering.Station.Sds011.Consts;

namespace Metering.Station.Sds011
{
    internal class SdsCommand
    {
        private SdsCommand(CommandType commandType, byte[] data = null)
        {
            CommandType = commandType;
            Data = data ?? new byte[0];
        }
        public CommandType CommandType { get; }
        public byte[] Data { get; private set; }

        public static SdsCommand CreatePauseCommand()
        {
            const int workingMode = 0; //0 - sleeping, 1 - measuring
            return (new SdsCommand(CommandType.WorkState, new byte[] { Sds011Consts.SettingMode, workingMode }));
        }

        public static SdsCommand CreateResumeCommand()
        {
            return new SdsCommand(CommandType.DutyCycle, new byte[] { Sds011Consts.SettingMode });
        }
    }
}
