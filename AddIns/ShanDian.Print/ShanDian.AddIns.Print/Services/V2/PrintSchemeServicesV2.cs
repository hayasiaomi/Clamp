using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.Services.Aop;
using Hydra.Framework.Services.Aop.RegistrationAttributes;
using Hydra.Framework.SqlContent;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.Print.Dto;
using ShanDian.AddIns.Print.Dto.View;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Model;
using ShanDian.AddIns.Print.BUSHelper;
using ShanDian.AddIns.Print.Data;
using Newtonsoft.Json;

namespace ShanDian.AddIns.Print.Module
{
    [As(typeof(IPrintSchemeServicesV2))]
    public class PrintSchemeServicesV2 : CommunicationServer, IPrintSchemeServicesV2
    {
        private IRepositoryContext _repositoryContext;

        public PrintSchemeServicesV2()
        {
            _repositoryContext = Global.RepositoryContext();
        }

        #region 获取本地默认打印
        /// <summary>
        /// 获取本地默认打印机
        /// </summary>
        /// <param name="pcId"></param>
        /// <returns></returns>
        public PrintConfigDto GetLocalPrint(string pcId)
        {
            PrintConfigDto printConfigDto = new PrintConfigDto();
            var localPrint = _repositoryContext.QueryFirstOrDefault<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where Machine = @Machine", new { Machine = pcId });
            if (localPrint == null)
            {
                Flag = false;
                Code = PrintErrorCode.Code.ResultNull;
                return null;
            }
            var printConfig = _repositoryContext.QueryFirstOrDefault<PrintConfig>("select id,printId,pcid,terminalName,printName,alias,connStyle,connAddress,connBrand,connPort,paperType,paperWidth,topMargin,leftMargin,modifyTime,isDefault,updated,enable,state from tb_printconfig where printId = @printId", new { printId = localPrint.PrintId });
            if (printConfig == null)
            {
                Flag = false;
                Code = PrintErrorCode.Code.ResultNull;
                return null;
            }

            printConfigDto.PrintId = printConfig.PrintId;
            printConfigDto.Pcid = printConfig.Pcid;
            printConfigDto.PrintName = printConfig.PrintName;
            printConfigDto.Alias = printConfig.Alias;
            printConfigDto.ConnStyle = printConfig.ConnStyle;
            printConfigDto.ConnAddress = printConfig.ConnAddress;
            printConfigDto.ConnBrand = printConfig.ConnBrand;
            printConfigDto.ConnPort = printConfig.ConnPort;
            printConfigDto.PaperType = printConfig.PaperType;
            printConfigDto.TerminalName = printConfig.TerminalName;
            printConfigDto.PaperWidth = printConfig.PaperWidth;
            printConfigDto.TopMargin = printConfig.TopMargin;
            printConfigDto.LeftMargin = printConfig.LeftMargin;
            printConfigDto.ModifyTime = printConfig.ModifyTime;
            printConfigDto.IsDefault = printConfig.IsDefault;
            printConfigDto.Updated = printConfig.Updated;
            printConfigDto.Enable = printConfig.Enable;
            printConfigDto.State = printConfig.State;

            Flag = true;
            return printConfigDto;
        }
        #endregion

        #region 添加/编辑本地默认打印
        /// <summary>
        /// 添加/编辑本地默认打印
        /// </summary>
        /// <param name="localPrint.Dto"></param>
        public void EditLocalPrint(LocalPrintMachineDto localPrintMachine)
        {
            if (localPrintMachine == null || localPrintMachine.Machine == null || string.IsNullOrEmpty(localPrintMachine.PrintId))
            {
                Flag = false;
                Message = "机器码为空或者打印配置ID为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }

            if (localPrintMachine.PrintId == "0")
            {
                var deleteResult = _repositoryContext.Execute("delete from tb_localPrint where machine = @machine", new { machine = localPrintMachine.Machine });

                return;
            }

            var queryFirstOrDefault = _repositoryContext.QueryFirstOrDefault<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where machine=@machine", new { machine = localPrintMachine.Machine });
            if (queryFirstOrDefault == null)
            {
                LocalPrintAction(localPrintMachine, 1);
            }
            else
            {
                LocalPrintAction(localPrintMachine, 2);
            }
            Flag = true;
        }

        /// <summary>
        /// 编辑本地默认打印方案
        /// </summary>
        /// <param name="localPrint.Dto"></param>
        /// <param name="mold">1. 新增 2. 编辑</param>
        private void LocalPrintAction(LocalPrintMachineDto localPrintMachine, int mold)
        {
            var localPrint = new LocalPrint
            {
                PrintId = localPrintMachine.PrintId,
                Machine = localPrintMachine.Machine
            };

            switch (mold)
            {
                case 1:
                    #region 添加本地默认打印方案
                    var queryFirstOrDefault = _repositoryContext.QueryFirstOrDefault<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where machine=@machine", new { machine = localPrintMachine.Machine });
                    if (queryFirstOrDefault != null)
                    {
                        Flag = false;
                        Message = "请勿重复创建";
                        this.Code = PrintErrorCode.Code.RepeatedError;
                        return;
                    }

                    var result = _repositoryContext.Execute("INSERT INTO tb_localPrint(PrintId,Machine)VALUES(@PrintId,@Machine)", localPrint);
                    if (result == 0)
                    {
                        Flag = false;
                        Message = "插入本地打印数据失败";
                        this.Code = PrintErrorCode.Code.UpdateError;
                        return;
                    }
                    var localPrints = _repositoryContext.GetSet<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where printId=@printId AND machine=@machine ORDER BY Id DESC", new { printId = localPrintMachine.PrintId, machine = localPrintMachine.Machine });
                    if (localPrints == null || localPrints.Count < 1)
                    {
                        Flag = false;
                        Message = "本地打印数据更新成功，但是没能获取到插入的本机打印机信息";
                        this.Code = PrintErrorCode.Code.UpdateError;
                        return;
                    }
                    #endregion
                    break;
                case 2:
                    #region 编辑本地默认打印方案
                    var upResult = _repositoryContext.Execute("UPDATE tb_localPrint SET PrintId=@PrintId WHERE Machine = @Machine", localPrint);
                    if (upResult == 0)
                    {
                        Flag = false;
                        Message = "更新打印数据信息失败";
                        this.Code = PrintErrorCode.Code.UpdateError;
                        return;
                    }
                    #endregion
                    break;
            }

            Flag = true;
        }
        #endregion

        #region 获取本地配置的打印组
        /// <summary>
        /// 获取本地配置的打印组
        /// </summary>
        /// <param name="pcId"></param>
        /// <returns></returns>
        public List<PrintGroupDto> GetPrintGroup(string pcId)
        {
            var printGroupList = new List<PrintGroupDto>();
            if (string.IsNullOrEmpty(pcId))
            {
                Flag = false;
                Message = "pcId为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return new List<PrintGroupDto>();
            }

            var sql = new StringBuilder();
            sql.Append("select id,name,printCode,groupState,createDate,sort from tb_printGroup ");
            sql.Append("order by sort asc ;");
            var printGroups = _repositoryContext.GetSet<PrintGroup>(sql.ToString());
            if (printGroups == null || printGroups.Count < 1)
            {
                Flag = false;
                Message = "获取到的打印方案组为空";
                this.Code = PrintErrorCode.Code.PrintGroupNullError;
                return new List<PrintGroupDto>();
            }

            foreach (var item in printGroups)
            {
                PrintGroupDto groupItem = new PrintGroupDto();
                groupItem.Id = item.Id;
                groupItem.Name = item.Name;
                groupItem.CreateDate = item.CreateDate;
                groupItem.PrintCode = item.PrintCode;
                groupItem.PrintGroupSchemes = new List<PrintGroupSchemeDto>();

                var sqlStr = new StringBuilder();
                sqlStr.Append("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme ");
                sqlStr.Append($"where groupId = {item.Id} ");
                sqlStr.Append("order by createTime asc ;");
                var printGroupSchemes = _repositoryContext.GetSet<PrintGroupScheme>(sqlStr.ToString());

                foreach (var scheme in printGroupSchemes)
                {
                    PrintGroupSchemeDto groupScheme = new PrintGroupSchemeDto();
                    groupScheme.Id = scheme.Id;
                    groupScheme.Pcid = scheme.Pcid;
                    groupScheme.PrintId = scheme.PrintId;
                    groupScheme.Name = scheme.Name;
                    groupScheme.PrintId = scheme.PrintId;
                    groupScheme.IsDefault = scheme.IsDefault;
                    groupScheme.State = scheme.State;
                    groupScheme.GroupId = scheme.GroupId;
                    groupScheme.CreateTime = scheme.CreateTime;
                    groupScheme.ModifyTime = scheme.ModifyTime;

                    groupItem.PrintGroupSchemes.Add(groupScheme);
                }

                printGroupList.Add(groupItem);
            }

            Flag = true;
            return printGroupList;
        }
        #endregion

        #region 打印方案

        /// <summary>
        /// 获取打印方案
        /// </summary>
        /// <param name="pcId"></param>
        /// <param name="printCode"></param>
        /// <returns></returns>
        public List<PrintGroupSchemeDto> GetGroupScheme(string pcId, int groupId)
        {
            var printGroupSchemeList = new List<PrintGroupSchemeDto>();
            if (string.IsNullOrEmpty(pcId))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return new List<PrintGroupSchemeDto>();
            }

            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where id = @id", new { id = groupId });
            if (printGroup == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return new List<PrintGroupSchemeDto>();
            }

            var sqlStr = new StringBuilder();
            sqlStr.Append("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme ");
            sqlStr.Append($"where groupId = {groupId} ");
            sqlStr.Append("order by createTime asc");
            var printGroupSchemes = _repositoryContext.GetSet<PrintGroupScheme>(sqlStr.ToString());

            foreach (var scheme in printGroupSchemes)
            {
                PrintGroupSchemeDto groupScheme = new PrintGroupSchemeDto();
                groupScheme.Id = scheme.Id;
                groupScheme.Pcid = scheme.Pcid;
                groupScheme.Name = scheme.Name;
                groupScheme.PrintId = scheme.PrintId;
                groupScheme.IsDefault = scheme.IsDefault;
                groupScheme.State = scheme.State;
                groupScheme.GroupId = scheme.GroupId;
                groupScheme.CreateTime = scheme.CreateTime;
                groupScheme.ModifyTime = scheme.ModifyTime;
                groupScheme.DishTypeCassify = new List<SchemeDishTypeDto>();
                groupScheme.TableClassify = new List<SchemeTableDto>();
                groupScheme.SetInfoList = new List<PrintSetInfoDto>();
                groupScheme.VoucherList = new List<SchemeVoucherDto>();

                #region 餐桌
                var schemeTables = _repositoryContext.GetSet<SchemeTable>("select id,schemeId,mkTableID,erpTableID,erpTableAreaID,tableName from tb_schemeTable where schemeId = @schemeId order by id asc", new { schemeId = scheme.Id });
                if (schemeTables != null && schemeTables.Count > 0)
                {
                    foreach (var table in schemeTables)
                    {
                        SchemeTableDto tableItem = new SchemeTableDto();
                        tableItem.Id = table.Id;
                        tableItem.SchemeId = table.SchemeId;
                        tableItem.MKTableID = table.MKTableID;
                        tableItem.ErpTableID = table.ErpTableID;
                        tableItem.ErpTableAreaID = table.ErpTableAreaID;
                        tableItem.TableName = table.TableName;

                        groupScheme.TableClassify.Add(tableItem);
                    }
                }
                #endregion

                #region 菜品信息
                var schemeDishTypes = _repositoryContext.GetSet<SchemeDishType>("select id,schemeId,mkDishTypeID,erpDishTypeID,dishTypeName from tb_schemeDishType where  schemeId = @schemeId order by id asc", new { schemeId = scheme.Id });
                if (schemeDishTypes != null && schemeDishTypes.Count > 0)
                {
                    foreach (var type in schemeDishTypes)
                    {
                        SchemeDishTypeDto dishTypeItem = new SchemeDishTypeDto();
                        dishTypeItem.Id = type.Id;
                        dishTypeItem.SchemeId = type.SchemeId;
                        dishTypeItem.MKDishTypeID = type.MKDishTypeID;
                        dishTypeItem.ErpDishTypeID = type.ErpDishTypeID;
                        dishTypeItem.DishTypeName = type.DishTypeName;

                        groupScheme.DishTypeCassify.Add(dishTypeItem);
                    }
                }
                #endregion

                #region 单据信息
                var schemeVouchers = _repositoryContext.GetSet<SchemeVoucher>("select id,name,describe,voucherCode,templateCode,isEnabled,printNum,pattern,schemeId from tb_schemeVoucher where schemeId = @schemeId order by id asc", new { schemeId = scheme.Id });
                if (schemeVouchers != null && schemeVouchers.Count > 0)
                {
                    foreach (var type in schemeVouchers)
                    {
                        SchemeVoucherDto voucher = new SchemeVoucherDto();
                        voucher.Id = type.Id;
                        voucher.Name = type.Name;
                        voucher.Describe = type.Describe;
                        voucher.VoucherCode = type.VoucherCode;
                        voucher.TemplateCode = type.TemplateCode;
                        voucher.IsEnabled = type.IsEnabled;
                        voucher.PrintNum = type.PrintNum;
                        voucher.Pattern = type.Pattern;
                        voucher.SchemeId = type.SchemeId;

                        groupScheme.VoucherList.Add(voucher);
                    }
                }
                #endregion

                #region 打印方案信息
                var printSetInfos = _repositoryContext.GetSet<PrintSetInfo>("select id,name,describe,key,value,range,combineId from tb_printSetInfo where combineId = @combineId and range = @range order by id asc", new { combineId = scheme.Id, range = 2 });
                if (printSetInfos != null && printSetInfos.Count > 0)
                {
                    foreach (var info in printSetInfos)
                    {
                        PrintSetInfoDto voucher = new PrintSetInfoDto();
                        voucher.Id = info.Id;
                        voucher.Name = info.Name;
                        voucher.Describe = info.Describe;
                        voucher.Key = info.Key;
                        voucher.Value = info.Value;
                        voucher.Range = info.Range;
                        voucher.CombineId = info.CombineId;

                        groupScheme.SetInfoList.Add(voucher);
                    }
                }
                #endregion

                switch (printGroup.PrintCode)
                {
                    case 10:
                        groupScheme.DishTypeCassify = null;
                        break;
                    case 20:
                        groupScheme.DishTypeCassify = null;
                        groupScheme.SetInfoList = null;
                        break;
                    case 30:
                        groupScheme.SetInfoList = null;
                        break;
                    case 40:
                        groupScheme.DishTypeCassify = null;
                        groupScheme.TableClassify = null;
                        groupScheme.SetInfoList = null;
                        break;
                    case 50:
                        groupScheme.DishTypeCassify = null;
                        groupScheme.TableClassify = null;
                        groupScheme.SetInfoList = null;
                        break;
                }

                printGroupSchemeList.Add(groupScheme);
            }

            Flag = true;
            return printGroupSchemeList;
        }

        /// <summary>
        /// 删除打印方案
        /// </summary>
        /// <param name="schemeId"></param>
        public void DeleteGroupScheme(int schemeId)
        {
            var printGroupSchemes = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme where id = @id", new { id = schemeId });

            if (printGroupSchemes != null)
            {
                _repositoryContext.Execute("delete from tb_printGroupScheme Where Id = @Id", new { Id = schemeId });
                _repositoryContext.Execute("delete from tb_schemeTable Where SchemeId = @SchemeId", new { SchemeId = schemeId });
                _repositoryContext.Execute("delete from tb_schemeDishType Where SchemeId = @SchemeId", new { SchemeId = schemeId });
                _repositoryContext.Execute("delete from tb_schemeVoucher Where SchemeId = @SchemeId", new { SchemeId = schemeId });
                _repositoryContext.Execute("delete from tb_printSetInfo Where CombineId = @SchemeId and range != 1 ", new { SchemeId = schemeId });

                SetPrintDefalut(printGroupSchemes.GroupId);
            }
            else
            {
                Flag = false;
                Message = "删除门店打印方案异常：没有找到对应的门店打印方案信息";
                this.Code = PrintErrorCode.Code.ResultNull;
                return;
            }

            Flag = true;
        }

        /// <summary>
        /// 添加/编辑打印方案
        /// </summary>
        /// <param name="printGroupScheme"></param>
        public void ModifyGroupScheme(PrintGroupSchemeDto printGroupSchemeDto)
        {
            if (printGroupSchemeDto == null)
            {
                Flag = false;
                Message = "添加/编辑打印方案：没有找到对应的打印方案信息";
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                return;
            }

            if (string.IsNullOrWhiteSpace(printGroupSchemeDto.PrintId) || string.IsNullOrWhiteSpace(printGroupSchemeDto.Pcid))
            {
                Flag = false;
                Message = "机器码和打印机ID不能为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }

            var print = _repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,terminalName,printName,alias,connStyle,connAddress,connBrand,connPort,paperType,paperWidth,topMargin,leftMargin,modifyTime,isDefault,updated,  enable,state from tb_printconfig where printId = @printId ", new { printId = printGroupSchemeDto.PrintId });
            if (print == null)
            {
                Flag = false;
                Message = "找不到对应的打印机";
                this.Code = PrintErrorCode.Code.PrintConfigNullError;
                return;
            }

            //按照升序排列出当前方案组下所有的打印方案
            var printGroupSchemes = _repositoryContext.GetSet<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme where groupId = @groupId order by createTime asc ", new { groupId = printGroupSchemeDto.GroupId });
            if (printGroupSchemes != null && printGroupSchemes.Count > 0)
            {
                //在打印方案里面获取除了自身之外其他的跟打印机ID一致的打印方案
                var tempGroupScheme = printGroupSchemes.FirstOrDefault(t => t.PrintId == printGroupSchemeDto.PrintId && t.Id != printGroupSchemeDto.Id);
                if (tempGroupScheme != null)
                {
                    Flag = false;
                    Message = "同一方案不可添加相同打印机";
                    this.Code = PrintErrorCode.Code.SamePrintSchemeError;
                    DataBaseLog.Writer.SendInfo(Message + ":" + JsonConvert.SerializeObject(printGroupSchemeDto));
                    return;
                }

                var printGroupSchemeTimrSet = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme where id = @id ", new { id = printGroupSchemeDto.Id });
                if (printGroupSchemeTimrSet == null)
                {
                    Flag = false;
                    Message = "当前编辑的打印方案错误";
                    this.Code = PrintErrorCode.Code.GetModifyPrintSchemeError;
                    return;
                }
                if (printGroupSchemeTimrSet.ModifyTime != printGroupSchemeDto.ModifyTime)
                {
                    Flag = false;
                    Message = "添加打印机时间戳错误";
                    this.Code = PrintErrorCode.Code.TimeStampError;
                    return;
                }

                var result = _repositoryContext.Execute("UPDATE tb_printGroupScheme SET pcid=@pcid,name=@name,printId=@printId,isDefault = @isDefault,state = @state,groupId=@groupId,modifyTime = datetime('now', 'localtime') WHERE id = @id", printGroupSchemeDto);
                if (result <= 0)
                {
                    Flag = false;
                    Message = "更新数据失败！";
                    this.Code = PrintErrorCode.Code.UpdateError;
                    return;
                }

                StringBuilder sqlSb = new StringBuilder();

                if (printGroupSchemeDto.TableClassify != null && printGroupSchemeDto.TableClassify.Count > 0)
                {
                    _repositoryContext.Execute("delete from tb_schemeTable Where schemeId = @schemeId", new { schemeId = printGroupSchemeTimrSet.Id });
                    LoadTableClassify(printGroupSchemeDto.TableClassify, sqlSb, printGroupSchemeTimrSet.Id);
                }

                if (printGroupSchemeDto.DishTypeCassify != null && printGroupSchemeDto.DishTypeCassify.Count > 0)
                {
                    _repositoryContext.Execute("delete from tb_schemeDishType Where schemeId = @schemeId", new { schemeId = printGroupSchemeTimrSet.Id });
                    LoadDishTypeCassify(printGroupSchemeDto.DishTypeCassify, sqlSb, printGroupSchemeTimrSet.Id);
                }

                if (printGroupSchemeDto.SetInfoList != null && printGroupSchemeDto.SetInfoList.Count > 0)
                {
                    _repositoryContext.Execute("delete from tb_printSetInfo Where combineId = @combineId", new { combineId = printGroupSchemeTimrSet.Id });
                    LoadSetInfoList(printGroupSchemeDto.SetInfoList, sqlSb, printGroupSchemeTimrSet.Id);
                }

                if (printGroupSchemeDto.VoucherList != null && printGroupSchemeDto.VoucherList.Count > 0)
                {
                    _repositoryContext.Execute("delete from tb_schemeVoucher Where schemeId = @schemeId", new { schemeId = printGroupSchemeTimrSet.Id });
                    LoadVoucherList(printGroupSchemeDto.VoucherList, sqlSb, printGroupSchemeTimrSet.Id);
                }

                try
                {
                    DBHelper.Instance.GetTransactionSql(sqlSb);
                    _repositoryContext.Execute(sqlSb.ToString(), null);
                }
                catch (Exception e)
                {
                    Flag = false;
                    Message = "添加/编辑打印方案失败：";
                    //Message = "添加/编辑打印方案失败：" + e.Message;
                    this.Code = PrintErrorCode.Code.UpdateError;
                    return;
                }
            }
            else
            {
                Flag = false;
                Message = "编辑打印方案失败";
                this.Code = PrintErrorCode.Code.ModifyPrintSchemeError;
                return;
            }
        }

        /// <summary>
        /// 添加打印方案
        /// </summary>
        /// <param name="printGroupSchemeDto"></param>
        public void CreateGroupScheme(PrintGroupSchemeDto printGroupSchemeDto)
        {
            if (printGroupSchemeDto == null)
            {
                Flag = false;
                Message = "添加打印方案为空";
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                return;
            }
            if (string.IsNullOrWhiteSpace(printGroupSchemeDto.PrintId) || string.IsNullOrWhiteSpace(printGroupSchemeDto.Pcid))
            {
                Flag = false;
                Message = "机器码和打印机ID不能为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var print = _repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,terminalName,printName,alias,connStyle,connAddress,connBrand,connPort,paperType,paperWidth,topMargin,leftMargin,modifyTime,isDefault,updated,  enable,state from tb_printconfig where printId = @printId ", new { printId = printGroupSchemeDto.PrintId });
            if (print == null)
            {
                Flag = false;
                Message = "找不到对应的打印机";
                this.Code = PrintErrorCode.Code.PrintConfigNullError;
                return;
            }

            //同一方案下不允许有同一个打印机
            var samePrinter = _repositoryContext.QueryFirstOrDefault<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme where groupId = @groupId and printId = @printId ", new { groupId = printGroupSchemeDto.GroupId, printId = printGroupSchemeDto.PrintId });
            if (samePrinter != null)
            {
                Flag = false;
                Message = "同一方案不可添加相同打印机";
                this.Code = PrintErrorCode.Code.SamePrinterError;
                DataBaseLog.Writer.SendInfo(Message);
                return;
            }

            #region 新增打印方案
            var printGroupScheme = new PrintGroupScheme
            {
                Id = printGroupSchemeDto.Id,
                Pcid = printGroupSchemeDto.Pcid,
                Name = printGroupSchemeDto.Name,
                PrintId = printGroupSchemeDto.PrintId,
                IsDefault = printGroupSchemeDto.IsDefault,
                State = printGroupSchemeDto.State,
                GroupId = printGroupSchemeDto.GroupId,
                CreateTime = printGroupSchemeDto.CreateTime,
                ModifyTime = printGroupSchemeDto.ModifyTime
            };

            var result = _repositoryContext.Execute("INSERT INTO tb_printGroupScheme(pcid,name,printId,isDefault,state,groupId)VALUES(@pcid,@name,@printId,@isDefault,@state,@groupId)", printGroupScheme);
            if (result == 0)
            {
                Flag = false;
                Message = "插入本地打印数据失败";
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            var prinrGroupSchemes = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,modifyTime,createTime from tb_printGroupScheme where printId = @printId and groupId = @groupId", new { printId = printGroupSchemeDto.PrintId, groupId = printGroupSchemeDto.GroupId });
            if (prinrGroupSchemes == null)
            {
                Flag = false;
                Message = "本地打印数据更新成功，但是没能获取到插入的本机打印机信息";
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }

            StringBuilder sqlSb = new StringBuilder();

            if (printGroupSchemeDto.TableClassify != null && printGroupSchemeDto.TableClassify.Count > 0)
            {
                LoadTableClassify(printGroupSchemeDto.TableClassify, sqlSb, prinrGroupSchemes.Id);
            }

            if (printGroupSchemeDto.DishTypeCassify != null && printGroupSchemeDto.DishTypeCassify.Count > 0)
            {
                LoadDishTypeCassify(printGroupSchemeDto.DishTypeCassify, sqlSb, prinrGroupSchemes.Id);
            }

            if (printGroupSchemeDto.SetInfoList != null && printGroupSchemeDto.SetInfoList.Count > 0)
            {
                LoadSetInfoList(printGroupSchemeDto.SetInfoList, sqlSb, prinrGroupSchemes.Id);
            }

            if (printGroupSchemeDto.VoucherList != null && printGroupSchemeDto.VoucherList.Count > 0)
            {
                LoadVoucherList(printGroupSchemeDto.VoucherList, sqlSb, prinrGroupSchemes.Id);
            }

            try
            {
                DBHelper.Instance.GetTransactionSql(sqlSb);
                _repositoryContext.Execute(sqlSb.ToString(), null);
            }
            catch (Exception e)
            {
                Flag = false;
                Message = "添加打印方案失败：";
                //Message = "添加打印方案失败：" + e.Message + "\r\n" + sqlSb;
                this.Code = PrintErrorCode.Code.UpdateError;
                DataBaseLog.Writer.SendInfo(Message);
                return;
            }

            Flag = true;
            #endregion

            SetPrintDefalut(printGroupSchemeDto.GroupId);
        }

        private void SetPrintDefalut(int groupId)
        {
            #region 设置默认打印
            var inDefault = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme where groupId = @groupId and isDefault = 1 ", new { groupId = groupId }); ;//判断是否有默认打印机的存在
            if (inDefault == null)//不存在默认打印方案
            {
                //获取第一个添加的打印方案
                var printGroupSchemeDefault = _repositoryContext.FirstOrDefault<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme where groupId = @groupId order by createTime asc  ", new { groupId = groupId });

                if (printGroupSchemeDefault != null)
                {
                    var defaultResult = _repositoryContext.Execute("update tb_printGroupScheme set isDefault = 1 where id = @id and groupId = @groupId", new { id = printGroupSchemeDefault.Id, groupId = groupId });

                    if (defaultResult > 0)
                    {
                        Flag = true;

                        var notDefaultResult = _repositoryContext.Execute("update tb_printGroupScheme set isDefault = 0 where id != @id and groupId = @groupId", new { id = printGroupSchemeDefault.Id, groupId = groupId });
                    }
                    else
                    {
                        Flag = false;
                        Message = "更新数据失败1！";
                        this.Code = PrintErrorCode.Code.UpdateError;
                    }
                }
            }
            #endregion
        }

        #region 方案下的常用更改
        /// <summary>
        /// 餐桌
        /// </summary>
        /// <param name="schemeTableDto"></param>
        /// <param name="sqlSb"></param>
        /// <param name="schemeId"></param>
        public void LoadTableClassify(List<SchemeTableDto> schemeTableDto, StringBuilder sqlSb, int schemeId)
        {
            foreach (var table in schemeTableDto)
            {
                sqlSb.Append($"insert into tb_schemeTable(schemeId,mkTableID,erpTableID,erpTableAreaID,tableName)VALUES({schemeId},'{table.MKTableID}','{table.ErpTableID}','{table.ErpTableAreaID}','{table.TableName}');");
            }
        }

        /// <summary>
        /// 菜品分类
        /// </summary>
        /// <param name="schemeDishTypeDto"></param>
        /// <param name="sqlSb"></param>
        /// <param name="schemeId"></param>
        public void LoadDishTypeCassify(List<SchemeDishTypeDto> schemeDishTypeDto, StringBuilder sqlSb, int schemeId)
        {
            foreach (var type in schemeDishTypeDto)
            {
                sqlSb.Append($"insert into tb_schemeDishType(schemeId,mkDishTypeID,erpDishTypeID,dishTypeName)VALUES({schemeId},'{type.MKDishTypeID}','{type.ErpDishTypeID}','{type.DishTypeName}');");
            }
        }

        /// <summary>
        /// 打印方案设置
        /// </summary>
        /// <param name="printSetInfoDto"></param>
        /// <param name="sqlSb"></param>
        /// <param name="schemeId"></param>
        public void LoadSetInfoList(List<PrintSetInfoDto> printSetInfoDto, StringBuilder sqlSb, int schemeId)
        {
            foreach (var info in printSetInfoDto)
            {
                info.Value = info.Value.ToLower();

                sqlSb.Append($"insert into tb_printSetInfo(name,describe,key,value,range,combineId)VALUES('{info.Name}','{info.Describe}','{info.Key}','{info.Value}',{info.Range},{schemeId});");
            }
        }

        /// <summary>
        /// 打印方案单据
        /// </summary>
        /// <param name="schemeVoucherDto"></param>
        /// <param name="sqlSb"></param>
        /// <param name="schemeId"></param>
        public void LoadVoucherList(List<SchemeVoucherDto> schemeVoucherDto, StringBuilder sqlSb, int schemeId)
        {
            foreach (var table in schemeVoucherDto)
            {
                sqlSb.Append($"insert into tb_schemeVoucher(name,describe,voucherCode,templateCode,isEnabled,printNum,pattern,schemeId)VALUES('{table.Name}','{table.Describe}',{table.VoucherCode},'{table.TemplateCode}',{table.IsEnabled},{table.PrintNum},{table.Pattern},{schemeId});");
            }
        }
        #endregion

        /// <summary>
        /// 测试打印方案
        /// </summary>
        /// <param name="printGroupSchemeDto"></param>
        public void PrintSchemeTest(PrintGroupSchemeDto printGroupSchemeDto)
        {
            if (printGroupSchemeDto == null)
            {
                Flag = false;
                Message = "添加打印方案为空";
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                return;
            }
            if (string.IsNullOrWhiteSpace(printGroupSchemeDto.PrintId) || string.IsNullOrWhiteSpace(printGroupSchemeDto.Pcid))
            {
                Flag = false;
                Message = "机器码和打印机ID不能为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var print = _repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,terminalName,printName,alias,connStyle,connAddress,connBrand,connPort,paperType,paperWidth,topMargin,leftMargin,modifyTime,isDefault,updated,enable,state from tb_printconfig where printId = @printId ", new { printId = printGroupSchemeDto.PrintId });
            if (print == null)
            {
                Flag = false;
                Message = "找不到对应的打印机";
                this.Code = PrintErrorCode.Code.PrintConfigNullError;
                return;
            }

            PrintConfigDto printConfigDto = new PrintConfigDto()
            {
                PrintId = print.PrintId,
                Pcid = print.Pcid,
                TerminalName = print.TerminalName,
                PrintName = print.PrintName,
                Alias = print.Alias,
                ConnStyle = print.ConnStyle,
                ConnAddress = print.ConnAddress,
                ConnBrand = print.ConnBrand,
                ConnPort = print.ConnPort,
                PaperType = print.PaperType,
                PaperWidth = print.PaperWidth,
                TopMargin = print.TopMargin,
                LeftMargin = print.LeftMargin,
                ModifyTime = print.ModifyTime,
                IsDefault = print.IsDefault,
                Updated = print.Updated,
                Enable = print.Enable,
                State = print.State,
                PrintNum = 1,
            };

            foreach (var voucher in printGroupSchemeDto.VoucherList)
            {
                var printDocumentServices = ServiceLocator.Instance.Resolve<IPrintDocumentServices>();

                if (voucher.IsEnabled == 0)
                {
                    continue;
                }
                printDocumentServices.PrintTestByTemplateCodeV2(voucher.TemplateCode, printConfigDto, voucher.VoucherCode);
            }
        }
        #endregion

        /// <summary>
        /// 获取高级设置
        /// </summary>
        /// <param name="pcId"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public List<PrintSetInfoDto> GetGroupSchemeTopSetInfo(string pcId, int groupId)
        {
            List<PrintSetInfoDto> result = new List<PrintSetInfoDto>();
            var printSetInfos = _repositoryContext.GetSet<PrintSetInfo>("select id,name,describe,key,value,range,combineId from tb_printSetInfo where combineId = @combineId and range = @range order by id asc", new { combineId = groupId, range = 1 });
            if (printSetInfos != null && printSetInfos.Count > 0)
            {
                foreach (var info in printSetInfos)
                {
                    PrintSetInfoDto voucher = new PrintSetInfoDto();
                    voucher.Id = info.Id;
                    voucher.Name = info.Name;
                    voucher.Describe = info.Describe;
                    voucher.Key = info.Key;
                    voucher.Value = info.Value;
                    voucher.Range = info.Range;
                    voucher.CombineId = info.CombineId;

                    result.Add(voucher);
                }
            }

            return result;
        }

        /// <summary>
        /// 添加打印方案
        /// </summary>
        /// <param name="groupId"></param>
        /// <returns></returns>
        public PrintGroupSchemeDto AddGroupScheme(int printCode)
        {
            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode", new { printCode = printCode });
            if (printGroup == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return new PrintGroupSchemeDto();
            }

            PrintGroupSchemeDto groupScheme = new PrintGroupSchemeDto();
            groupScheme.Id = 0;
            groupScheme.Pcid = "";
            groupScheme.Name = "";
            groupScheme.PrintId = "";
            groupScheme.IsDefault = 0;
            groupScheme.State = 1;
            groupScheme.GroupId = printGroup.Id;
            groupScheme.CreateTime = DateTime.Now;
            groupScheme.ModifyTime = DateTime.Now;
            bool isOpenQC = BusinessHelper.Instance._RestaurantsInfo.IsOpenQC;

            switch (printCode)
            {
                case 10:
                    groupScheme.TableClassify = new List<SchemeTableDto>();
                    groupScheme.SetInfoList = new List<PrintSetInfoDto>();
                    groupScheme.VoucherList = new List<SchemeVoucherDto>();
                    groupScheme.DishTypeCassify = null;
                    {
                        if (!isOpenQC)
                        {
                            SchemeVoucherDto voucherDcd = new SchemeVoucherDto();
                            voucherDcd.Id = 0;
                            voucherDcd.Name = "点菜单";
                            voucherDcd.Describe = "";
                            voucherDcd.VoucherCode = 10;
                            voucherDcd.TemplateCode = "PRT_SO_0001";
                            voucherDcd.IsEnabled = 1;
                            voucherDcd.PrintNum = 1;
                            voucherDcd.Pattern = isOpenQC ? 3 : 2;
                            voucherDcd.SchemeId = groupScheme.Id;

                            groupScheme.VoucherList.Add(voucherDcd);
                        }

                        SchemeVoucherDto voucherJzd = new SchemeVoucherDto();
                        voucherJzd.Id = 0;
                        voucherJzd.Name = "结账单";
                        voucherJzd.Describe = "";
                        voucherJzd.VoucherCode = 11;
                        voucherJzd.TemplateCode = "PRT_SO_0001";
                        voucherJzd.IsEnabled = 1;
                        voucherJzd.PrintNum = 1;
                        voucherJzd.Pattern = isOpenQC ? 3 : 2;
                        voucherJzd.SchemeId = groupScheme.Id;

                        groupScheme.VoucherList.Add(voucherJzd);

                        PrintSetInfoDto setInfoXdsb = new PrintSetInfoDto();
                        setInfoXdsb.Id = 0;
                        setInfoXdsb.Name = "下单失败打印《下单失败》";
                        setInfoXdsb.Describe = "《下单失败》";
                        setInfoXdsb.Key = "101";
                        setInfoXdsb.Value = "true";
                        setInfoXdsb.Range = 2;
                        setInfoXdsb.CombineId = groupScheme.Id;
                        groupScheme.SetInfoList.Add(setInfoXdsb);

                        PrintSetInfoDto setInfoHxsb = new PrintSetInfoDto();
                        setInfoHxsb.Id = 0;
                        setInfoHxsb.Name = "结账失败打印《核销失败》";
                        setInfoHxsb.Describe = "《核销失败》";
                        setInfoHxsb.Key = "102";
                        setInfoHxsb.Value = "true";
                        setInfoHxsb.Range = 2;
                        setInfoHxsb.CombineId = groupScheme.Id;
                        groupScheme.SetInfoList.Add(setInfoHxsb);
                    }

                    break;
                case 20:
                    groupScheme.DishTypeCassify = null;
                    groupScheme.TableClassify = new List<SchemeTableDto>();
                    groupScheme.SetInfoList = null;
                    groupScheme.VoucherList = new List<SchemeVoucherDto>();
                    {
                        SchemeVoucherDto voucherZfpz = new SchemeVoucherDto();
                        voucherZfpz.Id = 0;
                        voucherZfpz.Name = "支付凭证";
                        voucherZfpz.Describe = "";
                        voucherZfpz.VoucherCode = 20;
                        voucherZfpz.TemplateCode = "PRT_FI_0001";
                        voucherZfpz.IsEnabled = 1;
                        voucherZfpz.PrintNum = 1;
                        voucherZfpz.Pattern = isOpenQC ? 3 : 2;
                        voucherZfpz.SchemeId = 0;

                        groupScheme.VoucherList.Add(voucherZfpz);

                        SchemeVoucherDto voucherTkpz = new SchemeVoucherDto();
                        voucherTkpz.Id = 0;
                        voucherTkpz.Name = "退款凭证";
                        voucherTkpz.Describe = "";
                        voucherTkpz.VoucherCode = 21;
                        voucherTkpz.TemplateCode = "PRT_FI_0002";
                        voucherTkpz.IsEnabled = 1;
                        voucherTkpz.PrintNum = 1;
                        voucherTkpz.Pattern = isOpenQC ? 3 : 2;
                        voucherTkpz.SchemeId = 0;

                        groupScheme.VoucherList.Add(voucherTkpz);
                    }

                    break;
                case 30:
                    groupScheme.DishTypeCassify = new List<SchemeDishTypeDto>();
                    groupScheme.TableClassify = new List<SchemeTableDto>();
                    groupScheme.SetInfoList = null;
                    groupScheme.VoucherList = new List<SchemeVoucherDto>();
                    {
                        SchemeVoucherDto voucherZd = new SchemeVoucherDto();
                        voucherZd.Id = 0;
                        voucherZd.Name = "厨打总单";
                        voucherZd.Describe = "（一单一切）";
                        voucherZd.VoucherCode = 30;
                        voucherZd.TemplateCode = "PRT_SO_3001";
                        voucherZd.IsEnabled = 1;
                        voucherZd.PrintNum = 1;
                        voucherZd.Pattern = isOpenQC ? 3 : 2;
                        voucherZd.SchemeId = 0;

                        groupScheme.VoucherList.Add(voucherZd);

                        SchemeVoucherDto voucherFd = new SchemeVoucherDto();
                        voucherFd.Id = 0;
                        voucherFd.Name = "厨打分单";
                        voucherFd.Describe = "（一菜一切）";
                        voucherFd.VoucherCode = 31;
                        voucherFd.TemplateCode = "PRT_SO_3002";
                        voucherFd.IsEnabled = 1;
                        voucherFd.PrintNum = 1;
                        voucherFd.Pattern = isOpenQC ? 3 : 2;
                        voucherFd.SchemeId = 0;

                        groupScheme.VoucherList.Add(voucherFd);
                    }

                    break;
                case 40:
                    groupScheme.DishTypeCassify = null;
                    groupScheme.TableClassify = null;
                    groupScheme.SetInfoList = null;
                    groupScheme.VoucherList = new List<SchemeVoucherDto>();
                    {
                        SchemeVoucherDto voucher = new SchemeVoucherDto();
                        voucher.Id = 0;
                        voucher.Name = "预点单";
                        voucher.Describe = "";
                        voucher.VoucherCode = 40;
                        voucher.TemplateCode = "PRT_SO_1001";
                        voucher.IsEnabled = 1;
                        voucher.PrintNum = 1;
                        voucher.Pattern = isOpenQC ? 3 : 2;
                        voucher.SchemeId = 0;

                        groupScheme.VoucherList.Add(voucher);
                    }
                    break;
                case 50:
                    groupScheme.DishTypeCassify = null;
                    groupScheme.TableClassify = null;
                    groupScheme.SetInfoList = null;
                    groupScheme.VoucherList = new List<SchemeVoucherDto>();
                    {
                        SchemeVoucherDto voucher = new SchemeVoucherDto();
                        voucher.Id = 0;
                        voucher.Name = "外卖单";
                        voucher.Describe = "";
                        voucher.VoucherCode = 50;
                        voucher.TemplateCode = "PRT_TO_0001";
                        voucher.IsEnabled = 1;
                        voucher.PrintNum = 1;
                        voucher.Pattern = isOpenQC ? 3 : 2;
                        voucher.SchemeId = 0;

                        groupScheme.VoucherList.Add(voucher);
                    }
                    break;
            }

            Flag = true;
            return groupScheme;
        }

        public void ModifySetInfo(PrintSetInfoDto printSetInfoDto)
        {
            var printSetInfos = _repositoryContext.FirstOrDefault<PrintSetInfo>("select id,name,describe,key,value,range,combineId from tb_printSetInfo where combineId = @combineId and range = @range and id = @id", new { combineId = printSetInfoDto.CombineId, range = 1, id = printSetInfoDto.Id });
            if (printSetInfos == null)
            {
                Flag = false;
                Message = "编辑高级设置失败";
                Code = PrintErrorCode.Code.EditTopSetInfoError;
                return;
            }
            var result = _repositoryContext.Execute($"update tb_printSetInfo set value = '{printSetInfoDto.Value}' where id = {printSetInfoDto.Id}", null);
            if (result > 0)
            {
                Flag = true;
            }
            else
            {
                Flag = false;
                Message = "编辑高级设置失败";
                Code = PrintErrorCode.Code.EditTopSetInfoError;
            }
        }

        public void PutDefaultGroupScheme(int schemeId, int groupId)
        {
            var defaultResult = _repositoryContext.Execute($"update tb_printGroupScheme set isDefault = 1 where id = {schemeId} and groupId = {groupId} ; update tb_printGroupScheme set isDefault = 0 where id != {schemeId} and groupId = {groupId} ;", new { });

            if (defaultResult > 0)
            {
                Flag = true;
            }
            else
            {
                Flag = false;
                Message = "更新数据失败！";
                this.Code = PrintErrorCode.Code.UpdateError;
            }
        }
    }
}
