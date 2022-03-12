namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Centered slider (zero at center, positive and negative parts have different scales).
	/// </summary>
	public class CenteredSlider : CenteredSliderBase<int>
	{
		/// <summary>
		/// Value to position.
		/// </summary>
		/// <returns>Position.</returns>
		/// <param name="value">Value.</param>
		protected override float ValueToPosition(int value)
		{
			value = InBounds(value);
			var center = RangeSize() / 2f;
			if (value == 0)
			{
				return center + GetStartPoint();
			}

			if (value > 0)
			{
				var points_per_unit = center / limitMax;
				return (points_per_unit * value) + GetStartPoint() + center;
			}
			else
			{
				var points_per_unit = center / limitMin;
				return (points_per_unit * (limitMin - value)) + GetStartPoint();
			}
		}

		/// <summary>
		/// Position to value.
		/// </summary>
		/// <returns>Value.</returns>
		/// <param name="position">Position.</param>
		protected override int PositionToValue(float position)
		{
			var center = RangeSize() / 2f;
			if (!IsHorizontal())
			{
				position += center * 2;
			}

			if (position == center)
			{
				return 0;
			}

			float value;
			if (position > center)
			{
				var points_per_unit = center / limitMax;

				value = (position - center) / points_per_unit;
			}
			else
			{
				var points_per_unit = center / limitMin;

				value = (-position / points_per_unit) + LimitMin;
			}

			if (WholeNumberOfSteps)
			{
				return InBounds(Mathf.RoundToInt(value / step) * step);
			}
			else
			{
				return InBounds(Mathf.RoundToInt(value));
			}
		}

		/// <summary>
		/// Position range for handle.
		/// </summary>
		/// <returns>The position limits.</returns>
		protected override Vector2 PositionLimits()
		{
			return new Vector2(ValueToPosition(LimitMin), ValueToPosition(LimitMax));
		}

		/// <summary>
		/// Fit value to bounds.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Value in bounds.</returns>
		protected override int InBounds(int value)
		{
			var v = Mathf.Clamp(value, LimitMin, LimitMax);

			return UseValueLimits ? Mathf.Clamp(v, ValueMin, ValueMax) : v;
		}

		/// <summary>
		/// Increases the value.
		/// </summary>
		protected override void Increase()
		{
			Value += step;
		}

		/// <summary>
		/// Decreases the value.
		/// </summary>
		protected override void Decrease()
		{
			Value -= step;
		}

		/// <summary>
		/// Determines whether this instance is positive value.
		/// </summary>
		/// <returns><c>true</c> if this instance is positive value; otherwise, <c>false</c>.</returns>
		protected override bool IsPositiveValue()
		{
			return Value > 0;
		}
	}
}