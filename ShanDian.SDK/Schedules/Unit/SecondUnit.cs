namespace ShanDian.SDK.Schedules
{
    /// <summary>
    /// Unit of time in seconds.
    /// </summary>
    public sealed class SecondUnit : ITimeRestrictableUnit
    {
        private readonly int _duration;

        internal SecondUnit(ScheduleJob schedule, int duration)
        {
            _duration = duration;
            Schedule = schedule;
            Schedule.CalculateNextRun = x => x.AddSeconds(_duration);
        }

        internal ScheduleJob Schedule { get; private set; }

        ScheduleJob ITimeRestrictableUnit.Schedule { get { return this.Schedule; } }
    }
}
