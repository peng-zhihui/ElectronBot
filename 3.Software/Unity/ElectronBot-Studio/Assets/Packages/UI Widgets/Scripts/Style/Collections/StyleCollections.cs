namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the collections.
	/// </summary>
	[Serializable]
	public class StyleCollections : IStyleDefaultValues
	{
		/// <summary>
		/// The default color.
		/// </summary>
		[SerializeField]
		public Color DefaultColor = new Color32(255, 215, 115, 255);

		/// <summary>
		/// The default color of the background.
		/// </summary>
		[SerializeField]
		public Color DefaultBackgroundColor = new Color32(255, 255, 255, 255);

		/// <summary>
		/// The highlighted color.
		/// </summary>
		[SerializeField]
		public Color HighlightedColor = new Color32(0, 0, 0, 255);

		/// <summary>
		/// The highlighted color of the background.
		/// </summary>
		[SerializeField]
		public Color HighlightedBackgroundColor = new Color32(181, 122, 36, 255);

		/// <summary>
		/// The selected color.
		/// </summary>
		[SerializeField]
		public Color SelectedColor = new Color32(0, 0, 0, 255);

		/// <summary>
		/// The selected color of the background.
		/// </summary>
		[SerializeField]
		public Color SelectedBackgroundColor = new Color32(196, 156, 39, 255);

		/// <summary>
		/// The disabled color.
		/// </summary>
		[SerializeField]
		public Color DisabledColor = new Color32(200, 200, 200, 255);

		/// <summary>
		/// The fade duration.
		/// </summary>
		[SerializeField]
		public float FadeDuration = 0.3f;

		/// <summary>
		/// Style for the main background.
		/// </summary>
		[SerializeField]
		public StyleImage MainBackground;

		/// <summary>
		/// Style for the viewport.
		/// </summary>
		[SerializeField]
		public StyleImage Viewport;

		/// <summary>
		/// Style for the default item background.
		/// </summary>
		[SerializeField]
		public StyleImage DefaultItemBackground;

		/// <summary>
		/// Style for the default item text.
		/// </summary>
		[SerializeField]
		public StyleText DefaultItemText;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			MainBackground.SetDefaultValues();
			Viewport.SetDefaultValues();
			DefaultItemBackground.SetDefaultValues();
			DefaultItemText.SetDefaultValues();
		}
#endif
	}
}