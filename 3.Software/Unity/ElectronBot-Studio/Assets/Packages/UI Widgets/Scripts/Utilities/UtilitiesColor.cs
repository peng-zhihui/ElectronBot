namespace UIWidgets
{
	using System.Globalization;
	using UnityEngine;

	/// <summary>
	/// Color functions.
	/// </summary>
	public static class UtilitiesColor
	{
		/// <summary>
		/// Convert specified color to RGB hex.
		/// </summary>
		/// <returns>RGB hex.</returns>
		/// <param name="c">Color.</param>
		public static string RGB2Hex(Color32 c)
		{
			return string.Format("#{0}", ColorUtility.ToHtmlStringRGB(c));
		}

		/// <summary>
		/// Convert specified color to RGBA hex.
		/// </summary>
		/// <returns>RGBA hex.</returns>
		/// <param name="c">Color.</param>
		public static string RGBA2Hex(Color32 c)
		{
			return string.Format("#{0}", ColorUtility.ToHtmlStringRGBA(c));
		}

		/// <summary>
		/// Converts the string representation of a number to its Byte equivalent. A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <returns><c>true</c> if hex was converted successfully; otherwise, <c>false</c>.</returns>
		/// <param name="hex">A string containing a number to convert.</param>
		/// <param name="result">When this method returns, contains the 8-bit unsigned integer value equivalent to the number contained in s if the conversion succeeded, or zero if the conversion failed. The conversion fails if the s parameter is null or String.Empty, is not of the correct format, or represents a number less than MinValue or greater than MaxValue. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
		public static bool TryParseHex(string hex, out byte result)
		{
			return byte.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out result);
		}

		static char[] HexTrimChars = new char[] { '#', ';' };

		/// <summary>
		/// Converts the string representation of a color to its Color equivalent. A return value indicates whether the conversion succeeded or failed.
		/// </summary>
		/// <returns><c>true</c> if hex was converted successfully; otherwise, <c>false</c>.</returns>
		/// <param name="hex">A string containing a color to convert.</param>
		/// <param name="result">When this method returns, contains the color value equivalent to the color contained in hex if the conversion succeeded, or Color.black if the conversion failed. The conversion fails if the hex parameter is null or String.Empty, is not of the correct format. This parameter is passed uninitialized; any value originally supplied in result will be overwritten.</param>
		[System.Obsolete("Use ColorUtility.TryParseHtmlString")]
		public static bool TryHexToRGBA(string hex, out Color32 result)
		{
			result = Color.black;

			if (string.IsNullOrEmpty(hex))
			{
				return false;
			}

			var h = hex.Trim(HexTrimChars);
			byte r, g, b, a;

			if (h.Length == 8)
			{
				if (!TryParseHex(h.Substring(0, 2), out r))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(2, 2), out g))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(4, 2), out b))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(6, 2), out a))
				{
					return false;
				}
			}
			else if (h.Length == 6)
			{
				if (!TryParseHex(h.Substring(0, 2), out r))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(2, 2), out g))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(4, 2), out b))
				{
					return false;
				}

				a = 255;
			}
			else if (h.Length == 3)
			{
				if (!TryParseHex(h.Substring(0, 1), out r))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(1, 1), out g))
				{
					return false;
				}

				if (!TryParseHex(h.Substring(2, 1), out b))
				{
					return false;
				}

				r *= 17;
				g *= 17;
				b *= 17;
				a = 255;
			}
			else
			{
				return false;
			}

			result = new Color32(r, g, b, a);

			return true;
		}
	}
}