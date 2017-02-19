using SCHOTT.VisiLED.Enums;

namespace SCHOTT.VisiLED.Communication
{
    /// <summary>
    /// VisiLED Channel Settings
    /// </summary>
    public class ChannelObject
    {
        /// <summary>
        /// The Channel the object belongs too.
        /// </summary>
        public Channel Channel { get; set; }

        /// <summary>
        /// The segment code for this channel.
        /// </summary>
        public byte SegmentCode { get; set; }

        /// <summary>
        /// The alternate segment code for this channel.
        /// </summary>
        public byte SegmentCodeAlt { get; set; }

        /// <summary>
        /// The mode of this channel.
        /// </summary>
        public Mode Mode { get; set; }

        /// <summary>
        /// The output power of this channel.
        /// </summary>
        public int Power { get; set; }

        /// <summary>
        /// The output power of this channel.
        /// </summary>
        public int StrobePower { get; set; }

        /// <summary>
        /// Checks for equality against another ChannelObject
        /// </summary>
        /// <param name="channelObject">ChannelObject to compare against</param>
        /// <returns>True if equal</returns>
        public bool Equals(ChannelObject channelObject)
        {
            return SegmentCode == channelObject.SegmentCode &&
                   SegmentCodeAlt == channelObject.SegmentCodeAlt &&
                   Mode == channelObject.Mode &&
                   Power == channelObject.Power &&
                   StrobePower == channelObject.StrobePower;
        }

        /// <summary>
        /// Sets channel object from another ChannelObject
        /// </summary>
        /// <param name="channelObject">ChannelObject to copy</param>
        public void SetFrom(ChannelObject channelObject)
        {
            SegmentCode = channelObject.SegmentCode;
            SegmentCodeAlt = channelObject.SegmentCodeAlt;
            Mode = channelObject.Mode;
            Power = channelObject.Power;
            StrobePower = channelObject.StrobePower;
        }
    }
}
