using SCHOTT.Core.Communication;
using SCHOTT.Core.Communication.Serial;
using SCHOTT.Core.Extensions;
using SCHOTT.Core.Utilities;
using SCHOTT.VisiLED.Communication;
using SCHOTT.VisiLED.Enums;
using System;
using System.Linq;

namespace SCHOTT.VisiLED.Serial
{
    /// <summary>
    /// VisiLEDComPort extenstion class to add simplified connection methods for VisiLED units.
    /// </summary>
    public class VisiLEDComPort : ComPortBase
    {
        private readonly StatusObject _statusObject = new StatusObject();

        #region Initialization Functions

        /// <summary>
        /// Initialize a VisiLEDComPort using the supplied parameters.
        /// </summary>
        /// <param name="portName">The port name to connect too.</param>
        /// <param name="portParameters">The parameters to use when setting up the VisiLEDComPort</param>
        public VisiLEDComPort(string portName, ComParameters portParameters) : base(portName, portParameters)
        {
        }

        #endregion

        #region MessageBroker Functions

        /// <summary>
        /// Allows the user to register for updates to the connected VisiLED Channel. 
        /// </summary>
        /// <param name="channel">The channel to register for</param>
        /// <param name="context">The context in which to execture the action.</param>
        /// <param name="action">The action to execute on a message output event.</param>
        public void RegisterChannelStatusUpdate(Channel channel, MessageBroker.MessageContext context, Action<ChannelObject> action)
        {
            MessageBroker.Register($"VisiLED{channel}", context, action);
        }

        private void RunChannelStatusUpdate(Channel channel, ChannelObject args)
        {
            MessageBroker.RunActions($"VisiLED{channel}", args);
        }

        /// <summary>
        /// Allows the user to register for updates to the connected VisiLED Common Status. 
        /// </summary>
        /// <param name="context">The context in which to execture the action.</param>
        /// <param name="action">The action to execute on a message output event.</param>
        public void RegisterCommonStatusUpdate(MessageBroker.MessageContext context, Action<CommonObject> action)
        {
            MessageBroker.Register("VisiLEDCommon", context, action);
        }

        private void RunMasterCommonStatusUpdate(CommonObject args)
        {
            MessageBroker.RunActions("VisiLEDCommon", args);
        }

        /// <summary>
        /// Allows the user to register for updates to the connected VisiLED Full Status. 
        /// </summary>
        /// <param name="context">The context in which to execture the action.</param>
        /// <param name="action">The action to execute on a message output event.</param>
        public void RegisterFullStatusUpdate(MessageBroker.MessageContext context, Action<StatusObject> action)
        {
            MessageBroker.Register("VisiLEDStatus", context, action);
        }

        private void RunFullStatusUpdate(StatusObject args)
        {
            MessageBroker.RunActions("VisiLEDStatus", args);
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Extension to get the settings of the currently connected VisiLED unit. 
        /// This function will return the current status, but will also notify 
        /// any subscribers of the new section updates.
        /// </summary>
        /// <returns>The status object of the connected VisiLED unit.</returns>
        public StatusObject GetStatus()
        {
            // get first settings set
            var reply = SendCommandSingle("T0");
            var tokens = reply.Split(',').ToList();

            // see if the reply parses out to the correct number of strings
            if (tokens.Count != 14)
                return null;

            var status = new StatusObject
            {
                ChannelA = new ChannelObject
                {
                    Channel = Channel.A,
                    SegmentCode = (byte)(255 - byte.Parse(tokens[0])),
                    SegmentCodeAlt = (byte)(255 - byte.Parse(tokens[1])),
                    Power = int.Parse(tokens[2]),
                    StrobePower = (int)(int.Parse(tokens[3]) * 0.67),
                    Mode = (Mode)int.Parse(tokens[4])
                },
                ChannelB = new ChannelObject
                {
                    Channel = Channel.B,
                    SegmentCode = (byte)(255 - byte.Parse(tokens[5])),
                    SegmentCodeAlt = (byte)(255 - byte.Parse(tokens[6])),
                    Power = int.Parse(tokens[7]),
                    StrobePower = (int)(int.Parse(tokens[8]) * 0.67),
                    Mode = (Mode)int.Parse(tokens[9])
                },
                Common = new CommonObject
                {
                    RotationPeriod = int.Parse(tokens[11]) * 2,
                    StrobeOnTime = int.Parse(tokens[12]),
                    StrobePeriod = int.Parse(tokens[13])
                }
            };

            if (!_statusObject.Equals(status))
            {
                _statusObject.SetFrom(status);
                RunChannelStatusUpdate(Channel.A, status.ChannelA);
                RunChannelStatusUpdate(Channel.B, status.ChannelB);
                RunMasterCommonStatusUpdate(status.Common);
                RunFullStatusUpdate(status);
            }

            return status;
        }

        /// <summary>
        /// Set the output power of the VisiLED controller.
        /// </summary>
        /// <param name="channel">The channel to apply the new power setting too.</param>
        /// <param name="mode">The power mode to set the power for (normal or strobe).</param>
        /// <param name="power">The % power to apply to the channel (0-100).</param>
        public void SetPower(Channel channel, PowerMode mode, double power)
        {
            var boundPower = Math.Max(Math.Min(power, 100.0), 0.0);
            var commandString = mode == PowerMode.Strobe ? $"{CommandCase(channel, "p")}{boundPower * 3}" : $"{CommandCase(channel, "i")}{boundPower}";
            SendCommand(commandString, 0, true);
        }

        /// <summary>
        /// Rotates the segments of a given channel in the VisiLED controller.
        /// </summary>
        /// <param name="channel">The channel to apply the rotation too.</param>
        /// <param name="direction">The direction of rotation (CW or CCW)</param>
        /// <param name="count">How many segments to rotate by, 1 segment by default.</param>
        public void Rotate(Channel channel, RotationDirection direction, int count = 1)
        {
            var code = channel == Channel.A ? _statusObject.ChannelA.SegmentCode : _statusObject.ChannelB.SegmentCode;
            var newCode = direction == RotationDirection.Clockwise ? code.RotateLeft(count) : code.RotateRight(count);
            SetSegmentCode(channel, newCode);
        }

        /// <summary>
        /// Sets the segments using a binary code. Each bit represents a segment.
        /// </summary>
        /// <param name="channel">The channel to apply the code too.</param>
        /// <param name="code">The code to apply to the channel.</param>
        public void SetSegmentCode(Channel channel, byte code)
        {
            SendCommand($"{CommandCase(channel, "j")}{code}", 0, true);
            GetStatus();
        }

        /// <summary>
        /// Set the mode of the given channel to the selected mode.
        /// </summary>
        /// <param name="channel">The channel to apply the mode too.</param>
        /// <param name="mode">The mode to apply to the selected channel.</param>
        public void SetMode(Channel channel, Mode mode)
        {
            SendCommand($"{CommandCase(channel, "m")}{(int)mode}", 0, true);
            GetStatus();
        }

        /// <summary>
        /// Restore the settings from the provided memory location.
        /// </summary>
        /// <param name="location">The memory location to restore settings from.</param>
        public void RestoreFromMemory(MemoryLocation location)
        {
            SendCommand($"Y{(int)location}", 0, true);
            GetStatus();
        }

        /// <summary>
        /// Save the current unit settings to the provided memory location.
        /// </summary>
        /// <param name="location">Memory Location to store settings too.</param>
        public void SaveToMemory(MemoryLocation location)
        {
            SendCommand($"X{(int)location}", 0, true);
            GetStatus();
        }

        /// <summary>
        /// Saves the current configuration as the startup configuration.
        /// </summary>
        public void SaveStartupConfiguration()
        {
            SendCommand(@"X4", 0, true);
            GetStatus();
        }

        /// <summary>
        /// Sets the rotation period in milliseconds of the attached VisiLED device.
        /// </summary>
        /// <param name="periodInMilliseconds">Period in milliseconds of a complete rotation (2-500).</param>
        public void SetRotationPeriod(int periodInMilliseconds)
        {
            var period = Math.Max(Math.Min(periodInMilliseconds, 500), 2);
            SendCommand($"L{period}", 0, true);
        }

        /// <summary>
        /// Sets the strobe period in microseconds of the attached VisiLED device.
        /// </summary>
        /// <param name="periodInMicroseconds">Period in microseconds of a single strobe pulse (100-65000).</param>
        public void SetStrobePeriod(int periodInMicroseconds)
        {
            var period = Math.Max(Math.Min(periodInMicroseconds, 65000), 100);
            SendCommand($"O{period}", 0, true);
        }

        /// <summary>
        /// Sets the strobe on time in microseconds of the attached VisiLED device.
        /// </summary>
        /// <param name="onTimeInMicroseconds">The on time in microseconds of a single strobe pulse (80-65000).</param>
        public void SetStrobeOnTime(int onTimeInMicroseconds)
        {
            var onTime = Math.Max(Math.Min(onTimeInMicroseconds, 65000), 80);
            SendCommand($"N{onTime}", 0, true);
        }

        #endregion

        #region Static Functions

        private static string CommandCase(Channel channel, string command)
        {
            return channel == Channel.A ? command.ToUpper() : command.ToLower();
        }

        /// <summary>
        /// Find and connect to any VisiLED unit.
        /// </summary>
        public static VisiLEDComPort AutoConnectComPort()
        {
            var parameters = ComParameters();
            var comPorts = ComPortInfo.GetDescriptions();
            return comPorts.Any() ? AutoConnectComPort<VisiLEDComPort>(comPorts.Select(p => p.Port).ToList(), parameters) : null;
        }

        /// <summary>
        /// Find and connect to any VisiLED unit with the given parameters.
        /// </summary>
        /// <param name="portName">The port to connect to in format 'COM#'</param>
        public static VisiLEDComPort AutoConnectComPort(string portName)
        {
            var parameters = ComParameters();
            var comPorts = ComPortInfo.GetDescriptions().Where(p => p.Port == portName).ToList();
            return comPorts.Any() ? AutoConnectComPort<VisiLEDComPort>(comPorts.Select(p => p.Port).ToList(), parameters) : null;
        }

        /// <summary>
        /// Default ComParameters to use for VisiLED units.
        /// </summary>
        /// <returns>new ComParameter object</returns>
        public static ComParameters ComParameters()
        {
            return new ComParameters { Command = "t0", ExpectedResponce = "Rev", TimeoutMilliseconds = 100 };
        }

        #endregion

    }
}
