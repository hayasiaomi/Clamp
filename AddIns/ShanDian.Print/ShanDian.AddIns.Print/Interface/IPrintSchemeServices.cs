using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.View;

namespace ShanDian.AddIns.Print.Interface
{
    [RoutePrefix("", "Print")]
    public interface IPrintSchemeServices : ICommunicationServer
    {
        [Get("VoucherList", "获取打印凭证列表")]
        List<VoucherDto> GetVoucherList(string pcId, bool isGlobal);

        [Get("PrintScheme/VoucherId", "根据凭证获取指定的打印方案")]
        PageListDto<PrintSchemeDto> GetPrintScheme(int voucherId, int pageIndex, int pageSize = 6);

        [Get("LocalPrint", "获取本地打印机")]
        PrintConfigVDto GetLocalPrint(string pcId);

        [Put("LocalPrint", "添加/编辑本地打印机")]
        void UpdateLocalPrint(LocalPrint.Dto localPrint.Dto);

        [Post("PrintScheme", "添加打印方案")]
        void CreatePrintScheme(PrintSchemeDto printScheme);

        [Delete("PrintScheme/Id", "删除打印方案")]
        void DeletePrintScheme(int id);

        [Put("PrintScheme", "更新打印方案")]
        void UpdatePrintScheme(PrintSchemeDto printScheme);

        [Get("TagList/VoucherId", "获取标签列表")]
        List<PrintLabelDto> GetTagList(int[] ids);

        [Get("PrintSchemeLabelList/IdList", "批量获取指定的打印方案标签")]
        List<PrintSchemeLabelDto> GetPrintSchemeLabelDtoListByIdList(int[] ints);

        [Put("PrintSchemeLabelList", "批量更新打印方案标签")]
        void UpdatePrintSchemeLabelList(List<PrintSchemeLabelDto> printSchemeLabelDtoList, int printSchemeId);

        [Put("PrintSchemeLabelList/IdList", "覆盖打印方案标签")]
        void UpdatePrintSchemeLabelLists(List<PrintSchemeLabelDto> printSchemeLabelDtoList);

        [Get("PrintSchemeInfoList/TemplateCode", "根据打印模板编码获取打印方案(包含打印机信息)列表")]
        List<PrintSchemeInfoDto> GetPrintSchemeInfoListByTemplateCode(string templateCode);


    }
}