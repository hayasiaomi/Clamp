using Clamp.Common;
using Clamp.SDK.Listeners;
using Clamp.SDK.Pipelines;
using Clamp.SDK.Framework.Services;
using Clamp.Webwork;
using Clamp.Webwork.Bootstrapper;
using Clamp.Webwork.Extensions;
using Clamp.Webwork.Helpers;
using Clamp.Webwork.IO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using Clamp.Common.Commands;
using Clamp.SDK.Framework;
using Clamp.SDK.Services;
using Clamp.SDK.Schedules;
using Clamp.Common.Helpers;
using Clamp.SDK.Framework.Helpers;
using Clamp.AddIns;

namespace Clamp.SDK
{
    /// <summary>
    /// 善点执行者类
    /// </summary>
    class SDExecutor
    {
        private readonly ShanDianConfiguraction shanDianConfiguraction;
        private readonly MediumConfiguration mediumConfiguration;
        private readonly Dictionary<Type, ISDListener> listeners = new Dictionary<Type, ISDListener>();
        private SDBootstrapper bootstrapper;
        private IWebworkEngine engine;
        private SDTaskScheduler taskScheduler;

        public SDBootstrapper SDBootstrapper { get { return this.bootstrapper; } }

        public SDExecutor()
        {
            this.shanDianConfiguraction = new ShanDianConfiguraction();
            this.mediumConfiguration = new MediumConfiguration();
            this.bootstrapper = new SDBootstrapper();
        }

        public void Initialize()
        {
            //构造善点容器
            SDContainer container = new SDContainer();

            ObjectSingleton.ObjectProvider = container;

            container.Register(typeof(SDContainer), container, "SDContainer");

            //增加日志服务
            container.AddService(typeof(ILoggingService), new NLogLoggingService());

            SD.Log.Info("初始化善点的相关配置");

            this.shanDianConfiguraction.Initialize();
            this.mediumConfiguration.Initialize();

            SD.Log.Info("结束初始化配置");

            //注入执行类
            container.AddService(typeof(SDExecutor), this);

            //加载一些内定的服务
            container.AddService(typeof(ShanDianConfiguraction), this.shanDianConfiguraction);
            container.AddService(typeof(MediumConfiguration), this.mediumConfiguration);
            container.AddService(typeof(IWinFormService), new WinFormService());
            container.AddService(typeof(IPrinterService), new PrinterService());
            container.AddService(typeof(IMachineService), new MachineService());
            container.AddService(typeof(IMarkService), new MarkService());
            container.AddService(typeof(IInstallService), new InstallService());

            SD.Log.Info("初始化善点");

            this.bootstrapper.Initialise();
            this.engine = this.bootstrapper.GetEngine();

            SD.Log.Info("结束初始化善点");
        }
        /// <summary>
        /// 安装管道通信
        /// </summary>
        public void InstallPipelines()
        {
            if (!this.listeners.ContainsKey(typeof(SDPipelineListener)))
            {
                SDPipelineListener pipelineListener = new SDPipelineListener();
                pipelineListener.HandleMessage = this.HandlePiplineMessage;
                pipelineListener.StartListen();

                this.listeners.Add(typeof(SDPipelineListener), pipelineListener);

                SD.GetRequiredInstance<IWinFormService>().Setup(pipelineListener);

            }
            else if (!this.listeners[typeof(SDPipelineListener)].IsListening)
            {
                this.listeners[typeof(SDPipelineListener)].StartListen();
            }
        }



        /// <summary>
        /// 安装插件的HTTP功能
        /// </summary>
        public void InstallAddIns()
        {
            this.bootstrapper.InitializeAddIns();
        }


        /// <summary>
        /// 安装插件的MQTT功能
        /// </summary>
        public void InstallMqtt()
        {
            if (!this.listeners.ContainsKey(typeof(SDMqttListener)))
            {
                SDMqttListener mqttListener = new SDMqttListener();

                mqttListener.StartListen();

                this.listeners.Add(typeof(SDMqttListener), mqttListener);
            }
            else if (!this.listeners[typeof(SDMqttListener)].IsListening)
            {
                this.listeners[typeof(SDMqttListener)].StartListen();
            }
        }


        public void Abort()
        {
            if (this.listeners != null)
            {
                foreach (ISDListener listener in this.listeners.Values)
                {
                    listener.Shutdown();
                }
            }

            JobManager.Stop();

            SD.SDContainer.RemoveService(typeof(SDExecutor));
        }

        /// <summary>
        /// 执行善点
        /// </summary>
        public void Execute()
        {
            if (!this.listeners.ContainsKey(typeof(SDHttpListener)))
            {
                SDHttpListener httpListener = new SDHttpListener(this.mediumConfiguration);
                httpListener.HandleMessage = this.HandleHttpMessage;
                httpListener.StartListen();

                this.listeners.Add(typeof(SDHttpListener), httpListener);
            }
            else if (!this.listeners[typeof(SDHttpListener)].IsListening)
            {
                this.listeners[typeof(SDHttpListener)].StartListen();
            }

            TaskHelper.Run(() =>
            {
                SDDemand demand = SDHelper.GetDemand();

                if (demand != null)
                {
                    SD.Log.Info("开始安装管道通信");

                    this.InstallPipelines();

                    SD.Log.Info("结束安装管道通信");

                    //主机初始化
                    if (string.Equals(demand.RunMode, "main", StringComparison.CurrentCultureIgnoreCase))
                    {
                        if (!SD.IsActivited)
                        {
                            try
                            {
                                SD.Log.Info("开始初始化模块管理器");

                                AddInManager.Initialize();

                                SD.Log.Info("结束初始化模块管理器");

                                SD.Log.Info("开始安装模块内的HTTP功能");

                                this.InstallAddIns();

                                SD.Log.Info("结束安装模块内的HTTP功能");

                                SD.IsActivited = true;
                            }
                            catch (Exception ex)
                            {
                                SD.Log.Error("主机初始化时候出错", ex);
                                SD.IsActivited = false;
                            }
                        }
                    }

                    SD.Log.Info("开始善点后台工作线程");

                    if (this.taskScheduler == null)
                    {
                        this.taskScheduler = new SDTaskScheduler(this.shanDianConfiguraction, this.mediumConfiguration);

                        JobManager.Initialize(this.taskScheduler);
                        JobManager.Start();
                    }

                    SD.Log.Info("结束善点后台工作线程");
                }
                else
                {
                    SD.IsActivited = false;
                }
            });
        }

        private WebworkContext HandleHttpMessage(Request request)
        {
            return this.engine.HandleRequest(request);
        }

        private object HandlePiplineMessage(CommandPacket packet)
        {

            return null;
        }
    }
}
