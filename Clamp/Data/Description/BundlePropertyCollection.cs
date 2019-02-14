using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Description
{
    /// <summary>
    /// Bundle的属性集合
    /// </summary>
    public interface BundlePropertyCollection : IEnumerable<BundleProperty>
    {
        /// <summary>
        /// 获得属性的值，通过名称
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        string GetPropertyValue(string name);
        /// <summary>
        /// 获得属性的值，通过名称和本地化
        /// </summary>
        /// <param name="name"></param>
        /// <param name="locale"></param>
        /// <returns></returns>
        string GetPropertyValue(string name, string locale);

        /// <summary>
        /// 设置属性的值 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        void SetPropertyValue(string name, string value);

        /// <summary>
        /// 设置属性的值 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <param name="locale"></param>
        void SetPropertyValue(string name, string value, string locale);

        /// <summary>
        /// 移除属性值
        /// </summary>
        /// <param name="name"></param>
        void RemoveProperty(string name);

        /// <summary>
        ///  移除属性值
        /// </summary>
        /// <param name="name"></param>
        /// <param name="locale"></param>
        void RemoveProperty(string name, string locale);

        /// <summary>
        /// 是否存在指定的属性
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasProperty(string name);
    }

    class BundlePropertyCollectionImpl : List<BundleProperty>, BundlePropertyCollection
    {
        BundleDescription desc;

        public BundlePropertyCollectionImpl()
        {
        }

        public BundlePropertyCollectionImpl(BundleDescription desc)
        {
            this.desc = desc;
        }

        public BundlePropertyCollectionImpl(BundlePropertyCollection col)
        {
            AddRange(col);
        }

        public string GetPropertyValue(string name)
        {
            return GetPropertyValue(name, System.Threading.Thread.CurrentThread.CurrentCulture.ToString());
        }

        public string GetPropertyValue(string name, string locale)
        {
            locale = NormalizeLocale(locale);

            string lang = GetLocaleLang(locale);

            BundleProperty sameLangDifCountry = null;
            BundleProperty sameLang = null;
            BundleProperty defaultLoc = null;

            foreach (var p in this)
            {
                if (p.Name == name)
                {
                    if (p.Locale == locale)
                        return ParseString(p.Value);

                    string plang = GetLocaleLang(p.Locale);

                    if (plang == p.Locale && plang == lang) // No country specified
                        sameLang = p;
                    else if (plang == lang)
                        sameLangDifCountry = p;
                    else if (p.Locale == null)
                        defaultLoc = p;
                }
            }

            if (sameLang != null)
                return ParseString(sameLang.Value);
            else if (sameLangDifCountry != null)
                return ParseString(sameLangDifCountry.Value);
            else if (defaultLoc != null)
                return ParseString(defaultLoc.Value);
            else
                return string.Empty;
        }

        string ParseString(string s)
        {
            if (desc != null)
                return desc.ParseString(s);
            else
                return s;
        }

        string NormalizeLocale(string loc)
        {
            if (string.IsNullOrEmpty(loc))
                return null;

            return loc.Replace('_', '-');
        }

        string GetLocaleLang(string loc)
        {
            if (loc == null)
                return null;
            int i = loc.IndexOf('-');
            if (i != -1)
                return loc.Substring(0, i);
            else
                return loc;
        }

        public void SetPropertyValue(string name, string value)
        {
            SetPropertyValue(name, value, null);
        }

        public void SetPropertyValue(string name, string value, string locale)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("name can't be null or empty");

            if (value == null)
                throw new ArgumentNullException("value");

            locale = NormalizeLocale(locale);

            foreach (var p in this)
            {
                if (p.Name == name && p.Locale == locale)
                {
                    p.Value = value;

                    return;
                }
            }
            BundleProperty prop = new BundleProperty();

            prop.Name = name;
            prop.Value = value;
            prop.Locale = locale;

            this.Add(prop);
        }

        public void RemoveProperty(string name)
        {
            RemoveProperty(name, null);
        }

        public void RemoveProperty(string name, string locale)
        {
            locale = NormalizeLocale(locale);

            foreach (var p in this)
            {
                if (p.Name == name && p.Locale == locale)
                {
                    Remove(p);
                    return;
                }
            }
        }

        public bool HasProperty(string name)
        {
            return this.Any(p => p.Name == name);
        }

        internal string ExtractCoreProperty(string name, bool removeProperty)
        {
            foreach (var p in this)
            {
                if (p.Name == name && p.Locale == null)
                {
                    if (removeProperty)
                        Remove(p);
                    return p.Value;
                }
            }
            return null;
        }
    }
}
