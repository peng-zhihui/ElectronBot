namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test layout switcher.
	/// </summary>
	public class TestLayoutSwitcher : MonoBehaviour
	{
		/// <summary>
		/// Layout switcher.
		/// </summary>
		[SerializeField]
		public LayoutSwitcher Switcher;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			Switcher.LayoutSelector = Selector;
		}

		/// <summary>
		/// Example of the custom layout selector.
		/// </summary>
		/// <param name="layouts">Layouts list.</param>
		/// <param name="displaySize">Display size.</param>
		/// <param name="aspectRatio">Aspect ratio.</param>
		/// <returns>Layout.</returns>
		protected static UILayout Selector(List<UILayout> layouts, float displaySize, float aspectRatio)
		{
			var filtered = FilterLayouts(layouts, aspectRatio);
			if (aspectRatio < 1f)
			{
				var layout = SelectPortrait(filtered);
				if (layout != null)
				{
					return layout;
				}
			}

			return SelectLandscape(filtered, aspectRatio);
		}

		/// <summary>
		/// Select portrait layout.
		/// </summary>
		/// <param name="layouts">Layouts.</param>
		/// <returns>Layout.</returns>
		protected static UILayout SelectPortrait(List<UILayout> layouts)
		{
			if (layouts.Count == 0)
			{
				return null;
			}

			var result = layouts[0];

			for (int i = 1; i < layouts.Count; i++)
			{
				var layout = layouts[i];
				if (layout.AspectRatioFloat < result.AspectRatioFloat)
				{
					result = layout;
				}
			}

			return result;
		}

		/// <summary>
		/// Select landscape layout.
		/// </summary>
		/// <param name="layouts">Layouts.</param>
		/// <param name="aspectRatio">Aspect ratio.</param>
		/// <returns>Layout.</returns>
		protected static UILayout SelectLandscape(List<UILayout> layouts, float aspectRatio)
		{
			if (layouts.Count == 0)
			{
				return null;
			}

			var result = layouts[0];
			var result_value = Mathf.Abs(aspectRatio - result.AspectRatioFloat);

			for (int i = 1; i < layouts.Count; i++)
			{
				var layout = layouts[i];
				var layout_value = Mathf.Abs(aspectRatio - layout.AspectRatioFloat);
				if (layout_value < result_value)
				{
					result = layout;
					result_value = layout_value;
				}
			}

			return result;
		}

		/// <summary>
		/// Filter layouts.
		/// </summary>
		/// <param name="layouts">Layouts.</param>
		/// <param name="aspectRatio">Aspect ratio.</param>
		/// <returns>Filtered layouts.</returns>
		protected static List<UILayout> FilterLayouts(List<UILayout> layouts, float aspectRatio)
		{
			var result = new List<UILayout>(layouts.Count);
			foreach (var l in layouts)
			{
				if (l.AspectRatioFloat >= aspectRatio)
				{
					result.Add(l);
				}
			}

			return result;
		}

		/// <summary>
		/// Select 4:3 layout only for 4:3, in other cases use 16:9 layout.
		/// </summary>
		/// <param name="layouts">Layouts list.</param>
		/// <param name="displaySize">Display size.</param>
		/// <param name="aspectRatio">Aspect ratio.</param>
		/// <returns>Layout.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "displaySize", Justification = "Interface compatibility.")]
		protected static UILayout Selector43(List<UILayout> layouts, float displaySize, float aspectRatio)
		{
			// if aspect_ration 4:3
			if (Mathf.Approximately(4f / 3f, aspectRatio))
			{
				var layout_4_3_index = 0;
				return layouts[layout_4_3_index];
			}

			var layout_9_16_index = 1;
			return layouts[layout_9_16_index];
		}
	}
}