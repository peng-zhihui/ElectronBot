namespace UIWidgets.Menu
{
	/// <summary>
	/// HotKeyCode extensions.
	/// </summary>
	public static class HotKeyCodeExtensions
	{
		/// <summary>
		/// Convert HotKeyCode to human-readable string.
		/// </summary>
		/// <param name="code">Code.</param>
		/// <returns>Human-readable string.</returns>
		public static string ToHumanString(this HotKeyCode code)
		{
			switch (code)
			{
				case HotKeyCode.Quote:
					return "'";
				case HotKeyCode.Comma:
					return ",";
				case HotKeyCode.Minus:
					return "-";
				case HotKeyCode.Period:
					return ".";
				case HotKeyCode.Slash:
					return "/";
				case HotKeyCode.Semicolon:
					return ";";
				case HotKeyCode.Equals:
					return "=";
				case HotKeyCode.Backslash:
					return "\\";
				case HotKeyCode.LeftBracket:
					return "[";
				case HotKeyCode.RightBracket:
					return "]";
				case HotKeyCode.Alpha0:
					return "0";
				case HotKeyCode.Alpha1:
					return "1";
				case HotKeyCode.Alpha2:
					return "2";
				case HotKeyCode.Alpha3:
					return "3";
				case HotKeyCode.Alpha4:
					return "4";
				case HotKeyCode.Alpha5:
					return "5";
				case HotKeyCode.Alpha6:
					return "6";
				case HotKeyCode.Alpha7:
					return "7";
				case HotKeyCode.Alpha8:
					return "8";
				case HotKeyCode.Alpha9:
					return "9";
				case HotKeyCode.Multiply:
					return "*";
				case HotKeyCode.Plus:
					return "Num+";
				case HotKeyCode.UpArrow:
					return "↑";
				case HotKeyCode.DownArrow:
					return "↓";
				case HotKeyCode.RightArrow:
					return "→";
				case HotKeyCode.LeftArrow:
					return "←";
				default:
					return EnumHelper<HotKeyCode>.ToString(code);
			}
		}
	}
}