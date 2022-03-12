#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value of a CircularSlider depending on the System.Int32 data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CircularSlider Value Setter")]
	public class CircularSliderValueSetter : ComponentSingleSetter<UIWidgets.CircularSlider, System.Int32>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CircularSlider target, System.Int32 value)
		{
			target.Value = value;
		}
	}
}
#endif