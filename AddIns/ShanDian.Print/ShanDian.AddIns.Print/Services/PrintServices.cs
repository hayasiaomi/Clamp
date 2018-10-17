using Hydra.Framework.MqttUtility;
using Hydra.Framework.Partial.Mqtt;
using Hydra.Framework.Services.Aop.RegistrationAttributes;
using Hydra.Framework.SqlContent;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.Restaurant;
using ShanDian.AddIns.Print.Dto.View;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Model;
using ShanDian.AddIns.Print.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Module
{
    [As(typeof(IPrint))]
    public class Print : CommunicationServer, IPrint
    {
        private IRepositoryContext repositoryContext;
        //private IHydraCloudPrint _hydraCloudPrint;

        public Print()
        {
            this.repositoryContext = Global.RepositoryContext();
            // _hydraCloudPrint = CloudLoader.Load<IHydraCloudPrint>();
        }

        /// <summary>
        /// 本机打印配置分页
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListDto<PrintConfigPageVDto> GetPrintConfigPage(string pcid, int pageIndex, int pageSize = 6)
        {
            PageListDto<PrintConfigPageVDto> page = null;

            try
            {
                if (string.IsNullOrEmpty(pcid))
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.ParamsError;
                    return new PageListDto<PrintConfigPageVDto>(null, pageIndex, pageSize, 0);
                }
                pcid = pcid.Trim();
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                IEnumerable<PrintConfigPageVDto> print = repositoryContext.GetSet<PrintConfigPageVDto>("select id,printId,pcid,enable,state,updated,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,leftMargin, modifyTime, isDefault from tb_printconfig Where pcid=@pcid order by id", new { pcid = pcid });

                if (print == null || !print.Any())
                {
                    page = new PageListDto<PrintConfigPageVDto>(null, pageIndex, pageSize, 0);
                }
                else
                {
                    int skipCnt = (pageIndex - 1) * pageSize;
                    if (skipCnt < print.Count())
                    {
                        skipCnt = (pageIndex - 1) * pageSize;
                    }
                    else
                    {
                        var i = print.Count() % pageSize;
                        pageIndex = print.Count() / pageSize;
                        if (i > 0)
                        {
                            pageIndex++;
                        }
                        skipCnt = (pageIndex - 1) * pageSize;
                    }

                    page = new PageListDto<PrintConfigPageVDto>(print.Skip(skipCnt).Take(pageSize).ToList(), pageIndex, pageSize, print.Count());
                    var machineServices = MLandLoader.Load<IMachineServices>(Global.PartName);
                    var machineDtos = machineServices.GetAllMachines();
                    if (machineDtos != null && machineDtos.Count > 0)
                    {
                        foreach (var item in page.PageData)
                        {
                            var machineDto = machineDtos.FirstOrDefault(x => x.Code == item.Pcid);
                            if (machineDto != null && !string.IsNullOrEmpty(machineDto.Name))
                            {
                                item.TerminalName = machineDto.Name;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }
            return page;
        }

        /// <summary>
        /// 打印配置分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListDto<PrintConfigDto> GetEnablePrintConfigPage(int pageIndex, int pageSize = 6)
        {
            PageListDto<PrintConfigDto> page = null;

            try
            {
                var sql = new StringBuilder();

                sql.Append("select id,printId,pcid,state,enable,updated,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,leftMargin, modifyTime, isDefault from tb_printconfig Where enable!=0");
                sql.Append(" order by id");
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                IEnumerable<PrintConfig> print = repositoryContext.GetSet<PrintConfig>(sql.ToString(), null);

                if (print == null || !print.Any())
                {
                    page = new PageListDto<PrintConfigDto>(null, pageIndex, pageSize, 0);
                }
                else
                {
                    int skipCnt = (pageIndex - 1) * pageSize;
                    if (skipCnt < print.Count())
                    {
                        skipCnt = (pageIndex - 1) * pageSize;
                    }
                    else
                    {
                        var i = print.Count() % pageSize;
                        pageIndex = print.Count() / pageSize;
                        if (i > 0)
                        {
                            pageIndex++;
                        }
                        skipCnt = (pageIndex - 1) * pageSize;
                    }
                    var printConfigs = print.Skip(skipCnt).Take(pageSize).ToList();
                    var printConfigDtoList = GetPrintConfigDtoList(printConfigs);
                    page = new PageListDto<PrintConfigDto>(printConfigDtoList, pageIndex, pageSize, print.Count());
                }
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }
            return page;
        }

        /// <summary>
        /// 获取打印配置具体信息
        /// </summary>
        /// <param name="printId"></param>
        /// <returns></returns>
        public PrintConfigDto GetPrintConfig(string printId)
        {
            try
            {
                var printConfig = repositoryContext.FirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig where printId = @printId",
                       new { printId = printId });

                if (printConfig != null)
                {
                    PrintConfigDto printConfigDto = GetPrintConfigDto(printConfig);
                    Flag = true;
                    return printConfigDto;
                }
                else
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.PrintConfigNullError;
                    return null;
                }
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
                return null;
            }
        }

        /// <summary>
        /// 添加打印机配置
        /// </summary>
        /// <param name="printConfigDto"></param>
        public void AddPrintConfig(PrintConfigDto printConfigDto)
        {
            if (printConfigDto == null)
            {
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            printConfigDto.Enable = 1;
            UpdatePrintConfig(printConfigDto);
        }

        /// <summary>
        /// 更新打印机配置
        /// </summary>
        /// <param name="printConfigDto"></param>
        public void UpdatePrintConfig(PrintConfigDto printConfigDto)
        {
            if (printConfigDto == null)
            {
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            try
            {
                var printConfig = repositoryContext.FirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig where printId=@printId",
                        new { printId = printConfigDto.PrintId });
                if (printConfig == null)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.PrintConfigNullError;
                }
                else
                {
                    string machineName = String.Empty;
                    var machineServices = MLandLoader.Load<IMachineServices>(Global.PartName);
                    var machineDto = machineServices.GetMachineByCode(printConfigDto.Pcid);
                    if (machineDto != null)
                    {
                        machineName = machineDto.Name;
                    }
                    printConfig.Id = printConfig.Id;
                    printConfig.PrintId = printConfig.PrintId;
                    printConfig.Pcid = printConfig.Pcid;
                    printConfig.PrintName = printConfig.PrintName;
                    printConfig.Alias = printConfigDto.Alias;
                    printConfig.ConnStyle = printConfigDto.ConnStyle;
                    printConfig.PaperType = printConfigDto.PaperType;
                    printConfig.TerminalName = machineName;
                    printConfig.Updated = 0;
                    printConfig.IsDefault = printConfig.IsDefault;
                    printConfig.Enable = printConfigDto.Enable;
                    printConfig.PaperWidth = printConfigDto.PaperWidth;
                    printConfig.TopMargin = printConfigDto.TopMargin;
                    printConfig.LeftMargin = printConfigDto.LeftMargin;
                    printConfig.ModifyTime = DateTime.Now;

                    bool res = repositoryContext.Update(printConfig);
                    if (res)
                    {
                        //云更新
                        //_hydraCloudPrint.UpdatePrintConfig(printConfigDto);
                    }
                    else
                    {
                        Flag = false;
                        Code = PrintErrorCode.Code.UpdateError;
                    }
                }

            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }


        /// <summary>
        /// 删除打印配置(假删)
        /// </summary>
        /// <param name="printId"></param>
        public void DeletePrintConfig(string printId)
        {
            try
            {
                #region 旧代码初始版本
                //var printSchemeList = repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId from tb_printScheme Where PrintId=@PrintId", new { PrintId = printId });
                //if (printSchemeList != null && printSchemeList.Count > 0)
                //{
                //    Flag = false;
                //    Code = PrintErrorCode.Code.IllegalOperationError;
                //    return;
                //} 
                #endregion

                var printSchemeList = repositoryContext.GetSet<PrintScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme Where PrintId=@PrintId", new { PrintId = printId });
                if (printSchemeList != null && printSchemeList.Count > 0)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.IllegalOperationError;
                    return;
                }
                var printLocalPrint = repositoryContext.FirstOrDefault<LocalPrint>("select id,printId,machine from tb_localPrint Where PrintId=@PrintId", new { PrintId = printId });
                if (printLocalPrint != null)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.IllegalOperationError;
                    return;
                }

                var printConfig = repositoryContext.FirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig where printId = @printId",
                    new { printId = printId });
                if (printConfig == null)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.ResultNull;
                    return;
                }
                printConfig.Alias = String.Empty;
                printConfig.Enable = 0;
                printConfig.Updated = 0;
                printConfig.ModifyTime = DateTime.Now;
                bool result = repositoryContext.Update(printConfig);
                if (!result)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.UpdateError;
                    return;
                }
                //_hydraCloudPrint.DeletePrintConfig(pcid, printName);
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        #region 打印机接口
        /// <summary>
        /// 添加打印机
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="printName"></param>
        /// <param name="isDefault"></param>
        /// <param name="state"></param>
        public void AddPrintInfo(string pcid, string printName, int isDefault, int state = 1)
        {
            if (string.IsNullOrWhiteSpace(pcid) || string.IsNullOrWhiteSpace(printName))
            {
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            try
            {
                var printconfig = repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig where pcid = @pcid and printName=@printName", new { pcid = pcid, printName = printName });
                if (printconfig == null)
                {
                    string machineName = String.Empty;
                    var machineServices = MLandLoader.Load<IMachineServices>(Global.PartName);
                    var machineDto = machineServices.GetMachineByCode(pcid);
                    if (machineDto != null)
                    {
                        machineName = machineDto.Name;
                    }
                    var printConfig = new PrintConfig()
                    {
                        Pcid = pcid,
                        PrintId = Guid.NewGuid().ToString(),
                        TerminalName = machineName,
                        ConnStyle = 1,
                        PrintName = printName,
                        State = state,
                        ModifyTime = DateTime.Now,
                        Enable = 0,
                        IsDefault = isDefault,
                        PaperType = 3,
                        PaperWidth = 280,
                        TopMargin = 0,
                        LeftMargin = 0,
                        Updated = 0
                    };

                    var res = repositoryContext.Insert(printConfig);
                    if (res != 0)
                    {
                        Flag = true;
                    }
                    else
                    {
                        Flag = false;
                        Code = PrintErrorCode.Code.UpdateError;
                    }
                }
                else
                {
                    UpdatePrintInfo(pcid, printName, isDefault, state);
                }
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        /// <summary>
        /// 更新打印机
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="printName"></param>
        /// <param name="isDefault"></param>
        /// <param name="state"></param>
        public void UpdatePrintInfo(string pcid, string printName, int isDefault, int state = 1)
        {
            if (string.IsNullOrWhiteSpace(pcid) || string.IsNullOrWhiteSpace(printName))
            {
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            try
            {
                var printconfig = repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig Where pcid=@pcId And printName=@printName", new { pcid = pcid, printName = printName });
                if (printconfig != null)
                {
                    printconfig.State = state;
                    printconfig.IsDefault = isDefault;
                    bool res = repositoryContext.Update(printconfig);
                    if (!res)
                    {
                        Flag = false;
                        Code = PrintErrorCode.Code.UpdateError;
                    }
                    else
                    {
                        Flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        /// <summary>
        /// 删除打印机
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="printName"></param>
        public void DeletePrintInfo(string pcid, string printName)
        {
            if (string.IsNullOrWhiteSpace(pcid) || string.IsNullOrWhiteSpace(printName))
            {
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            try
            {
                var printconfig = repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig where pcid = @pcid and printName=@printName ", new { pcid = pcid, printName = printName });
                if (printconfig != null)
                {
                    printconfig.State = 0;
                    bool res = repositoryContext.Update(printconfig);
                    if (!res)
                    {
                        Flag = false;
                        Code = PrintErrorCode.Code.UpdateError;
                    }
                    else
                    {
                        Flag = true;
                    }
                }
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        /// <summary>
        /// 获取打印机名分页
        /// </summary>
        /// <param name="pcid"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        public PageListDto<PrintInfoVDto> GetPrintPage(string pcid, int pageIndex, int pageSize = 6)
        {
            if (string.IsNullOrWhiteSpace(pcid))
            {
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return null;
            }
            PageListDto<PrintInfoVDto> page = null;

            try
            {
                pageIndex = pageIndex < 1 ? 1 : pageIndex;
                var print = repositoryContext.GetSet<PrintConfig>(@"select pcid,printname,state,papertype from tb_printconfig where pcid = @pcid and updated=1 and Enable=1",
                            new { pcid = pcid });

                if (print == null || !print.Any())
                {
                    page = new PageListDto<PrintInfoVDto>(null, pageIndex, pageSize, 0);
                }
                else
                {
                    int skipCnt = (pageIndex - 1) * pageSize;
                    if (skipCnt < print.Count())
                    {
                        skipCnt = (pageIndex - 1) * pageSize;
                    }
                    else
                    {
                        var i = print.Count() % pageSize;
                        pageIndex = print.Count() / pageSize;
                        if (i > 0)
                        {
                            pageIndex++;
                        }
                        skipCnt = (pageIndex - 1) * pageSize;
                    }

                    var printInfos = print.Skip(skipCnt).Take(pageSize).ToList();
                    var printInfoVDtos = new List<PrintInfoVDto>();
                    foreach (var item in printInfos)
                    {
                        var printInfoVDto = new PrintInfoVDto()
                        {
                            PaperType = item.PaperType,
                            Pcid = item.Pcid,
                            PrintName = item.PrintName,
                            State = item.State
                        };
                        printInfoVDtos.Add(printInfoVDto);
                    }
                    page = new PageListDto<PrintInfoVDto>(printInfoVDtos, pageIndex, pageSize, print.Count());
                }
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.CheckpointError;
                PrintLogUtility.Writer.SendFullError(ex);
            }

            return page;
        }

        #endregion 打印机接口

        #region MyRegion

        /// <summary>
        /// 更新单据拓展
        /// </summary>
        /// <param name="voucherExpand"></param>
        public void UpdateVoucherExpand(VoucherExpandDto voucherExpand)
        {
            if (voucherExpand == null || string.IsNullOrEmpty(voucherExpand.TemplateCode))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var result = repositoryContext.QueryFirstOrDefault<VoucherExpand>("select Id,TemplateCode,Type,Alignment,Content from tb_voucherExpand Where templateCode=@templateCode AND type=@type", new { templateCode = voucherExpand.TemplateCode, type = voucherExpand.Type });
            if (string.IsNullOrWhiteSpace(voucherExpand.Content))
            {
                voucherExpand.Content = String.Empty;
            }
            if (result == null)
            {
                CreateVoucherExpand(voucherExpand);
            }
            else
            {
                UpdateVoucherExpandPrivate(voucherExpand);
            }
        }

        public void UpdateSMDCDFooter(string content, int isMiddle)
        {
            var voucherExpandDto = new VoucherExpandDto();
            if (isMiddle == 1)
            {
                voucherExpandDto.Alignment = 1;
            }
            else
            {
                voucherExpandDto.Alignment = 0;
            }
            voucherExpandDto.Content = content;
            voucherExpandDto.Type = 1;
            voucherExpandDto.TemplateCode = "PRT_SO_0001";
            UpdateVoucherExpand(voucherExpandDto);
        }

        /// <summary>
        /// 获取扫码点菜单页尾
        /// </summary>
        /// <returns></returns>
        public VoucherExpandDto GetSMDCDFooter()
        {
            var voucherExpandDto = new VoucherExpandDto();
            voucherExpandDto = GetVoucherExpand("PRT_SO_0001", 1);
            return voucherExpandDto;
        }

        /// <summary>
        /// 获取单据拓展
        /// </summary>
        /// <param name="templateCode"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public VoucherExpandDto GetVoucherExpand(string templateCode, int type)
        {
            var voucherExpand = repositoryContext.QueryFirstOrDefault<VoucherExpand>("select Id,TemplateCode,Type,Alignment,Content from tb_voucherExpand Where templateCode=@templateCode AND type=@type", new { templateCode = templateCode, type = type });
            return GetVoucherExpandDto(voucherExpand);
        }

        /// <summary>
        /// 根据模板编号获取单据拓展
        /// </summary>
        /// <param name="templateCode"></param>
        /// <returns></returns>
        public List<VoucherExpandDto> GetVoucherExpand(string templateCode)
        {
            var voucherExpandDtos = new List<VoucherExpandDto>();
            var voucherExpands = repositoryContext.GetSet<VoucherExpand>("select Id,TemplateCode,Type,Alignment,Content from tb_voucherExpand Where templateCode=@templateCode", new { templateCode = templateCode });
            if (voucherExpands == null || !voucherExpands.Any())
            {
                Flag = false;
                Code = PrintErrorCode.Code.ResultNull;
                return voucherExpandDtos;
            }
            foreach (var temp in voucherExpands)
            {
                voucherExpandDtos.Add(GetVoucherExpandDto(temp));
            }
            return voucherExpandDtos;
        }

        public void UpdateVoucherExpand(string templateCode, int type, string content)
        {
            var voucherExpandDto = new VoucherExpandDto();
            voucherExpandDto.Alignment = 0;
            voucherExpandDto.Content = content;
            voucherExpandDto.Type = type;
            voucherExpandDto.TemplateCode = templateCode;
            UpdateVoucherExpand(voucherExpandDto);
        }

        public List<CashBoxSetInfoDto> GetOpenCashBoxSetInfoList()
        {
            var cashBoxSetInfoDtos = new List<CashBoxSetInfoDto>();
            try
            {
                var cashBoxSetInfos = repositoryContext.GetSet<CashBoxSetInfo>("select Id,TerminaId,TerminalName,PrintId,PrintName,IsSet from tb_cashBoxSetInfo", null);
                IRestServices restaurantServices = null;
                var allMachines = restaurantServices.GetAllMachines();
                if (allMachines != null && allMachines.Count > 0)
                {
                    var addMachines = new List<MachineDto>();
                    var delMachineIds = new List<string>();
                    var updateMachines = new List<MachineDto>();
                    foreach (var machine in allMachines)
                    {
                        CashBoxSetInfo cashBoxSetInfo = null;
                        if (cashBoxSetInfos != null)
                        {
                            cashBoxSetInfo = cashBoxSetInfos.FirstOrDefault(x => x.TerminaId == machine.Code);
                        }
                        if (cashBoxSetInfo != null)
                        {
                            if (cashBoxSetInfo.PrintName != machine.Name)
                            {
                                updateMachines.Add(machine);
                            }
                        }
                        else
                        {
                            addMachines.Add(machine);
                        }
                    }
                    foreach (var info in cashBoxSetInfos)
                    {
                        var machine = allMachines.FirstOrDefault(x => x.Code == info.TerminaId);
                        if (machine == null)
                        {
                            delMachineIds.Add(info.TerminaId);
                        }
                    }
                    DeleteCashBoxSetInfos(delMachineIds);
                    UpdateCashBoxSetInfoTerminalName(updateMachines);
                    CreateCashBoxSetInfos(addMachines);
                    cashBoxSetInfos = repositoryContext.GetSet<CashBoxSetInfo>("select Id,TerminaId,TerminalName,PrintId,PrintName,IsSet from tb_cashBoxSetInfo", null);
                }

                var print = repositoryContext.GetSet<PrintConfig>("select id,printId,pcid,state,enable,updated,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,leftMargin, modifyTime, isDefault from tb_printconfig Where enable!=0", null);
                foreach (var item in cashBoxSetInfos)
                {
                    var config = print.FirstOrDefault(x => x.PrintId == item.PrintId);
                    var delList = new List<long>();
                    if (config != null)
                    {
                        if (config.PrintName != item.PrintName || config.Enable == 0)
                        {
                            delList.Add(item.Id);
                        }
                    }
                    else
                    {
                        delList.Add(item.Id);
                    }
                    InitCashBoxSetInfoPrintInto(delList);
                }
                cashBoxSetInfos = repositoryContext.GetSet<CashBoxSetInfo>("select Id,TerminaId,TerminalName,PrintId,PrintName,IsSet from tb_cashBoxSetInfo", null);
                Flag = true;
                cashBoxSetInfoDtos = GetCashBoxSetInfoDtoList(cashBoxSetInfos);
                return cashBoxSetInfoDtos;
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.UpdateError;
                PrintLogUtility.Writer.SendFullError(ex);
                return cashBoxSetInfoDtos;
            }
        }

        /// <summary>
        /// 根据Pcid获取开钱箱设置信息
        /// </summary>
        /// <param name="pcid"></param>
        /// <returns></returns>
        public CashBoxSetInfoDto GetOpenCashBoxSetInfoByPcid(string pcid)
        {
            if (string.IsNullOrWhiteSpace(pcid))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return null;
            }
            try
            {
                var cashBoxSetInfo = repositoryContext.QueryFirstOrDefault<CashBoxSetInfo>("select Id,TerminaId,TerminalName,PrintId,PrintName,IsSet from tb_cashBoxSetInfo where TerminaId=@TerminaId", new { TerminaId = pcid });
                if (cashBoxSetInfo == null)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.ResultNull;
                    return null;
                }
                var cashBoxSetInfoDto = GetCashBoxSetInfoDto(cashBoxSetInfo);
                return cashBoxSetInfoDto;
            }
            catch (Exception ex)
            {
                Flag = false;
                PrintLogUtility.Writer.SendFullError(ex);
                this.Code = PrintErrorCode.Code.DatabaseError;
                return null;
            }
        }

        public void UpdateOpenCashBoxSetInfo(CashBoxSetInfoDto cashBoxSetInfoDto)
        {
            if (cashBoxSetInfoDto == null)
            {
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            try
            {
                var cashBoxSetInfo = repositoryContext.QueryFirstOrDefault<CashBoxSetInfo>("select Id,TerminaId,TerminalName,PrintId,PrintName,IsSet from tb_cashBoxSetInfo where Id=@Id", new { Id = cashBoxSetInfoDto.Id });
                if (cashBoxSetInfo == null)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.ResultNull;
                    return;
                }
                cashBoxSetInfo.PrintId = cashBoxSetInfoDto.PrintId;
                cashBoxSetInfo.PrintName = cashBoxSetInfoDto.PrintName;
                if (!string.IsNullOrWhiteSpace(cashBoxSetInfo.PrintId) && !string.IsNullOrWhiteSpace(cashBoxSetInfo.PrintName))
                {
                    cashBoxSetInfo.IsSet = 1;
                }
                else
                {
                    cashBoxSetInfo.IsSet = 0;
                }
                bool res = repositoryContext.Update(cashBoxSetInfo);
                if (!res)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.UpdateError;
                }
                else
                {
                    Flag = true;
                }
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public List<CashBoxSetPrint.Dto> GetCashBoxSetPrintList()
        {
            var cashBoxSetPrint.Dtos = new List<CashBoxSetPrint.Dto>();
            try
            {
                var cashBoxSetInfos = repositoryContext.GetSet<CashBoxSetInfo>("select Id,TerminaId,TerminalName,PrintId,PrintName,IsSet from tb_cashBoxSetInfo", null);


                var print = repositoryContext.GetSet<PrintConfig>("select id,printId,pcid,state,enable,updated,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,leftMargin, modifyTime, isDefault from tb_printconfig Where enable!=0", null);

                foreach (var item in print)
                {
                    var cashBoxSetInfo = cashBoxSetInfos.FirstOrDefault(x => x.PrintId == item.PrintId);
                    var cashBoxSetPrint.Dto = new CashBoxSetPrint.Dto();
                    cashBoxSetPrint.Dto.PrintName = item.PrintName;
                    cashBoxSetPrint.Dto.PrintId = item.PrintId;
                    if (cashBoxSetInfo != null)
                    {
                        cashBoxSetPrint.Dto.IsSet = 1;
                    }
                    cashBoxSetPrint.Dtos.Add(cashBoxSetPrint.Dto);
                }
                Flag = true;
                return cashBoxSetPrint.Dtos;
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.UpdateError;
                PrintLogUtility.Writer.SendFullError(ex);
                return cashBoxSetPrint.Dtos;
            }
        }

        private void CreateCashBoxSetInfos(List<MachineDto> list)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("Begin;");
                foreach (var item in list)
                {
                    sbSql.Append(
                        $"INSERT INTO tb_cashBoxSetInfo(TerminaId,TerminalName,PrintId,PrintName,IsSet)VALUES('{item.Code}','{item.Name}','{string.Empty}','{string.Empty}',{0});");
                }

                sbSql.Append("Commit;");

                var result = repositoryContext.Execute(sbSql.ToString(), null);
                if (result >= 1)
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        private void DeleteCashBoxSetInfos(List<string> ids)
        {
            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("Begin;");
                foreach (var id in ids)
                {
                    sbSql.Append($"delete from tb_cashBoxSetInfo Where TerminaId='{id}';");
                }
                sbSql.Append("Commit;");

                var result = repositoryContext.Execute(sbSql.ToString(), null);
                if (result >= 1)
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        private void UpdateCashBoxSetInfoTerminalName(List<MachineDto> list)
        {
            if (list == null || !list.Any())
            {
                return;
            }

            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("Begin;");
                foreach (var item in list)
                {
                    sbSql.Append(
                        $"UPDATE tb_cashBoxSetInfo SET TerminalName='{item.Name}' WHERE TerminaId = '{item.Code}';");
                }

                sbSql.Append("Commit;");

                var result = repositoryContext.Execute(sbSql.ToString(), null);
                if (result >= 1)
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        private void InitCashBoxSetInfoByID(int id)
        {
            try
            {
                var cashBoxSetInfo = repositoryContext.QueryFirstOrDefault<CashBoxSetInfo>("select Id,TerminaId,TerminalName,PrintId,PrintName,IsSet from tb_cashBoxSetInfo where Id=@Id", new { Id = id });
                if (cashBoxSetInfo == null)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.ResultNull;
                    return;
                }
                cashBoxSetInfo.PrintId = string.Empty;
                cashBoxSetInfo.PrintName = string.Empty;
                cashBoxSetInfo.IsSet = 0;
                bool res = repositoryContext.Update(cashBoxSetInfo);
                if (!res)
                {
                    Flag = false;
                    Code = PrintErrorCode.Code.UpdateError;
                }
                else
                {
                    Flag = true;
                }
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        private void InitCashBoxSetInfoPrintInto(List<long> list)
        {
            if (list == null || !list.Any())
            {
                return;
            }

            try
            {
                StringBuilder sbSql = new StringBuilder();
                sbSql.Append("Begin;");
                foreach (var item in list)
                {
                    sbSql.Append(
                        $"UPDATE tb_cashBoxSetInfo SET PrintId='{string.Empty}',PrintName='{string.Empty}',IsSet={0} WHERE Id = {item};");
                }

                sbSql.Append("Commit;");

                var result = repositoryContext.Execute(sbSql.ToString(), null);
                if (result >= 1)
                {
                    Flag = true;
                }
                else
                {
                    Flag = false;
                }
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendFullError(ex);
            }
        }

        public CashBoxSetInfoDto GetCashBoxSetInfoDto(CashBoxSetInfo model)
        {
            var cashBoxSetInfoDto = new CashBoxSetInfoDto();
            if (model != null)
            {
                cashBoxSetInfoDto.Id = model.Id;
                cashBoxSetInfoDto.TerminaId = model.TerminaId;
                cashBoxSetInfoDto.TerminalName = model.TerminalName;
                cashBoxSetInfoDto.PrintId = model.PrintId;
                cashBoxSetInfoDto.PrintName = model.PrintName;
                cashBoxSetInfoDto.IsSet = model.IsSet;
            }
            return cashBoxSetInfoDto;
        }

        public List<CashBoxSetInfoDto> GetCashBoxSetInfoDtoList(List<CashBoxSetInfo> modelList)
        {
            if (modelList == null)
            {
                return new List<CashBoxSetInfoDto>();
            }

            var cashBoxSetInfoDtos = new List<CashBoxSetInfoDto>();
            foreach (var item in modelList)
            {
                var cashBoxSetInfoDto = GetCashBoxSetInfoDto(item);
                cashBoxSetInfoDtos.Add(cashBoxSetInfoDto);
            }
            return cashBoxSetInfoDtos;
        }

        private void CreateVoucherExpand(VoucherExpandDto voucherExpand)
        {
            if (voucherExpand == null || string.IsNullOrWhiteSpace(voucherExpand.TemplateCode))
            {
                Flag = false;
                Message = "模板编号不能为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var result = repositoryContext.QueryFirstOrDefault<VoucherExpand>("select Id,TemplateCode,Type,Alignment,Content from tb_voucherExpand Where templateCode=@templateCode AND type=@type", new { templateCode = voucherExpand.TemplateCode, type = voucherExpand.Type });
            if (result != null)
            {
                Flag = false;
                Message = "请勿重复创建";
                this.Code = PrintErrorCode.Code.RepeatedError;
                return;
            }
            var model = GetVoucherExpand(voucherExpand);
            var resultInsert = repositoryContext.Execute("INSERT INTO tb_voucherExpand(TemplateCode,Type,Alignment,Content)VALUES(@TemplateCode,@Type,@Alignment,@Content)", model);
            if (resultInsert == 0)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            Flag = true;
        }

        private void UpdateVoucherExpandPrivate(VoucherExpandDto voucherExpand)
        {
            if (voucherExpand == null)
            {
                Flag = false;
                Message = "模板编号不能为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var model = GetVoucherExpand(voucherExpand);
            var result = repositoryContext.Execute("UPDATE tb_voucherExpand SET Alignment=@Alignment,Content=@Content WHERE TemplateCode = @TemplateCode AND Type = @Type", model);
            if (result == 0)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            Flag = true;
        }
        #endregion

        public VoucherExpandDto GetVoucherExpandDto(VoucherExpand model)
        {
            var voucherExpandDto = new VoucherExpandDto();
            if (model != null)
            {
                voucherExpandDto.Id = model.Id;
                voucherExpandDto.Alignment = model.Alignment;
                voucherExpandDto.TemplateCode = model.TemplateCode;
                voucherExpandDto.Type = model.Type;
                voucherExpandDto.Content = model.Content;
            }
            return voucherExpandDto;
        }
        public VoucherExpand GetVoucherExpand(VoucherExpandDto dto)
        {
            var model = new VoucherExpand();
            if (dto != null)
            {
                model.Id = dto.Id;
                model.Alignment = dto.Alignment;
                model.TemplateCode = dto.TemplateCode;
                model.Type = dto.Type;
                model.Content = dto.Content;
            }
            return model;
        }

        private PrintConfigDto GetPrintConfigDto(PrintConfig printConfig)
        {
            if (printConfig == null)
            {
                return new PrintConfigDto();
            }
            PrintConfigDto printConfigDto = new PrintConfigDto
            {
                PrintId = printConfig.PrintId,
                Pcid = printConfig.Pcid,
                TerminalName = printConfig.TerminalName,
                PrintName = printConfig.PrintName,
                ConnStyle = printConfig.ConnStyle,
                Alias = printConfig.Alias,
                PaperType = printConfig.PaperType,
                PaperWidth = printConfig.PaperWidth,
                TopMargin = printConfig.TopMargin,
                LeftMargin = printConfig.LeftMargin,
                ModifyTime = printConfig.ModifyTime,
                Updated = printConfig.Updated,
                Enable = printConfig.Enable,
                State = printConfig.State
            };
            return printConfigDto;
        }

        private List<PrintConfigDto> GetPrintConfigDtoList(List<PrintConfig> printConfigs)
        {
            if (printConfigs == null)
            {
                return new List<PrintConfigDto>();
            }

            var printConfigDtolList = new List<PrintConfigDto>();
            foreach (var item in printConfigs)
            {
                var printConfigDto = new PrintConfigDto();
                printConfigDto = GetPrintConfigDto(item);
                printConfigDtolList.Add(printConfigDto);
            }
            return printConfigDtolList;
        }
    }
}
