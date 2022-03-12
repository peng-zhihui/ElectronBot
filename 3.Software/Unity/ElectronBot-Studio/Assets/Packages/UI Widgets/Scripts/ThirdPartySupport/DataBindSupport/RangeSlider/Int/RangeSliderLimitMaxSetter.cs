#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the LimitMax of a RangeSlider depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSlider LimitMax Setter")]
	public class RangeSliderLimitMaxSetter : ComponentSingleSetter<UIWidgets.RangeSlider, int>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSlider target, int value)
		{
			target.LimitMax = value;
		}
	}
}
#endif