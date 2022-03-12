namespace UIWidgets
{
	using System;
	using System.Globalization;

	/// <summary>
	/// Compare functions.
	/// </summary>
	public static class UtilitiesCompare
	{
		/// <summary>
		/// Culture.
		/// </summary>
		public static CultureInfo Culture = CultureInfo.InvariantCulture;

		/// <summary>
		/// Case sensitive compare options.
		/// </summary>
		public static CompareOptions OptionsCaseSensitive = CompareOptions.None;

		/// <summary>
		/// Case ignore compare options.
		/// </summary>
		public static CompareOptions OptionsCaseIgnore = CompareOptions.IgnoreCase;

		/// <summary>
		/// Determines whether the beginning of Target matches the Substring.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="substring">Substring.</param>
		/// <param name="caseSensitive">false to ignore case during the comparison; otherwise, true.</param>
		/// <returns>true if beginning of value matches the Target; otherwise, false.</returns>
		public static bool StartsWith(string target, string substring, bool caseSensitive = true)
		{
			var compare = Culture.CompareInfo;
			var options = caseSensitive ? OptionsCaseSensitive : OptionsCaseIgnore;

			return compare.IndexOf(target, substring, options) == 0;
		}

		/// <summary>
		/// Returns a value indicating whether Target occurs within specified Substring.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="substring">Substring.</param>
		/// <param name="caseSensitive">false to ignore case during the comparison; otherwise, true.</param>
		/// <returns>true if the Target occurs within specified Substring; otherwise, false.</returns>
		public static bool Contains(string target, string substring, bool caseSensitive = true)
		{
			var compare = Culture.CompareInfo;
			var options = caseSensitive ? OptionsCaseSensitive : OptionsCaseIgnore;

			return compare.IndexOf(target, substring, options) != -1;
		}

		/// <summary>
		/// Compares two specified String objects, ignoring or honoring their case, and using culture-specific information to influence the comparison, and returns an integer that indicates their relative position in the sort order.
		/// </summary>
		/// <param name="x">The first string to compare.</param>
		/// <param name="y">The second string to compare.</param>
		/// <param name="caseSensitive">false to ignore case during the comparison; otherwise, true.</param>
		/// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.</returns>
		public static int Compare(string x, string y, bool caseSensitive = true)
		{
			var compare = Culture.CompareInfo;
			var options = caseSensitive ? OptionsCaseSensitive : OptionsCaseIgnore;

			return compare.Compare(x, y, options);
		}

		/// <summary>
		/// Compares two specified objects and returns an integer that indicates their relative position in the sort order.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>A 32-bit signed integer that indicates the lexical relationship between the two comparands.</returns>
		/// <typeparam name="T">Type of arguments.</typeparam>
		public static int Compare<T>(T x, T y)
			where T : IComparable<T>
		{
			return x.CompareTo(y);
		}
	}
}