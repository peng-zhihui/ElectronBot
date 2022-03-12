#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an CenteredSlider.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] CenteredSlider Value Provider")]
	public class CenteredSliderValueProvider : ComponentDataProvider<UIWidgets.CenteredSlider, int>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.CenteredSlider target)
		{
			target.OnValueChanged.AddListener(OnValueChangedCenteredSlider);
		}

		/// <inheritdoc />
		protected override int GetValue(UIWidgets.CenteredSlider target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.CenteredSlider target)
		{
			target.OnValueChanged.RemoveListener(OnValueChangedCenteredSlider);
		}

		void OnValueChangedCenteredSlider(int arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif