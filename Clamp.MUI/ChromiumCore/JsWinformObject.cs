using Chromium;
using Chromium.Remote;
using Chromium.Remote.Event;
using Chromium.WebBrowser;
using Clamp.MUI.Biz;
using Clamp.MUI.Framework.Inputs;
using Clamp.MUI.Framework.UI;
using Clamp.MUI.Helpers;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Clamp.MUI.ChromiumCore
{
    /// <summary>
    /// 根浏览器交动的事件
    /// </summary>
    internal class JsWinformObject : JSObject
    {
        private InputSimulator input = new InputSimulator();
        private WinFormBridge winFormBridge;

        public JsWinformObject(WinFormBridge winFormBridge)
        {
            this.winFormBridge = winFormBridge;

            this.InitializeProperty();
            this.InitializeFunction();
            this.InitializeObject();
        }


        #region Privite Method

        /// <summary>
        /// 初始化需要的属性
        /// </summary>
        private void InitializeProperty()
        {
            //终端PCID
            JSDynamicProperty pcidDynamicProperty = this.AddDynamicProperty("PCID");

            pcidDynamicProperty.PropertyGet += PcidDynamicProperty_PropertyGet;

            // 店门ID
            JSDynamicProperty orgExtCodeDynamicProperty = this.AddDynamicProperty("orgExtCode");

            orgExtCodeDynamicProperty.PropertyGet += OrgExtCodeDynamicProperty_PropertyGet;

            // 终端版本号
            JSDynamicProperty finalVersionDynamicProperty = this.AddDynamicProperty("finalVersion");

            finalVersionDynamicProperty.PropertyGet += FinalVersionDynamicProperty_PropertyGet;
        }

        /// <summary>
        /// 初始化需要的方法
        /// </summary>
        private void InitializeFunction()
        {
            this.AddFunction("setStartingup").Execute += Execute_SetStartingup;
            this.AddFunction("getStartingup").Execute += Execute_GetStartingup;
            this.AddFunction("setFloatSwitch").Execute += Execute_SetFloatSwitch;
            this.AddFunction("getFloatSwitch").Execute += Execute_GetFloatSwitch;
            this.AddFunction("authorize").Execute += Execute_Authorize;
            this.AddFunction("getLicenseNumberInfo").Execute += Execute_GetLicenseNumberInfo;
            this.AddFunction("removeLicenseUsername").Execute += Execute_RemoveLicenseUsername;
            this.AddFunction("getLicenseUsernames").Execute += Execute_GetLicenseUsernames;
            this.AddFunction("setWindowMaxmize").Execute += Execute_SetWindowMaxmize;
            this.AddFunction("setWindowMinimize").Execute += Execute_SetWindowMinimize;
            this.AddFunction("open").Execute += Execute_Open;
            this.AddFunction("redirectAuthorize").Execute += Execute_RedirectAuthorize;
            this.AddFunction("close").Execute += Execute_close;
        }

        private void Execute_SetFloatSwitch(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                bool state = e.Arguments[0].BoolValue;

                this.winFormBridge.SetFloatSwitch(state);
            }
        }

        private void Execute_GetFloatSwitch(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            e.SetReturnValue(this.winFormBridge.GetFloatSwitch());
        }

        private void Execute_RedirectAuthorize(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            this.winFormBridge.RedirectAuthorize();
        }

        private void Execute_Open(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 6)
            {
                string url = e.Arguments[0].StringValue;
                string title = e.Arguments[1].StringValue;
                int width = e.Arguments[2].IntValue;
                int height = e.Arguments[3].IntValue;
                double left = e.Arguments[4].DoubleValue;
                double top = e.Arguments[5].DoubleValue;
                CfrV8Value callback = e.Arguments[6];

                var context = CfrV8Context.GetCurrentContext();

                this.winFormBridge.Open(url, title, width, height, left, top, id =>
                {
                    CfxRemoteCallContext rc = callback.CreateRemoteCallContext();
                    try
                    {
                        rc.Enter();

                        CfrTask cfrTask = new CfrTask();

                        cfrTask.Execute += delegate (object src, CfrEventArgs evt)
                        {
                            CfrV8Value[] argments = new CfrV8Value[] { id };

                            callback.ExecuteFunctionWithContext(context, null, argments);
                        };

                        context.TaskRunner.PostTask(cfrTask);
                    }
                    finally
                    {
                        rc.Exit();
                    }
                });
            }
        }

        private void Execute_close(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            this.winFormBridge.WinFormClose();
        }

        private void Execute_SetWindowMinimize(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            this.winFormBridge.WinFormMinimized();
        }

        private void Execute_SetWindowMaxmize(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            this.winFormBridge.WinFormMaxmized();
        }

        private void Execute_GetLicenseUsernames(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            e.SetReturnValue(this.winFormBridge.GetLicenseUsernames());
        }

        private void Execute_RemoveLicenseUsername(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                string username = e.Arguments[0].StringValue;

                this.winFormBridge.RemoveLicenseUsername(username);
            }
        }

        private void Execute_GetLicenseNumberInfo(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                string username = e.Arguments[0].StringValue;

                e.SetReturnValue(this.winFormBridge.GetLicenseNumberInfo(username));
            }
        }

        private void JsWinformObject_Execute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            //e.SetReturnValue("Hello" + e.Arguments[0].StringValue);

            Debug.WriteLine("Test:" + e.Arguments != null);

            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                Debug.WriteLine("TestValue:" + e.Arguments[0].StringValue);
            }
        }

        /// <summary>
        /// 初始化需要的对象
        /// </summary>
        private void InitializeObject()
        {
            this.Add("events", new JsEventsObject());
            this.Add("database", new JsDatabaseObject());
        }

        private void Execute_SetStartingup(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                bool state = e.Arguments[0].BoolValue;
                this.winFormBridge.SetRestarting(state);
            }
        }

        /// <summary>
        /// 获得是否开机起动的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Execute_GetStartingup(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            e.SetReturnValue(this.winFormBridge.GetRestarting());
        }

        private void Execute_Authorize(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            Debug.WriteLine("Authorize " + (e.Arguments == null));

            if (e.Arguments != null && e.Arguments.Length > 3)
            {
                string username = e.Arguments[0].StringValue;
                string password = e.Arguments[1].StringValue;
                bool isMemberPassword = e.Arguments[2].BoolValue;
                CfrV8Value callback = e.Arguments[3];

                var context = CfrV8Context.GetCurrentContext();

                this.winFormBridge.Authorize(username, password, isMemberPassword, (flag, code, message) =>
                {
                    CfxRemoteCallContext rc = callback.CreateRemoteCallContext();
                    try
                    {
                        rc.Enter();

                        CfrTask cfrTask = new CfrTask();

                        cfrTask.Execute += delegate (object src, CfrEventArgs evt)
                        {
                            CfrV8Value[] argments = new CfrV8Value[]
                            {
                                flag,
                                code,
                                message
                            };

                            callback.ExecuteFunctionWithContext(context, null, argments);
                        };

                        context.TaskRunner.PostTask(cfrTask);
                    }
                    finally
                    {
                        rc.Exit();
                    }


                });
            }
        }

        private void JsEventsObject_OnExecute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments.Length == 2)
            {
                string eventName = Convert.ToString(e.Arguments[0]);
            }
        }

        private void FinalVersionDynamicProperty_PropertyGet(object sender, CfrV8AccessorGetEventArgs e)
        {
            e.Retval = CfrV8Value.CreateString(ChromiumSettings.FinalVersion);
            e.SetReturnValue(true);
        }

        private void OrgExtCodeDynamicProperty_PropertyGet(object sender, CfrV8AccessorGetEventArgs e)
        {
            e.Retval = CfrV8Value.CreateString(ChromiumSettings.OrgExtCode);
            e.SetReturnValue(true);
        }

        private void PcidDynamicProperty_PropertyGet(object sender, CfrV8AccessorGetEventArgs e)
        {
            e.Retval = CfrV8Value.CreateString(ChromiumSettings.PCID);
            e.SetReturnValue(true);
        }
        #endregion


    }
}
