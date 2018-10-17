using Newtonsoft.Json;
using ShanDian.Common.Commands;
using ShanDian.SDK.Framework.Advices;
using ShanDian.SDK.Framework.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ShanDian.SDK.Cmds
{
    public class TESTNOTICE : CommandBase
    {
        IWinFormService winFormService;

        public TESTNOTICE()
        {
            winFormService = SD.GetRequiredInstance<IWinFormService>();
        }

        protected override object DoHandle(string data)
        {
            //        {
            //	IconName:"icon-success",
            //	NoticeCategory:10,
            //	SerialNumber:"MSG_SO_0001",
            //	Title:"下单成功",
            //	ShutCount:3,
            //	Content:"<span style=\"color:red;\">##TableCode##</span><span>扫码下单成功。</span>"
            //}

            Notice notice = new Notice();

            notice.IconName = "icon-success";
            notice.Category = NoticeCategory.Order;
            notice.SerialNumber = "MSG_SO_0001";
            notice.Title = "下单成功";
            notice.ShutCount = 3;
            notice.Content = "扫码下单成功。";

            winFormService.DisplayNotice(notice);

            return null;
        }
    }
}
