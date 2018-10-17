using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.View;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;

namespace ShanDian.AddIns.Print.Interface
{
    [RoutePrefix("","1.0.0.2", "Print")]
    public interface IPrintSchemeServicesV2
    {
        [Get("GetLocalPrintV2", "获取本地默认打印机")]
        PrintConfigDto GetLocalPrint(string pcId);

        [Put("EditLocalPrintV2", "添加/编辑本地默认打印")]
        void EditLocalPrint(LocalPrintMachineDto localPrintMachine);

        [Get("GetPrintGroupV2", "获取本地配置的打印组")]
        List<PrintGroupDto> GetPrintGroup(string pcId);
          
        [Get("GetGroupSchemeV2", "获取打印方案")]
        List<PrintGroupSchemeDto> GetGroupScheme(string pcId, int groupId);

        [Get("GetGroupSchemeTopSetInfoV2", "获取打印方案的高级设置信息")]
        List<PrintSetInfoDto> GetGroupSchemeTopSetInfo(string pcId, int groupId);

        [Put("ModifySetInfoV2", "编辑高级设置触发")]
        void ModifySetInfo(PrintSetInfoDto printSetInfoDto);

        [Delete("DeleteGroupSchemeV2", "删除打印方案")]
        void DeleteGroupScheme(int schemeId);

        [Get("AddGroupSchemeV2", "新增打印方案")]   
        PrintGroupSchemeDto AddGroupScheme(int printCode);

        [Post("CreateGroupSchemeV2", "添加打印方案")]
        void CreateGroupScheme(PrintGroupSchemeDto printGroupSchemeDto);
         
        [Put("ModifyGroupSchemeV2", "添加/编辑打印方案")]
        void ModifyGroupScheme(PrintGroupSchemeDto printGroupSchemeDto);

        [Put("PrintSchemeTestV2", "打印测试页")]
        void PrintSchemeTest(PrintGroupSchemeDto printGroupSchemeDto);

        [Put("PutDefaultGroupSchemeV2", "设置默认打印方案")]
        void PutDefaultGroupScheme(int schemeId,int groupId);
    }
}
