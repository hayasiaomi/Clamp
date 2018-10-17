using System;
using System.Threading;
using ShanDian.SDK.Framework.Services;
using System.Threading.Tasks;
using ShanDian.SDK.Framework.Model;
using System.Collections.Generic;
using LiteDB;
using System.IO;
using ShanDian.SDK.Framework.Helpers;
using ShanDian.SDK.Framework;
using System.Linq;

namespace ShanDian.SDK.Framework.Services
{
    public class PrinterService : IPrinterService
    {
        private readonly IWinFormService winFormService;
        public PrinterService()
        {
            this.winFormService = ObjectSingleton.GetRequiredInstance<IWinFormService>();
        }

        public void AddPrinter(string pcid, string printName, string address, int port, int state)
        {
            using (LiteDatabase db = this.GetPrinterDatabase())
            {
                LiteCollection<PrinterInfo> printersCollection = db.GetCollection<PrinterInfo>(Constants.DatabasePrinterName);

                PrinterInfo printerInfo = printersCollection.FindOne(t => t.PCID == pcid && t.PrintName == printName);

                if (printerInfo != null)
                {
                    printerInfo.IP = address;
                    printerInfo.Port = port;
                    printerInfo.State = state;
                    printerInfo.Enable = 1;

                    printersCollection.Update(printerInfo);
                }
                else
                {
                    printerInfo = new PrinterInfo();

                    printerInfo.PCID = pcid;
                    printerInfo.PrintName = printName;
                    printerInfo.IP = address;
                    printerInfo.Port = port;
                    printerInfo.State = state;
                    printerInfo.Enable = 1;

                    BsonValue bsonValue = printersCollection.Insert(printerInfo);
                }
            }

        }

        public void AddPrinter(PrinterInfo printerInfo)
        {
            this.AddPrinter(printerInfo.PCID, printerInfo.PrintName, printerInfo.IP, printerInfo.Port, printerInfo.State);
        }

        public PrinterInfo GetPrinter(string pcid, string printName)
        {
            using (LiteDatabase db = this.GetPrinterDatabase())
            {
                LiteCollection<PrinterInfo> printersCollection = db.GetCollection<PrinterInfo>(Constants.DatabasePrinterName);

                return printersCollection.FindOne(t => t.PCID == pcid && t.PrintName == printName && t.Enable == 1);
            }
        }

        public List<PrinterInfo> GetPrinters()
        {
            List<PrinterInfo> printerInfos = new List<PrinterInfo>();

            using (LiteDatabase db = this.GetPrinterDatabase())
            {
                LiteCollection<PrinterInfo> printersCollection = db.GetCollection<PrinterInfo>(Constants.DatabasePrinterName);

                List<PrinterInfo> mPrinterInfos = printersCollection.Find(pi => pi.Enable == 1).ToList();

                if (mPrinterInfos != null && mPrinterInfos.Count > 0)
                {
                    printerInfos.AddRange(mPrinterInfos);
                }
            }

            return printerInfos;
        }

        private LiteDatabase GetPrinterDatabase()
        {
            string dbDir = Path.Combine(SDHelper.GetSDRootPath(), "DB");
            string dbFile = Path.Combine(dbDir, "printers.db");

            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);

            if (!File.Exists(dbFile))
                File.Create(dbFile).Close();

            return new LiteDatabase(dbFile);
        }
    }
}