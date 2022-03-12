#if UIWIDGETS_DATABIND_SUPPORT && UNITY_EDITOR
namespace UIWidgets.DataBindSupport
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	/// <summary>
	/// DataBind events info.
	/// </summary>
	public class DataBindEvents : IFormattable
	{
		/// <summary>
		/// Classname.
		/// </summary>
		public string ClassName;

		/// <summary>
		/// Event name.
		/// </summary>
		public string EventName;

		/// <summary>
		/// Arguments.
		/// </summary>
		public List<string> Arguments;

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <returns>String.</returns>
		public string ToString(string format)
		{
			return ToString(format, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="formatProvider">Format provider.</param>
		/// <returns>String.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			switch (format)
			{
				case "FuncName":
					return FirstLetterToUpper(EventName) + ClassName;
				case "EventName":
					return EventName;
				case "Arguments":
					return GetArgs(Arguments);
				default:
					throw new ArgumentOutOfRangeException("Unsupported format: " + format);
			}
		}

		/// <summary>
		/// Get arguments.
		/// </summary>
		/// <param name="types">Types list,</param>
		/// <returns>Arguments string.</returns>
		protected virtual string GetArgs(List<string> types)
		{
			var result = new List<string>(types.Count);

			for (int i = 0; i < types.Count; i++)
			{
				result.Add(types[i] + " arg" + i);
			}

			return string.Join(", ", result.ToArray());
		}

		/// <summary>
		/// Convert first letter to uppercase.
		/// </summary>
		/// <param name="input">Input string.</param>
		/// <returns>Result.</returns>
		protected static string FirstLetterToUpper(string input)
		{
			return input[0].ToString().ToUpper() + input.Substring(1);
		}
	}
}
#endif