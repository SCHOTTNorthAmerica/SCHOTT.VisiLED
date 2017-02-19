namespace SCHOTT.VisiLED.Communication
{
    /// <summary>
    /// VisiLED common settings
    /// </summary>
    public class CommonObject
    {
        /// <summary>
        /// Rotation Period in Milliseconds
        /// </summary>
        public int RotationPeriod { get; set; }

        /// <summary>
        /// Strobe period in microseconds
        /// </summary>
        public int StrobePeriod { get; set; }

        /// <summary>
        /// Strobe on time in microseconds
        /// </summary>
        public int StrobeOnTime { get; set; }

        /// <summary>
        /// Checks for equality against another CommonObject
        /// </summary>
        /// <param name="commonObject">CommonObject to compare against</param>
        /// <returns>True if equal</returns>
        public bool Equals(CommonObject commonObject)
        {
            return RotationPeriod == commonObject.RotationPeriod &&
                   StrobePeriod == commonObject.StrobePeriod &&
                   StrobeOnTime == commonObject.StrobeOnTime;
        }

        /// <summary>
        /// Sets this CommonObject from another CommonObject
        /// </summary>
        /// <param name="commonObject">CommonObject to copy</param>
        public void SetFrom(CommonObject commonObject)
        {
            RotationPeriod = commonObject.RotationPeriod;
            StrobePeriod = commonObject.StrobePeriod;
            StrobeOnTime = commonObject.StrobeOnTime;
        }
    }
}
