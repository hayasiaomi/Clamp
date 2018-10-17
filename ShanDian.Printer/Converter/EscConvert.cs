using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using QRCoder;
using ShanDian.Printer.Config;

namespace ShanDian.Printer.Converter
{
    public class EscConvert
    {
        private static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = System.Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }

        public static List<byte> ToEscByte(List<string> contentList,PrinterConfig config,int rowSize,int left = 0, int width = 588)
        {
            var sendData = new List<byte>();

            //初始化打印机
            sendData.AddRange(strToToHexByte(config.PrinterInit));

            //设置左边距和宽度
            if (left > 0)
            {
                sendData.AddRange(config.GetLeft(left));
                sendData.AddRange(config.GetWidth(width));
            }

            //设置默认行间距
            sendData.AddRange(strToToHexByte(config.DefaultRowSpacing));

            foreach (var content in contentList)
            {
                var str = content;
                var size = 1;
                sendData.AddRange(strToToHexByte(config.CancelBoldFont));

                if (str.Contains("## "))
                {
                    sendData.AddRange(strToToHexByte(config.SetFontSize));
                    sendData.Add(config.GetFontSize(2,2));
                    sendData.AddRange(strToToHexByte(config.SetRowSpacing));
                    sendData.Add(config.GetRowSpacing(5));
                    str = str.Replace("## ", "");
                    size = 2;
                }
                if (str.Contains("#2 "))
                {
                    sendData.AddRange(strToToHexByte(config.SetFontSize));
                    sendData.Add(config.GetFontSize(1, 2));
                    sendData.AddRange(strToToHexByte(config.SetRowSpacing));
                    sendData.Add(config.GetRowSpacing(5));
                    str = str.Replace("#2 ", "");
                    size = 1;
                }
                if (str.Contains("# "))
                {
                    sendData.AddRange(strToToHexByte(config.SetFontSize));
                    sendData.Add(config.GetFontSize(1, 1));
                    sendData.AddRange(strToToHexByte(config.SetRowSpacing));
                    sendData.Add(config.GetRowSpacing(3));
                    str = str.Replace("# ", "");
                    size = 1;
                }
                if (str.Contains("** "))
                {
                    sendData.AddRange(strToToHexByte(config.BoldFont));
                    str = str.Replace("** ", "");
                }

                if (str.Contains("^ "))
                {
                    sendData.AddRange(strToToHexByte(config.AlignCenter));
                    str = str.Replace("^ ", "");
                }
                if (str.Contains("> "))
                {
                    sendData.AddRange(strToToHexByte(config.AlignRight));
                    str = str.Replace("> ", "");
                }
                if (str.Contains("< "))
                {
                    sendData.AddRange(strToToHexByte(config.AlignLeft));
                    str = str.Replace("< ", "");
                }
                if ("--".Equals(str))
                {
                    var s = GetStr("-", rowSize);
                    sendData.AddRange(strToToHexByte(config.SetFontSize));
                    sendData.Add(config.GetFontSize(1, 1));
                    sendData.AddRange(strToToHexByte(config.SetRowSpacing));
                    sendData.Add(config.GetRowSpacing(0));
                    sendData.AddRange(Encoding.Default.GetBytes(s));
                    str = "";
                }
                if (str.StartsWith("img "))
                {
                    sendData.AddRange(strToToHexByte(config.SetRowSpacing));
                    sendData.Add(config.GetRowSpacing(0));
                    str = str.Replace("img ", "");
                    if (File.Exists(str.Trim()))
                    {
                        var bmp = new Bitmap(str.Trim());
                        var picData = PrintPic(bmp);
                        sendData.AddRange(picData);
                    }
                    str = "";
                }
                if (str.StartsWith("qr "))
                {
                    sendData.AddRange(strToToHexByte(config.SetRowSpacing));
                    sendData.Add(config.GetRowSpacing(0));
                    str = str.Replace("qr ", "");
                    var qrBitmap = GetQrCodeBitmap(str.Trim());
                    var picData = PrintPic(qrBitmap);
                    sendData.AddRange(picData);
                    str = "";
                }
                if (!string.IsNullOrEmpty(str))
                {
                    if (str.StartsWith("|")) //表格展示，|:应收金额|0.05:|
                    {
                        var s = "";
                        var list = str.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                        var tdn = list.Count; //=2
                        var perTdSize = Math.Ceiling((double)rowSize / tdn); //48/2=24
                        foreach (var tdStr in list)
                        {
                            var str2 = tdStr; //:应收金额
                            var isLeft = str2.StartsWith(":"); // true
                            var isRight = str2.EndsWith(":"); //false
                            if (isLeft)
                            {
                                str2 = str2.Substring(1); //应收金额
                            }
                            if (isRight && str2.Length > 0)
                            {
                                str2 = str2.Substring(0, str2.Length - 1);
                            }
                            if (str2.StartsWith("(")) //列占比计算
                            {
                                var index = str2.IndexOf(")", StringComparison.Ordinal);
                                var sper = System.Convert.ToInt32(str2.Substring(1, index - 1));
                                str2 = str2.Substring(index + 1);
                                perTdSize = Math.Ceiling((double)rowSize * sper / 12);
                            }

                            var strL = Encoding.Default.GetBytes(str2).Length; //8
                            var blankL = (int)Math.Ceiling((perTdSize - strL * size) / size); //(24 - 8)/1 = 16
                            if (isLeft && !isRight)
                            {
                                s += str2 + GetStr(" ", blankL);
                            }
                            else if (!isLeft && isRight)
                            {
                                s += GetStr(" ", blankL) + str2;
                            }
                            else
                            {
                                s += GetStr(" ", blankL / 2) + str2 + GetStr(" ", blankL / 2);
                            }
                        }
                        sendData.AddRange(Encoding.Default.GetBytes(s));
                    }
                    else
                    {
                        sendData.AddRange(Encoding.Default.GetBytes(str));
                    }
                }
                sendData.AddRange(Encoding.Default.GetBytes("\n"));

            }

            sendData.AddRange(strToToHexByte(config.PrintAndMove));
            sendData.Add(config.GetMove(5));
            sendData.AddRange(strToToHexByte(config.Cut));

            return sendData;
        }

        static string GetStr(string charStr, int n)
        {
            var s = "";
            for (var i = 0; i < n; i++)
            {
                s += charStr;
            }
            return s;
        }

        static List<byte> PrintPic(Bitmap bmp)
        {
            var width = bmp.Width;
            var height = bmp.Height;
            List<byte> picData = new List<byte>();

            byte[] data = new byte[] { 0x1B, 0x33, 0x00 };
            data[0] = (byte)'\x00';
            data[1] = (byte)'\x00';
            data[2] = (byte)'\x00';    // Clear to Zero.

            Color pixelColor;

            // ESC * m nL nH 点阵图
            byte[] escBmp = new byte[] { 0x1B, 0x2A, 0x00, 0x00, 0x00 };

            escBmp[2] = (byte)'\x21';

            //nL, nH
            escBmp[3] = (byte)(width % 256);
            escBmp[4] = (byte)(width / 256);

            // data
            for (int i = 0; i < (height / 24) + 1; i++)
            {
                picData.AddRange(escBmp);
                for (int j = 0; j < width; j++)
                {
                    for (int k = 0; k < 24; k++)
                    {
                        if (((i * 24) + k) < height)   // if within the BMP size
                        {
                            pixelColor = bmp.GetPixel(j, (i * 24) + k);
                            if (pixelColor.R < 150)
                            {
                                data[k / 8] += (byte)(128 >> (k % 8));
                            }
                        }
                    }
                    picData.AddRange(data);
                    data[0] = (byte)'\x00';
                    data[1] = (byte)'\x00';
                    data[2] = (byte)'\x00';    // Clear to Zero.
                }

                picData.AddRange(Encoding.UTF8.GetBytes("\n"));

            } // data

            picData.AddRange(Encoding.UTF8.GetBytes("\n"));

            return picData;
        }

        static Bitmap GetQrCodeBitmap(string strCode)
        {
            // 生成二维码的内容
            var qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(strCode, QRCodeGenerator.ECCLevel.Q);
            QRCode qrcode = new QRCode(qrCodeData);
            
            Bitmap qrCodeImage = qrcode.GetGraphic(5, Color.Black, Color.White, null, 15, 6, false);
            return qrCodeImage;
        }
    }
}