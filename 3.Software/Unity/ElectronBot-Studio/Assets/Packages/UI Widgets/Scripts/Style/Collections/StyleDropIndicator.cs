namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for drop indicator.
	/// </summary>
	[Serializable]
	public class StyleDropIndicator : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the image.
		/// </summary>
		[SerializeField]
		public StyleImage Image = new StyleImage();

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Image.SetDefaultValues();
		}
#endif
	}
}