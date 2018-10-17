using System.Runtime.InteropServices;

namespace Clamp.Printer.Printer
{
    public class LPTPrinter : EscPrinter
    {

        public override void Print(string ip, string port, byte[] data)
        {
            Open(port, false);
            Write(data);
            Close();
        }

        private struct OVERLAPPED
        {
            private int Internal;

            private int InternalHigh;

            private int Offset;

            private int OffSetHigh;

            private int hEvent;
        }

        [DllImport("kernel32.dll")]
        private static extern int CreateFile(string lpFileName, uint dwDesiredAccess, int dwShareMode, int lpSecurityAttributes, int dwCreationDisposition, int dwFlagsAndAttributes, int hTemplateFile);

        [DllImport("kernel32.dll")]
        private static extern bool WriteFile(int hFile, byte[] lpBuffer, int nNumberOfBytesToWrite, out int lpNumberOfBytesWritten, out LPTPrinter.OVERLAPPED lpOverlapped);

        [DllImport("kernel32.dll")]
        private static extern bool CloseHandle(int hObject);

        private string _printerName;
        private int _handler;


        public void Open(string aPortName, bool isGetStatus)
        {
            _printerName = aPortName;
            _handler = CreateFile(_printerName, 1073741824u, 0, 0, 3, 0, 0);
            
        }

        public bool Write(byte[] bdata)
        {
            int i;
            LPTPrinter.OVERLAPPED x;
            return bdata.Length != 0 && _handler != -1 && WriteFile(_handler, bdata, bdata.Length, out i, out x);
        }

        public bool Close()
        {
            return CloseHandle(_handler);
        }
    }
}
