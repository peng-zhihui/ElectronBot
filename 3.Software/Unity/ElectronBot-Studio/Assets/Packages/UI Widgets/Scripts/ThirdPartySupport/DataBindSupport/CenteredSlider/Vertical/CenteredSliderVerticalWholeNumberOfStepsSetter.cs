#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the WholeNumberOfSteps of a CenteredSliderVertical depending on the System.Boolean data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CenteredSliderVertical WholeNumberOfSteps Setter")]
	public class CenteredSliderVerticalWholeNumberOfStepsSetter : ComponentSingleSetter<UIWidgets.CenteredSliderVertical, bool>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CenteredSliderVertical target, bool value)
		{
			target.WholeNumberOfSteps = value;
		}
	}
}
#endif