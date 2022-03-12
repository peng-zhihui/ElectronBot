namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Scale for the Slider widget.
	/// </summary>
	[RequireComponent(typeof(Slider))]
	public class SliderScale : SliderScaleBase
	{
		Slider slider;

		/// <summary>
		/// Slider.
		/// </summary>
		protected Slider Slider
		{
			get
			{
				if (slider == null)
				{
					slider = GetComponent<Slider>();
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
				Slider.onValueChanged.AddListener(OnSliderUpdate);
			}
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();

			if (slider != null)
			{
				slider.onValueChanged.AddListener(OnSliderUpdate);
			}
		}

		/// <summary>
		/// Process slider value changes.
		/// </summary>
		/// <param name="v">Value.</param>
		protected virtual void OnSliderUpdate(float v)
		{
			UpdateScale();
		}

		/// <inheritdoc/>
		public override void UpdateScale()
		{
			Scale.Set(Value2MarkDataDelegate, Slider.minValue, Slider.maxValue, Slider.value);
		}

		/// <inheritdoc/>
		protected override Vector2 Value2Anchor(float value)
		{
			var v = (value - Slider.minValue) / (Slider.maxValue - Slider.minValue);
			switch (Slider.direction)
			{
				case Slider.Direction.LeftToRight:
					return new Vector2(v, 0.5f);
				case Slider.Direction.RightToLeft:
					return new Vector2(1f - v, 0.5f);
				case Slider.Direction.TopToBottom:
					return new Vector2(0.5f, 1f - v);
				case Slider.Direction.BottomToTop:
					return new Vector2(0.5f, v);
				default:
					throw new NotSupportedException(string.Format("Unknown slider direction: {0}", EnumHelper<Slider.Direction>.ToString(Slider.direction)));
			}
		}
	}
}