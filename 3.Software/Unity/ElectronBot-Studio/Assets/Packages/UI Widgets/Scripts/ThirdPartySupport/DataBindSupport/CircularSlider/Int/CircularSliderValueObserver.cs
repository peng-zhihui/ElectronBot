#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Value of an CircularSlider.
	/// </summary>
	public class CircularSliderValueObserver : ComponentDataObserver<UIWidgets.CircularSlider, System.Int32>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.CircularSlider target)
		{

			target.OnChange.AddListener(OnChangeCircularSlider);
		}

		/// <inheritdoc />
		protected override System.Int32 GetValue(UIWidgets.CircularSlider target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.CircularSlider target)
		{

			target.OnChange.RemoveListener(OnChangeCircularSlider);
		}


		void OnChangeCircularSlider()
		{
			OnTargetValueChanged();
		}

	}
}
#endif