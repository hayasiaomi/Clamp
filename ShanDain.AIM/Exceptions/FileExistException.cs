using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.AIM.Exceptions
{
    public abstract class BasicFileException : Exception
    {
        public string FileName { get; private set; }

        public BasicFileException(string fileName)
        {
            this.FileName = fileName;
        }
    }

    public class FileExistException : BasicFileException
    {
        public FileExistException(string file) : base(file)
        {
        }
    }

    public class TempFileFormatterException : BasicFileException
    {
        public TempFileFormatterException(string file) : base(file)
        {
        }
    }

    public class CheckSumFormatException : Exception
    {

    }
}
