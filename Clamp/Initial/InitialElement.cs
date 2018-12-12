using System;
using System.Collections.Generic;

namespace Clamp.OSG.Initial
{
    /// <summary>
    /// INI文件的元素类，一般来讲有节，和参数
    /// </summary>
    public abstract class InitialElement
    {
        internal InitialElement(string name)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentNullException("name");

            Name = name;
        }

        /// <summary>
        /// 元素的名称
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 获得元素的对应的注解
        /// </summary>
        public string Comment { get; set; }

        /// <summary>
        /// 前注解
        /// </summary>
        public string PreComment { get; set; }

        public override string ToString()
        {
            return ToString(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="includeComments"></param>
        /// <returns></returns>
        public string ToString(bool includeComments)
        {
            string stringExpr = GetStringExpression();

            if (includeComments)
            {
                if (Comment != null && PreComment != null)
                {
                    return string.Format("{0}{1}{2} {3}", GetFormattedPreComment(), Environment.NewLine, stringExpr, GetFormattedComment());
                }
                else if (Comment != null)
                {
                    return string.Format("{0} {1}", stringExpr, GetFormattedComment());
                }
                else if (PreComment != null)
                {
                    return string.Format("{0}{1}{2}", GetFormattedPreComment(), Environment.NewLine, stringExpr);
                }
            }

            return stringExpr;
        }

        private string GetFormattedComment()
        {
            string comment = Comment;

            int iNewLine = Comment.IndexOfAny(Environment.NewLine.ToCharArray());
            if (iNewLine >= 0)
                comment = comment.Substring(0, iNewLine);

            return (InitialFile.PreferredCommentChar + " " + comment);
        }

        private string GetFormattedPreComment()
        {
            string[] lines = PreComment.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);

            return string.Join(Environment.NewLine, Array.ConvertAll(lines, s => InitialFile.PreferredCommentChar + " " + s));
        }

        protected abstract string GetStringExpression();
    }
}
