namespace ShanDian.SDK.Schedules
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;

    /// <summary>
    /// A Scheduler of job schedules.
    /// </summary>
    internal class Scheduler
    {
        private bool _allJobsConfiguredAsNonReentrant;

        internal List<ScheduleJob> Schedules { get; private set; }

        /// <summary>
        /// Default ctor.
        /// </summary>
        public Scheduler()
        {
            _allJobsConfiguredAsNonReentrant = false;
            Schedules = new List<ScheduleJob>();
        }

        /// <summary>
        /// Sets all jobs in this schedule as non reentrant.
        /// </summary>
        public void NonReentrantAsDefault()
        {
            _allJobsConfiguredAsNonReentrant = true;
            lock (((ICollection)Schedules).SyncRoot)
            {
                foreach (var schedule in Schedules)
                    schedule.NonReentrant();
            }
        }

        /// <summary>
        /// Schedules a new job in the Scheduler.
        /// </summary>
        /// <param name="job">Job to run.</param>
        public ScheduleJob Schedule(Action job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            return Schedule(job, null);
        }

        /// <summary>
        /// Schedules a new job in the Scheduler.
        /// </summary>
        /// <param name="job">Job to run.</param>
        public ScheduleJob Schedule(IJob job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            return Schedule(JobManager.GetJobAction(job), null);
        }

        /// <summary>
        /// Schedules a new job in the Scheduler.
        /// </summary>
        /// <typeparam name="T">Job to schedule.</typeparam>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter",
            Justification = "The 'T' requirement is on purpose.")]
        public ScheduleJob Schedule<T>() where T : IJob
        {
            return Schedule(JobManager.GetJobAction<T>(), typeof(T).Name);
        }

        /// <summary>
        /// Schedules a new job in the Scheduler.
        /// </summary>
        /// <param name="job">Factory method creating a IJob instance to run.</param>
        public ScheduleJob Schedule(Func<IJob> job)
        {
            if (job == null)
                throw new ArgumentNullException("job");

            return Schedule(JobManager.GetJobAction(job), null);
        }

        private ScheduleJob Schedule(Action action, string name)
        {
            var schedule = new ScheduleJob(action);

            if (_allJobsConfiguredAsNonReentrant)
                schedule.NonReentrant();

            lock (((ICollection)Schedules).SyncRoot)
            {
                Schedules.Add(schedule);
            }

            schedule.Name = name; 

            return schedule;
        }
    }
}
