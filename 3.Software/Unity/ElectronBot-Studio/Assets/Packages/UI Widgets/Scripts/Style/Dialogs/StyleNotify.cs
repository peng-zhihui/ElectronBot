namespace UIWidgets.Styles
{
	using System;

	/// <summary>
	/// Style for the notification.
	/// </summary>
	[Serializable]
	public class StyleNotify : IStyleDefaultValues
	{
		/// <summary>
		/// The background.
		/// </summary>
		public StyleImage Background;

		/// <summary>
		/// The text.
		/// </summary>
		public StyleText Text;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Text.SetDefaultValues();
		}
#endif
	}
}