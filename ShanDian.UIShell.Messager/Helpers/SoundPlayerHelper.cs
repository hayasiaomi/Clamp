using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Media;
using System.Text;

namespace ShanDian.UIShell.Assist.Helpers
{
    public class SoundPlayerHelper
    {
        private static object objLock = new object();
        private static List<SoundPlayer> iSoundPlayers = new List<SoundPlayer>();
        private static BackgroundWorker iSPBackgroundWorker;

        public static void AddAndPlay(SoundPlayer soundPlayer)
        {
            Add(soundPlayer);
            Play();
        }

        public static void Add(SoundPlayer soundPlayer)
        {
            iSoundPlayers.Add(soundPlayer);
        }

        public static void Play()
        {
            if (iSPBackgroundWorker == null)
            {
                iSPBackgroundWorker = new BackgroundWorker();
                iSPBackgroundWorker.WorkerSupportsCancellation = true;
                iSPBackgroundWorker.DoWork += ISPBackgroundWorker_DoWork;
            }

            if (!iSPBackgroundWorker.IsBusy)
                iSPBackgroundWorker.RunWorkerAsync();

        }

        public static void Stop()
        {
            lock (objLock)
            {
                if (iSPBackgroundWorker != null)
                {
                    if (iSPBackgroundWorker.IsBusy)
                        iSPBackgroundWorker.CancelAsync();

                    iSPBackgroundWorker.Dispose();

                    iSPBackgroundWorker = null;
                }

                while (iSoundPlayers.Count > 0)
                    iSoundPlayers[0].Dispose();
            }
        }

        private static void ISPBackgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (iSoundPlayers.Count > 0)
            {
                using (SoundPlayer sp = iSoundPlayers[0])
                {
                    sp.PlaySync();
                    iSoundPlayers.Remove(sp);
                }
            }
        }
    }
}
