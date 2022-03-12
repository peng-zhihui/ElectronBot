#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Date of an Calendar.
	/// </summary>
	public class CalendarDateObserver : ComponentDataObserver<UIWidgets.DateBase, System.DateTime>
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