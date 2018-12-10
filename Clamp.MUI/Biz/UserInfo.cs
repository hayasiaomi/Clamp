using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Biz
{
    internal class UserInfo
    {
        //"UserId":13,"UserName":"超级管理员","EmployeeNo":"0000","Pwd":"e10adc3949ba59abbe56e057f20f883e","Sex":0,"Mobile":"18450098210","Status":1,"SuperFlag":0

        public UserInfo()
        {
            this.RoleData = new List<RoleInfo>();
        }

        public int UserId { set; get; }
        public string UserName { set; get; }
        public string EmployeeNo { set; get; }

        public string Pwd { set; get; }

        public string Sex { set; get; }


        public string Mobile { set; get; }

        public string Status { set; get; }

        public int SuperFlag { set; get; }

        public List<RoleInfo> RoleData { set; get; }

    }

   
}
