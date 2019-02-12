using System;
using System.IO;
using System.Text;

namespace Clamp.Cfg
{

	/// <summary>
	/// This class is used to read properties lines.  These lines do
	/// not terminate with new-line chars but rather when there is no
	/// backslash sign a the end of the line.  This is used to
	/// concatenate multiple lines for readability.
	/// </summary>
	internal class PropertiesReader : StreamReader
	{
		/// <summary>
		/// Constructor.
		/// </summary>
		/// <param name="reader">A Reader.</param>
		public PropertiesReader(StreamReader reader) : base(reader.BaseStream)
		{
		}

		/// <summary>
		/// Read a property.
		/// </summary>
		/// <returns>A String.</returns>
		public String ReadProperty()
		{
			StringBuilder buffer = new StringBuilder();


			while(true)
			{
				String line = ReadLine();

				if (line == null)
				{
					return null;
				}

				line = line.Trim();

				if ((line.Length != 0) && (line[0] != '#'))
				{
					if (line.EndsWith(@"\"))
					{
						line = line.Substring(0, (line.Length - 1) - (0));
						buffer.Append(line);
					}
					else
					{
						buffer.Append(line);
						break;
					}
				}
			}


			return buffer.ToString();
		}
	}
}