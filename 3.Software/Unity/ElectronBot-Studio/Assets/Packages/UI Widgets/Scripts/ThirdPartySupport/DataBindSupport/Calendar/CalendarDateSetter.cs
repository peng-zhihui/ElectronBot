#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Date of a Calendar depending on the System.DateTime data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] Calendar Date Setter")]
	public class CalendarDateSetter : ComponentSingleSetter<UIWidgets.DateBase, System.DateTime>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.DateBase target, System.DateTime value)
		{
			target.Date = value;
		}
	}
}
#endif