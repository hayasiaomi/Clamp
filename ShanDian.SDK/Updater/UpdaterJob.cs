using ShanDian.SDK.Framework;
using ShanDian.SDK.Framework.Helpers;
using ShanDian.SDK.Schedules;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Updater
{
    class UpdaterJob : IJob
    {
        private readonly ShanDianConfiguraction shanDianConfiguraction;
        private readonly MediumConfiguration mediumConfiguration;

        public UpdaterJob(ShanDianConfiguraction shanDianConfiguraction, MediumConfiguration mediumConfiguration)
        {
            this.shanDianConfiguraction = shanDianConfiguraction;
            this.mediumConfiguration = mediumConfiguration;
        }

        public void Execute()
        {
            UpdateChecker shanDainUpdateChecker;

            if (SDHelper.IsMain())
            {
                shanDainUpdateChecker = new MainUpdateChecker(
                    this.shanDianConfiguraction.UpdateProcessPath,
                    this.shanDianConfiguraction.UpdateCheckURL, Path.Combine(SD.GetSDRootPath(),
                    this.shanDianConfiguraction.UpdateDownloadLocation));
            }
            else
            {

                shanDainUpdateChecker = new SubUpdateChecker(
                    this.shanDianConfiguraction.UpdateProcessPath,
                    $"http://{SDHelper.GetDemand().Server}:{this.mediumConfiguration.MainListener}/sd/upgrades/check",
                    Path.Combine(SD.GetSDRootPath(), this.shanDianConfiguraction.UpdateDownloadLocation),
                   this.mediumConfiguration.MainListener);
            }

            if (shanDainUpdateChecker.CheckUpdate() == UpdateStatus.UpdateAvailable)
            {
                shanDainUpdateChecker.DownloadUpdate();
            }
        }
    }
}
