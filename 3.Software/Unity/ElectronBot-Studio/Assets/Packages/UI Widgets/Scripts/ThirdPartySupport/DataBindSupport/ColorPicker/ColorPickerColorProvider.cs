#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the Color of an ColorPicker.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ColorPicker Color Provider")]
	public class ColorPickerColorProvider : ComponentDataProvider<UIWidgets.ColorPicker, UnityEngine.Color>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ColorPicker target)
		{
			target.OnChange.AddListener(OnChangeColorPicker);
		}

		/// <inheritdoc />
		protected override UnityEngine.Color GetValue(UIWidgets.ColorPicker target)
		{
			return target.Color;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ColorPicker target)
		{
			target.OnChange.RemoveListener(OnChangeColorPicker);
		}

		void OnChangeColorPicker(UnityEngine.Color32 arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif