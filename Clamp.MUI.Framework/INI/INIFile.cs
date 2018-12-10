
using Clamp.MUI.Framework.INI.TypeConverter;
using Clamp.MUI.Framework.Properties;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Clamp.MUI.Framework.INI
{
    /// <summary>
    /// 字典类，即是ini文件
    /// </summary>
    public partial class INIFile : IEnumerable<Section>
    {
        private static CultureInfo _cultureInfo;
        private static char _preferredCommentChar;
        private static char _arrayElementSeparator;
        private static ITypeStringConverter _fallbackConverter;
        private static Dictionary<Type, ITypeStringConverter> _typeStringConverters;

        /// <summary>
        /// 节的集合。
        /// </summary>
        internal readonly List<Section> mSections;

        static INIFile()
        {
            _cultureInfo = (CultureInfo)CultureInfo.InvariantCulture.Clone();

            ValidCommentChars = new[] { '#' };
            _preferredCommentChar = '#';
            _arrayElementSeparator = ',';

            _fallbackConverter = new FallbackStringConverter();

            _typeStringConverters = new Dictionary<Type, ITypeStringConverter>()
                                      {
                                        { typeof(bool), new BoolStringConverter() },
                                        { typeof(byte), new ByteStringConverter() },
                                        { typeof(char), new CharStringConverter() },
                                        { typeof(DateTime), new DateTimeStringConverter() },
                                        { typeof(decimal), new DecimalStringConverter() },
                                        { typeof(double), new DoubleStringConverter() },
                                        { typeof(Enum), new EnumStringConverter() },
                                        { typeof(short), new Int16StringConverter() },
                                        { typeof(int), new Int32StringConverter() },
                                        { typeof(long), new Int64StringConverter() },
                                        { typeof(sbyte), new SByteStringConverter() },
                                        { typeof(float), new SingleStringConverter() },
                                        { typeof(string), new StringStringConverter() },
                                        { typeof(ushort), new UInt16StringConverter() },
                                        { typeof(uint), new UInt32StringConverter() },
                                        { typeof(ulong), new UInt64StringConverter() }
                                      };

            IgnoreInlineComments = false;
            IgnorePreComments = false;
        }

        public INIFile()
        {
            mSections = new List<Section>();
        }

        /// <summary>
        /// 实现于IEnumerable的接口,
        /// </summary>
        /// <returns></returns>
        public IEnumerator<Section> GetEnumerator()
        {
            return mSections.GetEnumerator();
        }

        /// <summary>
        /// 实现于IEnumerable的接口
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// 增加一个节到ini文件里面去
        /// </summary>
        /// <param name="section"></param>
        public void Add(Section section)
        {
            if (section == null)
                throw new ArgumentNullException("section");

            if (Contains(section))
                throw new ArgumentException("The specified section already exists in the configuration.");

            mSections.Add(section);
        }

        /// <summary>
        /// 增加一个节到ini文件里面去
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public Section Add(string sectionName)
        {
            var section = new Section(sectionName);
            Add(section);
            return section;
        }

        /// <summary>
        /// 移除一个节
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public bool Remove(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            return Remove(FindSection(sectionName));
        }

        /// <summary>
        /// 移除一个节
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool Remove(Section section)
        {
            return mSections.Remove(section);
        }

        /// <summary>
        /// 移除所有指定节名的节
        /// </summary>
        /// <param name="sectionName"></param>
        public void RemoveAllNamed(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            while (Remove(sectionName)) ;
        }

        /// <summary>
        /// 清楚所有的节
        /// </summary>
        public void Clear()
        {
            mSections.Clear();
        }

        /// <summary>
        /// 是否包含一个指定的节
        /// </summary>
        /// <param name="section"></param>
        /// <returns></returns>
        public bool Contains(Section section)
        {
            return mSections.Contains(section);
        }

        /// <summary>
        /// 是否包含一个指定的节
        /// </summary>
        /// <param name="sectionName"></param>
        /// <returns></returns>
        public bool Contains(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            return FindSection(sectionName) != null;
        }
        /// <summary>
        /// 是否包含指定节名和字典名
        /// </summary>
        /// <param name="sectionName"></param>
        /// <param name="sectionItemName"></param>
        /// <returns></returns>
        public bool Contains(string sectionName, string sectionItemName)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            if (string.IsNullOrEmpty(sectionItemName))
                throw new ArgumentNullException("settingName");

            Section section = FindSection(sectionName);
            return section != null;
        }

        /// <summary>
        /// 注册类型转化器
        /// </summary>
        /// <param name="converter"></param>
        public static void RegisterTypeStringConverter(ITypeStringConverter converter)
        {
            if (converter == null)
                throw new ArgumentNullException("converter");

            var type = converter.ConvertibleType;

            if (_typeStringConverters.ContainsKey(type))
                throw new InvalidOperationException(string.Format(StringResources.INI_NotFoundFileName, type.FullName));
            else
                _typeStringConverters.Add(type, converter);
        }

        /// <summary>
        /// 注消类型转化器
        /// </summary>
        /// <param name="type"></param>
        public static void DeregisterTypeStringConverter(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            if (!_typeStringConverters.ContainsKey(type))
                throw new InvalidOperationException(string.Format(StringResources.INI_TypeStringConverte_NoRegiester, type.FullName));
            else
                _typeStringConverters.Remove(type);
        }

        /// <summary>
        /// 查找指定的类型转化器
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static ITypeStringConverter FindTypeStringConverter(Type type)
        {
            if (type.IsEnum)
                type = typeof(Enum);

            ITypeStringConverter converter = null;
            if (!_typeStringConverters.TryGetValue(type, out converter))
                converter = _fallbackConverter;

            return converter;
        }

        internal static ITypeStringConverter FallbackConverter
        {
            get { return _fallbackConverter; }
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        public void SaveToFile(string filename)
        {
            SaveToFile(filename, null);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="encoding"></param>
        public void SaveToFile(string filename, Encoding encoding)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                SaveToStream(stream, encoding);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream"></param>
        public void SaveToStream(Stream stream)
        {
            SaveToStream(stream, null);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="encoding"></param>
        public void SaveToStream(Stream stream, Encoding encoding)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            INIWriter.WriteToStreamTextual(this, stream, encoding);
        }



        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        public void SaveToBinaryFile(string filename)
        {
            SaveToBinaryFile(filename, null);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="writer"></param>
        public void SaveToBinaryFile(string filename, BinaryWriter writer)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                SaveToBinaryStream(stream, writer);
        }
        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream"></param>
        public void SaveToBinaryStream(Stream stream)
        {
            SaveToBinaryStream(stream, null);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="writer"></param>
        public void SaveToBinaryStream(Stream stream, BinaryWriter writer)
        {
            if (stream == null)
                throw new ArgumentNullException("stream");

            INIWriter.WriteToStreamBinary(this, stream, writer);
        }

        /// <summary>
        /// 获得节的数量
        /// </summary>
        public int SectionCount
        {
            get { return mSections.Count; }
        }

        /// <summary>
        /// 节的索引器
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Section this[int index]
        {
            get
            {
                if (index < 0 || index >= mSections.Count)
                    throw new ArgumentOutOfRangeException("index");

                return mSections[index];
            }
        }

        /// <summary>
        /// 节的索引器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Section this[string name]
        {
            get
            {
                var section = FindSection(name);

                if (section == null)
                {
                    section = new Section(name);
                    Add(section);
                }

                return section;
            }
        }

        /// <summary>
        /// 通过节名称获得所有的节
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IEnumerable<Section> GetSectionsNamed(string name)
        {
            var sections = new List<Section>();

            foreach (var section in mSections)
            {
                if (string.Equals(section.Name, name, StringComparison.OrdinalIgnoreCase))
                    sections.Add(section);
            }

            return sections;
        }

        /// <summary>
        /// 通过名称获得节
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private Section FindSection(string name)
        {
            foreach (var section in mSections)
            {
                if (string.Equals(section.Name, name, StringComparison.OrdinalIgnoreCase))
                    return section;
            }

            return null;
        }


    }
}
