namespace UIWidgets
{
	/// <summary>
	/// Progressbar types.
	/// </summary>
	public enum ProgressbarTypes
	{
		/// <summary>
		/// Allow specify Max progress.
		/// </summary>
		Determinate = 0,

		/// <summary>
		/// Indeterminate.
		/// </summary>
		Indeterminate = 1,
	}

	/// <summary>
	/// Progressbar text types.
	/// </summary>
	public enum ProgressbarTextTypes
	{
		/// <summary>
		/// Don't show text.
		/// </summary>
		None = 0,

		/// <summary>
		/// Show progress with percent.
		/// </summary>
		Percent = 1,

		/// <summary>
		/// Show progress with range.
		/// </summary>
		Range = 2,
	}

	/// <summary>
	/// Progressbar direction.
	/// </summary>
	public enum ProgressbarDirection
	{
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Progressbar speed type.
	/// </summary>
	public enum ProgressbarSpeedType
	{
		/// <summary>
		/// Speed is time to change progress on 1.
		/// </summary>
		TimeToValueChangedOnOne = 0,

		/// <summary>
		/// Speed is time to change progress from 0 to Max. If value changed from 0 to Max/2 than animation takes speed/2 seconds.
		/// </summary>
		ConstantSpeed = 1,

		/// <summary>
		/// Speed is time to change progress from current value to new value.
		/// </summary>
		ConstantTime = 2,
	}
}