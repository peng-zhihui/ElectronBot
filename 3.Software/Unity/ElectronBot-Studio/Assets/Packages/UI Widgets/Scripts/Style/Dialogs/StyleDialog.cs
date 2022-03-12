namespace UIWidgets.Styles
{
	using System;

	/// <summary>
	/// Style for the dialog.
	/// </summary>
	[Serializable]
	public class StyleDialog : IStyleDefaultValues
	{
		/// <summary>
		/// The background.
		/// </summary>
		public StyleImage Background;

		/// <summary>
		/// The title.
		/// </summary>
		public StyleText Title;

		/// <summary>
		/// The content background.
		/// </summary>
		public StyleImage ContentBackground;

		/// <summary>
		/// The content text.
		/// </summary>
		public StyleText ContentText;

		/// <summary>
		/// The delimiter.
		/// </summary>
		public StyleImage Delimiter;

		/// <summary>
		/// The button.
		/// </summary>
		public StyleButton Button;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Title.SetDefaultValues();

			ContentBackground.SetDefaultValues();
			ContentText.SetDefaultValues();

			Delimiter.SetDefaultValues();
			Button.SetDefaultValues();
		}
#endif
	}
}