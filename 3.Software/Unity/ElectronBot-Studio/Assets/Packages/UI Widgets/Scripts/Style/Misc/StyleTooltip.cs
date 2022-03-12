namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the tooltip.
	/// </summary>
	[Serializable]
	public class StyleTooltip : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Style for the text.
		/// </summary>
		[SerializeField]
		public StyleText Text;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			Background.SetDefaultValues();
			Text.SetDefaultValues();
		}
#endif
	}
}