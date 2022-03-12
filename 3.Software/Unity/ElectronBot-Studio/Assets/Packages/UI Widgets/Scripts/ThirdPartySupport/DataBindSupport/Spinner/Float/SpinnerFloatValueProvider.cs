#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an SpinnerFloat.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] SpinnerFloat Value Provider")]
	public class SpinnerFloatValueProvider : ComponentDataProvider<UIWidgets.SpinnerFloat, float>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.SpinnerFloat target)
		{
			target.onValueChangeFloat.AddListener(OnValueChangeFloatSpinnerFloat);
		}

		/// <inheritdoc />
		protected override float GetValue(UIWidgets.SpinnerFloat target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.SpinnerFloat target)
		{
			target.onValueChangeFloat.RemoveListener(OnValueChangeFloatSpinnerFloat);
		}

		void OnValueChangeFloatSpinnerFloat(float arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif