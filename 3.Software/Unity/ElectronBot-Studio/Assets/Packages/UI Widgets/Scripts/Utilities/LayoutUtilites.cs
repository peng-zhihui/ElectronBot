namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Layout utilities.
	/// </summary>
	[Obsolete("Renamed to LayoutUtilities.")]
	public static class LayoutUtilites
	{
		/// <summary>
		/// Updates the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		[Obsolete("Replaced with LayoutUtilities.UpdateLayout().")]
		public static void UpdateLayout(LayoutGroup layout)
		{
			LayoutUtilities.UpdateLayout(layout);
		}

		/// <summary>
		/// Updates the layouts for component and all children components.
		/// </summary>
		/// <param name="component">Component.</param>
		[Obsolete("Use LayoutRebuilder.ForceRebuildLayoutImmediate().")]
		public static void UpdateLayoutsRecursive(Component component)
		{
			LayoutUtilities.UpdateLayoutsRecursive(component);
		}

		/// <summary>
		/// Set padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		[Obsolete("Replaced with LayoutUtilities.SetPaddingLeft().")]
		public static void SetPaddingLeft(LayoutGroup layout, float size)
		{
			LayoutUtilities.SetPaddingLeft(layout, size);
		}

		/// <summary>
		/// Set padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		[Obsolete("Replaced with LayoutUtilities.SetPaddingRight().")]
		public static void SetPaddingRight(LayoutGroup layout, float size)
		{
			LayoutUtilities.SetPaddingRight(layout, size);
		}

		/// <summary>
		/// Set padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		[Obsolete("Replaced with LayoutUtilities.SetPaddingTop().")]
		public static void SetPaddingTop(LayoutGroup layout, float size)
		{
			LayoutUtilities.SetPaddingTop(layout, size);
		}

		/// <summary>
		/// Set padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		[Obsolete("Replaced with LayoutUtilities.SetPaddingBottom().")]
		public static void SetPaddingBottom(LayoutGroup layout, float size)
		{
			LayoutUtilities.SetPaddingBottom(layout, size);
		}

		/// <summary>
		/// Get padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		[Obsolete("Replaced with LayoutUtilities.GetPaddingLeft().")]
		public static float GetPaddingLeft(LayoutGroup layout)
		{
			return LayoutUtilities.GetPaddingLeft(layout);
		}

		/// <summary>
		/// Get padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		[Obsolete("Replaced with LayoutUtilities.GetPaddingRight().")]
		public static float GetPaddingRight(LayoutGroup layout)
		{
			return LayoutUtilities.GetPaddingRight(layout);
		}

		/// <summary>
		/// Get padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		[Obsolete("Replaced with LayoutUtilities.GetPaddingTop().")]
		public static float GetPaddingTop(LayoutGroup layout)
		{
			return LayoutUtilities.GetPaddingTop(layout);
		}

		/// <summary>
		/// Get padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		[Obsolete("Replaced with LayoutUtilities.GetPaddingBottom().")]
		public static float GetPaddingBottom(LayoutGroup layout)
		{
			return LayoutUtilities.GetPaddingBottom(layout);
		}

		/// <summary>
		/// Is target width under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target width under layout group or fitter control; otherwise false.</returns>
		[Obsolete("Replaced with LayoutUtilities.IsWidthControlled().")]
		public static bool IsWidthControlled(RectTransform target)
		{
			return LayoutUtilities.IsWidthControlled(target);
		}

		/// <summary>
		/// Is target height under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target height under layout group or fitter control; otherwise false.</returns>
		[Obsolete("Replaced with LayoutUtilities.IsHeightControlled().")]
		public static bool IsHeightControlled(RectTransform target)
		{
			return LayoutUtilities.IsHeightControlled(target);
		}
	}
}