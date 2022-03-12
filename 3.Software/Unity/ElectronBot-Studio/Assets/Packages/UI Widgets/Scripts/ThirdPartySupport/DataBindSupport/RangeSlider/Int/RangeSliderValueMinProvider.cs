#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the ValueMin of an RangeSlider.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] RangeSlider ValueMin Provider")]
	public class RangeSliderValueMinProvider : ComponentDataProvider<UIWidgets.RangeSlider, int>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.RangeSlider target)
		{
			target.OnValuesChanged.AddListener(OnValuesChangedRangeSlider);
		}

		/// <inheritdoc />
		protected override int GetValue(UIWidgets.RangeSlider target)
		{
			return target.ValueMin;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.RangeSlider target)
		{
			target.OnValuesChanged.RemoveListener(OnValuesChangedRangeSlider);
		}

		void OnValuesChangedRangeSlider(int arg0, int arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif