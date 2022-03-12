namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the ScrollBlock.
	/// </summary>
	[Serializable]
	public class StyleScroller : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the highlight.
		/// </summary>
		[SerializeField]
		public StyleImage Highlight;

		/// <summary>
		/// Style for the current date.
		/// </summary>
		[SerializeField]
		public StyleText Text;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Highlight.SetDefaultValues();
			Text.SetDefaultValues();
		}
#endif
	}
}