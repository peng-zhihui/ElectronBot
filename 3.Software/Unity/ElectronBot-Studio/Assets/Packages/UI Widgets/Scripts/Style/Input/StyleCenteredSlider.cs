namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the centered slider.
	/// </summary>
	[Serializable]
	public class StyleCenteredSlider : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the usable range.
		/// </summary>
		[SerializeField]
		public StyleImage UsableRange;

		/// <summary>
		/// Style for the fill.
		/// </summary>
		[SerializeField]
		public StyleImage Fill;

		/// <summary>
		/// Style for the handle.
		/// </summary>
		[SerializeField]
		public StyleImage Handle;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			UsableRange.SetDefaultValues();
			Fill.SetDefaultValues();
			Handle.SetDefaultValues();
		}
#endif
	}
}