namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the tabs.
	/// </summary>
	[Serializable]
	public class StyleTabs : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the default button.
		/// </summary>
		[SerializeField]
		public StyleButton DefaultButton;

		/// <summary>
		/// Style for the active button.
		/// </summary>
		[SerializeField]
		public StyleButton ActiveButton;

		/// <summary>
		/// Style for the content background.
		/// </summary>
		[SerializeField]
		public StyleImage ContentBackground;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			DefaultButton.SetDefaultValues();
			ActiveButton.SetDefaultValues();
			ContentBackground.SetDefaultValues();
		}
#endif
	}
}