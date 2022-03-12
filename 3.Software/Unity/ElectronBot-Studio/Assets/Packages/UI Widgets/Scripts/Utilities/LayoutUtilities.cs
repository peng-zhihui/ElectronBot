namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Layout utilities.
	/// </summary>
	public static class LayoutUtilities
	{
		/// <summary>
		/// Updates the layout.
		/// </summary>
		/// <param name="layout">Layout.</param>
		public static void UpdateLayout(LayoutGroup layout)
		{
			if (layout == null)
			{
				return;
			}

			layout.CalculateLayoutInputHorizontal();
			layout.SetLayoutHorizontal();
			layout.CalculateLayoutInputVertical();
			layout.SetLayoutVertical();
		}

		static List<LayoutGroup> layouts = new List<LayoutGroup>();

		/// <summary>
		/// Updates the layouts for component and all children components.
		/// </summary>
		/// <param name="component">Component.</param>
		[Obsolete("Use LayoutRebuilder.ForceRebuildLayoutImmediate().")]
		public static void UpdateLayoutsRecursive(Component component)
		{
			if (component == null)
			{
				return;
			}

			component.GetComponentsInChildren(layouts);
			layouts.Reverse();

			foreach (var l in layouts)
			{
				UpdateLayout(l);
			}

			layouts.Clear();
		}

		/// <summary>
		/// Set padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingLeft(LayoutGroup layout, float size)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				hv.padding.left = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				var p = el.PaddingInner;
				p.Left = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingRight(LayoutGroup layout, float size)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				hv.padding.right = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				var p = el.PaddingInner;
				p.Right = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingTop(LayoutGroup layout, float size)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				hv.padding.top = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				var p = el.PaddingInner;
				p.Top = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Set padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="size">New padding.</param>
		public static void SetPaddingBottom(LayoutGroup layout, float size)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				hv.padding.bottom = Mathf.RoundToInt(size);
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				var p = el.PaddingInner;
				p.Bottom = size;
				el.PaddingInner = p;
				LayoutRebuilder.MarkLayoutForRebuild(layout.transform as RectTransform);
				return;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding left.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingLeft(LayoutGroup layout)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				return hv.padding.left;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				return el.PaddingInner.Left;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding right.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingRight(LayoutGroup layout)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				return hv.padding.right;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				return el.PaddingInner.Right;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding top.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingTop(LayoutGroup layout)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				return hv.padding.top;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				return el.PaddingInner.Top;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Get padding bottom.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <returns>Padding.</returns>
		public static float GetPaddingBottom(LayoutGroup layout)
		{
			var hv = layout as HorizontalOrVerticalLayoutGroup;
			if (hv != null)
			{
				return hv.padding.bottom;
			}

			var el = layout as EasyLayout;
			if (el != null)
			{
				return el.PaddingInner.Bottom;
			}

			throw new ArgumentException("Unsupported layout type.");
		}

		/// <summary>
		/// Is target width under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target width under layout group or fitter control; otherwise false.</returns>
		public static bool IsWidthControlled(RectTransform target)
		{
			var ignorer = Compatibility.GetComponent<ILayoutIgnorer>(target);
			if (ignorer != null && ignorer.ignoreLayout)
			{
				return false;
			}

			var fitter = target.GetComponent<ContentSizeFitter>();
			if ((fitter != null) && (fitter.horizontalFit != ContentSizeFitter.FitMode.Unconstrained))
			{
				return true;
			}

			var parent = target.transform.parent as RectTransform;
			if (parent != null)
			{
				var layout_group = parent.GetComponent<LayoutGroup>();
				if (layout_group == null)
				{
					return false;
				}

				var g_layout_group = layout_group as GridLayoutGroup;
				if ((g_layout_group != null) && g_layout_group.enabled)
				{
					return true;
				}

				var hv_layout_group = layout_group as HorizontalOrVerticalLayoutGroup;
				if ((hv_layout_group != null) && hv_layout_group.enabled)
				{
					return Compatibility.GetLayoutChildControlWidth(hv_layout_group);
				}

				var e_layout_group = layout_group as EasyLayout;
				if ((e_layout_group != null) && e_layout_group.enabled)
				{
					return e_layout_group.ChildrenWidth != ChildrenSize.DoNothing;
				}
			}

			return false;
		}

		/// <summary>
		/// Is target height under layout group or fitter control?
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if target height under layout group or fitter control; otherwise false.</returns>
		public static bool IsHeightControlled(RectTransform target)
		{
			var ignorer = Compatibility.GetComponent<ILayoutIgnorer>(target);
			if ((ignorer != null) && ignorer.ignoreLayout)
			{
				return false;
			}

			var fitter = target.GetComponent<ContentSizeFitter>();
			if ((fitter != null) && (fitter.verticalFit != ContentSizeFitter.FitMode.Unconstrained))
			{
				return true;
			}

			var parent = target.transform.parent as RectTransform;
			if (parent != null)
			{
				var layout_group = parent.GetComponent<LayoutGroup>();
				if (layout_group == null)
				{
					return false;
				}

				var g_layout_group = layout_group as GridLayoutGroup;
				if ((g_layout_group != null) && g_layout_group.enabled)
				{
					return true;
				}

				var hv_layout_group = layout_group as HorizontalOrVerticalLayoutGroup;
				if ((hv_layout_group != null) && hv_layout_group.enabled)
				{
					return Compatibility.GetLayoutChildControlHeight(hv_layout_group);
				}

				var e_layout_group = layout_group as EasyLayout;
				if ((e_layout_group != null) && e_layout_group.enabled)
				{
					return e_layout_group.ChildrenHeight != ChildrenSize.DoNothing;
				}
			}

			return false;
		}
	}
}