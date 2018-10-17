using ShanDian.SDK.Framework;
using ShanDian.SDK.Schedules;
using ShanDian.SDK.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK
{
    class SDTaskScheduler : Scheduler
    {
        public SDTaskScheduler(ShanDianConfiguraction shanDianConfiguraction, MediumConfiguration mediumConfiguration)
        {

            this.Schedule(() => { return new UpdaterJob(shanDianConfiguraction, mediumConfiguration); })
                .WithName("sdupdater")
                .ToRunOnceIn(shanDianConfiguraction.UpdateInterval)
                .Seconds();
        }
    }
}
