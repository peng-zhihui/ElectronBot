namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Circular Slider.
	/// </summary>
	public class CircularSlider : CircularSliderBase<int>
	{
		/// <summary>
		/// Value changed event.
		/// </summary>
		[Serializable]
		public class ValueChanged : UnityEvent<int>
		{
		}

		/// <summary>
		/// Value changed event.
		/// </summary>
		[SerializeField]
		public ValueChanged OnValueChanged = new ValueChanged();

		/// <inheritdoc/>
		protected override void InvokeValueChanged(int value)
		{
			base.InvokeValueChanged(value);

			OnValueChanged.Invoke(value);
		}

		/// <inheritdoc/>
		protected override int ClampValue(int v)
		{
			v = Mathf.Clamp(v, MinValue, MaxValue);
			if (Step != 0)
			{
				v = Mathf.RoundToInt((v - MinValue) / Step) * Step;
			}

			if (v > MaxValue)
			{
				v = MaxValue;
			}

			return v;
		}

		/// <inheritdoc/>
		public override int Angle2Value(float angle)
		{
			while (angle < 0f)
			{
				var n = Mathf.Ceil(-angle / 360f);
				angle += n * 360f;
			}

			var range = 360f / (MaxValue - MinValue);

			return Mathf.RoundToInt(angle / range);
		}

		/// <inheritdoc/>
		public override float Value2Angle(int value)
		{
			var range = 360f / (MaxValue - MinValue);

			return range * value;
		}
	}
}