namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// CircularSliderFloat.
	/// </summary>
	public class CircularSliderFloat : CircularSliderBase<float>
	{
		/// <summary>
		/// Value changed event.
		/// </summary>
		[Serializable]
		public class ValueChanged : UnityEvent<float>
		{
		}

		/// <summary>
		/// Value changed event.
		/// </summary>
		[SerializeField]
		public ValueChanged OnValueChanged = new ValueChanged();

		/// <inheritdoc/>
		protected override void InvokeValueChanged(float value)
		{
			base.InvokeValueChanged(value);

			OnValueChanged.Invoke(value);
		}

		/// <inheritdoc/>
		protected override float ClampValue(float v)
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
		public override float Angle2Value(float angle)
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
		public override float Value2Angle(float value)
		{
			var range = 360f / (MaxValue - MinValue);

			return range * value;
		}
	}
}