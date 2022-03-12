#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Value of an CircularSliderFloat.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] CircularSliderFloat Value Provider")]
	public class CircularSliderFloatValueProvider : ComponentDataProvider<UIWidgets.CircularSliderFloat, System.Single>
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