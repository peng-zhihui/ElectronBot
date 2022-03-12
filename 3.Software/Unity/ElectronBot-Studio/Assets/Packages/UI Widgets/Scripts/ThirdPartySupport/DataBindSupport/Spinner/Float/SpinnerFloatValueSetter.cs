#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Value of a SpinnerFloat depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SpinnerFloat Value Setter")]
	public class SpinnerFloatValueSetter : ComponentSingleSetter<UIWidgets.SpinnerFloat, float>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SpinnerFloat target, float value)
		{
			target.Value = value;
		}
	}
}
#endif