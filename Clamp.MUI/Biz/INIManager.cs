using Clamp.MUI.Framework.INI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Clamp.MUI.Biz
{
    public class INIManager 
    {
        public const string DefaultSettingFileName = "ClampSettings.ini";

        private INIFile _settingFile;

        public void Initialize()
        {
            this._settingFile = INIFile.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetCallingAssembly().Location), DefaultSettingFileName));
        }

        public string Get(string key)
        {
            if (this._settingFile.Contains(key))
                return this._settingFile[key].StringValueTrimmed;
            return string.Empty;
        }

        public bool GetBoolean(string key)
        {
            if (this._settingFile.Contains(key))
                return this._settingFile[key].BoolValue;
            return false;
        }
    }
}
