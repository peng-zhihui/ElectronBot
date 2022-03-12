#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an CenteredSliderVertical.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] CenteredSliderVertical Value Provider")]
	public class CenteredSliderVerticalValueProvider : ComponentDataProvider<UIWidgets.CenteredSliderVertical, int>
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