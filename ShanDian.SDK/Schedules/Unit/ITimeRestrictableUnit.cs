namespace ShanDian.SDK.Schedules
{
    /// <summary>
    /// Common interface of units that can be restricted by time.
    /// </summary>
    internal interface ITimeRestrictableUnit
    {
        /// <summary>
        /// The schedule being affected.
        /// </summary>
        ScheduleJob Schedule { get; }
    }
}