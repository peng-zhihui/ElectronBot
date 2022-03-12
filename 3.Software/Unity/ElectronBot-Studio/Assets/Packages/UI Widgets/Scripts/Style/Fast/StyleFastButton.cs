namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Fast style for button.
	/// </summary>
	[Serializable]
	public class StyleFastButton : IStyleDefaultValues
	{
		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Border.
		/// </summary>
		[SerializeField]
		public StyleImage Border;

		/// <summary>
		/// Mask.
		/// </summary>
		[SerializeField]
		public StyleImage Mask;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Border.SetDefaultValues();
			Mask.SetDefaultValues();
		}
#endif
	}
}