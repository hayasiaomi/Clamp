using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace Clamp.OSG.Initial
{
    internal static class InitialWriter
    {
        private class NonClosingBinaryWriter : BinaryWriter
        {
            public NonClosingBinaryWriter(Stream stream)
                : base(stream)
            { }

            protected override void Dispose(bool disposing)
            { }
        }

        internal static void WriteToStreamTextual(InitialFile settingFile, Stream stream, Encoding encoding)
        {
            Debug.Assert(settingFile != null);

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (encoding == null)
                encoding = new UTF8Encoding();

            var sb = new StringBuilder();


            foreach (var sectionItem in settingFile)
                sb.AppendLine(sectionItem.ToString(true));



            string str = sb.ToString();

            var byteBuffer = new byte[encoding.GetByteCount(str)];
            int byteCount = encoding.GetBytes(str, 0, str.Length, byteBuffer, 0);

            stream.Write(byteBuffer, 0, byteCount);
            stream.Flush();
        }

        internal static void WriteToStreamBinary(InitialFile cfg, Stream stream, BinaryWriter writer)
        {
            Debug.Assert(cfg != null);

            if (stream == null)
                throw new ArgumentNullException("stream");

            if (writer == null)
                writer = new NonClosingBinaryWriter(stream);

            writer.Write(cfg.SectionCount);

            foreach (var setting in cfg)
            {
                writer.Write(setting.Name);
                writer.Write(setting.StringValue);

                WriteCommentsBinary(writer, setting);
            }

            writer.Close();
        }

        private static void WriteCommentsBinary(BinaryWriter writer, InitialElement element)
        {
            writer.Write(element.Comment != null);
            if (element.Comment != null)
            {
                writer.Write(' ');
                writer.Write(element.Comment);
            }

            writer.Write(element.PreComment != null);
            if (element.PreComment != null)
            {
                writer.Write(' ');
                writer.Write(element.PreComment);
            }
        }

    }
}