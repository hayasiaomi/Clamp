using Chromium;
using Chromium.Remote;
using Chromium.Remote.Event;
using Chromium.WebBrowser;
using Clamp.MUI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.MUI.ChromiumCore
{
    internal class JsDatabaseObject : JSObject
    {
        public JsDatabaseObject()
        {
            this.AddFunction("get").Execute += JsDatabaseObject_GetExecute;
            this.AddFunction("add").Execute += JsDatabaseObject_AddExecute;
            this.AddFunction("remove").Execute += JsDatabaseObject_RemoveExecute;
            this.AddFunction("exist").Execute += JsDatabaseObject_ExistExecute;
            this.AddFunction("modify").Execute += JsDatabaseObject_ModifyExecute;
        }

        private void JsDatabaseObject_ModifyExecute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 1)
            {
                CDBHelper.Modify(Convert.ToString(e.Arguments[0]), Convert.ToString(e.Arguments[1]));
            }
        }

        private void JsDatabaseObject_ExistExecute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                e.SetReturnValue(CDBHelper.Exist(Convert.ToString(e.Arguments[0])));
            }
            else
            {
                e.SetReturnValue(false);
            }
        }

        private void JsDatabaseObject_RemoveExecute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                CDBHelper.Remove(Convert.ToString(e.Arguments[0]));
            }
        }

        private void JsDatabaseObject_AddExecute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 1)
            {
                CDBHelper.Add(Convert.ToString(e.Arguments[0]), Convert.ToString(e.Arguments[1]));
            }
        }

        private void JsDatabaseObject_GetExecute(object sender, CfrV8HandlerExecuteEventArgs e)
        {
            if (e.Arguments != null && e.Arguments.Length > 0)
            {
                CfrV8Value keyValue = e.Arguments[0];
                if (keyValue.IsString)
                    e.SetReturnValue(CDBHelper.Get(keyValue.StringValue));
            }
            else
            {
                e.SetReturnValue(string.Empty);
            }

        }

    }
}
