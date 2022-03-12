namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test DoubleCarousel resize.
	/// </summary>
	public class TestDoubleCarousel : MonoBehaviour
	{
		/// <summary>
		/// DoubleCarousel.
		/// </summary>
		[SerializeField]
		protected RectTransform DoubleCarousel;

		/// <summary>
		/// Test resize.
		/// </summary>
		public void TestResize()
		{
			var size = (DoubleCarousel.rect.width == 512)
				? new Vector2(300, 300)
				: new Vector2(512, 512);

			DoubleCarousel.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
			DoubleCarousel.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
		}
	}
}