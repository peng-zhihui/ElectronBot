namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Cursors selector by DPI.
	/// </summary>
	[ExecuteInEditMode]
	public class CursorsDPISelector : MonoBehaviour, IUpdatable
	{
		/// <summary>
		/// Cursors for the specified DPI.
		/// </summary>
		[Serializable]
		public class CursorsDPI
		{
			/// <summary>
			/// Cursors.
			/// </summary>
			[SerializeField]
			public Cursors Cursors;

			/// <summary>
			/// DPI.
			/// </summary>
			[SerializeField]
			public float DPI;
		}

		/// <summary>
		/// Default cursors.
		/// </summary>
		[SerializeField]
		public Cursors DefaultCursors;

		/// <summary>
		/// Cursors per DPI.
		/// </summary>
		[SerializeField]
		public List<CursorsDPI> CursorsPerDPI = new List<CursorsDPI>();

		/// <summary>
		/// Current DPI.
		/// </summary>
		protected float currentDPI;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			DPIChanged(Screen.dpi);
		}

		/// <summary>
		/// DPI changed.
		/// </summary>
		/// <param name="dpi">DPI.</param>
		protected virtual void DPIChanged(float dpi)
		{
			var current = DefaultCursors;
			var current_diff = float.MaxValue;

			foreach (var c in CursorsPerDPI)
			{
				var diff = Mathf.Abs(c.DPI - dpi);
				if (diff < current_diff)
				{
					current = c.Cursors;
					diff = current_diff;
				}
			}

			currentDPI = dpi;
			UICursor.Cursors = current;
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (Application.isPlaying)
			{
				Updater.Add(this);
			}
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			if (Application.isPlaying)
			{
				Updater.Remove(this);
			}
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			UICursor.Cursors = null;
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (currentDPI != Screen.dpi)
			{
				DPIChanged(Screen.dpi);
			}
		}

		/// <summary>
		/// Require.
		/// </summary>
		/// <param name="original">A component that requires cursors.</param>
		public static void Require(Component original)
		{
			#if UNITY_EDITOR
			if (UICursor.HasCursors)
			{
				return;
			}

			#if UNITY_2020_3_OR_NEWER
			var obj = FindObjectOfType<CursorsDPISelector>(true);
			#else
			var obj = FindObjectOfType<CursorsDPISelector>();
			#endif
			if (obj != null)
			{
				return;
			}

			var canvas = UtilitiesUI.FindTopmostCanvas(original.transform);
			if (canvas == null)
			{
				return;
			}

			if (canvas.GetComponent<CursorsDPISelector>() != null)
			{
				return;
			}

			UnityEditor.Undo.AddComponent<CursorsDPISelector>(canvas.gameObject);
			UnityEditor.EditorUtility.SetDirty(canvas);
			#endif
		}
	}
}