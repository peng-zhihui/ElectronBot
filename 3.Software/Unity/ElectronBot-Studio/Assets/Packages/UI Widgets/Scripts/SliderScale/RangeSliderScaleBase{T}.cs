namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Base class for the RangeSlider scale.
	/// </summary>
	/// <typeparam name="T">RangeSlider value type.</typeparam>
	public abstract class RangeSliderScaleBase<T> : SliderScaleBase
		where T : struct
	{
		RangeSliderBase<T> slider;

		/// <summary>
		/// Slider.
		/// </summary>
		protected RangeSliderBase<T> Slider
		{
			get
			{
				if (slider == null)
				{
					slider = GetComponent<RangeSliderBase<T>>();
				}

				return slider;
			}
		}

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();

			if (Slider != null)
			{
				Slider.OnChange.AddListener(UpdateScale);
			}
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();

			if (Slider != null)
			{
				Slider.OnChange.RemoveListener(UpdateScale);
			}
		}

		/// <inheritdoc/>
		protected override Vector2 Value2Anchor(float value)
		{
			var v = Slider.Value01(value);

			return Slider.IsHorizontal()
				? new Vector2(v, 0.5f)
				: new Vector2(0.5f, v);
		}
	}
}