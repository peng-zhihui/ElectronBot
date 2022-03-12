namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the context menu.
	/// </summary>
	[Serializable]
	public class StyleContextMenu : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the main background.
		/// </summary>
		[SerializeField]
		public StyleImage MainBackground = new StyleImage();

		/// <summary>
		/// Style for the item background image.
		/// </summary>
		[SerializeField]
		public StyleImage ItemBackground = new StyleImage();

		/// <summary>
		/// Style for the background item selectable.
		/// </summary>
		[SerializeField]
		public StyleSelectable ItemBackgroundSelectable = new StyleSelectable();

		/// <summary>
		/// Style for the text.
		/// </summary>
		[SerializeField]
		public StyleText ItemText = new StyleText();

		/// <summary>
		/// Style for the text selectable.
		/// </summary>
		[SerializeField]
		public StyleSelectable ItemTextSelectable = new StyleSelectable();

		/// <summary>
		/// Style for the delimiter image.
		/// </summary>
		[SerializeField]
		public StyleImage DelimiterImage = new StyleImage();

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			MainBackground.SetDefaultValues();
			ItemBackground.SetDefaultValues();
			ItemText.SetDefaultValues();
			DelimiterImage.SetDefaultValues();

			ItemBackgroundSelectable.SetDefaultValues();
			ItemTextSelectable.SetDefaultValues();
		}
#endif
	}
}