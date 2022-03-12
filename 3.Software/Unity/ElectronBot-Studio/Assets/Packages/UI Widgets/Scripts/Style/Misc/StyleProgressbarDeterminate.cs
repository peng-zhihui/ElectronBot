namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the determinate progressbar.
	/// </summary>
	[Serializable]
	public class StyleProgressbarDeterminate : IStyleDefaultValues
	{
		/// <summary>
		/// FullBar texture.
		/// </summary>
		[SerializeField]
		public StyleImage FullbarImage;

		/// <summary>
		/// FullBar mask.
		/// </summary>
		[SerializeField]
		public StyleImage FullbarMask;

		/// <summary>
		/// FullBar border.
		/// </summary>
		[SerializeField]
		public StyleImage FullbarBorder;

		/// <summary>
		/// Empty bar.
		/// </summary>
		[SerializeField]
		public StyleImage EmptyBar;

		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Empty bar text.
		/// </summary>
		[SerializeField]
		public StyleText EmptyBarText;

		/// <summary>
		/// Full bar text.
		/// </summary>
		[SerializeField]
		public StyleText FullBarText;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			FullbarImage.SetDefaultValues();
			FullbarMask.SetDefaultValues();
			FullbarBorder.SetDefaultValues();
			EmptyBar.SetDefaultValues();
			Background.SetDefaultValues();

			EmptyBarText.SetDefaultValues();
			FullBarText.SetDefaultValues();
		}
#endif
	}
}