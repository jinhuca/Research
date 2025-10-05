using Module.Infrastructure.Properties;
using System;
using System.Collections.Generic;

namespace Module.Infrastructure.Extensions
{
	public static class StringExtensions
	{
		private static IEnumerable<string> SplitInParts(this string s, int partLength)
		{
			if (s == null)
			{
				throw new ArgumentNullException(nameof(s));
			}
			if (partLength <= 0)
			{
				throw new ArgumentException(Resources.SubstringLengthException, nameof(partLength));
			}

			for (var i = 0; i < s.Length; i += partLength)
				yield return s.Substring(i, Math.Min(partLength, s.Length - i));
		}

		public static string SeparateBy(this string s, int partLength, string separator) => string.Join(separator, s.SplitInParts(partLength));
	}
}
