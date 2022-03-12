namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Slide for the customized double carousel.
	/// </summary>
	public class DoubleCarouselCustomizedSlide : MonoBehaviour
	{
		/// <summary>
		/// Background RectTransform.
		/// </summary>
		[SerializeField]
		public RectTransform BackgroundRectTransform;

		/// <summary>
		/// Background Graphic.
		/// </summary>
		[SerializeField]
		public Graphic BackgroundGraphic;

		/// <summary>
		/// Graphics.
		/// </summary>
		[SerializeField]
		public Graphic[] Graphics;

		/// <summary>
		/// Labels.
		/// </summary>
		[SerializeField]
		public RectTransform[] Labels;
	}
}