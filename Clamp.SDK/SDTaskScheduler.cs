using Clamp.SDK.Framework;
using Clamp.SDK.Schedules;
using Clamp.SDK.Updater;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.SDK
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
