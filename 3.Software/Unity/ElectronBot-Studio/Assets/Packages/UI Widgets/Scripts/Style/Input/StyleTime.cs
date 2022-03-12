namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the time widget.
	/// </summary>
	[Serializable]
	public class StyleTime : IStyleDefaultValues
	{
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
		/// Style for the increase button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonIncrease;

		/// <summary>
		/// Style for the decrease button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonDecrease;

		/// <summary>
		/// Style for the AMPM background.
		/// </summary>
		[SerializeField]
		public StyleImage AMPMBackground;

		/// <summary>
		/// Style for the AMPM text.
		/// </summary>
		[SerializeField]
		public StyleText AMPMText;

		/// <summary>
		/// Style for the hour label.
		/// </summary>
		[SerializeField]
		public StyleText HourLabel;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			InputBackground.SetDefaultValues();
			InputText.SetDefaultValues();
			ButtonIncrease.SetDefaultValues();
			ButtonDecrease.SetDefaultValues();
			AMPMBackground.SetDefaultValues();
			AMPMText.SetDefaultValues();
			HourLabel.SetDefaultValues();
		}
#endif
	}
}