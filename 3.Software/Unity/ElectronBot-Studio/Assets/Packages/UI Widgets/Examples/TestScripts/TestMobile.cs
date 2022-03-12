namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Resize some elements for the mobile screens.
	/// </summary>
	public class TestMobile : MonoBehaviour
	{
		/// <summary>
		/// Resizables.
		/// </summary>
		[SerializeField]
		protected List<Resizable> Resizables = new List<Resizable>();

		/// <summary>
		/// Horizontal splitters.
		/// </summary>
		[SerializeField]
		protected List<LayoutElement> HorizontalSplitters = new List<LayoutElement>();

		/// <summary>
		/// Vertical splitters.
		/// </summary>
		[SerializeField]
		protected List<LayoutElement> VerticalSplitters = new List<LayoutElement>();

		/// <summary>
		/// Layouts.
		/// </summary>
		[SerializeField]
		protected List<EasyLayoutNS.EasyLayout> Layouts = new List<EasyLayoutNS.EasyLayout>();

		/// <summary>
		/// DefaultDPI.
		/// </summary>
		[SerializeField]
		protected float DefaultDPI = 96f;

		#if UNITY_ANDROID || UNITY_IOS
		/// <summary>
		/// Start this instance.
		/// </summary>
		protected void Start()
		{
			var ratio = Screen.dpi / DefaultDPI;

			foreach (var resizable in Resizables)
			{
				resizable.ActiveRegion *= ratio;
			}

			foreach (var splitter in HorizontalSplitters)
			{
				var width = Mathf.Max(splitter.minWidth, splitter.preferredWidth) * ratio;
				splitter.minWidth = width;
				splitter.preferredWidth = width;
			}

			foreach (var splitter in VerticalSplitters)
			{
				var height = Mathf.Max(splitter.minHeight, splitter.preferredHeight) * ratio;
				splitter.minHeight = height;
				splitter.preferredHeight = height;
			}

			foreach (var layout in Layouts)
			{
				var spacing = layout.Spacing;
				layout.Spacing = new Vector2(spacing.x, spacing.y * ratio * 2f);
			}
		}
		#endif
	}
}