using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Clamp.Data.Annotation
{
    /// <summary>
    /// 扩展属性
    /// </summary>
    public class ExtensionAttributeAttribute : Attribute
    {
        private Type targetType;
        private string targetTypeName;
        private string name;
        private string val;
        private string path;

        public ExtensionAttributeAttribute(string name, string value)
        {
            this.name = name;
            this.val = value;
        }

        public ExtensionAttributeAttribute(Type type, string name, string value)
        {
            this.name = name;
            this.val = value;
            this.targetType = type;
        }

      
        public ExtensionAttributeAttribute(string path, string name, string value)
        {
            this.name = name;
            this.val = value;
            this.path = path;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        /// <summary>
        /// 值
        /// </summary>
        public string Value
        {
            get { return this.val; }
            set { this.val = value; }
        }

        /// <summary>
        /// 路径
        /// </summary>
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        /// <summary>
        /// 类型
        /// </summary>
        public Type Type
        {
            get { return targetType; }
            set { targetType = value; targetTypeName = targetType.FullName; }
        }

        /// <summary>
        /// 类型名
        /// </summary>
        internal string TypeName
        {
            get { return targetTypeName ?? string.Empty; }
            set { targetTypeName = value; }
        }
    }
}
