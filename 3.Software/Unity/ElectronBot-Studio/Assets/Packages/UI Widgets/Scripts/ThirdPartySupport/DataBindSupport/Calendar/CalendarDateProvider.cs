#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Date of an Calendar.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] Calendar Date Provider")]
	public class CalendarDateProvider : ComponentDataProvider<UIWidgets.DateBase, System.DateTime>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.DateBase target)
		{
			target.OnDateChanged.AddListener(OnDateChangedCalendar);
		}

		/// <inheritdoc />
		protected override System.DateTime GetValue(UIWidgets.DateBase target)
		{
			return target.Date;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.DateBase target)
		{
			target.OnDateChanged.RemoveListener(OnDateChangedCalendar);
		}

		void OnDateChangedCalendar(System.DateTime arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif