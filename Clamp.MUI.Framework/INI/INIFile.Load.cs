using Clamp.MUI.Framework.Properties;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Clamp.MUI.Framework.INI
{
    public partial class INIFile : IEnumerable<Section>
    {

        /// <summary>
        /// 通过指定的文件名来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static INIFile LoadFromFile(string filename)
        {
            return LoadFromFile(filename, null);
        }

        /// <summary>
        /// 指定文件名和编码来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static INIFile LoadFromFile(string filename, Encoding encoding)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            if (!File.Exists(filename))
                throw new FileNotFoundException(StringResources.INI_NotFoundFileName, filename);

            return (encoding == null) ? LoadFromString(File.ReadAllText(filename)) : LoadFromString(File.ReadAllText(filename, encoding));
        }
        /// <summary>
        /// 指定流来加载
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static INIFile LoadFromStream(Stream stream)
        {
            return LoadFromStream(stream, null);
        }

        /// <summary>
        /// 指定流和编码来加载 
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        /// <returns></returns>
        public static INIFile LoadFromStream(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            string source = null;

            var reader = (encoding == null) ?
                new StreamReader(stream) :
                new StreamReader(stream, encoding);

            using (reader)
                source = reader.ReadToEnd();

            return LoadFromString(source);
        }

        /// <summary>
        /// 指定文本来加载
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static INIFile LoadFromString(string source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            return INIReader.ReadFromString(source);
        }



        /// <summary>
        /// 指定一个二进制文件名来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static INIFile LoadFromBinaryFile(string filename)
        {
            return LoadFromBinaryFile(filename, null);
        }

        /// <summary>
        /// 指定的二进制和二进制编码来加载
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static INIFile LoadFromBinaryFile(string filename, BinaryReader reader)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            using (var stream = File.OpenRead(filename))
                return LoadFromBinaryStream(stream, reader);
        }

        /// <summary>
        /// 指定的二进制流来加载
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static INIFile LoadFromBinaryStream(Stream stream)
        {
            return LoadFromBinaryStream(stream, null);
        }

        /// <summary>
        /// 指定的二进制流和二进制编码来加载
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="reader"></param>
        /// <returns></returns>
        public static INIFile LoadFromBinaryStream(Stream stream, BinaryReader reader)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            return INIReader.ReadFromBinaryStream(stream, reader);
        }

    }
}
