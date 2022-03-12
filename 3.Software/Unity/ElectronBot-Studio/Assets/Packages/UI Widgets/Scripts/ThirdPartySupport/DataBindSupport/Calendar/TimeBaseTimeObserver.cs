#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Time of an TimeBase.
	/// </summary>
	public class TimeBaseTimeObserver : ComponentDataObserver<UIWidgets.TimeBase, System.TimeSpan>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.TimeBase target)
		{
			target.OnTimeChanged.AddListener(OnTimeChangedTimeBase);
		}

		/// <inheritdoc />
		protected override System.TimeSpan GetValue(UIWidgets.TimeBase target)
		{
			return target.Time;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.TimeBase target)
		{
			target.OnTimeChanged.RemoveListener(OnTimeChangedTimeBase);
		}

		void OnTimeChangedTimeBase(System.TimeSpan arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif