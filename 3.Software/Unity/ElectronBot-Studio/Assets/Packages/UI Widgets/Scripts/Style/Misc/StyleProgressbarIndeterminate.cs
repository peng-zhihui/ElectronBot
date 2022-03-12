namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the indeterminate progress bar.
	/// </summary>
	[Serializable]
	public class StyleProgressbarIndeterminate : IStyleDefaultValues
	{
		/// <summary>
		/// Progress bar texture.
		/// </summary>
		[SerializeField]
		public StyleRawImage Texture;

		/// <summary>
		/// Progress bar mask.
		/// </summary>
		[SerializeField]
		public StyleImage Mask;

		/// <summary>
		/// Progress bar border.
		/// </summary>
		[SerializeField]
		public StyleImage Border;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Texture.SetDefaultValues();
			Mask.SetDefaultValues();
			Border.SetDefaultValues();
		}
#endif
	}
}