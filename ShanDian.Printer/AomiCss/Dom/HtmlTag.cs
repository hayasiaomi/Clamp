using System.Collections.Generic;
using Hydra.AomiCss.Utils;

namespace Hydra.AomiCss.Dom
{
    /// <summary>
    /// HTML�ڵ���
    /// </summary>
    internal sealed class HtmlTag
    {
        /// <summary>
        /// �ڵ�����
        /// </summary>
        private readonly string _name;

        /// <summary>
        /// �Ƿ�Ϊ�ڵ�
        /// </summary>
        private readonly bool _isSingle;

        /// <summary>
        /// �����ź�
        /// </summary>
        private readonly Dictionary<string, string> _attributes;

        public HtmlTag(string name, bool isSingle, Dictionary<string, string> attributes = null)
        {
            ArgChecker.AssertArgNotNullOrEmpty(name, "name");

            _name = name;
            _isSingle = isSingle;
            _attributes = attributes;
        }

        /// <summary>
        /// �ڵ�����
        /// </summary>
        public string Name
        {
            get { return _name; }
        }

        /// <summary>
        /// ���Լ���
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return _attributes; }
        }

        /// <summary>
        /// ���ڵ�
        /// </summary>
        public bool IsSingle
        {
            get { return _isSingle; }
        }

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        /// <returns></returns>
        public bool HasAttributes()
        {
            return _attributes != null && _attributes.Count > 0;
        }

        /// <summary>
        /// �Ƿ���ָ��attribute
        /// </summary>
        /// <param name="attribute"></param>
        /// <returns></returns>
        public bool HasAttribute(string attribute)
        {
            return _attributes != null && _attributes.ContainsKey(attribute);
        }

        /// <summary>
        /// ���ָ��������ֵ������û�еĻ����ͷ���Ĭ�ϵ�ֵ
        /// </summary>
        /// <param name="attribute"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public string TryGetAttribute(string attribute, string defaultValue = null)
        {
            return _attributes != null && _attributes.ContainsKey(attribute) ? _attributes[attribute] : defaultValue;
        }

        public override string ToString()
        {
            return string.Format("<{0}>", _name);
        }
    }
}