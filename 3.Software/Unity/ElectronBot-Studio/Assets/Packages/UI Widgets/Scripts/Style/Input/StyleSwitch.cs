namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the switch.
	/// </summary>
	[Serializable]
	public class StyleSwitch : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the border.
		/// </summary>
		[SerializeField]
		public StyleImage Border;

		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage BackgroundDefault;

		/// <summary>
		/// The color of the background for on state.
		/// </summary>
		[SerializeField]
		public Color BackgroundOnColor;

		/// <summary>
		/// The color of the background for off state.
		/// </summary>
		[SerializeField]
		public Color BackgroundOffColor;

		/// <summary>
		/// Style for the mark.
		/// </summary>
		[SerializeField]
		public StyleImage MarkDefault;

		/// <summary>
		/// The color of the mark for on state.
		/// </summary>
		[SerializeField]
		public Color MarkOnColor;

		/// <summary>
		/// The color of the mark for off state.
		/// </summary>
		[SerializeField]
		public Color MarkOffColor;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Border.SetDefaultValues();
			BackgroundDefault.SetDefaultValues();
			MarkDefault.SetDefaultValues();
		}
#endif
	}
}