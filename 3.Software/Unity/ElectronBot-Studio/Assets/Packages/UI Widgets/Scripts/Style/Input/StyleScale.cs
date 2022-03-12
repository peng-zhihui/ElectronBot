namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the Scale.
	/// </summary>
	[Serializable]
	public class StyleScale : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage MainLine;

		/// <summary>
		/// Style for the usable range.
		/// </summary>
		[SerializeField]
		public StyleImage MarkLine;

		/// <summary>
		/// Style for the fill.
		/// </summary>
		[SerializeField]
		public StyleText MarkLabel;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			MainLine.SetDefaultValues();
			MarkLine.SetDefaultValues();
			MarkLabel.SetDefaultValues();
		}
#endif
	}
}