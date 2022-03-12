namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Wrapper for the Cursor class.
	/// Required to support multiple behavior components on the same GameObject (like Resizable and Rotatable).
	/// </summary>
	public static class UICursor
	{
		/// <summary>
		/// Allow to replace cursor with another version.
		/// Use to replace default cursors with Hi-DPI version.
		/// </summary>
		[Obsolete("Replaced with CursorSelector.")]
		public static Func<Cursors.Cursor, Cursors.Cursor> Replacement = null;

		/// <summary>
		/// No replacement.
		/// </summary>
		/// <param name="cursor">Cursor.</param>
		/// <returns>New cursor.</returns>
		[Obsolete("No more used.")]
		public static Cursors.Cursor NoReplacement(Cursors.Cursor cursor)
		{
			return cursor;
		}

		/// <summary>
		/// Default cursor.
		/// </summary>
		[Obsolete("Replaced with Cursors.Default.")]
		public static Texture2D DefaultCursor;

		/// <summary>
		/// Default cursor hot spot.
		/// </summary>
		[Obsolete("Replaced with Cursors.Default.")]
		public static Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// Default cursor.
		/// </summary>
		[Obsolete("Replaced with Cursors.Default.")]
		public static Cursors.Cursor Default;

		static bool cursorsWarningDisplayed;

		static Cursors cursors;

		/// <summary>
		/// Cursors.
		/// </summary>
		public static Cursors Cursors
		{
			get
			{
				if ((cursors == null) && !cursorsWarningDisplayed)
				{
					Debug.LogWarning("Cursors are not specified. Please specify cursors using the unique CursorsDPISelector component or with a field at component.");
					cursorsWarningDisplayed = true;
				}

				return cursors;
			}

			set
			{
				cursors = value;
			}
		}

		/// <summary>
		/// Has cursors.
		/// </summary>
		public static bool HasCursors
		{
			get
			{
				return cursors != null;
			}
		}

		/// <summary>
		/// Current cursor owner.
		/// </summary>
		static Component currentOwner;

		/// <summary>
		/// Current cursor.
		/// </summary>
		static Cursors.Cursor Current;

		/// <summary>
		/// Cursor mode.
		/// </summary>
		public static CursorMode Mode =
#if UNITY_WEBGL
			CursorMode.ForceSoftware;
#else
			CursorMode.Auto;
#endif

		/// <summary>
		/// Can the specified component set cursor?
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public static Func<Component, bool> CanSet = DefaultCanSet;

		/// <summary>
		/// Set cursor.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public static Action<Component, Cursors.Cursor> Set = DefaultSet;

		/// <summary>
		/// Reset cursor.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required.")]
		public static Action<Component> Reset = DefaultReset;

		#if UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void Init()
		{
			Cursors = null;
			cursorsWarningDisplayed = false;
			currentOwner = null;

			Mode =
#if UNITY_WEBGL
			CursorMode.ForceSoftware;
#else
			CursorMode.Auto;
#endif

			CanSet = DefaultCanSet;
			Set = DefaultSet;
			Reset = DefaultReset;
		}
		#endif

		/// <summary>
		/// Can the specified component set cursor?
		/// </summary>
		/// <param name="owner">Component.</param>
		/// <returns>true if component can set cursor; otherwise false.</returns>
		public static bool DefaultCanSet(Component owner)
		{
			if (owner == null)
			{
				throw new ArgumentNullException("owner");
			}

			if (currentOwner == null)
			{
				return true;
			}

			return owner == currentOwner;
		}

		/// <summary>
		/// Set cursor.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="texture">Cursor texture.</param>
		/// <param name="hotspot">Cursor hot spot.</param>
		[Obsolete("Replaced with Set(Component owner, Cursors.Cursor cursor).")]
		public static void DefaultSet(Component owner, Texture2D texture, Vector2 hotspot)
		{
			if (!CanSet(owner))
			{
				return;
			}

			currentOwner = owner;
			SetCursor(new Cursors.Cursor(texture, hotspot));
		}

		/// <summary>
		/// Set cursor.
		/// </summary>
		/// <param name="owner">Owner.</param>
		/// <param name="cursor">Cursor.</param>
		public static void DefaultSet(Component owner, Cursors.Cursor cursor)
		{
			if (!CanSet(owner))
			{
				return;
			}

			currentOwner = owner;
			SetCursor(cursor);
		}

		static void SetCursor(Cursors.Cursor cursor)
		{
			if (Current == cursor)
			{
				return;
			}

			Current = cursor;

			#pragma warning disable 0618
			var actual = (Replacement != null) ? Replacement(Current) : Current;
			#pragma warning restore 0618

			Cursor.SetCursor(actual.Texture, actual.Hotspot, Mode);
		}

		/// <summary>
		/// Reset cursor.
		/// </summary>
		/// <param name="owner">Owner.</param>
		public static void DefaultReset(Component owner)
		{
			if (!CanSet(owner))
			{
				return;
			}

			currentOwner = null;
			SetCursor(Cursors != null ? Cursors.Default : default(Cursors.Cursor));
		}

		/// <summary>
		/// Show obsolete warning,
		/// </summary>
		public static void ObsoleteWarning()
		{
			Debug.LogWarning("Cursors texture and hot spot fields are obsolete and replaced with UICursorSettings component. Set DefaultCursorTexture to null to disable warning or reset component.");
		}
	}
}