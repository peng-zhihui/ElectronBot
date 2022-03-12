namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Interface for colorpicker blocks.
	/// </summary>
	public interface IColorpickerBlock
	{
		/// <summary>
		/// Gets or sets the input mode.
		/// </summary>
		/// <value>The input mode.</value>
		ColorPickerInputMode InputMode
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the palette mode.
		/// </summary>
		/// <value>The palette mode.</value>
		ColorPickerPaletteMode PaletteMode
		{
			get;
			set;
		}

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		/// <value>The on change RG.</value>
		ColorRGBChangedEvent OnChangeRGB
		{
			get;
		}

		/// <summary>
		/// OnChangeHSV event.
		/// </summary>
		/// <value>The on change HS.</value>
		ColorHSVChangedEvent OnChangeHSV
		{
			get;
		}

		/// <summary>
		/// OnChangeAlpha event.
		/// </summary>
		/// <value>The on change alpha.</value>
		ColorAlphaChangedEvent OnChangeAlpha
		{
			get;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		void Init();

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		void SetColor(Color32 color);

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		void SetColor(ColorHSV color);
	}
}