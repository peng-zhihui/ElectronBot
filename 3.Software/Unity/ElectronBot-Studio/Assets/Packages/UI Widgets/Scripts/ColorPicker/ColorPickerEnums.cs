namespace UIWidgets
{
	/// <summary>
	/// Color picker input mode.
	/// </summary>
	public enum ColorPickerInputMode
	{
		/// <summary>
		/// Don't display sliders.
		/// </summary>
		None = -1,

		/// <summary>
		/// Display RGB sliders.
		/// </summary>
		RGB = 0,

		/// <summary>
		/// Display HSV sliders.
		/// </summary>
		HSV = 1,
	}

	/// <summary>
	/// Color picker palette mode.
	/// Specified value used in vertical slider, others used in palette.
	/// </summary>
	public enum ColorPickerPaletteMode
	{
		/// <summary>
		/// None.
		/// </summary>
		None = -1,

		/// <summary>
		/// Red.
		/// </summary>
		Red = 0,

		/// <summary>
		/// Green.
		/// </summary>
		Green = 1,

		/// <summary>
		/// Blue.
		/// </summary>
		Blue = 2,

		/// <summary>
		/// Hue.
		/// </summary>
		Hue = 3,

		/// <summary>
		/// Saturation.
		/// </summary>
		Saturation = 4,

		/// <summary>
		/// Value.
		/// </summary>
		Value = 5,

		/// <summary>
		/// HSV circle.
		/// </summary>
		HSVCircle = 7,

		/// <summary>
		/// Image.
		/// </summary>
		Image = 6,
	}
}