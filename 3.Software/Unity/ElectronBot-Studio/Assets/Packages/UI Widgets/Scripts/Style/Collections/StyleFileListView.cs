namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the FileListView.
	/// </summary>
	[Serializable]
	public class StyleFileListView : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the toggle button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonToggle;

		/// <summary>
		/// Style for the up button.
		/// </summary>
		[SerializeField]
		public StyleImage ButtonUp;

		/// <summary>
		/// Style for the path item background.
		/// </summary>
		[SerializeField]
		public StyleImage PathItemBackground;

		/// <summary>
		/// Style for the path item text.
		/// </summary>
		[SerializeField]
		public StyleText PathItemText;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			ButtonToggle.SetDefaultValues();
			ButtonUp.SetDefaultValues();
			PathItemBackground.SetDefaultValues();
			PathItemText.SetDefaultValues();
		}
#endif
	}
}