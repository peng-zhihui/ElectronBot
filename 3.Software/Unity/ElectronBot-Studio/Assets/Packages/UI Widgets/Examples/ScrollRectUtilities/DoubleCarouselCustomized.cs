namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Customized double carousel.
	/// </summary>
	public class DoubleCarouselCustomized : MonoBehaviour
	{
		/// <summary>
		/// Double carousel.
		/// </summary>
		[SerializeField]
		protected DoubleCarouselSample carousel;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Awake()
		{
			carousel.CustomSetDirectSlideState = DirectSlideState;
			carousel.CustomSetReverseSlideState = ReverseSlideState;
		}

		void DirectSlideState(DoubleCarouselCustomizedSlide slide, float ratio)
		{
			var scale = Mathf.Lerp(carousel.Scale, 1f, ratio);
			var scale_vector = new Vector3(scale, scale, scale);

			foreach (var label in slide.Labels)
			{
				label.localScale = scale_vector;
			}

			foreach (var graphic in slide.Graphics)
			{
				var color = graphic.color;
				color.a = 1f - ratio;
				graphic.color = color;
			}
		}

		void ReverseSlideState(DoubleCarouselCustomizedSlide slide, float ratio)
		{
			if (slide.BackgroundRectTransform != null)
			{
				var bg_scale = Mathf.Lerp(1f, carousel.Scale, ratio);
				slide.BackgroundRectTransform.localScale = new Vector3(bg_scale, bg_scale, bg_scale);
			}

			if (slide.BackgroundGraphic != null)
			{
				var color = slide.BackgroundGraphic.color;
				color.a = 1f - ratio;
				slide.BackgroundGraphic.color = color;
			}

			var scale = Mathf.Lerp(carousel.Scale, 1f, ratio);
			var scale_vector = new Vector3(scale, scale, scale);

			foreach (var label in slide.Labels)
			{
				label.localScale = scale_vector;
			}

			foreach (var graphic in slide.Graphics)
			{
				var color = graphic.color;
				color.a = 1f - ratio;
				graphic.color = color;
			}
		}
	}
}