using Clamp.MUI.Framework.Properties;
using System;
using System.IO;
using System.Text;

namespace Clamp.MUI.Framework.INI
{
    /// <summary>
    /// Ini文件的读取类
    /// </summary>
    internal static class INIReader
    {

        /// <summary>
        /// 读取文本
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        internal static INIFile ReadFromString(string source)
        {
            var config = new INIFile();

            using (var reader = new StringReader(source))
            {
                Parse(reader, config);
            }

            return config;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reader"></param>
        /// <param name="config"></param>
        private static void Parse(StringReader reader, INIFile config)
        {
            var preCommentBuilder = new StringBuilder();

            int newlineLength = Environment.NewLine.Length;

            string line = null;
            int lineNumber = 0;

            while ((line = reader.ReadLine()) != null)
            {
                lineNumber++;

                line = line.Trim();

                if (string.IsNullOrEmpty(line))
                    continue;

                int commentIndex = 0;
                var comment = ParseComment(line, out commentIndex);

                if (!INIFile.IgnorePreComments && commentIndex == 0)
                {
                    preCommentBuilder.AppendLine(comment);
                    continue;
                }
                else if (!INIFile.IgnoreInlineComments && commentIndex > 0)
                {
                    line = line.Remove(commentIndex).Trim();
                }

                var sectionItem = ParseSectionItem(line, lineNumber);

                if (!INIFile.IgnoreInlineComments)
                    sectionItem.Comment = comment;

                if (!INIFile.IgnorePreComments && preCommentBuilder.Length > 0)
                {
                    preCommentBuilder.Remove(preCommentBuilder.Length - newlineLength, newlineLength);
                    sectionItem.PreComment = preCommentBuilder.ToString();
                    preCommentBuilder.Length = 0;
                }

                config.mSections.Add(sectionItem);
            }
        }

        /// <summary>
        /// 判断是否存在“”里面
        /// </summary>
        /// <param name="line"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        private static bool IsInQuoteMarks(string line, int startIndex)
        {
            int i = startIndex;
            bool left = false;

            while (--i >= 0)
            {
                if (line[i] == '\"')
                {
                    left = true;
                    break;
                }
            }

            bool right = (line.IndexOf('\"', startIndex) > 0);

            return (left && right);
        }

        /// <summary>
        /// 转化为注解的文本
        /// </summary>
        /// <param name="line"></param>
        /// <param name="commentIndex"></param>
        /// <returns></returns>
        private static string ParseComment(string line, out int commentIndex)
        {
            string comment = null;
            commentIndex = -1;

            do
            {
                commentIndex = line.IndexOfAny(INIFile.ValidCommentChars, commentIndex + 1);

                if (commentIndex < 0)
                    break;

                if (commentIndex > 0 && line[commentIndex - 1] == '\\')
                    return null;

                if (IsInQuoteMarks(line, commentIndex))
                    continue;

                comment = line.Substring(commentIndex + 1).Trim();

                break;
            }
            while (commentIndex >= 0);

            return comment;
        }

        /// <summary>
        /// 转化节项
        /// </summary>
        /// <param name="line"></param>
        /// <param name="lineNumber"></param>
        /// <returns></returns>
        private static Section ParseSectionItem(string line, int lineNumber)
        {
            string sectionItemName = null;

            int equalSignIndex = -1;

            bool isQuotedName = line.StartsWith("\"");

            if (isQuotedName)
            {
                int index = 0;
                do
                {
                    index = line.IndexOf('\"', index + 1);
                }
                while (index > 0 && line[index - 1] == '\\');

                if (index < 0)
                {
                    throw new ParserException(StringResources.INI_ClosingQuoteMissing, lineNumber);
                }

                sectionItemName = line.Substring(1, index - 1);

                equalSignIndex = line.IndexOf('=', index + 1);
            }
            else
            {
                equalSignIndex = line.IndexOf('=');
            }

            if (equalSignIndex < 0)
                throw new ParserException(StringResources.INI_EquatMissing, lineNumber);

            if (!isQuotedName)
            {
                sectionItemName = line.Substring(0, equalSignIndex).Trim();
            }

            string sectionItemValue = line.Substring(equalSignIndex + 1);

            sectionItemValue = sectionItemValue.Trim();

            if (string.IsNullOrEmpty(sectionItemName))
                throw new ParserException(StringResources.INI_SectionItemNameMissing, lineNumber);

            return new Section(sectionItemName, sectionItemValue);
        }

        internal static INIFile ReadFromBinaryStream(Stream stream, BinaryReader reader)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            if (reader == null)
                reader = new BinaryReader(stream);

            var config = new INIFile();

            int sectionCount = reader.ReadInt32();

            for (int i = 0; i < sectionCount; ++i)
            {
                string sectionName = reader.ReadString();
                int settingCount = reader.ReadInt32();

                //var section = new Section(sectionName);

                //ReadCommentsBinary(reader, section);

                //for (int j = 0; j < settingCount; j++)
                //{
                //    var setting = new SectionItem(reader.ReadString(), reader.ReadString());

                //    ReadCommentsBinary(reader, setting);

                //    section.Add(setting);
                //}

                //config.Add(section);
            }

            return config;
        }

        private static void ReadCommentsBinary(BinaryReader reader, INIElement element)
        {
            bool hasComment = reader.ReadBoolean();
            if (hasComment)
            {
                reader.ReadChar();
                element.Comment = reader.ReadString();
            }

            bool hasPreComment = reader.ReadBoolean();
            if (hasPreComment)
            {
                reader.ReadChar();
                element.PreComment = reader.ReadString();
            }
        }
    }
}
