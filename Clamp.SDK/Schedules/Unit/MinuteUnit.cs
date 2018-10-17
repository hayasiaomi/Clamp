namespace Clamp.SDK.Schedules
{
    /// <summary>
    /// Unit of time in minutes.
    /// </summary>
    public sealed class MinuteUnit : ITimeRestrictableUnit
    {
        private readonly int _duration;

        internal MinuteUnit(ScheduleJob schedule, int duration)
        {
            _duration = duration;
            Schedule = schedule;
            Schedule.CalculateNextRun = x => x.AddMinutes(_duration);
        }

        internal ScheduleJob Schedule { get; private set; }

        ScheduleJob ITimeRestrictableUnit.Schedule { get { return this.Schedule; } }
    }
}
