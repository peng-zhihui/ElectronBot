#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the LimitMin of a RangeSlider depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSlider LimitMin Setter")]
	public class RangeSliderLimitMinSetter : ComponentSingleSetter<UIWidgets.RangeSlider, int>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSlider target, int value)
		{
			target.LimitMin = value;
		}
	}
}
#endif