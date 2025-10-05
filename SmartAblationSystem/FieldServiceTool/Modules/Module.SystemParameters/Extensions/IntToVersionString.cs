using System;

namespace Module.SystemParameters.Extensions
{
	public static class IntToVersionString
	{
		public static string ToVersionString(this Int32 value)
		{
			string valueConverted = Convert.ToInt64(value).ToString("X");
			if (valueConverted != null)
			{
				int length = valueConverted.Length;
				switch (length)
				{
					case 1:
						valueConverted += ".0.0.0";
						break;
					case 2:
						valueConverted = valueConverted.Insert(length - 1, ".") + "0.0";
						break;
					case 3:
						valueConverted = valueConverted.Insert(length, ".").Insert(length - 1, ".").Insert(length - 2, ".") + "0";
						break;
					case 4:
						valueConverted = valueConverted.Insert(3, ".").Insert(2, ".").Insert(1, ".");
						break;
				}
			}
			return valueConverted;
		}

		public static string ToVersionString(this string value)
		{
			const string EmptyVersion = "0.0.0.0";
			if (value != null)
			{
				int length = value.Length;
				switch (length)
				{
					case 1:
						value += ".0.0.0";
						break;
					case 2:
						value = value.Insert(length - 1, ".") + ".0.0";
						break;
					case 3:
						value = value.Insert(length, ".").Insert(length - 1, ".").Insert(length - 2, ".") + "0";
						break;
					case 4:
						value = value.Insert(3, ".").Insert(2, ".").Insert(1, ".");
						break;
				}
			}
			else
			{
				value = EmptyVersion;
			}

			return value;
		}
	}
}
