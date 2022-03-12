namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Fast style for scrollbar.
	/// </summary>
	[Serializable]
	public class StyleFastScrollbar : IStyleDefaultValues
	{
		/// <summary>
		/// Background.
		/// </summary>
		[SerializeField]
		public StyleImage Background;

		/// <summary>
		/// Handle.
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