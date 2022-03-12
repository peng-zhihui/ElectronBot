#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an Spinner.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] Spinner Value Provider")]
	public class SpinnerValueProvider : ComponentDataProvider<UIWidgets.Spinner, int>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.Spinner target)
		{
			target.onValueChangeInt.AddListener(OnValueChangeIntSpinner);
		}

		/// <inheritdoc />
		protected override int GetValue(UIWidgets.Spinner target)
		{
			return target.Value;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.Spinner target)
		{
			target.onValueChangeInt.RemoveListener(OnValueChangeIntSpinner);
		}

		void OnValueChangeIntSpinner(int arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif