using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hydra.Framework.MqttUtility;
using Hydra.Framework.NancyExpand;
using Hydra.Framework.Services.Aop.RegistrationAttributes;
using Hydra.Framework.Services.Utility;
using Hydra.Framework.SqlContent;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.PrintDataDto;
using ShanDian.AddIns.Print.Dto.View;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Model;
using ShanDian.AddIns.Print.Data;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Module
{
    public class PrintSchemeServices : CommunicationServer, IPrintSchemeServices
    {
        private IRepositoryContext _repositoryContext;

        public PrintSchemeServices()
        {
            _repositoryContext = Global.RepositoryContext();
        }

        /// <summary>
        /// 获取打印列表
        /// </summary>
        /// <param name="pcId"></param>
        /// <param name="isGlobal"></param>
        /// <returns></returns>
        public List<VoucherDto> GetVoucherList(string pcId, bool isGlobal)
        {
            var voucherDtos = new List<VoucherDto>();

            if (string.IsNullOrEmpty(pcId))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return new List<VoucherDto>();
            }

            //var printSchemes = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where PcId=@PcId", new { PcId = pcId });
            var sql = new StringBuilder();
            sql.Append(
                "select Id,VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Sort,Path from tb_voucher");
            if (isGlobal)
            {
                sql.Append(" where globalVoucher =1");
            }
            else
            {
                sql.Append(" where localVoucher =1");
            }
            sql.Append(" ORDER BY Sort");
            var vouchers = _repositoryContext.GetSet<Voucher>(sql.ToString());
            if (vouchers == null || vouchers.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.VoucherNullError;
                return voucherDtos;
            }
            var ints = new List<int>();
            foreach (var item in vouchers)
            {
                ints.Add(item.Id);
            }

            var printSchemes = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme t WHERE t.VoucherId IN @ids", new { ids = ints });

            foreach (var voucher in vouchers)
            {
                var schemes = printSchemes.Where(x => x.VoucherId == voucher.Id && x.LocalMachine != 0).ToList();
                var voucherDto = new VoucherDto
                {
                    Id = voucher.Id,
                    VoucherName = voucher.VoucherName,
                    TemplateCode = voucher.TemplateCode,
                    GroupCode = voucher.GroupCode,
                    Enble = voucher.Enble,
                    Overall = voucher.Overall,
                    Sort = voucher.Sort,
                    SchemeNum = schemes.Count(),
                    Path = voucher.Path
                };

                voucherDtos.Add(voucherDto);
            }
            //if (printSchemes != null)
            //{
            //    foreach (var item in printSchemes)
            //    {
            //        var enumerable = voucherDtos.FirstOrDefault(x => x.Id == item.VoucherId);
            //        if (enumerable != null)
            //        {
            //            enumerable.PrintNum = item.PrintNum;
            //        }
            //    }
            //}

            Flag = true;
            return voucherDtos;
        }

        /// <summary>
        /// 获取指定的打印方案
        /// </summary>
        /// <param name="voucherId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public PageListDto<PrintSchemeDto> GetPrintScheme(int voucherId, int pageIndex, int pageSize = 6)
        {
            PageListDto<PrintSchemeDto> page = null;
            if (voucherId == 0 || pageIndex == 0 || pageSize == 0)
            {
                page = new PageListDto<PrintSchemeDto>(null, pageIndex, pageSize, 0);
                Flag = false;
                Code = PrintErrorCode.Code.ParamsError;
                return page;
            }
            try
            {
                var printSchemeDtos = new List<PrintSchemeDto>();

                List<PrintScheme> printSchemeList;
                printSchemeList = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where VoucherId=@VoucherId AND LocalMachine!=0", new { VoucherId = voucherId });
                if (printSchemeList == null || printSchemeList.Count < 1)
                {
                    page = new PageListDto<PrintSchemeDto>(null, pageIndex, pageSize, 0);
                    Flag = true;
                    Code = PrintErrorCode.Code.ResultNull;
                    return page;
                }
                List<int> ids = new List<int>();
                List<string> printIds = new List<string>();
                foreach (var printScheme in printSchemeList)
                {
                    ids.Add(printScheme.Id);
                    printIds.Add(printScheme.PrintId);
                }
                var printconfigs = _repositoryContext.GetSet<PrintConfig>("select id,printId,pcid,state,printName,alias from tb_printconfig t WHERE t.PrintId IN @ids", new { ids = printIds });
                List<PrintSchemeLabelDto> printSchemeLabelDtos = GetPrintSchemeLabelDtoList(ids);
                foreach (var printScheme in printSchemeList)
                {
                    var printSchemeDto = new PrintSchemeDto
                    {
                        Id = printScheme.Id,
                        Name = printScheme.Name,
                        PrintId = printScheme.PrintId,
                        VoucherId = printScheme.VoucherId,
                        LocalMachine = printScheme.LocalMachine,
                        PcId = printScheme.PcId,
                        PrintNum = printScheme.PrintNum,
                        TagList = new List<PrintSchemeLabelDto>()
                    };
                    if (printconfigs != null && printconfigs.Count > 0)
                    {
                        var config = printconfigs.FirstOrDefault(x => x.PrintId == printScheme.PrintId);
                        if (config!=null)
                        {
                            if (!string.IsNullOrEmpty(config.Alias))
                            {
                                printSchemeDto.Name = config.Alias;
                            }
                        }
                    }
                    Guid schemeCode = Guid.Empty;
                    Guid.TryParse(printScheme.SchemeCode, out schemeCode);
                    printSchemeDto.SchemeCode = schemeCode.ToString();
                    if (printSchemeDto.LocalMachine != 0)
                    {
                        var tagList = printSchemeLabelDtos.Where(x => x.PrintSchemeId == printSchemeDto.Id);
                        printSchemeDto.TagList.AddRange(tagList);
                    }
                    printSchemeDtos.Add(printSchemeDto);
                }
                int skipCnt = (pageIndex - 1) * pageSize;
                if (skipCnt < printSchemeDtos.Count())
                {
                    skipCnt = (pageIndex - 1) * pageSize;
                }
                else
                {
                    var i = printSchemeDtos.Count() % pageSize;
                    pageIndex = printSchemeDtos.Count() / pageSize;
                    if (i > 0)
                    {
                        pageIndex++;
                    }
                    skipCnt = (pageIndex - 1) * pageSize;
                }

                page = new PageListDto<PrintSchemeDto>(printSchemeDtos.Skip(skipCnt).Take(pageSize).ToList(), pageIndex, pageSize, printSchemeDtos.Count());

                Flag = true;
                return page;
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.UpdateError;
                PrintLogUtility.Writer.SendFullError(ex);
                return page;
            }
        }

        private List<PrintSchemeDto> GetPrintSchemeDtos(List<PrintScheme> list)
        {
            var schemeDtos = new List<PrintSchemeDto>();
            if (list == null)
            {
                return schemeDtos;
            }
            foreach (var item in list)
            {
                var printSchemeDto = GetPrintScheme(item);
                schemeDtos.Add(printSchemeDto);
            }
            return schemeDtos;
        }

        private PrintSchemeDto GetPrintScheme(PrintScheme printScheme)
        {
            if (printScheme == null)
            {
                return new PrintSchemeDto();
            }
            var printSchemeDto = new PrintSchemeDto()
            {
                Id = printScheme.Id,
                LocalMachine = printScheme.LocalMachine,
                Name = printScheme.Name,
                PcId = printScheme.PcId,
                PrintId = printScheme.PrintId,
                PrintNum = printScheme.PrintNum,
                TagList = new List<PrintSchemeLabelDto>(),
                VoucherId = printScheme.VoucherId
            };
            printSchemeDto.TagList = new List<PrintSchemeLabelDto>();
            return printSchemeDto;
        }
        /// <summary>
        /// 获取打印方案标签
        /// </summary>
        /// <param name="printSchemeIdList"></param>
        /// <returns></returns>
        private List<PrintSchemeLabelDto> GetPrintSchemeLabelDtoList(List<int> printSchemeIdList)
        {
            var printSchemeLabelDtos = new List<PrintSchemeLabelDto>();
            var printSchemeLabels = _repositoryContext.GetSet<PrintSchemeLabel>("select Id,PrintSchemeId,LabelId,LabelName,LabelGroupCode,SchemeCode from tb_printSchemeLabel t WHERE t.PrintSchemeId IN @ids", new { ids = printSchemeIdList });

            foreach (var printSchemeLabel in printSchemeLabels)
            {
                var printSchemeLabelDto = new PrintSchemeLabelDto
                {
                    Id = printSchemeLabel.Id,
                    PrintSchemeId = printSchemeLabel.PrintSchemeId,
                    LabelId = printSchemeLabel.LabelId,
                    LabelName = printSchemeLabel.LabelName,
                    LabelGroupCode = printSchemeLabel.LabelGroupCode,
                    SchemeCode = printSchemeLabel.SchemeCode
                };

                printSchemeLabelDtos.Add(printSchemeLabelDto);
            }

            return printSchemeLabelDtos;
        }

        /// <summary>
        /// 批量获取指定的打印方案标签
        /// </summary>
        /// <param name="ints"></param>
        /// <returns></returns>
        public List<PrintSchemeLabelDto> GetPrintSchemeLabelDtoListByIdList(int[] ints)
        {
            var printSchemeLabelDtos = new List<PrintSchemeLabelDto>();
            var printSchemeLabels = _repositoryContext.GetSet<PrintSchemeLabel>("select Id,PrintSchemeId,LabelId,LabelName,LabelGroupCode from tb_printSchemeLabel t WHERE t.Id IN @ids", new { ids = ints });

            foreach (var printSchemeLabel in printSchemeLabels)
            {
                var printSchemeLabelDto = new PrintSchemeLabelDto
                {
                    Id = printSchemeLabel.Id,
                    PrintSchemeId = printSchemeLabel.PrintSchemeId,
                    LabelId = printSchemeLabel.LabelId,
                    LabelName = printSchemeLabel.LabelName,
                    LabelGroupCode = printSchemeLabel.LabelGroupCode
                };

                printSchemeLabelDtos.Add(printSchemeLabelDto);
            }

            Flag = true;
            return printSchemeLabelDtos;
        }

        /// <summary>
        /// 获取本地打印机
        /// </summary>
        /// <param name="pcId"></param>
        /// <returns></returns>
        public PrintConfigVDto GetLocalPrint(string pcId)
        {
            PrintConfigVDto printConfigVDto = new PrintConfigVDto();
            var localPrint = _repositoryContext.QueryFirstOrDefault<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where Machine=@Machine", new { Machine = pcId });
            if (localPrint == null)
            {
                Flag = true;
                Code = PrintErrorCode.Code.ResultNull;
                return null;
            }
            var printConfig = _repositoryContext.QueryFirstOrDefault<PrintConfig>("select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,leftMargin, modifyTime, isDefault, enable from tb_printconfig where printId=@printId", new { printId = localPrint.PrintId });
            if (printConfig == null)
            {
                Flag = true;
                Code = PrintErrorCode.Code.ResultNull;
                return null;
            }

            printConfigVDto.LocalPrintId = localPrint.Id;
            printConfigVDto.PrintId = printConfig.PrintId;
            printConfigVDto.Pcid = printConfig.Pcid;
            printConfigVDto.PrintName = printConfig.PrintName;
            printConfigVDto.Alias = printConfig.Alias;
            printConfigVDto.PaperType = printConfig.PaperType;
            printConfigVDto.TerminalName = printConfig.TerminalName;
            printConfigVDto.ConnStyle = printConfig.ConnStyle;
            printConfigVDto.PaperWidth = printConfig.PaperWidth;
            printConfigVDto.TopMargin = printConfig.TopMargin;
            printConfigVDto.LeftMargin = printConfig.LeftMargin;
            printConfigVDto.ModifyTime = printConfig.ModifyTime;

            printConfigVDto.PrintSchemeList = new List<PrintSchemeDto>();
            List<PrintScheme> printSchemeList = new List<PrintScheme>();
            printSchemeList = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where PrintId=@PrintId And LocalMachine=0 And PcId=@PcId", new { PrintId = printConfigVDto.PrintId, PcId = pcId });
            var printSchemeDtos = GetPrintSchemeDtos(printSchemeList);
            printConfigVDto.PrintSchemeList = printSchemeDtos;

            Flag = true;
            return printConfigVDto;
        }

        /// <summary>
        /// 添加本地打印机
        /// </summary>
        /// <param name="localPrint.Dto"></param>
        private void CreateLocalPrint(LocalPrint.Dto localPrint.Dto)
        {
            if (localPrint.Dto == null || string.IsNullOrWhiteSpace(localPrint.Dto.Machine) || string.IsNullOrWhiteSpace(localPrint.Dto.PrintId))
            {
                Flag = false;
                Message = "机器码和打印机ID不能为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            if (localPrint.Dto.VoucherList == null || localPrint.Dto.VoucherList.Count < 1)
            {
                Flag = false;
                Message = "请选择打印凭证";
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var localPrint = new LocalPrint
            {
                PrintId = localPrint.Dto.PrintId,
                Machine = localPrint.Dto.Machine
            };
            var queryFirstOrDefault = _repositoryContext.QueryFirstOrDefault<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where machine=@machine", new { machine = localPrint.Dto.Machine });
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
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            var localPrints = _repositoryContext.GetSet<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where printId=@printId AND machine=@machine ORDER BY Id DESC", new { printId = localPrint.Dto.PrintId, machine = localPrint.Dto.Machine });
            if (localPrints == null || localPrints.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            var voucherIds = localPrint.Dto.VoucherList.Select(x => x.Key).ToList();
            var print = localPrints.FirstOrDefault();
            var vouchers = _repositoryContext.GetSet<Voucher>("select Id,VoucherName,TemplateCode,GroupCode,Enble,Overall,Sort,Path from tb_voucher where Id in @voucherIds", new { voucherIds });
            if (vouchers != null && vouchers.Count > 0)
            {
                var printSchemes = new List<PrintScheme>();
                foreach (var voucher in vouchers)
                {
                    var printScheme = new PrintScheme
                    {
                        Name = voucher.VoucherName,
                        PrintId = print.PrintId,
                        VoucherId = voucher.Id,
                        LocalMachine = 0,
                        PcId = print.Machine,
                        PrintNum = localPrint.Dto.VoucherList[voucher.Id],
                        SchemeCode = Guid.NewGuid().ToString()
                    };

                    printSchemes.Add(printScheme);
                }
                CreatePrintSchemes(printSchemes);
            }
            Flag = true;
        }

        /// <summary>
        /// 批量创建打印方案
        /// </summary>
        /// <param name="printSchemes"></param>
        private void CreatePrintSchemes(List<PrintScheme> printSchemes)
        {
            _repositoryContext.Execute("INSERT INTO tb_printScheme(Name,PrintId,VoucherId,LocalMachine,PcId,PrintNum)VALUES (@Name,@PrintId,@VoucherId,@LocalMachine,@PcId,@PrintNum)", printSchemes);
        }

        public void UpdateLocalPrint(LocalPrint.Dto localPrint.Dto)
        {
            if (localPrint.Dto?.Machine == null || string.IsNullOrEmpty(localPrint.Dto.PrintId))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var queryFirstOrDefault = _repositoryContext.QueryFirstOrDefault<LocalPrint>("select Id,PrintId,Machine from tb_localPrint where machine=@machine", new { machine = localPrint.Dto.Machine });
            if (queryFirstOrDefault == null)
            {
                CreateLocalPrint(localPrint.Dto);
            }
            else
            {
                UpdateLocalPrintPrivate(localPrint.Dto);
            }
            Flag = true;
        }

        /// <summary>
        /// 设置本地打印机
        /// </summary>
        /// <param name="localPrint.Dto"></param>
        private void UpdateLocalPrintPrivate(LocalPrint.Dto localPrint.Dto)
        {
            var localPrint = new LocalPrint
            {
                Id = localPrint.Dto.Id,
                PrintId = localPrint.Dto.PrintId,
                Machine = localPrint.Dto.Machine
            };
            var result = _repositoryContext.Execute("UPDATE tb_localPrint SET PrintId=@PrintId WHERE Machine = @Machine", localPrint);
            if (result == 0)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            var printSchemeList = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where PcId=@PcId And LocalMachine=0", new { PcId = localPrint.Dto.Machine });
            if (printSchemeList == null)
            {
                Flag = true;
                return;
            }
            var printSchemeDtos = GetPrintSchemeDtos(printSchemeList);
            foreach (var voucherTuple in localPrint.Dto.VoucherList)
            {
                var printSchemeDto = printSchemeDtos.FirstOrDefault(x => x.VoucherId == voucherTuple.Key);
                if (printSchemeDto != null)
                {
                    printSchemeDto.PrintNum = voucherTuple.Value;
                    printSchemeDto.PrintId = localPrint.Dto.PrintId;
                    UpdatePrintScheme(printSchemeDto);
                    printSchemeDtos.Remove(printSchemeDto);
                }
                else
                {
                    var voucher = _repositoryContext.QueryFirstOrDefault<Voucher>("select Id,VoucherName,TemplateCode,GroupCode,Enble,Overall,Sort,Path from tb_voucher where Id=@Id", new { Id = voucherTuple.Key });
                    if (voucher != null)
                    {
                        var printScheme = new PrintSchemeDto
                        {
                            Name = voucher.VoucherName,
                            PrintId = localPrint.Dto.PrintId,
                            VoucherId = voucher.Id,
                            LocalMachine = 0,
                            PcId = localPrint.Dto.Machine,
                            PrintNum = localPrint.Dto.VoucherList[voucher.Id]
                        };
                        CreatePrintScheme(printScheme);
                    }
                }
            }
            if (printSchemeDtos.Any())
            {
                var ints = printSchemeDtos.Select(x => x.Id).ToList();
                foreach (var id in ints)
                {
                    DeletePrintScheme(id);
                }
            }
            Flag = true;
        }

        /// <summary>
        /// 添加打印方案
        /// </summary>
        /// <param name="dto"></param>
        public void CreatePrintScheme(PrintSchemeDto dto)
        {
            if (dto == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var print = _repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig where printId=@printId ", new { printId = dto.PrintId });
            if (print == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            dto.SchemeCode = Guid.NewGuid().ToString();
            var printScheme = new PrintScheme()
            {
                Id = dto.Id,
                Name = dto.Name,
                PrintId = dto.PrintId,
                VoucherId = dto.VoucherId,
                LocalMachine = dto.LocalMachine,
                PcId = dto.PcId,
                PrintNum = dto.PrintNum,
                SchemeCode = dto.SchemeCode.ToString()
            };
            if (printScheme.LocalMachine != 0)
            {
                var samePrinter = _repositoryContext.QueryFirstOrDefault<PrintScheme>(@"select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where PrintId=@PrintId AND LocalMachine!=0 AND VoucherId=@VoucherId", new { PrintId = dto.PrintId, VoucherId = dto.VoucherId });
                if (samePrinter != null)
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.SamePrinterError;
                    return;
                }
            }
            var rst = _repositoryContext.Execute("INSERT INTO tb_printScheme(Name,PrintId,VoucherId,LocalMachine,PcId,PrintNum,SchemeCode)VALUES (@Name,@PrintId,@VoucherId,@LocalMachine,@PcId,@PrintNum,@SchemeCode)", printScheme);
            if (rst == 0)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            var newPrintScheme = _repositoryContext.QueryFirstOrDefault<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where SchemeCode=@SchemeCode", new { SchemeCode = printScheme.SchemeCode });
            if (dto.TagList != null)
            {
                foreach (var item in dto.TagList)
                {
                    item.PrintSchemeId = newPrintScheme.Id;
                    item.SchemeCode = newPrintScheme.SchemeCode;
                }
                UpdatePrintSchemeLabelList(dto.TagList, newPrintScheme.Id);
            }
            Flag = true;
        }
        /// <summary>
        /// 删除打印方案
        /// </summary>
        /// <param name="id"></param>
        public void DeletePrintScheme(int id)
        {
            _repositoryContext.Execute("delete from tb_printScheme Where Id=@Id", new { Id = id });
            _repositoryContext.Execute("delete from tb_printSchemeLabel Where PrintSchemeId=@PrintSchemeId", new { PrintSchemeId = id });
            Flag = true;
        }

        /// <summary>
        /// 更新打印方案
        /// </summary>
        /// <param name="printScheme"></param>
        public void UpdatePrintScheme(PrintSchemeDto printScheme)
        {
            if (printScheme == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var model = new PrintScheme
            {
                Id = printScheme.Id,
                Name = printScheme.Name,
                PrintId = printScheme.PrintId,
                PrintNum = printScheme.PrintNum
            };
            var print = _repositoryContext.QueryFirstOrDefault<PrintConfig>(@"select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,
                            leftMargin,modifyTime,isDefault,enable from tb_printconfig where printId=@printId ", new { printId = printScheme.PrintId });
            if (print == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            if (printScheme.LocalMachine != 0)
            {
                var samePrinter = _repositoryContext.QueryFirstOrDefault<PrintScheme>(@"select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where PrintId=@PrintId AND LocalMachine!=0 AND VoucherId=@VoucherId", new { PrintId = printScheme.PrintId, VoucherId = printScheme.VoucherId });
                if (samePrinter != null && samePrinter.SchemeCode != printScheme.SchemeCode.ToString())
                {
                    Flag = false;
                    this.Code = PrintErrorCode.Code.SamePrinterError;
                    return;
                }
            }

            var result = _repositoryContext.Execute("UPDATE tb_printScheme SET Name=@Name,PrintId=@PrintId,PrintNum=@PrintNum WHERE id = @id", model);
            if (result == 0)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            var printSchemeLabelDtos = printScheme.TagList;
            if (printSchemeLabelDtos == null || printSchemeLabelDtos.Count < 1)
            {
                Flag = true;
                return;
            }
            foreach (var item in printSchemeLabelDtos)
            {
                item.PrintSchemeId = printScheme.Id;
                item.SchemeCode = printScheme.SchemeCode;
            }
            UpdatePrintSchemeLabelList(printSchemeLabelDtos, printScheme.Id);
            Flag = true;
        }

        /// <summary>
        /// 批量更新打印方案标签
        /// </summary>
        /// <param name="printSchemeLabelDtoList"></param>
        /// <param name="printSchemeId"></param>
        public void UpdatePrintSchemeLabelList(List<PrintSchemeLabelDto> printSchemeLabelDtoList, int printSchemeId)
        {
            if (printSchemeLabelDtoList == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            _repositoryContext.Execute("delete from tb_printSchemeLabel Where PrintSchemeId=@PrintSchemeId", new { PrintSchemeId = printSchemeId });

            CreatePrintSchemeLabelList(printSchemeLabelDtoList);
            Flag = true;
        }

        /// <summary>
        /// 更新打印方案标签
        /// </summary>
        /// <param name="printSchemeLabelDto"></param>
        private void UpdatePrintSchemeLabel(PrintSchemeLabelDto printSchemeLabelDto)
        {
            if (printSchemeLabelDto == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            var printSchemeLabel = GetPrintSchemeLabel(printSchemeLabelDto);

            var result = _repositoryContext.Execute("UPDATE tb_printSchemeLabel SET PrintSchemeId=@PrintSchemeId,LabelGroupCode=@LabelGroupCode,LabelId=@LabelId WHERE id = @id", printSchemeLabel);
            if (result == 0)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.UpdateError;
                return;
            }
            Flag = true;
        }

        /// <summary>
        /// 覆盖打印方案标签
        /// </summary>
        public void UpdatePrintSchemeLabelLists(List<PrintSchemeLabelDto> printSchemeLabelDtoList)
        {
            if (printSchemeLabelDtoList == null || printSchemeLabelDtoList.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return;
            }
            _repositoryContext.Execute("DELETE FROM tb_printSchemeLabel", null);
            foreach (var label in printSchemeLabelDtoList)
            {
                CreatePrintSchemeLabel(label);
            }
            Flag = true;
        }

        /// <summary>
        /// 创建打印方案标签
        /// </summary>
        /// <param name="dto"></param>
        private bool CreatePrintSchemeLabel(PrintSchemeLabelDto dto)
        {
            var result = _repositoryContext.Execute("INSERT INTO tb_printSchemeLabel(PrintSchemeId,LabelId,LabelGroupCode,SchemeCode,LabelName)VALUES(@PrintSchemeId,@LabelId,@LabelGroupCode,@SchemeCode,@LabelName)", new
            {
                PrintSchemeId = dto.PrintSchemeId,
                LabelId = dto.LabelId,
                LabelGroupCode = dto.LabelGroupCode,
                SchemeCode = dto.SchemeCode,
                LabelName = dto.LabelName
            });
            return result == 1;
        }
        private bool CreatePrintSchemeLabelList(List<PrintSchemeLabelDto> list)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Append("Begin;");

            //string sql = @"INSERT INTO tb_printSchemeLabel(PrintSchemeId,LabelId,LabelGroupCode,SchemeCode,LabelName)VALUES(@PrintSchemeId,@LabelId,@LabelGroupCode,@SchemeCode,@LabelName)";

            if (list != null && list.Count > 0)
            {
                foreach (var printSchemeLabel in list)
                {
                    sbSql.Append(string.Format("INSERT INTO tb_printSchemeLabel(PrintSchemeId,LabelId,LabelGroupCode,SchemeCode,LabelName)VALUES({0},'{1}',{2},'{3}','{4}');", printSchemeLabel.PrintSchemeId, printSchemeLabel.LabelId, printSchemeLabel.LabelGroupCode, printSchemeLabel.SchemeCode, printSchemeLabel.LabelName));
                }
            }
            sbSql.Append("Commit;");

            var result = _repositoryContext.Execute(sbSql.ToString(), null);
            return result >= 1;
        }

        public List<PrintSchemeInfoDto> GetPrintSchemeInfoListByTemplateCode(string templateCode)
        {
            if (string.IsNullOrWhiteSpace(templateCode))
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return new List<PrintSchemeInfoDto>();
            }
            var voucher = _repositoryContext.QueryFirstOrDefault<Voucher>("select Id,VoucherName,TemplateCode,GroupCode,Enble,Overall,localVoucher,globalVoucher,Sort,Path from tb_voucher Where TemplateCode=@TemplateCode", new { TemplateCode = templateCode });
            if (voucher==null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ResultNull;
                Message = "无该模板信息";
                return new List<PrintSchemeInfoDto>();
            }
            try
            {
                var printSchemeInfoDtos = new List<PrintSchemeInfoDto>();

                List<PrintScheme> printSchemeList;
                printSchemeList = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where VoucherId=@VoucherId AND LocalMachine!=0", new { VoucherId = voucher.Id });
                if (printSchemeList == null || printSchemeList.Count < 1)
                {
                    Flag = true;
                    Code = PrintErrorCode.Code.ResultNull;
                    return new List<PrintSchemeInfoDto>();
                }
                List<int> ids = new List<int>();
                List<string> printIds = new List<string>();
                foreach (var printScheme in printSchemeList)
                {
                    ids.Add(printScheme.Id);
                    printIds.Add(printScheme.PrintId);
                }
                var printconfigs = _repositoryContext.GetSet<PrintConfig>("select id,printId,pcid,state,printName,alias,connStyle,paperType,terminalName,paperWidth,topMargin,leftMargin, modifyTime, isDefault,enable from tb_printconfig t WHERE t.PrintId IN @ids", new { ids = printIds });
                List<PrintSchemeLabelDto> printSchemeLabelDtos = GetPrintSchemeLabelDtoList(ids);
                foreach (var printScheme in printSchemeList)
                {
                    var printSchemeInfoDto = new PrintSchemeInfoDto
                    {
                        Id = printScheme.Id,
                        Name = printScheme.Name,
                        VoucherId = printScheme.VoucherId,
                        LocalMachine = printScheme.LocalMachine,
                        PcId = printScheme.PcId,
                        TagList = new List<PrintSchemeLabelDto>()
                    };
                    if (printconfigs != null && printconfigs.Count > 0)
                    {
                        var config = printconfigs.FirstOrDefault(x => x.PrintId == printScheme.PrintId);
                        if (config != null)
                        {
                            if (!string.IsNullOrEmpty(config.Alias))
                            {
                                printSchemeInfoDto.Name = config.Alias;
                            }
                            printSchemeInfoDto.PrintConfig=new PrintConfigDto()
                            {
                                PrintId = config.PrintId,
                                Pcid=config.Pcid,
                                TerminalName = config.TerminalName,
                                PrintName = config.PrintName,
                                ConnStyle=config.ConnStyle,
                                Alias=config.Alias,
                                PaperType=config.PaperType,
                                PaperWidth=config.PaperWidth,
                                TopMargin = config.TopMargin,
                                LeftMargin = config.LeftMargin,
                                ModifyTime=config.ModifyTime,
                                IsDefault = config.IsDefault,
                                Updated=config.Updated,
                                Enable = config.Enable,
                                PrintNum = printScheme.PrintNum,
                                State=config.State
                            };
                        }
                    }
                    Guid schemeCode = Guid.Empty;
                    Guid.TryParse(printScheme.SchemeCode, out schemeCode);
                    printSchemeInfoDto.SchemeCode = schemeCode.ToString();
                    if (printSchemeInfoDto.LocalMachine != 0)
                    {
                        var tagList = printSchemeLabelDtos.Where(x => x.PrintSchemeId == printSchemeInfoDto.Id);
                        printSchemeInfoDto.TagList.AddRange(tagList);
                    }
                    printSchemeInfoDtos.Add(printSchemeInfoDto);
                }
                Flag = true;
                return printSchemeInfoDtos;
            }
            catch (Exception ex)
            {
                Flag = false;
                Code = PrintErrorCode.Code.UpdateError;
                PrintLogUtility.Writer.SendFullError(ex);
                return new List<PrintSchemeInfoDto>();
            }
        }

        private PrintSchemeLabel GetPrintSchemeLabel(PrintSchemeLabelDto printSchemeLabelDto)
        {
            if (printSchemeLabelDto == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return null;
            }
            var printSchemeLabel = new PrintSchemeLabel
            {
                Id = printSchemeLabelDto.Id,
                PrintSchemeId = printSchemeLabelDto.PrintSchemeId,
                LabelId = printSchemeLabelDto.LabelId,
                LabelName = printSchemeLabelDto.LabelName,
                LabelGroupCode = printSchemeLabelDto.LabelGroupCode
            };

            Flag = true;
            return printSchemeLabel;
        }

        /// <summary>
        /// 获取标签列表
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public List<PrintLabelDto> GetTagList(int[] ids)
        {
            var printLabelDtos = new List<PrintLabelDto>();
            var printLabelList = _repositoryContext.GetSet<PrintLabel>("select Id,LabelGroupCode,GroupId,GroupName,LabelName,LabelId from tb_printLabel where LabelGroupCode in @Ids", new { Ids = ids });

            foreach (var printLabel in printLabelList)
            {
                var printLabelDto = new PrintLabelDto
                {
                    Id = printLabel.Id,
                    GroupId = printLabel.GroupId,
                    GroupName = printLabel.GroupName,
                    LabelId = printLabel.LabelId,
                    LabelName = printLabel.LabelName
                };
                printLabelDtos.Add(printLabelDto);
            }
            Flag = true;
            return printLabelDtos;
        }

        public List<PrintConfig> GetPrintListConfig(PrintData printData, Voucher voucher)
        {
            var printConfigs = new List<PrintConfig>();
            if (printData == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                Message = "参数不可为空";
                return printConfigs;
            }
            if (voucher == null)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                Message = "打印模板不可为空";
                return printConfigs;
            }
            var printSchemes = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printScheme Where VoucherId=@VoucherId", new { VoucherId = voucher.Id });
            if (printSchemes == null || printSchemes.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ResultNull;
                Message = "未找到相关打印方案";
                return printConfigs;
            }
            if (printData.PcId != null)
            {
                List<string> ids = new List<string>();
                foreach (var printScheme in printSchemes)
                {
                    ids.Add(printScheme.PrintId);
                }
                var configs = _repositoryContext.GetSet<PrintConfig>("select Id,PrintSchemeId,LabelId,LabelGroupCode from tb_printSchemeLabel t WHERE t.PrintSchemeId IN @ids And t.Pcid=@Pcid", new { ids = ids, Pcid = printData.PcId });

                Flag = true;
                return configs;
            }
            if (printData.TagList == null || printData.TagList.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.ParamsError;
                return printConfigs;
            }

            var printSchemeLabels = _repositoryContext.GetSet<PrintSchemeLabel>("select Id,PrintSchemeId,LabelId,LabelGroupCode from tb_printSchemeLabel t WHERE t.LabelId IN @lables", new { lables = printData.TagList });
            List<int> idList = new List<int>();
            foreach (var label in printSchemeLabels)
            {
                idList.Add(label.PrintSchemeId);
            }
            var printSchemelList = _repositoryContext.GetSet<PrintScheme>("select Id,Name,PrintId,LocalMachine,PcId,PrintNum,VoucherId,SchemeCode from tb_printSchemet t WHERE t.Id IN @ids", new { ids = idList });
            if (printSchemelList == null || printSchemelList.Count < 1)
            {
                Flag = false;
                this.Code = PrintErrorCode.Code.PrintSchemeNullError;
                Message = "未找到相关打印方案";
                return printConfigs;
            }
            List<string> configIds = new List<string>();
            foreach (var printScheme in printSchemelList)
            {
                configIds.Add(printScheme.PrintId);
            }
            var configList = _repositoryContext.GetSet<PrintConfig>("select Id,PrintSchemeId,LabelId,LabelGroupCode from tb_printSchemeLabel t WHERE t.PrintSchemeId IN @ids", new { ids = configIds });
            Flag = true;
            return configList;
        }
    }
}
