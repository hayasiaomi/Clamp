using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.OSG.Initial
{
    public partial class InitialFile : IEnumerable<InitialProperty>
    {

        /// <summary>
        /// 通过指定的文件名来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static InitialFile LoadFromFile(string filename)
        {
            return LoadFromFile(filename, Encoding.UTF8);
        }

        /// <summary>
        /// 指定文件名和编码来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static InitialFile LoadFromFile(string filename, Encoding encoding)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                File.Create(filename).Close();

            return LoadFromString(File.ReadAllText(filename, encoding));
        }
        /// <summary>
        /// 指定流来加载
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static InitialFile LoadFromStream(Stream stream)
        {
            return LoadFromStream(stream, Encoding.UTF8);
        }

        /// <summary>
        /// 指定流和编码来加载 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static InitialFile LoadFromStream(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string source = null;

            var reader = new StreamReader(stream, encoding);

            using (reader)
                source = reader.ReadToEnd();

            return LoadFromString(source);
        }

        /// <summary>
        /// 指定文本来加载
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static InitialFile LoadFromString(string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return InitialReader.ReadFromString(source);
        }



        /// <summary>
        /// 指定一个二进制文件名来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static InitialFile LoadFromBinaryFile(string filename)
        {
            return LoadFromBinaryFile(filename, null);
        }

        /// <summary>
        /// 指定的二进制和二进制编码来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static InitialFile LoadFromBinaryFile(string filename, BinaryReader reader)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                File.Create(filename).Close();

            using (var stream = File.OpenRead(filename))
                return LoadFromBinaryStream(stream, reader);
        }

        /// <summary>
        /// 指定的二进制流来加载
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static InitialFile LoadFromBinaryStream(Stream stream)
        {
            return LoadFromBinaryStream(stream, null);
        }

        /// <summary>
        /// 指定的二进制流和二进制编码来加载
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static InitialFile LoadFromBinaryStream(Stream stream, BinaryReader reader)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            return InitialReader.ReadFromBinaryStream(stream, reader);
        }

    }
}
