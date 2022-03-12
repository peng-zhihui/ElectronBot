#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Min of a SpinnerFloat depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SpinnerFloat Min Setter")]
	public class SpinnerFloatMinSetter : ComponentSingleSetter<UIWidgets.SpinnerFloat, float>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SpinnerFloat target, float value)
		{
			target.Min = value;
		}
	}
}
#endif