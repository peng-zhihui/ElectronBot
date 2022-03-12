#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the IsOn of an Switch.
	/// </summary>
	public class SwitchIsOnObserver : ComponentDataObserver<UIWidgets.Switch, bool>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.Switch target)
		{
			target.OnValueChanged.AddListener(OnValueChangedSwitch);
		}

		/// <inheritdoc />
		protected override bool GetValue(UIWidgets.Switch target)
		{
			return target.IsOn;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.Switch target)
		{
			target.OnValueChanged.RemoveListener(OnValueChangedSwitch);
		}

		void OnValueChangedSwitch(bool arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif