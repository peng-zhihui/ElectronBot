#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the Value of an CircularSliderFloat.
	/// </summary>
	public class CircularSliderFloatValueObserver : ComponentDataObserver<UIWidgets.CircularSliderFloat, System.Single>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.CircularSliderFloat target)
		{

			target.OnChange.AddListener(OnChangeCircularSliderFloat);
		}

		/// <inheritdoc />
		protected override System.Single GetValue(UIWidgets.CircularSliderFloat target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.CircularSliderFloat target)
		{

			target.OnChange.RemoveListener(OnChangeCircularSliderFloat);
		}


		void OnChangeCircularSliderFloat()
		{
			OnTargetValueChanged();
		}

	}
}
#endif