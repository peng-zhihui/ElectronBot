namespace UIWidgets.Menu
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using UnityEngine;

	/// <summary>
	/// Hot key.
	/// </summary>
	[Serializable]
	public class HotKey : IEquatable<HotKey>
	{
		[SerializeField]
		HotKeyCode key;

		/// <summary>
		/// Key.
		/// </summary>
		public HotKeyCode Key
		{
			get
			{
				return key;
			}
		}

		[SerializeField]
		bool ctrl;

		/// <summary>
		/// Should be control key pressed?
		/// </summary>
		public bool Ctrl
		{
			get
			{
				return ctrl;
			}
		}

		[SerializeField]
		bool alt;

		/// <summary>
		/// Should be alt key pressed?
		/// </summary>
		public bool Alt
		{
			get
			{
				return alt;
			}
		}

		[SerializeField]
		bool shift;

		/// <summary>
		/// Should be shift key pressed?
		/// </summary>
		public bool Shift
		{
			get
			{
				return shift;
			}
		}

		/// <summary>
		/// Is pressed?
		/// </summary>
		public bool IsPressed
		{
			get
			{
				if (!Valid)
				{
					return false;
				}

				var ctrl = Ctrl == CompatibilityInput.CtrlPressed;
				var shift = Shift == CompatibilityInput.ShiftPressed;
				var alt = Alt == CompatibilityInput.AltPressed;
				var key = CompatibilityInput.IsPressed(Key);

				return ctrl && shift && alt && key;
			}
		}

		int lastFramePressed;

		/// <summary>
		/// Is up?
		/// </summary>
		public bool IsUp
		{
			get
			{
				if (!Valid)
				{
					return false;
				}

				if (IsPressed)
				{
					lastFramePressed = UtilitiesTime.GetFrameCount();
				}

				return (lastFramePressed > 0) && (lastFramePressed == (UtilitiesTime.GetFrameCount() - 1));
			}
		}

		/// <summary>
		/// Is hot key valid?
		/// </summary>
		public bool Valid
		{
			get
			{
				return key != HotKeyCode.None;
			}
		}

		/// <summary>
		/// Modifiers count.
		/// </summary>
		public int Modifiers
		{
			get
			{
				var result = 0;
				if (ctrl)
				{
					result += 1;
				}

				if (alt)
				{
					result += 1;
				}

				if (shift)
				{
					result += 1;
				}

				return result;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="HotKey"/> class.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <param name="ctrl">Should be control key pressed?</param>
		/// <param name="alt">Should be alt key pressed?</param>
		/// <param name="shift">Should be shift key pressed?</param>
		public HotKey(HotKeyCode key, bool ctrl, bool alt, bool shift)
		{
			this.key = key;
			this.ctrl = ctrl;
			this.alt = alt;
			this.shift = shift;
			lastFramePressed = -2;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="obj">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public override bool Equals(object obj)
		{
			if (obj is HotKey)
			{
				return Equals((HotKey)obj);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
		public bool Equals(HotKey other)
		{
			if (ReferenceEquals(other, null))
			{
				return false;
			}

			return (key == other.key) && (ctrl == other.ctrl) && (shift == other.shift) && (alt == other.alt);
		}

		/// <summary>
		/// Hash function.
		/// </summary>
		/// <returns>A hash code for the current object.</returns>
		public override int GetHashCode()
		{
			var code = EqualityComparer<HotKeyCode>.Default.GetHashCode(key);
			return code ^ ctrl.GetHashCode() ^ shift.GetHashCode() ^ alt.GetHashCode();
		}

		/// <summary>
		/// Compare specified hot keys.
		/// </summary>
		/// <param name="hotkey1">First hot key.</param>
		/// <param name="hotkey2">Second hot key.</param>
		/// <returns>true if the hot keys are equal; otherwise, false.</returns>
		public static bool operator ==(HotKey hotkey1, HotKey hotkey2)
		{
			if (ReferenceEquals(hotkey1, null))
			{
				return ReferenceEquals(hotkey2, null);
			}

			return hotkey1.Equals(hotkey2);
		}

		/// <summary>
		/// Compare specified hot keys.
		/// </summary>
		/// <param name="hotkey1">First hot key.</param>
		/// <param name="hotkey2">Second hot key.</param>
		/// <returns>true if the hot keys not equal; otherwise, false.</returns>
		public static bool operator !=(HotKey hotkey1, HotKey hotkey2)
		{
			return !(hotkey1 == hotkey2);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>A string that represents the current object.</returns>
		public override string ToString()
		{
			if (!Valid)
			{
				return string.Empty;
			}

			var s = new StringBuilder();
			if (ctrl)
			{
				s.Append("Ctrl+");
			}

			if (shift)
			{
				s.Append("Shift+");
			}

			if (alt)
			{
				s.Append("Alt+");
			}

			s.Append(key.ToHumanString());

			return s.ToString();
		}
	}
}