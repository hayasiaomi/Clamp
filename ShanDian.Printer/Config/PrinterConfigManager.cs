using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;
using ShanDian.Printer.Config;

namespace ShanDian.Print
{
    public static class PrinterConfigManager
    {

        private static Dictionary<string, Dictionary<string, PrinterConfig>> _printConfigs;

        private static void Init()
        {
            if (_printConfigs == null)
            {
                _printConfigs = new Dictionary<string, Dictionary<string, PrinterConfig>>();
                //读取配置文件获取打印配置
                var configPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\PrintCmd";
                var files = Directory.GetFiles(configPath, "*.json");

                foreach (var file in files)
                {
                    var str = File.ReadAllText(file, Encoding.UTF8);
                    var dict = JsonConvert.DeserializeObject<Dictionary<string, PrinterConfig>>(str);
                    var fileInfo = new FileInfo(file);
                    var fileName = fileInfo.Name;
                    var index = fileName.LastIndexOf(".");
                    var name = fileName.Substring(0, index);
                    _printConfigs[name] = dict;
                }
                
            }
        }
        public static PrinterConfig GetConfig(string printerBrand, PrintType printType)
        {
            Init();
            if (!_printConfigs.ContainsKey(printerBrand))
            {
                return new PrinterConfig();
            }
            var dict = _printConfigs[printerBrand];
            if (!dict.ContainsKey(printType.ToString().ToLower()))
            {
                return new PrinterConfig();
            }
            return dict[printType.ToString().ToLower()];


        }

    }
}
