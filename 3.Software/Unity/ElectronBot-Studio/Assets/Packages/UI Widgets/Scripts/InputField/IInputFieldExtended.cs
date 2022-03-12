namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// Interface for InputField with exposed functions to validate input.
	/// </summary>
	public interface IInputFieldExtended : IInputFieldProxy
	{
		/// <summary>
		/// Function to validate input.
		/// </summary>
		InputField.OnValidateInput Validation
		{
			get;
			set;
		}

		/// <summary>
		/// Function to process changed value.
		/// </summary>
		UnityAction<string> ValueChanged
		{
			get;
			set;
		}

		/// <summary>
		/// Function to process end edit.
		/// </summary>
		UnityAction<string> ValueEndEdit
		{
			get;
			set;
		}

		/// <summary>
		/// Current value.
		/// </summary>
		string Value
		{
			get;
			set;
		}

		/// <summary>
		/// Start selection position.
		/// </summary>
		int SelectionStart
		{
			get;
		}

		/// <summary>
		/// End selection position.
		/// </summary>
		int SelectionEnd
		{
			get;
		}

		/// <summary>
		/// Set widget properties from specified style.
		/// </summary>
		/// <param name="styleSpinner">InputField style.</param>
		/// <param name="style">Style.</param>
		void SetStyle(StyleSpinner styleSpinner, Style style);

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleSpinner">InputField style.</param>
		/// <param name="style">Style.</param>
		void GetStyle(StyleSpinner styleSpinner, Style style);
	}
}