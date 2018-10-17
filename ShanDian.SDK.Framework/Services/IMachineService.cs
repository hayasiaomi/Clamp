using ShanDian.SDK.Framework.Model;
using System.Collections.Generic;

namespace ShanDian.SDK.Framework.Services
{
    public interface IMachineService
    {
        void SetupStore(Store store);

        Store GetStore();

        void SetupComputer(string code, string name, string ipString, int mainListener, string runMode);

        Computer GetComputerByCode(string code);

        List<Computer> GetComputers();
    }
}