using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.UIShell
{
    public interface IChromiumWindow
    {
        /// <summary>
        /// 浏览器的ID
        /// </summary>
        string Identity { get; }

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

        /// <summary>
        /// UI线程移动
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="left"></param>
        /// <param name="top"></param>
        void InvokeMove(int width, int height, double left, double top);

        void InvokeOpenScreenKeyboard();
    }
}
