#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Max of a SpinnerFloat depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] SpinnerFloat Max Setter")]
	public class SpinnerFloatMaxSetter : ComponentSingleSetter<UIWidgets.SpinnerFloat, float>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.SpinnerFloat target, float value)
		{
			target.Max = value;
		}
	}
}
#endif