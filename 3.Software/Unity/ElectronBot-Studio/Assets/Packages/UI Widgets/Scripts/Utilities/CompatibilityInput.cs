namespace UIWidgets
{
	using System;
	using UIWidgets.Menu;
	using UnityEngine;
#if ENABLE_INPUT_SYSTEM
	using UnityEngine.InputSystem;
#endif

	/// <summary>
	/// Input compatibility functions.
	/// </summary>
	public static class CompatibilityInput
	{
		/// <summary>
		/// Mouse position.
		/// </summary>
		public static Vector2 MousePosition
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return MousePresent ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
				return Input.mousePosition;
#endif
			}
		}

		/// <summary>
		/// Is mouse present?
		/// </summary>
		public static bool MousePresent
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return Mouse.current != null;
#else
				return Input.mousePresent;
#endif
			}
		}

		/// <summary>
		/// Is left mouse button pressed?
		/// </summary>
		public static bool IsMouseLeftButtonPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return MousePresent && Mouse.current.leftButton.isPressed;
#else
				return Input.GetMouseButton(0);
#endif
			}
		}

		/// <summary>
		/// Is right mouse button pressed?
		/// </summary>
		public static bool IsMouseRightButtonPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return MousePresent && Mouse.current.rightButton.isPressed;
#else
				return Input.GetMouseButton(1);
#endif
			}
		}

		/// <summary>
		/// Is middle mouse button pressed?
		/// </summary>
		public static bool IsMouseMiddleButtonPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return MousePresent && Mouse.current.middleButton.isPressed;
#else
				return Input.GetMouseButton(2);
#endif
			}
		}

		/// <summary>
		/// Touches count.
		/// </summary>
		public static int TouchCount
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
				return UnityEngine.InputSystem.Enhanced​Touch.Touch.activeTouches.Count;
#else
				return Input.touchCount;
#endif
			}
		}

#if ENABLE_INPUT_SYSTEM
		static UnityEngine.TouchPhase ConvertTouchPhase(UnityEngine.InputSystem.TouchPhase phase)
		{
			switch (phase)
			{
				case UnityEngine.InputSystem.TouchPhase.None:
					return UnityEngine.TouchPhase.Ended;
				case UnityEngine.InputSystem.TouchPhase.Began:
					return UnityEngine.TouchPhase.Began;
				case UnityEngine.InputSystem.TouchPhase.Moved:
					return UnityEngine.TouchPhase.Moved;
				case UnityEngine.InputSystem.TouchPhase.Ended:
					return UnityEngine.TouchPhase.Ended;
				case UnityEngine.InputSystem.TouchPhase.Canceled:
					return UnityEngine.TouchPhase.Canceled;
				case UnityEngine.InputSystem.TouchPhase.Stationary:
					return UnityEngine.TouchPhase.Stationary;
			}

			return UnityEngine.TouchPhase.Ended;
		}

		static UnityEngine.Touch ConvertTouch(UnityEngine.InputSystem.EnhancedTouch.Touch touch)
		{
			return new Touch()
			{
				fingerId = touch.finger.index,
				position = touch.screenPosition,
				rawPosition = touch.screenPosition,
				deltaPosition = touch.delta,
				deltaTime = (float)(touch.time - touch.startTime),
				tapCount = touch.tapCount,
				phase = ConvertTouchPhase(touch.phase),
				pressure = touch.pressure,
				maximumPossiblePressure = 1f,
				type = TouchType.Direct,
				radius = touch.radius.magnitude,
			};
		}
#endif

		/// <summary>
		/// Get touch.
		/// </summary>
		/// <param name="index">Touch index.</param>
		/// <returns>Touch.</returns>
		public static UnityEngine.Touch GetTouch(int index)
		{
#if ENABLE_INPUT_SYSTEM
			UnityEngine.InputSystem.EnhancedTouch.EnhancedTouchSupport.Enable();
			return ConvertTouch(UnityEngine.InputSystem.Enhanced​Touch.Touch.activeTouches[index]);
#else
			return Input.GetTouch(index);
#endif
		}

		/// <summary>
		/// Is keyboard present?
		/// </summary>
		public static bool KeyboardPresent
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return Keyboard.current != null;
#else
				return true;
#endif
			}
		}

		/// <summary>
		/// Is left arrow down?
		/// </summary>
		public static bool IsLeftArrowDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.leftArrowKey.wasPressedThisFrame;
#else
				return Input.GetKeyDown(KeyCode.LeftArrow);
#endif
			}
		}

		/// <summary>
		/// Is right arrow down?
		/// </summary>
		public static bool IsRightArrowDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.rightArrowKey.wasPressedThisFrame;
#else
				return Input.GetKeyDown(KeyCode.RightArrow);
#endif
			}
		}

		/// <summary>
		/// Is up arrow down?
		/// </summary>
		public static bool IsUpArrowDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.upArrowKey.wasPressedThisFrame;
#else
				return Input.GetKeyDown(KeyCode.UpArrow);
#endif
			}
		}

		/// <summary>
		/// Is down arrow down?
		/// </summary>
		public static bool IsDownArrowDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.downArrowKey.wasPressedThisFrame;
#else
				return Input.GetKeyDown(KeyCode.DownArrow);
#endif
			}
		}

		/// <summary>
		/// Is tab key down?
		/// </summary>
		public static bool IsTabDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.tabKey.wasPressedThisFrame;
#else
				return Input.GetKeyDown(KeyCode.Tab);
#endif
			}
		}

		/// <summary>
		/// Is enter key down?
		/// </summary>
		public static bool IsEnterDown
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && (Keyboard.current.enterKey.wasPressedThisFrame || Keyboard.current.numpadEnterKey.wasPressedThisFrame);
#else
				return Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
#endif
			}
		}

		/// <summary>
		/// Is enter key pressed?
		/// </summary>
		public static bool IsEnterPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && (Keyboard.current.enterKey.isPressed || Keyboard.current.numpadEnterKey.isPressed);
#else
				return Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter);
#endif
			}
		}

		/// <summary>
		/// Is shift key pressed?
		/// </summary>
		public static bool IsShiftPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.shiftKey.isPressed;
#else
				return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
			}
		}

		/// <summary>
		/// Is pointer over screen?
		/// </summary>
		/// <returns>true if pointer over screen; otherwise false.</returns>
		public static bool IsPointerOverScreen
		{
			get
			{
#if UNITY_EDITOR
				var screen_size = UnityEditor.Handles.GetMainGameViewSize();
#else
				var screen_size = new Vector2(Screen.width, Screen.height);
#endif
				var mouse = MousePosition;

				if ((mouse.x <= 0)
					|| (mouse.y <= 0)
					|| (mouse.x >= (screen_size.x - 1))
					|| (mouse.y >= (screen_size.y - 1)))
				{
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Is context menu key pressed?
		/// </summary>
		public static bool ContextMenuPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.contextMenuKey.isPressed;
#else
				return Input.GetKey(KeyCode.Menu);
#endif
			}
		}

		/// <summary>
		/// Is context menu key up?
		/// </summary>
		public static bool ContextMenuUp
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.contextMenuKey.wasReleasedThisFrame;
#else
				return Input.GetKeyUp(KeyCode.Menu);
#endif
			}
		}

		/// <summary>
		/// Is Ctrl key pressed?
		/// </summary>
		public static bool CtrlPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.ctrlKey.isPressed;
#else
				return Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl);
#endif
			}
		}

		/// <summary>
		/// Is Shift key pressed?
		/// </summary>
		public static bool ShiftPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.shiftKey.isPressed;
#else
				return Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
#endif
			}
		}

		/// <summary>
		/// Is Alt key pressed?
		/// </summary>
		public static bool AltPressed
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return KeyboardPresent && Keyboard.current.altKey.isPressed;
#else
				return Input.GetKey(KeyCode.LeftAlt) || Input.GetKey(KeyCode.RightAlt);
#endif
			}
		}

#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// Keys group for InputSystem.
		/// </summary>
		public struct InputSystemKeysGroup : IEquatable<InputSystemKeysGroup>
		{
			/// <summary>
			/// Keys.
			/// </summary>
			public readonly Key[] Keys;

			/// <summary>
			/// Initializes a new instance of the <see cref="InputSystemKeysGroup"/> struct.
			/// </summary>
			/// <param name="keys">Keys.</param>
			public InputSystemKeysGroup(params Key[] keys)
			{
				Keys = keys;
			}

			/// <summary>
			/// Is any key in group is pressed?
			/// </summary>
			public bool IsPressed
			{
				get
				{
					if (!KeyboardPresent)
					{
						return false;
					}

					for (int i = 0; i < Keys.Length; i++)
					{
						var key = Keys[i];
						if (Keyboard.current[key].isPressed)
						{
							return true;
						}
					}

					return false;
				}
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is InputSystemKeysGroup)
				{
					return Equals((InputSystemKeysGroup)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="other">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(InputSystemKeysGroup other)
			{
				return Keys == other.Keys;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return Keys.GetHashCode();
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(InputSystemKeysGroup a, InputSystemKeysGroup b)
			{
				return a.Equals(b);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="a">First instance.</param>
			/// <param name="b">Second instance.</param>
			/// <returns>true if the instances not equal; otherwise, false.</returns>
			public static bool operator !=(InputSystemKeysGroup a, InputSystemKeysGroup b)
			{
				return !a.Equals(b);
			}
		}

		static readonly InputSystemKeysGroup[] HotKey2InputSystem = new[]
		{
			new InputSystemKeysGroup(Key.None),

			new InputSystemKeysGroup(Key.Backspace),
			new InputSystemKeysGroup(Key.Tab),
			new InputSystemKeysGroup(Key.Enter, Key.NumpadEnter),
			new InputSystemKeysGroup(Key.Pause),
			new InputSystemKeysGroup(Key.Escape),
			new InputSystemKeysGroup(Key.Space),

			new InputSystemKeysGroup(Key.Quote),
			new InputSystemKeysGroup(Key.Backquote),

			new InputSystemKeysGroup(Key.Period, Key.NumpadPeriod),
			new InputSystemKeysGroup(Key.Comma),
			new InputSystemKeysGroup(Key.Semicolon),

			new InputSystemKeysGroup(Key.Minus, Key.NumpadMinus),
			new InputSystemKeysGroup(Key.NumpadPlus),
			new InputSystemKeysGroup(Key.NumpadMultiply),
			new InputSystemKeysGroup(Key.Slash, Key.NumpadDivide),
			new InputSystemKeysGroup(Key.Equals, Key.NumpadEquals),

			new InputSystemKeysGroup(Key.Backslash),
			new InputSystemKeysGroup(Key.LeftBracket),
			new InputSystemKeysGroup(Key.RightBracket),

			new InputSystemKeysGroup(Key.Digit0),
			new InputSystemKeysGroup(Key.Digit1),
			new InputSystemKeysGroup(Key.Digit2),
			new InputSystemKeysGroup(Key.Digit3),
			new InputSystemKeysGroup(Key.Digit4),
			new InputSystemKeysGroup(Key.Digit5),
			new InputSystemKeysGroup(Key.Digit6),
			new InputSystemKeysGroup(Key.Digit7),
			new InputSystemKeysGroup(Key.Digit8),
			new InputSystemKeysGroup(Key.Digit9),

			new InputSystemKeysGroup(Key.A),
			new InputSystemKeysGroup(Key.B),
			new InputSystemKeysGroup(Key.C),
			new InputSystemKeysGroup(Key.D),
			new InputSystemKeysGroup(Key.E),
			new InputSystemKeysGroup(Key.F),
			new InputSystemKeysGroup(Key.G),
			new InputSystemKeysGroup(Key.H),
			new InputSystemKeysGroup(Key.I),
			new InputSystemKeysGroup(Key.J),
			new InputSystemKeysGroup(Key.K),
			new InputSystemKeysGroup(Key.L),
			new InputSystemKeysGroup(Key.M),
			new InputSystemKeysGroup(Key.N),
			new InputSystemKeysGroup(Key.O),
			new InputSystemKeysGroup(Key.P),
			new InputSystemKeysGroup(Key.Q),
			new InputSystemKeysGroup(Key.R),
			new InputSystemKeysGroup(Key.S),
			new InputSystemKeysGroup(Key.T),
			new InputSystemKeysGroup(Key.U),
			new InputSystemKeysGroup(Key.V),
			new InputSystemKeysGroup(Key.W),
			new InputSystemKeysGroup(Key.X),
			new InputSystemKeysGroup(Key.Y),
			new InputSystemKeysGroup(Key.Z),

			new InputSystemKeysGroup(Key.Numpad0),
			new InputSystemKeysGroup(Key.Numpad1),
			new InputSystemKeysGroup(Key.Numpad2),
			new InputSystemKeysGroup(Key.Numpad3),
			new InputSystemKeysGroup(Key.Numpad4),
			new InputSystemKeysGroup(Key.Numpad5),
			new InputSystemKeysGroup(Key.Numpad6),
			new InputSystemKeysGroup(Key.Numpad7),
			new InputSystemKeysGroup(Key.Numpad8),
			new InputSystemKeysGroup(Key.Numpad9),

			new InputSystemKeysGroup(Key.UpArrow),
			new InputSystemKeysGroup(Key.DownArrow),
			new InputSystemKeysGroup(Key.RightArrow),
			new InputSystemKeysGroup(Key.LeftArrow),

			new InputSystemKeysGroup(Key.Insert),
			new InputSystemKeysGroup(Key.Delete),
			new InputSystemKeysGroup(Key.Home),
			new InputSystemKeysGroup(Key.End),
			new InputSystemKeysGroup(Key.PageUp),
			new InputSystemKeysGroup(Key.PageDown),

			new InputSystemKeysGroup(Key.F1),
			new InputSystemKeysGroup(Key.F2),
			new InputSystemKeysGroup(Key.F3),
			new InputSystemKeysGroup(Key.F4),
			new InputSystemKeysGroup(Key.F5),
			new InputSystemKeysGroup(Key.F6),
			new InputSystemKeysGroup(Key.F7),
			new InputSystemKeysGroup(Key.F8),
			new InputSystemKeysGroup(Key.F9),
			new InputSystemKeysGroup(Key.F10),
			new InputSystemKeysGroup(Key.F11),
			new InputSystemKeysGroup(Key.F12),

			new InputSystemKeysGroup(Key.PrintScreen),

			new InputSystemKeysGroup(Key.NumLock),
			new InputSystemKeysGroup(Key.CapsLock),
			new InputSystemKeysGroup(Key.ScrollLock),
		};
#else
		/// <summary>
		/// Keys group for legacy input.
		/// </summary>
		public struct LegacyInputKeysGroup : IEquatable<LegacyInputKeysGroup>
		{
			/// <summary>
			/// Keys.
			/// </summary>
			public readonly KeyCode[] Keys;

			/// <summary>
			/// Constructor.
			/// </summary>
			/// <param name="keys">Keys.</param>
			public LegacyInputKeysGroup(params KeyCode[] keys)
			{
				Keys = keys;
			}

			/// <summary>
			/// Is any key in group is pressed?
			/// </summary>
			public bool IsPressed
			{
				get
				{
					for (int i = 0; i < Keys.Length; i++)
					{
						var key = Keys[i];
						if (Input.GetKey(key))
						{
							return true;
						}
					}

					return false;
				}
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public override bool Equals(object obj)
			{
				if (obj is LegacyInputKeysGroup)
				{
					return Equals((LegacyInputKeysGroup)obj);
				}

				return false;
			}

			/// <summary>
			/// Determines whether the specified object is equal to the current object.
			/// </summary>
			/// <param name="obj">The object to compare with the current object.</param>
			/// <returns>true if the specified object is equal to the current object; otherwise, false.</returns>
			public bool Equals(LegacyInputKeysGroup other)
			{
				return Keys == other.Keys;
			}

			/// <summary>
			/// Hash function.
			/// </summary>
			/// <returns>A hash code for the current object.</returns>
			public override int GetHashCode()
			{
				return Keys.GetHashCode();
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="color1">First instance.</param>
			/// <param name="color2">Second instance.</param>
			/// <returns>true if the instances are equal; otherwise, false.</returns>
			public static bool operator ==(LegacyInputKeysGroup a, LegacyInputKeysGroup b)
			{
				return a.Equals(b);
			}

			/// <summary>
			/// Compare specified instances.
			/// </summary>
			/// <param name="color1">First instance.</param>
			/// <param name="color2">Second instance.</param>
			/// <returns>true if the instances not equal; otherwise, false.</returns>
			public static bool operator !=(LegacyInputKeysGroup a, LegacyInputKeysGroup b)
			{
				return !a.Equals(b);
			}
		}

		static readonly LegacyInputKeysGroup[] HotKey2LegacyInput = new[] {
			new LegacyInputKeysGroup(KeyCode.None),

			new LegacyInputKeysGroup(KeyCode.Backspace),
			new LegacyInputKeysGroup(KeyCode.Tab),
			new LegacyInputKeysGroup(KeyCode.Return, KeyCode.KeypadEnter),
			new LegacyInputKeysGroup(KeyCode.Pause),
			new LegacyInputKeysGroup(KeyCode.Escape),
			new LegacyInputKeysGroup(KeyCode.Space),

			new LegacyInputKeysGroup(KeyCode.Quote),
			new LegacyInputKeysGroup(KeyCode.BackQuote),

			new LegacyInputKeysGroup(KeyCode.Comma),
			new LegacyInputKeysGroup(KeyCode.Period, KeyCode.KeypadPeriod),
			new LegacyInputKeysGroup(KeyCode.Semicolon),

			new LegacyInputKeysGroup(KeyCode.Minus, KeyCode.KeypadMinus),
			new LegacyInputKeysGroup(KeyCode.KeypadPlus),
			new LegacyInputKeysGroup(KeyCode.KeypadMultiply),
			new LegacyInputKeysGroup(KeyCode.Slash, KeyCode.KeypadDivide),
			new LegacyInputKeysGroup(KeyCode.Equals, KeyCode.KeypadEquals),

			new LegacyInputKeysGroup(KeyCode.Backslash),
			new LegacyInputKeysGroup(KeyCode.LeftBracket),
			new LegacyInputKeysGroup(KeyCode.RightBracket),

			new LegacyInputKeysGroup(KeyCode.Alpha0),
			new LegacyInputKeysGroup(KeyCode.Alpha1),
			new LegacyInputKeysGroup(KeyCode.Alpha2),
			new LegacyInputKeysGroup(KeyCode.Alpha3),
			new LegacyInputKeysGroup(KeyCode.Alpha4),
			new LegacyInputKeysGroup(KeyCode.Alpha5),
			new LegacyInputKeysGroup(KeyCode.Alpha6),
			new LegacyInputKeysGroup(KeyCode.Alpha7),
			new LegacyInputKeysGroup(KeyCode.Alpha8),
			new LegacyInputKeysGroup(KeyCode.Alpha9),

			new LegacyInputKeysGroup(KeyCode.A),
			new LegacyInputKeysGroup(KeyCode.B),
			new LegacyInputKeysGroup(KeyCode.C),
			new LegacyInputKeysGroup(KeyCode.D),
			new LegacyInputKeysGroup(KeyCode.E),
			new LegacyInputKeysGroup(KeyCode.F),
			new LegacyInputKeysGroup(KeyCode.G),
			new LegacyInputKeysGroup(KeyCode.H),
			new LegacyInputKeysGroup(KeyCode.I),
			new LegacyInputKeysGroup(KeyCode.J),
			new LegacyInputKeysGroup(KeyCode.K),
			new LegacyInputKeysGroup(KeyCode.L),
			new LegacyInputKeysGroup(KeyCode.M),
			new LegacyInputKeysGroup(KeyCode.N),
			new LegacyInputKeysGroup(KeyCode.O),
			new LegacyInputKeysGroup(KeyCode.P),
			new LegacyInputKeysGroup(KeyCode.Q),
			new LegacyInputKeysGroup(KeyCode.R),
			new LegacyInputKeysGroup(KeyCode.S),
			new LegacyInputKeysGroup(KeyCode.T),
			new LegacyInputKeysGroup(KeyCode.U),
			new LegacyInputKeysGroup(KeyCode.V),
			new LegacyInputKeysGroup(KeyCode.W),
			new LegacyInputKeysGroup(KeyCode.X),
			new LegacyInputKeysGroup(KeyCode.Y),
			new LegacyInputKeysGroup(KeyCode.Z),

			new LegacyInputKeysGroup(KeyCode.Keypad0),
			new LegacyInputKeysGroup(KeyCode.Keypad1),
			new LegacyInputKeysGroup(KeyCode.Keypad2),
			new LegacyInputKeysGroup(KeyCode.Keypad3),
			new LegacyInputKeysGroup(KeyCode.Keypad4),
			new LegacyInputKeysGroup(KeyCode.Keypad5),
			new LegacyInputKeysGroup(KeyCode.Keypad6),
			new LegacyInputKeysGroup(KeyCode.Keypad7),
			new LegacyInputKeysGroup(KeyCode.Keypad8),
			new LegacyInputKeysGroup(KeyCode.Keypad9),

			new LegacyInputKeysGroup(KeyCode.UpArrow),
			new LegacyInputKeysGroup(KeyCode.DownArrow),
			new LegacyInputKeysGroup(KeyCode.RightArrow),
			new LegacyInputKeysGroup(KeyCode.LeftArrow),

			new LegacyInputKeysGroup(KeyCode.Insert),
			new LegacyInputKeysGroup(KeyCode.Delete),
			new LegacyInputKeysGroup(KeyCode.Home),
			new LegacyInputKeysGroup(KeyCode.End),
			new LegacyInputKeysGroup(KeyCode.PageUp),
			new LegacyInputKeysGroup(KeyCode.PageDown),

			new LegacyInputKeysGroup(KeyCode.F1),
			new LegacyInputKeysGroup(KeyCode.F2),
			new LegacyInputKeysGroup(KeyCode.F3),
			new LegacyInputKeysGroup(KeyCode.F4),
			new LegacyInputKeysGroup(KeyCode.F5),
			new LegacyInputKeysGroup(KeyCode.F6),
			new LegacyInputKeysGroup(KeyCode.F7),
			new LegacyInputKeysGroup(KeyCode.F8),
			new LegacyInputKeysGroup(KeyCode.F9),
			new LegacyInputKeysGroup(KeyCode.F10),
			new LegacyInputKeysGroup(KeyCode.F11),
			new LegacyInputKeysGroup(KeyCode.F12),

			new LegacyInputKeysGroup(KeyCode.Print),

			new LegacyInputKeysGroup(KeyCode.Numlock),
			new LegacyInputKeysGroup(KeyCode.CapsLock),
			new LegacyInputKeysGroup(KeyCode.ScrollLock),
		};
#endif

#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// Get keys group.
		/// </summary>
		/// <param name="key">HotKey code.</param>
		/// <returns>Keys group</returns>
		public static InputSystemKeysGroup KeysGroup(HotKeyCode key)
		{
			return HotKey2InputSystem[(int)key];
		}
#endif

		/// <summary>
		/// Is hot key pressed?
		/// </summary>
		/// <param name="key">Key.</param>
		/// <returns>true if hot key pressed; otherwise false.</returns>
		public static bool IsPressed(HotKeyCode key)
		{
#if ENABLE_INPUT_SYSTEM
			return HotKey2InputSystem[(int)key].IsPressed;
#else
			return HotKey2LegacyInput[(int)key].IsPressed;
#endif
		}
	}
}