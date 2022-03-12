namespace UIWidgets.Examples
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ListViewImages item.
	/// </summary>
	[Serializable]
	public class ListViewImagesItem
	{
		/// <summary>
		/// URL.
		/// </summary>
		[SerializeField]
		public string Url;

		[SerializeField]
		Texture2D texture;

		/// <summary>
		/// Texture.
		/// </summary>
		public Texture2D Texture
		{
			get
			{
				return texture;
			}

			set
			{
				texture = value;
				sprite = null;
			}
		}

		[NonSerialized]
		Sprite sprite;

		/// <summary>
		/// Sprite.
		/// </summary>
		public Sprite Sprite
		{
			get
			{
				if ((sprite == null) && (Texture != null))
				{
					sprite = Sprite.Create(Texture, new Rect(0, 0, Texture.width, Texture.height), new Vector2(0.5f, 0.5f));
				}

				return sprite;
			}
		}
	}
}