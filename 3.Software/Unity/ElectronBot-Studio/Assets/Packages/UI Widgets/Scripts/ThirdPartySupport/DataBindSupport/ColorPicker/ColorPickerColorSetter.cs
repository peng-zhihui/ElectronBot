#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Color of a ColorPicker depending on the UnityEngine.Color data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] ColorPicker Color Setter")]
	public class ColorPickerColorSetter : ComponentSingleSetter<UIWidgets.ColorPicker, UnityEngine.Color>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.ColorPicker target, UnityEngine.Color value)
		{
			target.Color = value;
		}
	}
}
#endif