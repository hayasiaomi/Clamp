using System.IO.Ports;
using Clamp.Print;

namespace Clamp.Printer.Printer
{
    public class ComPrinter : EscPrinter
    {
        public override void Print(string ip, string port, byte[] data)
        {
            Open(port);
            Write(data);
            Dispose();
        }

        private SerialPort serialPort;
        
        public void Open(string portName, int aBoundRate = 9600)
        {
            try
            {
                this.serialPort = new SerialPort(portName);
                this.serialPort.BaudRate = aBoundRate;
                this.serialPort.DataBits = 8;
                this.serialPort.StopBits = StopBits.One;
                this.serialPort.Parity = Parity.None;
                this.serialPort.Open();
            }
            catch
            {
            }
        }

        public bool Write(byte[] bdata)
        {
            bool result;
            try
            {
                if (this.IsOpen())
                {
                    this.serialPort.Write(bdata, 0, bdata.Length);
                    result = true;
                }
                else
                {
                    result = false;
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public bool IsOpen()
        {
            bool Result = false;
            if (this.serialPort != null && this.serialPort.IsOpen)
            {
                Result = true;
            }
            return Result;
        }

        public bool Dispose()
        {
            bool Result = true;
            if (this.serialPort != null)
            {
                if (this.serialPort.IsOpen)
                {
                    try
                    {
                        this.serialPort.Close();
                        this.serialPort.Dispose();
                    }
                    catch
                    {
                        Result = false;
                    }
                }
            }
            return Result;
        }
    }
}