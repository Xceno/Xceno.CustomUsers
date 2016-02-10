// I ripped this somewhere from the Interwebz
namespace Xceno.CustomUsers.Helpers
{
	using System;
	using System.Collections.Generic;
	using System.IO;

	public static class StringExtensions
	{
		/// <summary> Trims a string or returns an empty string, if the original string is null </summary>
		public static string TrimSafe(this string s)
		{
			return s == null ? string.Empty : s.Trim();
		}

		/// <summary> Trims a string or returns null, if the original string is null </summary>
		public static string TrimSafeNull(this string s)
		{
			return s == null ? null : s.Trim();
		}

		public static bool Contains(this string source, string value, StringComparison comparison)
		{
			return source.IndexOf(value, comparison) >= 0;
		}

		public static bool ContainsSafe(this string self, string value, StringComparison comparison = StringComparison.Ordinal)
		{
			return !string.IsNullOrWhiteSpace(self) && self.IndexOf(value, comparison) >= 0;
		}

		public static bool HasValue(this string self)
		{
			return !string.IsNullOrWhiteSpace(self);
		}

		/// <summary>
		/// Returns all indivual lines (separated by CR, LF or CRLF).
		/// </summary>
		/// <param name="source">The string </param>
		/// <returns>A sequence of individual lines.</returns>
		public static IEnumerable<string> GetAllLines(this string source)
		{
			if ( source == null )
				yield break;

			using ( var reader = new StringReader(source) )
				foreach ( var line in reader.GetAllLines() )
					yield return line;
		}

		public static IEnumerable<string> GetAllLines(this TextReader source)
		{
			string line;
			while ( ( line = source.ReadLine() ) != null )
				yield return line;
		}

		////public static string ToUpperFirstLetter(this string source)
		////{
		////	if ( string.IsNullOrEmpty(source) )
		////		return string.Empty;
		////	var letters = source.ToCharArray();
		////	letters[0] = char.ToUpper(letters[0]);
		////	return new string(letters);
		////}
	}
}