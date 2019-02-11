using System;
using System.Text;

namespace Clamp.Cfg
{
	/// <summary>
	/// This class divides into tokens a property value.  Token
	/// separator is "," but commas into the property value are escaped
	/// using the backslash in front.
	/// </summary>
	internal class PropertiesTokenizer : StringTokenizer
	{
		/// <summary>
		/// The property delimiter used while parsing (a comma).
		/// </summary>
		internal const string DELIMITER = ",";

		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="str">A String</param>
		public PropertiesTokenizer(string str) : base(str, DELIMITER)
		{
		}


		/// <summary>
		/// Get next token.
		/// </summary>
		/// <returns>A String</returns>
		public override string NextToken()
		{
			StringBuilder buffer = new StringBuilder();

			while(HasMoreTokens())
			{
                string token = base.NextToken();

				if (token.EndsWith(@"\"))
				{
					buffer.Append(token.Substring(0, (token.Length - 1) - (0)));
					buffer.Append(DELIMITER);
				}
				else
				{
					buffer.Append(token);
					break;
				}
			}

			return buffer.ToString().Trim();
		}
	}
}