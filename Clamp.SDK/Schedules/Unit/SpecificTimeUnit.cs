using System;

namespace Clamp.SDK.Schedules
{
    /// <summary>
    /// Unit of specific time of the day.
    /// </summary>
    internal sealed class SpecificTimeUnit
    {
        internal SpecificTimeUnit(ScheduleJob schedule)
        {
            Schedule = schedule;
        }

        internal ScheduleJob Schedule { get; private set; }

        /// <summary>
        /// Also runs the job according to the given interval.
        /// </summary>
        /// <param name="interval">Interval to wait.</param>
        public TimeUnit AndEvery(int interval)
        {
            var parent = Schedule.Parent ?? Schedule;

            var child =
                new ScheduleJob(Schedule.Jobs)
                {
                    Parent = parent,
                    Reentrant = parent.Reentrant,
                    Name = parent.Name,
                };

            if (parent.CalculateNextRun != null)
            {
                var now = JobManager.Now;
                var delay = parent.CalculateNextRun(now) - now;

                if (delay > TimeSpan.Zero)
                    child.DelayRunFor = delay;
            }

            child.Parent.AdditionalSchedules.Add(child);
            return child.ToRunEvery(interval);
        }
    }
}
