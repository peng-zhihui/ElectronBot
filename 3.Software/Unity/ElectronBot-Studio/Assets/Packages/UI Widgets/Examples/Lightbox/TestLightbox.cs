namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test Lightbox.
	/// </summary>
	public class TestLightbox : MonoBehaviour
	{
		/// <summary>
		/// Lightbox to display image.
		/// </summary>
		[SerializeField]
		public Lightbox Lightbox;

		/// <summary>
		/// Image to display.
		/// </summary>
		[SerializeField]
		public Sprite Image;

		/// <summary>
		/// Show lightbox with image.
		/// </summary>
		public void ShowLightbox()
		{
			Lightbox.Show(Image);
		}
	}
}