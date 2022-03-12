namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Test ColorPicker.
	/// </summary>
	public class TestColorPicker : MonoBehaviour
	{
		/// <summary>
		/// ColorPicker.
		/// </summary>
		[SerializeField]
		public ColorPicker ColorPicker;

		/// <summary>
		/// Image.
		/// </summary>
		[SerializeField]
		public Image Image;

		/// <summary>
		/// Update color.
		/// </summary>
		public void UpdateColor()
		{
			if (Image == null)
			{
				return;
			}

			if (ColorPicker == null)
			{
				return;
			}

			Image.color = ColorPicker.Color;
		}

		/// <summary>
		/// Set color.
		/// </summary>
		public void SetColor()
		{
			ColorPicker.Color = Color.cyan;
		}

		/// <summary>
		/// Set another color.
		/// </summary>
		public void SetColor2()
		{
			ColorPicker.Color = new Color(106f / 255f, 213f / 255f, 35f / 255f);
		}

		/// <summary>
		/// Set another color.
		/// </summary>
		public void SetColor3()
		{
			ColorPicker.Color = new Color(241f / 255f, 220f / 255f, 192f / 255f);
		}

		/// <summary>
		/// Set color with alpha.
		/// </summary>
		public void SetColorAlpha()
		{
			ColorPicker.Color = new Color(241f / 255f, 220f / 255f, 192f / 255f, 128f / 255f);
		}
	}
}