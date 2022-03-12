namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the ColorPicker.
	/// </summary>
	[Serializable]
	public class StyleColorPicker : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the palette toggle.
		/// </summary>
		[SerializeField]
		public StyleButton PaletteToggle;

		/// <summary>
		/// Style for the palette border.
		/// </summary>
		[SerializeField]
		public StyleImage PaletteBorder;

		/// <summary>
		/// Style for the palette cursor.
		/// </summary>
		[SerializeField]
		public StyleImage PaletteCursor;

		/// <summary>
		/// Style for the vertical slider handle.
		/// </summary>
		[SerializeField]
		public StyleImage SliderVerticalHandle;

		/// <summary>
		/// Style for the horizontal slider handle.
		/// </summary>
		[SerializeField]
		public StyleImage SliderHorizontalHandle;

		/// <summary>
		/// Style for the input toggle.
		/// </summary>
		[SerializeField]
		public StyleButton InputToggle;

		/// <summary>
		/// Style for the input channel label.
		/// </summary>
		[SerializeField]
		public StyleText InputChannelLabel;

		/// <summary>
		/// Style for the input spinner.
		/// </summary>
		[SerializeField]
		public StyleSpinner InputSpinner;

		/// <summary>
		/// Style for the hex input background.
		/// </summary>
		[SerializeField]
		public StyleImage HexInputBackground;

		/// <summary>
		/// Style for the hex input text.
		/// </summary>
		[SerializeField]
		public StyleText HexInputText;

		/// <summary>
		/// Style for the hex input placeholder.
		/// </summary>
		[SerializeField]
		public StyleText HexInputPlaceholder;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			PaletteToggle.SetDefaultValues();
			PaletteBorder.SetDefaultValues();
			PaletteCursor.SetDefaultValues();
			SliderVerticalHandle.SetDefaultValues();
			SliderHorizontalHandle.SetDefaultValues();
			InputToggle.SetDefaultValues();
			InputChannelLabel.SetDefaultValues();
			InputSpinner.SetDefaultValues();
			HexInputBackground.SetDefaultValues();
			HexInputText.SetDefaultValues();
			HexInputPlaceholder.SetDefaultValues();
		}
#endif
	}
}