namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the ColorPickerRange.
	/// </summary>
	[Serializable]
	public class StyleColorPickerRange : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

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
			Handle.SetDefaultValues();
		}
#endif
	}
}