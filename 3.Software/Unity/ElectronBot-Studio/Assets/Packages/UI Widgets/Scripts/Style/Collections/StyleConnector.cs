namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Style for drop indicator.
	/// </summary>
	[Serializable]
	public class StyleConnector : IStyleDefaultValues
	{
		/// <summary>
		/// Style for the image.
		/// </summary>
		[SerializeField]
		public Sprite Sprite;

		/// <summary>
		/// Style for the image.
		/// </summary>
		[SerializeField]
		public Color Color = Color.white;

		/// <summary>
		/// Style for the image.
		/// </summary>
		[SerializeField]
		public Material Material;

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			// nothing required
		}
#endif
	}
}