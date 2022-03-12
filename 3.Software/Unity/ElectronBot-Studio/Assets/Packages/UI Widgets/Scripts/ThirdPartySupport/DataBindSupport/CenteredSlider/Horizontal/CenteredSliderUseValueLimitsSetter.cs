#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the UseValueLimits of a CenteredSlider depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CenteredSlider UseValueLimits Setter")]
	public class CenteredSliderUseValueLimitsSetter : ComponentSingleSetter<UIWidgets.CenteredSlider, bool>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSlider target, bool value)
		{
			target.UseValueLimits = value;
		}
	}
}
#endif