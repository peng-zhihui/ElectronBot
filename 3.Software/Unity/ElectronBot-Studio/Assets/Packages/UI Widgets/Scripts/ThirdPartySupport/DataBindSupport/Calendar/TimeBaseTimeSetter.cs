#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Time of a TimeBase depending on the System.TimeSpan data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TimeBase Time Setter")]
	public class TimeBaseTimeSetter : ComponentSingleSetter<UIWidgets.TimeBase, System.TimeSpan>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TimeBase target, System.TimeSpan value)
		{
			target.Time = value;
		}
	}
}
#endif