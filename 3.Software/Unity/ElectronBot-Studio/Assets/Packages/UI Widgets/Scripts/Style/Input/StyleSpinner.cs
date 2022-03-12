namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the spinner.
	/// </summary>
	[Serializable]
	public class StyleSpinner : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the input background.
		/// </summary>
		[SerializeField]
		public StyleImage InputBackground;

		/// <summary>
		/// Style for the input text.
		/// </summary>
		[SerializeField]
		public StyleText InputText;

		/// <summary>
		/// Style for the input placeholder.
		/// </summary>
		[SerializeField]
		public StyleText InputPlaceholder;

		/// <summary>
		/// Style for the plus button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonPlus;

		/// <summary>
		/// Style for the minus button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonMinus;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			InputBackground.SetDefaultValues();
			InputText.SetDefaultValues();
			InputPlaceholder.SetDefaultValues();
			ButtonPlus.SetDefaultValues();
			ButtonMinus.SetDefaultValues();
		}
#endif
	}
}