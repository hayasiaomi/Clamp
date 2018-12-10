using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI
{
    public interface IChromiumWinForm
    {
        object InvokeMethod(Delegate method);

        object InvokeMethod(Delegate method, params object[] args);

        /// <summary>
        /// 打开一个窗体
        /// </summary>
        /// <param name="url"></param>
        /// <param name="title"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="openCallback"></param>
        void InvokeOpen(string url, string title, int width, int height, double left, double top, Action<string> openCallback);

        /// <summary>
        /// 跳转
        /// </summary>
        /// <param name="url"></param>
        void InvokeRedirect(string url);

        /// <summary>
        /// UI线程关闭
        /// </summary>
        void InvokeClose();

        /// <summary>
        /// 退出应用
        /// </summary>
        void InvokeQuit();


        /// <summary>
        /// UI线程执行最小化
        /// </summary>
        void InvokeMinimized();

        /// <summary>
        /// UI线程执行最大化
        /// </summary>
        void InvokeMaximized();

        void InvokeOpenScreenKeyboard();
    }
}
