namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Scale for the CenteredSlider widget.
	/// </summary>
	[RequireComponent(typeof(CenteredSlider))]
	public class CenteredSliderScale : SliderScaleBase
	{
		[SerializeField]
		float negativeStepRate = 1f;

		/// <summary>
		/// Value format.
		/// </summary>
		public float NegativeStepRate
		{
			get
			{
				return negativeStepRate;
			}

			set
			{
				if (negativeStepRate != value)
				{
					negativeStepRate = value;
					UpdateScale();
				}
			}
		}

		[SerializeField]
		float positiveStepRate = 1f;

		/// <summary>
		/// Positive step rate.
		/// </summary>
		public float PositiveStepRate
		{
			get
			{
				return positiveStepRate;
			}

			set
			{
				if (positiveStepRate != value)
				{
					positiveStepRate = value;
					UpdateScale();
				}
			}
		}

		CenteredSlider slider;

		/// <summary>
		/// CenteredSlider.
		/// </summary>
		protected CenteredSlider Slider
		{
			get
			{
				if (slider == null)
				{
					slider = GetComponent<CenteredSlider>();
				}

				return slider;
			}
		}

		/// <inheritdoc/>
		public override void Init()
		{
			Scale.MarkValuesGenerator = Generator;

			base.Init();
		}

		/// <summary>
		/// Scale values generator.
		/// </summary>
		/// <param name="min">Min value.</param>
		/// <param name="max">Max value.</param>
		/// <param name="step">Step.</param>
		/// <param name="output">Output.</param>
		protected void Generator(float min, float max, float step, List<float> output)
		{
			var center = 0f;
			var step_min = step * NegativeStepRate;
			for (var v = center; v >= min; v -= step_min)
			{
				output.Add(v);
			}

			var step_max = step * PositiveStepRate;
			for (var v = center; v <= max; v += step_max)
			{
				output.Add(v);
			}
		}

		/// <inheritdoc/>
		protected override void AddListeners()
		{
			base.AddListeners();

			if (Slider != null)
			{
				Slider.OnValueChanged.AddListener(OnSliderUpdate);
			}
		}

		/// <inheritdoc/>
		protected override void RemoveListeners()
		{
			base.RemoveListeners();

			if (slider != null)
			{
				slider.OnValueChanged.AddListener(OnSliderUpdate);
			}
		}

		/// <summary>
		/// Process slider value changes.
		/// </summary>
		/// <param name="v">Value.</param>
		protected virtual void OnSliderUpdate(int v)
		{
			UpdateScale();
		}

		/// <inheritdoc/>
		public override void UpdateScale()
		{
			Scale.Set(Value2MarkDataDelegate, Slider.LimitMin, Slider.LimitMax, Slider.Value);
		}

		/// <inheritdoc/>
		protected override Vector2 Value2Anchor(float value)
		{
			float position = 0.5f;
			if (value < 0)
			{
				if (Slider.LimitMin != 0)
				{
					position -= (value / Slider.LimitMin) / 2f;
				}
			}
			else
			{
				if (Slider.LimitMax != 0)
				{
					position += (value / Slider.LimitMax) / 2f;
				}
			}

			return Slider.IsHorizontal()
				? new Vector2(position, 0.5f)
				: new Vector2(0.5f, position);
		}
	}
}