using System;
using System.Collections.Generic;

namespace Clamp.Printer.Config
{
    public class PrinterConfig
    {

        public PrinterConfig()
        {
            PrinterInit = "1b40";
            SetLeft = "1d4c";
            SetWidth = "1d57";
            DefaultRowSpacing = "1b32";
            SetRowSpacing = "1b33";
            CancelBoldFont = "1b4500";
            BoldFont = "1b4501";
            SetFontSize = "1d21";
            AlignRight = "1b6132";
            AlignCenter = "1b6131";
            AlignLeft = "1b6130";
            PrintAndMove = "1b64";
            Cut = "1d5600";
        }
        

        public string PrinterInit { get; set; }
        public string SetLeft { get; set; }
        public string SetWidth { get; set; }
        public string DefaultRowSpacing { get; set; }
        public string SetRowSpacing { get; set; }

        public string CancelBoldFont { get; set; }
        public string BoldFont { get; set; }

        public string SetFontSize { get; set; }

        public string AlignRight { get; set; }
        public string AlignCenter { get; set; }
        public string AlignLeft { get; set; }

        public string PrintAndMove { get; set; }

        public string Cut { get; set; }

        public List<byte> GetLeft(int left)
        {
            var nl = left % 256;
            var nh = left / 256;
            var bytes = new List<byte>();
            bytes.AddRange(strToToHexByte(SetLeft));
            bytes.AddRange(new byte[] { Convert.ToByte(nl), Convert.ToByte(nh) });
            return bytes;
        }

        public List<byte> GetWidth(int width)
        {
            var nl = width % 256;
            var nh = width / 256;
            var bytes = new List<byte>();
            bytes.AddRange(strToToHexByte(SetWidth));
            bytes.AddRange(new byte[] { Convert.ToByte(nl), Convert.ToByte(nh) });
            return bytes;
        }

        /// <summary>
        ///
        /// </summary>
        public byte GetFontSize(int orientation, int direction)
        {
            int n = (orientation - 1) * 16 + direction - 1;
            return Convert.ToByte(n);
        }

        public byte GetRowSpacing(int n)
        {
            return Convert.ToByte(n * 137 / 6);
        }

        public byte GetMove(int n)
        {
            return Convert.ToByte(n);
        }

        private byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
            return returnBytes;
        }
    }
}