using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Web;
using ShanDian.Common.Exceptions;
using ShanDian.Common.Extensions;
using ShanDian.SDK.Framework.Services;

namespace ShanDian.Audio
{
    public class AudioHelper
    {
        private static string _audioFolder = AppDomain.CurrentDomain.BaseDirectory + "\\audio";

        public static void Text2AudioAndPlay(List<string> textList,int spd = 5, int pit = 5,int vol = 5,int per = 0,int interval = 500,bool overrided = false)
        {
            Text2Audio(textList,spd,pit,vol,per, overrided);
            PlayAudio(textList,interval);
        }

        public static void Text2Audio(List<string> textList, int spd = 5, int pit = 5, int vol = 5, int per = 0, bool overrided = false)
        {
            LoggingService.Debug("开始语音合成");
            BaiduAudio.GrantToken();
            foreach (var text in textList)
            {
                if (text.Length > 1024)
                {
                    throw new ShanDianException(400, "语音文本不能超过1024个字符");
                }
                var param = $"tex={HttpUtility.UrlEncode(text, Encoding.UTF8)}"
                            + $"&tok={BaiduAudio.Access_token}&cuid={Guid.NewGuid()}&ctp=1&lan=zh&spd={spd}&pit={pit}&vol={vol}&per={per}&aue=6";
                var textMd5 = text.ToLower().Md5Encrypt().ToLower();
                var audioPath = $"{_audioFolder}\\{textMd5}.wav";
                BaiduAudio.DownloadAudio(param, audioPath, overrided);
            }
            LoggingService.Debug("语音合成完成");
        }

        public static void PlayAudio(List<string> textList,int interval = 500)
        {
            LoggingService.Debug("开始播放语音");
            var audioPaths = new List<string>();
            foreach (var text in textList)
            {
                var textMd5 = text.ToLower().Md5Encrypt().ToLower();
                var audioPath = $"{_audioFolder}\\{textMd5}.wav";
                if (!File.Exists(audioPath))
                {
                    throw new ShanDianException(400,"找不到音频文件");
                }
                audioPaths.Add(audioPath);
            }
            Play(audioPaths, interval);
            LoggingService.Debug("语音播放完毕");
        }

       
//        public static uint SND_ASYNC = 0x0001;
//        public static uint SND_FILENAME = 0x00020000;
        [DllImport("winmm.dll", CharSet = CharSet.Auto)]
        public static extern uint mciSendString(string lpstrCommand,
            string lpstrReturnString, uint uReturnLength, uint hWndCallback);
        public static void Play(List<string> audioPaths, int interval = 500)
        {
            LoggingService.Debug($"播放语音文件：{audioPaths.ToJson()}");
            foreach (var audioPath in audioPaths)
            {
                mciSendString("close all", null, 0, 0);
                mciSendString("open \"" + audioPath + "\" alias media", null, 0, 0);
                mciSendString("play media", null, 0, 0);

                bool wait = true;
                var currow = 1;
                while (wait && currow < 10)
                {
                    string temp = "".PadLeft(32, ' ');
                    mciSendString("status media mode", temp, 32, 0);
//                    AllLog.Writer.SendDebug($"文件播放状态：{temp}");
                    if (temp.Trim().ToLower().StartsWith("stopped"))
                    {
                        wait = false;
                    }
                    currow++;
                    Thread.Sleep(200);
                }
                Thread.Sleep(interval);
            }
            mciSendString("close all", null, 0, 0);


            //            mciSendString(@"close temp_alias", null, 0, 0);
            //            mciSendString(@"open """ + audioPath + @""" alias temp_alias", null, 0, 0);
            //            mciSendString("play temp_alias repeat", null, 0, 0);
        }
    }
}