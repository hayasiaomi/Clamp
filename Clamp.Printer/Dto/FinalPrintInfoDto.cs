using Clamp.Printer.Config;

namespace Clamp.Printer.Dto
{
    public class FinalPrintInfoDto
    {

        /// <summary>
        /// 顶部间距
        /// </summary>
        public int Top { set; get; }
        /// <summary>
        /// 左边间距
        /// </summary>
        public int Left { set; get; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int Width { set; get; }
        /// <summary>
        /// 每行打印的标准字符数
        /// 用于指令打印
        /// </summary>
        public int RowSize { get; set; }
        /// <summary>
        /// 打印内容
        /// </summary>
        public string PrintingBody { set; get; }
        /// <summary>
        /// 打印机名
        /// 网口：打印机IP
        /// 并口、串口无效
        /// </summary>
        public string PrintingName { set; get; }
        

        /// <summary>
        /// 打印机品牌
        /// </summary>
        public string PrintBrand { get; set; }
        
        /// <summary>
        /// 打印机端口
        /// 网口：9100
        /// 并口：LPT1
        /// 串口：COM1
        /// </summary>
        public string PrintPort { get; set; }

        /// <summary>
        /// 打印机类型
        /// </summary>
        public PrintType PrintType { get; set; }

        /// <summary>
        /// 打印次数
        /// </summary>
        public int Copies { set; get; }
    }
}