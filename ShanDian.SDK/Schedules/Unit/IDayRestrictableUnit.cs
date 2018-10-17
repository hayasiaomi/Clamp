namespace ShanDian.SDK.Schedules
{
    using System;

    /// <summary>
    /// Common interface of units that can be restricted by day.
    /// </summary>
    internal interface IDayRestrictableUnit
    {
        /// <summary>
        /// The schedule being affected.
        /// </summary>
        ScheduleJob Schedule { get; }

        /// <summary>
        /// Increment the given days.
        /// </summary>
        /// <param name="increment">Days to increment</param>
        /// <returns>The resulting date</returns>
        DateTime DayIncrement(DateTime increment);
    }
}