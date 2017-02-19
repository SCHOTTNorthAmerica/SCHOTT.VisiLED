namespace SCHOTT.VisiLED.Enums
{
    /// <summary>
    /// Operation modes of the VisiLED Controller. Each channel has an indipendant mode.
    /// </summary>
    public enum Mode
    {
        /// <summary>
        /// Channel is off.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Channel is in continuous operation mode.
        /// </summary>
        Normal = 1,

        /// <summary>
        /// Channel is rotating clockwise.
        /// </summary>
        Clockwise = 2,

        /// <summary>
        /// Channel is rotating counter clockwise.
        /// </summary>
        CounterClockwise = 6,

        /// <summary>
        /// Channel is strobing.
        /// </summary>
        Strobe = 8,

        /// <summary>
        /// Channel is rotating clockwise while strobing.
        /// </summary>
        StrobeClockwise = 10,

        /// <summary>
        /// Channel is rotating counter clockwise while strobing.
        /// </summary>
        StrobeCounterClockwise = 14,

        /// <summary>
        /// Channel is in triggered strobe mode. 
        /// </summary>
        Trigger = 24,

        /// <summary>
        /// Channel is using its alternate pattern.
        /// </summary>
        Alternate = 32
    }

    /// <summary>
    /// Allows the user to select the driver channel
    /// </summary>
    public enum Channel
    {
        /// <summary>
        /// Channel A
        /// </summary>
        A,

        /// <summary>
        /// Channel B
        /// </summary>
        B
    }

    /// <summary>
    /// Indicates which mode to set the power for.
    /// </summary>
    public enum PowerMode
    {
        /// <summary>
        /// Normal or Continuous Power
        /// </summary>
        Normal,

        /// <summary>
        /// Strobe Power
        /// </summary>
        Strobe
    }

    /// <summary>
    /// Indicates direction of rotation
    /// </summary>
    public enum RotationDirection
    {
        /// <summary>
        /// Clockwise rotation
        /// </summary>
        Clockwise,

        /// <summary>
        /// Counter Clockwise Rotation
        /// </summary>
        CounterClockwise
    }

    /// <summary>
    /// Sample Segment Codes
    /// </summary>
    public enum SegmentCodes
    {
        /// <summary>
        /// No Segments are lit.
        /// </summary>
        None = 0x0,

        /// <summary>
        /// One segment is lit.
        /// </summary>
        Eigth = 0x1,

        /// <summary>
        /// Every other segment is lit.
        /// </summary>
        EigthSpaced = 0x55,

        /// <summary>
        /// One quarter is lit (2 adjoined segments)
        /// </summary>
        Quarter = 0x03,

        /// <summary>
        /// Two quarters are lit (oposite sides)
        /// </summary>
        QuarterSpaced = 0x33,

        /// <summary>
        /// Half of the unit is lit.
        /// </summary>
        Half = 0x0F,

        /// <summary>
        /// The full unit is lit.
        /// </summary>
        Full = 0xFF
    }

    /// <summary>
    /// Available memory storage locations. These corispond to M1 - M4 on the controller.
    /// </summary>
    public enum MemoryLocation
    {
        /// <summary>
        /// Memory Location 1
        /// </summary>
        M1,

        /// <summary>
        /// Memory Location 2
        /// </summary>
        M2,

        /// <summary>
        /// Memory Location 3
        /// </summary>
        M3,

        /// <summary>
        /// Memory Location 4
        /// </summary>
        M4
    }

}
