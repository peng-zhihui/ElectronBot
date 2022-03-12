namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for the paginator.
	/// </summary>
	[Serializable]
	public class StylePaginator : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the default background.
		/// </summary>
		[SerializeField]
		public StyleImage DefaultBackground;

		/// <summary>
		/// Style for the default text.
		/// </summary>
		[SerializeField]
		public StyleText DefaultText;

		/// <summary>
		/// Style for the active background.
		/// </summary>
		[SerializeField]
		public StyleImage ActiveBackground;

		/// <summary>
		/// Style for the active text.
		/// </summary>
		[SerializeField]
		public StyleText ActiveText;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			DefaultBackground.SetDefaultValues();
			DefaultText.SetDefaultValues();
			ActiveBackground.SetDefaultValues();
			ActiveText.SetDefaultValues();
		}
#endif
	}
}