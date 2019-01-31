using System;

namespace Clamp.Linker.ViewEngines.Benben
{
	public class DescriptionAttribute : Attribute
	{
		public string Description { get; set; }

		public DescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}