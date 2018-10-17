using Hydra.Framework.NancyExpand;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.MqttUtility;

namespace ShanDian.AddIns.Print.Interface
{
    [RoutePrefix("", "Print")]
    public interface IPrint 
    {
        [Get("PrintConfigPage", "本机打印配置分页")]
        PageListDto<PrintConfigPageVDto> GetPrintConfigPage(string pcid, int pageIndex, int pageSize);

        [Get("EnablePrintConfigPage", "打印配置分页")]
        PageListDto<PrintConfigDto> GetEnablePrintConfigPage(int pageIndex, int pageSize = 6);

        [Get("PrintConfig", "获取打印配置信息")]
        PrintConfigDto GetPrintConfig(string printId);

        [Post("PrintConfig", "添加打印配置")]
        void AddPrintConfig(PrintConfigDto printConfigDto);

        [Put("PrintConfig", "更新打印配置")]
        void UpdatePrintConfig(PrintConfigDto printConfigDto);

        [Delete("PrintConfig", "删除打印配置")]
        void DeletePrintConfig(string printId);

        #region 打印机接口


        [Post("PrintInfo", "添加打印机")]
        void AddPrintInfo(string pcid, string printName, int isDefault, int state = 1);

        [Put("PrintInfo", "更新打印机")]
        void UpdatePrintInfo(string pcid, string printName, int isDefault, int state = 1);

        [Delete("PrintInfo", "删除打印机")]
        void DeletePrintInfo(string pcid, string printName);

        [Get("PrintPage", "打印机分页")]
        PageListDto<PrintInfoVDto> GetPrintPage(string pcid, int pageIndex, int pageSize);

        #endregion 打印机接口

        [Put("SMDCDFooter", "更新扫码点菜单页尾")]
        void UpdateSMDCDFooter(string content, int isMiddle);

        void UpdateVoucherExpand(VoucherExpandDto voucherExpand);

        [Get("SMDCDFooter", "获取扫码点菜单页尾")]
        VoucherExpandDto GetSMDCDFooter();

        VoucherExpandDto GetVoucherExpand(string templateCode, int type);

        [Get("VoucherExpands/TemplateCode", "根据模板编号获取单据拓展")]
        List<VoucherExpandDto> GetVoucherExpand(string templateCode);

        [Put("VoucherExpand", "更新单据拓展配置")]
        void UpdateVoucherExpand(string templateCode, int type, string content);

        [Get("CashBoxSetInfos", "获取开钱箱设置信息")]
        List<CashBoxSetInfoDto> GetOpenCashBoxSetInfoList();

        CashBoxSetInfoDto GetOpenCashBoxSetInfoByPcid(string pcid);

        [Put("CashBoxSetInfo", "更新开钱箱设置信息")]
        void UpdateOpenCashBoxSetInfo(CashBoxSetInfoDto cashBoxSetInfoDto);

        [Get("CashBoxSetPrints", "获取开钱箱打印机信息列表")]
        List<CashBoxSetPrint.Dto> GetCashBoxSetPrintList();
    }
}
