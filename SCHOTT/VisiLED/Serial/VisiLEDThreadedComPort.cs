using SCHOTT.Core.Communication;
using SCHOTT.Core.Communication.Serial;
using SCHOTT.Core.Extensions;
using SCHOTT.Core.Threading;
using SCHOTT.Core.Utilities;
using SCHOTT.VisiLED.Communication;
using SCHOTT.VisiLED.Enums;
using System;
using System.Collections.Generic;

namespace SCHOTT.VisiLED.Serial
{
    /// <summary>
    /// VisiLED Implimentation of a ThreadedComPortBase. This port type should be used when the unit connection
    /// state is unknown. Functions are provided to be notified of connections and status changes. When using
    /// a static configuration, consider using the VisiLEDComPort classes instead.
    /// </summary>
    public class VisiLEDThreadedComPort : ThreadedComPortBase
    {
        #region Function Overrides for Derived Class

        /// <summary>
        /// Allows access to the CurrentConnection
        /// </summary>
        public new VisiLEDComPort CurrentConnection => (VisiLEDComPort)base.CurrentConnection;

        /// <summary>
        /// AutoConnect function called by the VisiLEDThreadedComPort class. A derived class can override this function
        /// to return a different derived type of VisiLEDComPort for the connect function. This allows for extension of
        /// the VisiLEDThreadedComPort.
        /// </summary>
        /// <param name="portsToCheck">Which ports to check for a connection.</param>
        /// <param name="portParameters">Which parameters to use when checking ports.</param>
        /// <returns></returns>
        protected override ComPortBase AutoConnectComPort(List<string> portsToCheck, ComParameters portParameters)
        {
            return ComPortBase.AutoConnectComPort<VisiLEDComPort>(portsToCheck, portParameters);
        }

        #endregion

        #region Initialization Functions

        /// <summary>
        /// Create a VisiLEDThreadedComPort for a CVLS unit. This port type should be used when the unit connection
        /// state is unknown. Functions are provided to be notified of connections and status changes. When using
        /// a static configuration, consider using the VisiLEDComPort classes instead.
        /// </summary>
        /// <param name="threadName">Name of the thread.</param>
        /// <param name="closingWorker">The ClosingWorker to add this thread too</param>
        public VisiLEDThreadedComPort(string threadName, ClosingWorker closingWorker)
            : base(threadName, closingWorker, VisiLEDComPort.ComParameters(), ConnectionMode.AnyCom)
        {
        }

        /// <summary>
        /// Create a VisiLEDThreadedComPort for a CVLS unit. This port type should be used when the unit connection
        /// state is unknown. Functions are provided to be notified of connections and status changes. When using
        /// a static configuration, consider using the VisiLEDComPort classes instead.
        /// </summary>
        /// <param name="threadName">Name of the thread.</param>
        /// <param name="closingWorker">The ClosingWorker to add this thread too</param>
        /// <param name="portName">The port to connect to in format 'COM#'</param>
        public VisiLEDThreadedComPort(string threadName, ClosingWorker closingWorker, string portName)
            : base(threadName, closingWorker, VisiLEDComPort.ComParameters(), ConnectionMode.SelectionRule, port => port.Port == portName)
        {
        }

        #endregion

        #region Messenger Functions

        /// <summary>
        /// Register events when a VisiLEDComPort connects
        /// </summary>
        protected override void ConnectionEventRegister()
        {
            base.ConnectionEventRegister();
            CurrentConnection.RegisterChannelStatusUpdate(Channel.A, MessageBroker.MessageContext.DirectToData, args => RunChannelStatusUpdate(Channel.A, args));
            CurrentConnection.RegisterChannelStatusUpdate(Channel.B, MessageBroker.MessageContext.DirectToData, args => RunChannelStatusUpdate(Channel.B, args));
            CurrentConnection.RegisterCommonStatusUpdate(MessageBroker.MessageContext.DirectToData, RunMasterStatusUpdate);
            CurrentConnection.GetStatus();
        }

        /// <summary>
        /// Allows the user to register for updates to the connected VisiLED Channel. 
        /// </summary>
        /// <param name="channel">The channel to register for</param>
        /// <param name="context">The context in which to execture the action.</param>
        /// <param name="action">The action to execute on a message output event.</param>
        public void RegisterChannelStatusUpdate(Channel channel, MessageBroker.MessageContext context, Action<ChannelObject> action)
        {
            MessageBroker.Register($"VisiLEDChannel{channel}", context, action);
        }

        private void RunChannelStatusUpdate(Channel channel, ChannelObject args)
        {
            MessageBroker.RunActions($"VisiLEDChannel{channel}", args);
        }

        /// <summary>
        /// Allows the user to register for updates to the connected VisiLED Common Status. 
        /// </summary>
        /// <param name="context">Allows the user to specify how the update should arrive for syncing with GUI applications.</param>
        /// <param name="action">The lambda expression to execute on updates.</param>
        public void RegisterMasterStatusUpdate(MessageBroker.MessageContext context, Action<CommonObject> action)
        {
            MessageBroker.Register("VisiLEDMaster", context, action);
        }

        private void RunMasterStatusUpdate(CommonObject args)
        {
            MessageBroker.RunActions("VisiLEDMaster", args);
        }

        #endregion

        #region External Functions

        /// <summary>
        /// Change the connection method of the VisiLEDThreadedComPort to select any VisiLED.
        /// </summary>
        public void ChangeMode()
        {
            ConnectionParameters.CopyFrom(VisiLEDComPort.ComParameters());
            base.ChangeMode(ConnectionMode.AnyCom);
        }

        /// <summary>
        /// Change the connection method of the VisiLEDThreadedComPort.
        /// </summary>
        /// <param name="portName">The port to connect to in format 'COM#'</param>
        public void ChangeMode(string portName)
        {
            ConnectionParameters.CopyFrom(VisiLEDComPort.ComParameters());
            base.ChangeMode(ConnectionMode.SelectionRule, port => port.Port == portName);
        }

        /// <summary>
        /// Extension to get the settings of the currently connected VisiLED unit. 
        /// This function will return the current status, but will also notify 
        /// any subscribers of the new section updates.
        /// </summary>
        /// <returns>The status object of the connected VisiLED unit.</returns>
        public StatusObject GetStatus()
        {
            return CurrentConnection?.GetStatus();
        }

        /// <summary>
        /// Set the output power of the VisiLED controller.
        /// </summary>
        /// <param name="channel">The channel to apply the new power setting too.</param>
        /// <param name="mode">The power mode to set the power for (normal or strobe).</param>
        /// <param name="power">The % power to apply to the channel (0-100).</param>
        public void SetPower(Channel channel, PowerMode mode, double power)
        {
            CurrentConnection?.SetPower(channel, mode, power);
        }

        /// <summary>
        /// Rotates the segments of a given channel in the VisiLED controller.
        /// </summary>
        /// <param name="channel">The channel to apply the rotation too.</param>
        /// <param name="direction">The direction of rotation (CW or CCW)</param>
        /// <param name="count">How many segments to rotate by, 1 segment by default.</param>
        public void Rotate(Channel channel, RotationDirection direction, int count = 1)
        {
            CurrentConnection?.Rotate(channel, direction, count);
        }

        /// <summary>
        /// Sets the segments using a binary code. Each bit represents a segment.
        /// </summary>
        /// <param name="channel">The channel to apply the code too.</param>
        /// <param name="code">The code to apply to the channel.</param>
        public void SetSegmentCode(Channel channel, byte code)
        {
            CurrentConnection?.SetSegmentCode(channel, code);
        }

        /// <summary>
        /// Set the mode of the given channel to the selected mode.
        /// </summary>
        /// <param name="channel">The channel to apply the mode too.</param>
        /// <param name="mode">The mode to apply to the selected channel.</param>
        public void SetMode(Channel channel, Mode mode)
        {
            CurrentConnection?.SetMode(channel, mode);
        }

        /// <summary>
        /// Restore the settings from the provided memory location.
        /// </summary>
        /// <param name="location">The memory location to restore settings from.</param>
        public void RestoreFromMemory(MemoryLocation location)
        {
            CurrentConnection?.RestoreFromMemory(location);
        }

        /// <summary>
        /// Save the current unit settings to the provided memory location.
        /// </summary>
        /// <param name="location">Memory Location to store settings too.</param>
        public void SaveToMemory(MemoryLocation location)
        {
            CurrentConnection?.SaveToMemory(location);
        }

        /// <summary>
        /// Saves the current configuration as the startup configuration.
        /// </summary>
        public void SaveStartupConfiguration()
        {
            CurrentConnection?.SaveStartupConfiguration();
        }

        /// <summary>
        /// Sets the rotation period in milliseconds of the attached VisiLED device.
        /// </summary>
        /// <param name="periodInMilliseconds">Period in milliseconds of a complete rotation (2-500).</param>
        public void SetRotationPeriod(int periodInMilliseconds)
        {
            CurrentConnection?.SetRotationPeriod(periodInMilliseconds);
        }

        /// <summary>
        /// Sets the strobe period in microseconds of the attached VisiLED device.
        /// </summary>
        /// <param name="periodInMicroseconds">Period in microseconds of a single strobe pulse (100-65000).</param>
        public void SetStrobePeriod(int periodInMicroseconds)
        {
            CurrentConnection?.SetStrobePeriod(periodInMicroseconds);
        }

        /// <summary>
        /// Sets the strobe on time in microseconds of the attached VisiLED device.
        /// </summary>
        /// <param name="onTimeInMicroseconds">The on time in microseconds of a single strobe pulse (80-65000).</param>
        public void SetStrobeOnTime(int onTimeInMicroseconds)
        {
            CurrentConnection?.SetStrobeOnTime(onTimeInMicroseconds);
        }

        #endregion

    }
}
