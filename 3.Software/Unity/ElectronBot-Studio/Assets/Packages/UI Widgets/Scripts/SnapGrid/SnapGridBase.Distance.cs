namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Base class for the SnapGrid.
	/// </summary>
	public abstract partial class SnapGridBase : UIBehaviourConditional
	{
		/// <summary>
		/// Distance.
		/// </summary>
		public struct Distance
		{
			DistanceValue left;

			DistanceValue right;

			DistanceValue top;

			DistanceValue bottom;

			/// <summary>
			/// Initializes a new instance of the <see cref="Distance"/> struct.
			/// </summary>
			/// <param name="defaultValue">Default value.</param>
			public Distance(float defaultValue)
			{
				left = new DistanceValue(defaultValue);
				right = new DistanceValue(defaultValue);
				top = new DistanceValue(defaultValue);
				bottom = new DistanceValue(defaultValue);
			}

			/// <summary>
			/// Update distance at left direction.
			/// </summary>
			/// <param name="value">New value.</param>
			public void Left(float value)
			{
				left.Update(value);
			}

			/// <summary>
			/// Update distance at right direction.
			/// </summary>
			/// <param name="value">New value.</param>
			public void Right(float value)
			{
				right.Update(value);
			}

			/// <summary>
			/// Update distance at top direction.
			/// </summary>
			/// <param name="value">New value.</param>
			public void Top(float value)
			{
				top.Update(value);
			}

			/// <summary>
			/// Update distance at bottom direction.
			/// </summary>
			/// <param name="value">New value.</param>
			public void Bottom(float value)
			{
				bottom.Update(value);
			}

			float X(float snapDistance, out bool snapped)
			{
				if (left.Abs < right.Abs)
				{
					if (left.Abs <= snapDistance)
					{
						snapped = true;
						return left.Value;
					}
				}
				else
				{
					if (right.Abs <= snapDistance)
					{
						snapped = true;
						return right.Value;
					}
				}

				snapped = false;
				return 0f;
			}

			float Y(float snapDistance, out bool snapped)
			{
				if (top.Abs < bottom.Abs)
				{
					if (top.Abs <= snapDistance)
					{
						snapped = true;
						return top.Value;
					}
				}
				else
				{
					if (bottom.Abs <= snapDistance)
					{
						snapped = true;
						return bottom.Value;
					}
				}

				snapped = false;
				return 0f;
			}

			/// <summary>
			/// Snap.
			/// </summary>
			/// <param name="snapDistance">Snap distance.</param>
			/// <returns>Distance to the nearest lines.</returns>
			public Result Snap(Vector2 snapDistance)
			{
				bool snappedX;
				bool snappedY;
				var delta = new Vector2(X(snapDistance.x, out snappedX), Y(snapDistance.y, out snappedY));

				return new Result(delta, snappedX, snappedY);
			}

			/// <summary>
			/// Converts this instance to the string.
			/// </summary>
			/// <returns>String.</returns>
			public override string ToString()
			{
				var args = new object[] { left.Value.ToString(), right.Value.ToString(), top.Value.ToString(), bottom.Value.ToString() };
				return string.Format("left: {0}; right: {1}; top: {2}; bottom: {3}", args);
			}
		}
	}
}