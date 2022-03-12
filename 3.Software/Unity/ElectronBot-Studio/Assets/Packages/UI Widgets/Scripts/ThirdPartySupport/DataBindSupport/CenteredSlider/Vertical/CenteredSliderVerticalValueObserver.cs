#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Value of an CenteredSliderVertical.
	/// </summary>
	public class CenteredSliderVerticalValueObserver : ComponentDataObserver<UIWidgets.CenteredSliderVertical, int>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.CenteredSliderVertical target)
		{
			target.OnValueChanged.AddListener(OnValueChangedCenteredSliderVertical);
		}

		/// <inheritdoc />
		protected override int GetValue(UIWidgets.CenteredSliderVertical target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.CenteredSliderVertical target)
		{
			target.OnValueChanged.RemoveListener(OnValueChangedCenteredSliderVertical);
		}

		void OnValueChangedCenteredSliderVertical(int arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif