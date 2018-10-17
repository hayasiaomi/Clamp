using Hydra.Framework.MqttUtility;
using Hydra.Framework.Services.Aop;
using Hydra.Framework.Services.Aop.RegistrationAttributes;
using ShanDian.AddIns.PrintCommon.Tool;
using ShanDian.AddIns.Print.Dto.StandardVerSite;
using ShanDian.AddIns.Print.Dto.View;
using ShanDian.AddIns.Print.Interface;
using ShanDian.AddIns.Print.Interface.StandardVerSite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Hydra.Framework.SqlContent;
using ShanDian.AddIns.Print.Dto;
using ShanDian.AddIns.Print.Dto.WebPrint.Dto;
using ShanDian.AddIns.Print.Model;
using ShanDian.AddIns.Print.BUSHelper;
using ShanDian.AddIns.Print.Data;
using PrintInfo = ShanDian.AddIns.Print.Dto.StandardVerSite.PrintInfo;
using ShanDian.SDK.Framework.DB;

namespace ShanDian.AddIns.Print.Module
{
    [As(typeof(IStandardVerSiteService))]
    public class StandardVerSiteService : CommunicationServer, IStandardVerSiteService
    {
        private IRepositoryContext _repositoryContext;

        public StandardVerSiteService()
        {
            _repositoryContext = Global.RepositoryContext();
        }

        /// <summary>
        /// 第三方打印
        /// </summary>
        /// <param name="thirdPartPrint"></param>
        public BaseResult ThirdPartPrint(ThirdPartPrint.Dto thirdPartPrintRq)
        {
            PrintLogUtility.Writer.SendDebug(string.Format("第三方打印：{0}", JsonConvert.SerializeObject(thirdPartPrintRq)));
            BaseResult result = new BaseResult();

            List<PrintInfo> thirdPartPrintList = null;
            try
            {
                thirdPartPrintList = JsonConvert.DeserializeObject<List<PrintInfo>>(thirdPartPrintRq.PrintInfos);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendDebug(string.Format("第三方打印1：{0},{1}", JsonConvert.SerializeObject(thirdPartPrintList), ex.ToString()));
            }

            if (thirdPartPrintList != null && thirdPartPrintList.Count > 0)
            {
                string msg = "";
                foreach (var printInfo in thirdPartPrintList)
                {
                    if (string.IsNullOrWhiteSpace(printInfo.TemplateData)
                       || printInfo.TemplateData == "[]"
                       || string.IsNullOrWhiteSpace(printInfo.TemplateName)
                       || printInfo.PrintConfigs == null && printInfo.PrintConfigs.Count <= 0)
                    {
                        msg += "数据验证不通过|";
                        continue;
                    }

                    string serverExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    //string sdMainDirectory = Path.GetFullPath("..");

                    string fileDirectory = Path.Combine(serverExeDirectory, "PrintTemplate", "Thirdparty");

                    //模板文件地址
                    string fileAddress = Path.Combine(fileDirectory, string.Format("{0}{1}", printInfo.TemplateName, ".html"));
                    if (!File.Exists(fileAddress))
                    {
                        msg += "文件不存在:" + fileAddress + "|";
                        continue;
                    }

                    //模板数据
                    Dictionary<string, object> TemplateData = JsonConvert.DeserializeObject<Dictionary<string, object>>(printInfo.TemplateData);
                    Dictionary<string, object> templatePrintInfo = new Dictionary<string, object>();
                    foreach (var item in TemplateData)
                    {
                        if (item.Value != null && item.Value.GetType().Name == "JArray")
                        {
                            JArray jArray = (JArray)item.Value;
                            var jsonOutString = TemplateHelper.ToArray(jArray);

                            templatePrintInfo.Add(item.Key, jsonOutString);
                        }
                        else
                        {
                            if (item.Key.EndsWith("Img"))
                            {
                                //整理打印图片的路径
                                var imgFilePath = Path.Combine(fileDirectory, item.Value.ToString());
                                if (System.IO.File.Exists(imgFilePath))
                                {
                                    templatePrintInfo.Add(item.Key, Path.Combine(fileDirectory, item.Value.ToString()));
                                }
                                else
                                {
                                    templatePrintInfo.Add(item.Key, "");
                                }
                            }
                            else
                            {
                                templatePrintInfo.Add(item.Key, item.Value);
                            }
                        }
                    }

                    foreach (var printConfig in printInfo.PrintConfigs)
                    {
                        //打印
                        var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
                        htmlPrintingServices.Print(fileAddress, printConfig.PrintName, templatePrintInfo, Convert.ToInt32(printConfig.LeftMargin), Convert.ToInt32(printConfig.TopMargin), Convert.ToInt32(printConfig.PaperWidth), printConfig.PrintNum, printConfig.Pcid);

                        //PrintLogUtility.Writer.SendDebug(string.Format("第三方打印：发送至打印的信息：地址：{0}，打印机名：{1}，张数：{2}，Pcid：{3}", fileAddress, printConfig.PrintName, printConfig.PrintNum, printConfig.Pcid));
                    }
                    msg += "已成功打印|";
                }
                result.ResultType = "1";
                result.Message = msg;
            }
            else
            {
                result.Message = "信息为空！";
                result.ErrorCode = "2020";
            }
            return result;
        }

        /// <summary>
        /// 第三方打印
        /// </summary>
        /// <param name="thirdPartPrint"></param>
        public BaseResult ThirdPartPrintV2(ThirdPartPrint.DtoV2 thirdPartPrintRq)
        {
            PrintLogUtility.Writer.SendDebug(string.Format("第三方打印：{0}", JsonConvert.SerializeObject(thirdPartPrintRq)));
            BaseResult result = new BaseResult();

            List<PrintInfoV2> thirdPartPrintList = null;
            try
            {
                thirdPartPrintList = JsonConvert.DeserializeObject<List<PrintInfoV2>>(thirdPartPrintRq.PrintInfos);
            }
            catch (Exception ex)
            {
                PrintLogUtility.Writer.SendDebug(string.Format("第三方打印1：{0},{1}", JsonConvert.SerializeObject(thirdPartPrintList), ex.ToString()));
            }

            if (thirdPartPrintList != null && thirdPartPrintList.Count > 0)
            {
                string msg = "";
                foreach (var printInfo in thirdPartPrintList)
                {
                    if (string.IsNullOrWhiteSpace(printInfo.TemplateData)
                       || printInfo.TemplateData == "[]"
                       || string.IsNullOrWhiteSpace(printInfo.TemplateName)
                       || printInfo.PrintConfigs == null && printInfo.PrintConfigs.Count <= 0)
                    {
                        msg += "数据验证不通过|";
                        continue;
                    }

                    string serverExeDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    //string sdMainDirectory = Path.GetFullPath("..");

                    string fileDirectory = Path.Combine(serverExeDirectory, "PrintTemplate", "Thirdparty");

                    //模板文件地址
                    string fileAddress = Path.Combine(fileDirectory, string.Format("{0}{1}", printInfo.TemplateName, ".html"));
                    if (!File.Exists(fileAddress))
                    {
                        msg += "文件不存在:" + fileAddress + "|";
                        continue;
                    }

                    //模板数据
                    Dictionary<string, object> TemplateData = JsonConvert.DeserializeObject<Dictionary<string, object>>(printInfo.TemplateData);
                    Dictionary<string, object> templatePrintInfo = new Dictionary<string, object>();
                    foreach (var item in TemplateData)
                    {
                        if (item.Value != null && item.Value.GetType().Name == "JArray")
                        {
                            JArray jArray = (JArray)item.Value;
                            var jsonOutString = TemplateHelper.ToArray(jArray);

                            templatePrintInfo.Add(item.Key, jsonOutString);
                        }
                        else
                        {
                            if (item.Key.EndsWith("Img"))
                            {
                                //整理打印图片的路径
                                var imgFilePath = Path.Combine(fileDirectory, item.Value.ToString());
                                if (System.IO.File.Exists(imgFilePath))
                                {
                                    templatePrintInfo.Add(item.Key, Path.Combine(fileDirectory, item.Value.ToString()));
                                }
                                else
                                {
                                    templatePrintInfo.Add(item.Key, "");
                                }
                            }
                            else
                            {
                                templatePrintInfo.Add(item.Key, item.Value);
                            }
                        }
                    }

                    foreach (var printConfig in printInfo.PrintConfigs)
                    {
                        //打印
                        var htmlPrintingServices = ServiceLocator.Instance.Resolve<IHtmlPrintingServices>();
                        htmlPrintingServices.Print(fileAddress, printConfig.PrintName, templatePrintInfo, Convert.ToInt32(printConfig.LeftMargin), Convert.ToInt32(printConfig.TopMargin), Convert.ToInt32(printConfig.PaperWidth), printConfig.ThirdPrintNum, printConfig.Pcid);

                        PrintLogUtility.Writer.SendDebug(string.Format("第三方打印：发送至打印的信息：地址：{0}，打印机名：{1}，张数：{2}，Pcid：{3}", fileAddress, printConfig.PrintName, printConfig.ThirdPrintNum, printConfig.Pcid));
                    }
                    msg += "已成功打印|";
                }
                result.ResultType = "1";
                result.Message = msg;
            }
            else
            {
                result.Message = "信息为空！";
                result.ErrorCode = "2020";
            }
            return result;
        }

        public PrintSchemeInfoV2 ThirdPartPrintConfigList(PrintGetInfoDtoV2 PrintGetInfo)
        {
            var printCode = PrintGetInfo.PrintCode;
            if (string.IsNullOrWhiteSpace(printCode))
            {
                Flag = false;
                Message = "传入的参数为空";
                this.Code = PrintErrorCode.Code.ParamsError;
                return new PrintSchemeInfoV2();
            }

            PrintSchemeInfoV2 result = new PrintSchemeInfoV2()
            {
                PrintSchemes = new List<ErpPrintSchemeInfoDtoV2>(),
            };

            var printGroup = _repositoryContext.FirstOrDefault<PrintGroup>("select id,name,printCode,groupState,createDate,sort from tb_printGroup where printCode = @printCode order by sort asc ;", new { printCode = printCode });
            if (printGroup == null)
            {
                result.ResultType = "0";
                result.Message = "获取到的打印方案组为空";
                result.ErrorCode = PrintErrorCode.Code.PrintGroupNullError.ToString();
                return new PrintSchemeInfoV2();
            }

            var printGroupScheme = _repositoryContext.GetSet<PrintGroupScheme>("select id,pcid,name,printId,isDefault,state,groupId,createTime,modifyTime from tb_printGroupScheme where groupId = @groupId ", new { groupId = printGroup.Id });
            if (printGroupScheme != null && printGroupScheme.Count > 0)//没有门店打印方案
            {
                foreach (var scheme in printGroupScheme)
                {
                    var printConfig = _repositoryContext.FirstOrDefault<PrintConfig>("select id,printId,pcid,terminalName,printName,alias,connStyle,connAddress,connBrand,connPort,paperType,paperWidth,topMargin,leftMargin,modifyTime,isDefault,updated,enable,state from tb_printconfig where printId=@printId", new { printId = scheme.PrintId });
                    if (printConfig != null)
                    {
                        ErpPrintSchemeInfoDtoV2 erpPrint = new ErpPrintSchemeInfoDtoV2();
                        erpPrint.DishTypeCassify = new List<SchemeDishTypeDto>();
                        erpPrint.TableClassify = new List<SchemeTableDto>();
                        erpPrint.VoucherList = new List<SchemeVoucherDto>();
                        erpPrint.SetInfoList = new List<PrintSetInfoDto>();

                        #region 打印机信息
                        erpPrint.PrintConfig = new ErpPrintConfigDtoV2()
                        {
                            PrintId = printConfig.PrintId,
                            Pcid = printConfig.Pcid,
                            TerminalName = printConfig.TerminalName,
                            PrintName = printConfig.PrintName,
                            Alias = printConfig.Alias,

                            ConnStyle = printConfig.ConnStyle,
                            ConnAddress = printConfig.ConnAddress,
                            ConnBrand = printConfig.ConnBrand,
                            ConnPort = printConfig.ConnPort,
                            PaperType = printConfig.PaperType,

                            PaperWidth = printConfig.PaperWidth,
                            TopMargin = printConfig.TopMargin,
                            LeftMargin = printConfig.LeftMargin,
                            ModifyTime = printConfig.ModifyTime,
                            IsDefault = printConfig.IsDefault,

                            Updated = printConfig.Updated,
                            Enable = printConfig.Enable,
                            State = printConfig.State,
                        };
                        #endregion

                        erpPrint.VoucherList.AddRange(PrintConfigHelper.Instance.GetSchemeVoucherList(scheme));
                        erpPrint.TableClassify.AddRange(PrintConfigHelper.Instance.GetSchemeTableList(scheme));
                        result.PrintSchemes.Add(erpPrint);
                    }
                }
                result.Message = "获取打印机信息成功";
                result.ErrorCode = "";
                result.ResultType = "1";
                return result;
            }
            else
            {
                result.ResultType = "0";
                result.Message = "当前打印方案组没有任何打印方案记录";
                result.ErrorCode = PrintErrorCode.Code.PrintSchemeNullError.ToString();
                return new PrintSchemeInfoV2();
            }
        }


    }
}
