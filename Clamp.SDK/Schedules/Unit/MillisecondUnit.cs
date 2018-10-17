namespace Clamp.SDK.Schedules
{
    /// <summary>
    /// Unit of time in milliseconds.
    /// </summary>
    public sealed class MillisecondUnit : ITimeRestrictableUnit
    {
        private readonly int _duration;

        internal MillisecondUnit(ScheduleJob schedule, int duration)
        {
            _duration = duration;
            Schedule = schedule;
            Schedule.CalculateNextRun = x => x.AddMilliseconds(_duration);
        }

        internal ScheduleJob Schedule { get; private set; }

        ScheduleJob ITimeRestrictableUnit.Schedule { get { return this.Schedule; } }
    }
}
