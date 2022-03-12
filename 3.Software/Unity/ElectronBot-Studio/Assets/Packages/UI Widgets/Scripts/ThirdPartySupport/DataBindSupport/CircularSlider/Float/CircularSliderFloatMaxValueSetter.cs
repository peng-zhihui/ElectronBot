#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the MaxValue of a CircularSliderFloat depending on the System.Single data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] CircularSliderFloat MaxValue Setter")]
	public class CircularSliderFloatMaxValueSetter : ComponentSingleSetter<UIWidgets.CircularSliderFloat, System.Single>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.CircularSliderFloat target, System.Single value)
		{
			target.MaxValue = value;
		}
	}
}
#endif