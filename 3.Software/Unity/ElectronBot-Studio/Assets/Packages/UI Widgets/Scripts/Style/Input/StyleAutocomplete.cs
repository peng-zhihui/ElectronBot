namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the autocomplete.
	/// </summary>
	[Serializable]
	public class StyleAutocomplete : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the input field.
		/// </summary>
		[SerializeField]
		public StyleText InputField;

		/// <summary>
		/// Style for the placeholder.
		/// </summary>
		[SerializeField]
		public StyleText Placeholder;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			InputField.SetDefaultValues();
			Placeholder.SetDefaultValues();
		}
#endif
	}
}