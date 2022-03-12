#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the ValueMin of a RangeSliderFloat depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] RangeSliderFloat ValueMin Setter")]
	public class RangeSliderFloatValueMinSetter : ComponentSingleSetter<UIWidgets.RangeSliderFloat, float>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.RangeSliderFloat target, float value)
		{
			target.ValueMin = value;
		}
	}
}
#endif