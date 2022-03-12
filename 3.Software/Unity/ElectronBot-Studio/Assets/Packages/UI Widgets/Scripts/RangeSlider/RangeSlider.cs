namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Range slider.
	/// </summary>
	public class RangeSlider : RangeSliderBase<int>
	{
		/// <summary>
		/// Value to position.
		/// </summary>
		/// <returns>Position.</returns>
		/// <param name="value">Value.</param>
		protected override float ValueToPosition(int value)
		{
			var points_per_unit = FillSize() / (limitMax - limitMin);

			return (points_per_unit * (InBounds(value) - limitMin)) + GetStartPoint();
		}

		/// <summary>
		/// Position to value.
		/// </summary>
		/// <returns>Value.</returns>
		/// <param name="position">Position.</param>
		protected override int PositionToValue(float position)
		{
			var points_per_unit = FillSize() / (limitMax - limitMin);

			var value = (position / points_per_unit) + LimitMin;

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
		/// Position range for minimum handle.
		/// </summary>
		/// <returns>The position limits.</returns>
		protected override Vector2 MinPositionLimits()
		{
			return new Vector2(ValueToPosition(LimitMin), ValueToPosition(valueMax - step));
		}

		/// <summary>
		/// Position range for maximum handle.
		/// </summary>
		/// <returns>The position limits.</returns>
		protected override Vector2 MaxPositionLimits()
		{
			return new Vector2(ValueToPosition(valueMin + step), ValueToPosition(limitMax));
		}

		/// <summary>
		/// Fit value to bounds.
		/// </summary>
		/// <returns>Value in bounds.</returns>
		/// <param name="value">Value.</param>
		protected override int InBounds(int value)
		{
			return Mathf.Clamp(value, limitMin, limitMax);
		}

		/// <summary>
		/// Fit minimum value to bounds.
		/// </summary>
		/// <returns>Value in bounds.</returns>
		/// <param name="value">Value.</param>
		protected override int InBoundsMin(int value)
		{
			return Mathf.Clamp(value, limitMin, valueMax - step);
		}

		/// <summary>
		/// Fit maximum value to bounds.
		/// </summary>
		/// <returns>Value in bounds.</returns>
		/// <param name="value">Value.</param>
		protected override int InBoundsMax(int value)
		{
			return Mathf.Clamp(value, valueMin + step, limitMax);
		}

		/// <summary>
		/// Increases the minimum value.
		/// </summary>
		protected override void IncreaseMin()
		{
			ValueMin += step;
		}

		/// <summary>
		/// Decreases the minimum value.
		/// </summary>
		protected override void DecreaseMin()
		{
			ValueMin -= step;
		}

		/// <summary>
		/// Increases the maximum value.
		/// </summary>
		protected override void IncreaseMax()
		{
			ValueMax += step;
		}

		/// <summary>
		/// Decreases the maximum value.
		/// </summary>
		protected override void DecreaseMax()
		{
			ValueMax -= step;
		}

		/// <inheritdoc/>
		public override float Value01(float value)
		{
			return (value - limitMin) / (limitMax - limitMin);
		}
	}
}