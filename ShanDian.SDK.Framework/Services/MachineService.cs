using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using LiteDB;
using ShanDian.SDK.Framework.Helpers;
using ShanDian.SDK.Framework.Model;

namespace ShanDian.SDK.Framework.Services
{
    public class MachineService : IMachineService
    {
        public void SetupStore(Store store)
        {
            using (LiteDatabase dbLite = this.GetShanDianLiteDatabase())
            {
                LiteCollection<Store> storeCollection = dbLite.GetCollection<Store>("Stores");

                Store liteStore = storeCollection.FindAll().FirstOrDefault();

                if (liteStore != null)
                {
                    liteStore.Logo = store.Logo;
                    liteStore.Name = store.Name;
                    liteStore.Phone = store.Phone;
                    liteStore.SecureKey = store.SecureKey;
                    liteStore.SubName = store.SubName;
                    liteStore.ActiveTime = store.ActiveTime;
                    liteStore.Address = store.Address;
                    liteStore.AppId = store.AppId;
                    liteStore.BrandId = store.BrandId;
                    liteStore.BrandName = store.BrandName;

                    storeCollection.Update(liteStore);
                }
                else
                {
                    liteStore = new Store();

                    liteStore.MikeRestId = store.MikeRestId;
                    liteStore.Logo = store.Logo;
                    liteStore.Name = store.Name;
                    liteStore.Phone = store.Phone;
                    liteStore.SecureKey = store.SecureKey;
                    liteStore.SubName = store.SubName;
                    liteStore.ActiveTime = store.ActiveTime;
                    liteStore.Address = store.Address;
                    liteStore.AppId = store.AppId;
                    liteStore.BrandId = store.BrandId;
                    liteStore.BrandName = store.BrandName;

                    storeCollection.Insert(liteStore);
                }

            }
        }

        public Store GetStore()
        {
            Store liteStore = null;

            using (LiteDatabase dbLite = this.GetShanDianLiteDatabase())
            {
                liteStore = dbLite.GetCollection<Store>("Stores").FindAll().FirstOrDefault();
            }

            return liteStore;
        }

        private LiteDatabase GetShanDianLiteDatabase()
        {
            string dbDir = Path.Combine(SDHelper.GetSDRootPath(), "DB");
            string dbFile = Path.Combine(dbDir, "shandian.db");

            if (!Directory.Exists(dbDir))
                Directory.CreateDirectory(dbDir);

            if (!File.Exists(dbFile))
                File.Create(dbFile).Close();

            return new LiteDatabase(dbFile);
        }

        public void SetupComputer(string code, string name, string ipString, int mainListener, string runMode)
        {
            using (LiteDatabase dbLite = this.GetShanDianLiteDatabase())
            {
                LiteCollection<Computer> machineCollection = dbLite.GetCollection<Computer>("Machines");

                Computer computer = machineCollection.FindOne(m => m.Code == code);

                if (computer != null)
                {
                    computer.Name = name;
                    computer.IpString = ipString;
                    computer.MainListener = mainListener;
                    computer.RunMode = runMode;

                    machineCollection.Update(computer);
                }
                else
                {
                    computer = new Computer();

                    computer.Code = code;
                    computer.Name = name;
                    computer.IpString = ipString;
                    computer.MainListener = mainListener;
                    computer.RunMode = runMode;

                    machineCollection.Insert(computer);
                }
            }
        }

        public Computer GetComputerByCode(string code)
        {
            Computer machine;

            using (LiteDatabase dbLite = this.GetShanDianLiteDatabase())
            {
                machine = dbLite.GetCollection<Computer>("Machines").FindOne(m => m.Code == code);
            }

            return machine;
        }

        public List<Computer> GetComputers()
        {
            List<Computer> computers = new List<Computer>();

            using (LiteDatabase dbLite = this.GetShanDianLiteDatabase())
            {
                LiteCollection<Computer> machineCollection = dbLite.GetCollection<Computer>("Machines");

                List<Computer> mComputers = machineCollection.Find(c => c.RunMode == "sub").ToList();

                if (mComputers != null && mComputers.Count > 0)
                {
                    computers.AddRange(mComputers);
                }
            }

            return computers;
        }
    }
}
