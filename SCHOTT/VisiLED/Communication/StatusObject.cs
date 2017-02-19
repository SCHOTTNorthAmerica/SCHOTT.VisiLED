namespace SCHOTT.VisiLED.Communication
{
    /// <summary>
    /// Status object that defines all controls available in the system.
    /// </summary>
    public class StatusObject
    {
        /// <summary>
        /// The status of Channel A
        /// </summary>
        public ChannelObject ChannelA { get; set; } = new ChannelObject();

        /// <summary>
        /// The status of Channel B
        /// </summary>
        public ChannelObject ChannelB { get; set; } = new ChannelObject();

        /// <summary>
        /// The status of the Common Controls
        /// </summary>
        public CommonObject Common { get; set; } = new CommonObject();
        
        /// <summary>
        /// Checks for equality against another StatusObject
        /// </summary>
        /// <param name="status">StatusObject to compare against</param>
        /// <returns>True if equal</returns>
        public bool Equals(StatusObject status)
        {
            return ChannelA.Equals(status.ChannelA) &&
                   ChannelB.Equals(status.ChannelB) &&
                   Common.Equals(status.Common);
        }

        /// <summary>
        /// Sets this StatusObject from another StatusObject
        /// </summary>
        /// <param name="status">StatusObject to copy</param>
        public void SetFrom(StatusObject status)
        {
            ChannelA.SetFrom(status.ChannelA);
            ChannelB.SetFrom(status.ChannelB);
            Common.SetFrom(status.Common);
        }
    }
}
