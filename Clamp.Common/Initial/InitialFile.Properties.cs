using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Clamp.Common.Initial
{
    public partial class InitialFile : IEnumerable<InitialProperty>
    {
        /// <summary>
        /// 当前语言文化
        /// </summary>
        public static CultureInfo CultureInfo
        {
            get { return _cultureInfo; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");

                _cultureInfo = value;
            }
        }

        /// <summary>
        /// 有效的验证符号
        /// </summary>
        public static char[] ValidCommentChars { get; private set; }

        /// <summary>
        /// 用于指定注解符号
        /// </summary>
        public static char PreferredCommentChar
        {
            get { return _preferredCommentChar; }
            set
            {
                if (!Array.Exists(ValidCommentChars, c => c == value))
                    throw new ArgumentException(string.Format("不可能出现非法的字符{0}", value));

                _preferredCommentChar = value;
            }
        }

        /// <summary>
        /// 用于分离组数的符号
        /// </summary>
        public static char ArrayElementSeparator
        {
            get { return _arrayElementSeparator; }
            set
            {
                if (value == '\0')
                    throw new ArgumentException("不可能出现\0的值");

                _arrayElementSeparator = value;
            }
        }

        /// <summary>
        /// 是否内部注解
        /// </summary>
        public static bool IgnoreInlineComments { get; set; }

        /// <summary>
        /// 是否忽略前注解
        /// </summary>
        public static bool IgnorePreComments { get; set; }
    }
}
