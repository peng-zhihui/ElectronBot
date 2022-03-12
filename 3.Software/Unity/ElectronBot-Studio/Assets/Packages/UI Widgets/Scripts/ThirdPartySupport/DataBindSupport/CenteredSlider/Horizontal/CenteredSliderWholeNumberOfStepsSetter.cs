#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the WholeNumberOfSteps of a CenteredSlider depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CenteredSlider WholeNumberOfSteps Setter")]
	public class CenteredSliderWholeNumberOfStepsSetter : ComponentSingleSetter<UIWidgets.CenteredSlider, bool>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSlider target, bool value)
		{
			target.WholeNumberOfSteps = value;
		}
	}
}
#endif