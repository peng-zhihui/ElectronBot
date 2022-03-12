namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// RangeSlider scale.
	/// </summary>
	[RequireComponent(typeof(RangeSlider))]
	public class RangeSliderScale : RangeSliderScaleBase<int>
	{
		/// <inheritdoc/>
		public override void UpdateScale()
		{
			if (Slider.Type == RangeSliderType.DisableHandleOverlay)
			{
				Scale.Clear();
				Scale.gameObject.SetActive(false);
				return;
			}

			Scale.gameObject.SetActive(true);
			Scale.Set(Value2MarkDataDelegate, Slider.LimitMin, Slider.LimitMax, Slider.ValueMin, Slider.ValueMax);
		}
	}
}