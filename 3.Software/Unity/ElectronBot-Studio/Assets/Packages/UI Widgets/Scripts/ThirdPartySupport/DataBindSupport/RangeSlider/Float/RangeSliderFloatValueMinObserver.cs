#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the ValueMin of an RangeSliderFloat.
	/// </summary>
	public class RangeSliderFloatValueMinObserver : ComponentDataObserver<UIWidgets.RangeSliderFloat, float>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.RangeSliderFloat target)
		{
			target.OnValuesChanged.AddListener(OnValuesChangedRangeSliderFloat);
		}

		/// <inheritdoc />
		protected override float GetValue(UIWidgets.RangeSliderFloat target)
		{
			return target.ValueMin;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.RangeSliderFloat target)
		{
			target.OnValuesChanged.RemoveListener(OnValuesChangedRangeSliderFloat);
		}

		void OnValuesChangedRangeSliderFloat(float arg0, float arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif