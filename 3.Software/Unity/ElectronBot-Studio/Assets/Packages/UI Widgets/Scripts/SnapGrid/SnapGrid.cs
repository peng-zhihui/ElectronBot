namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Snap grid.
	/// </summary>
	public class SnapGrid : SnapGridBase
	{
		/// <summary>
		/// Margin.
		/// </summary>
		[SerializeField]
		protected Vector2 padding = new Vector2(0f, 0f);

		/// <summary>
		/// Padding.
		/// Distance from border to the first line.
		/// </summary>
		public Vector2 Padding
		{
			get
			{
				return padding;
			}

			set
			{
				if (padding != value)
				{
					padding = value;
					UpdateLines();
				}
			}
		}

		/// <summary>
		/// Grid step.
		/// </summary>
		[SerializeField]
		protected Vector2 step = new Vector2(10f, 10f);

		/// <summary>
		/// Grid step.
		/// </summary>
		public Vector2 Step
		{
			get
			{
				return step;
			}

			set
			{
				if (step != value)
				{
					step = value;
					UpdateLines();
				}
			}
		}

		/// <summary>
		/// Spacing.
		/// </summary>
		[SerializeField]
		protected Vector2 spacing = Vector2.zero;

		/// <summary>
		/// Spacing.
		/// </summary>
		public Vector2 Spacing
		{
			get
			{
				return spacing;
			}

			set
			{
				if (spacing != value)
				{
					spacing = value;
					UpdateLines();
				}
			}
		}

		/// <summary>
		/// Snap to spacing.
		/// </summary>
		[SerializeField]
		protected bool snapToSpacing = false;

		/// <summary>
		/// Snap to spacing.
		/// </summary>
		public bool SnapToSpacing
		{
			get
			{
				return snapToSpacing;
			}

			set
			{
				if (snapToSpacing != value)
				{
					snapToSpacing = value;
					UpdateLines();
				}
			}
		}

		/// <inheritdoc/>
		protected override void UpdateLines()
		{
			var size = RectTransform.rect.size;

			LinesX.Clear();
			if (SnapToSpacing || (Spacing.x == 0f))
			{
				CalculateLinesWithSnapSpacing(size.x, Padding.x, Step.x, Spacing.x, LineX.Create, LinesX);
			}
			else
			{
				CalculateLines(size.x, Padding.x, Step.x, Spacing.x, LineX.Create, LinesX);
			}

			LinesY.Clear();
			if (SnapToSpacing || (Spacing.y == 0f))
			{
				CalculateLinesWithSnapSpacing(size.y, Padding.y, Step.y, Spacing.y, LineY.Create, LinesY);
			}
			else
			{
				CalculateLines(size.y, Padding.y, Step.y, Spacing.y, LineY.Create, LinesY);
			}

			OnLinesChanged.Invoke();
		}

		/// <summary>
		/// Calculate lines.
		/// </summary>
		/// <typeparam name="T">Line type.</typeparam>
		/// <param name="size">Size.</param>
		/// <param name="margin">Margin.</param>
		/// <param name="step">Step.</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="constructor">Line constructor.</param>
		/// <param name="output">Result list.</param>
		protected virtual void CalculateLines<T>(float size, float margin, float step, float spacing, Func<float, bool, bool, T> constructor, List<T> output)
		{
			if (step <= 0f)
			{
				return;
			}

			var value = 0f;
			size -= margin;

			if (margin > 0f)
			{
				value += margin;

				if (value < size)
				{
					output.Add(constructor(value, true, false));
				}

				value += step;
				if (value < size)
				{
					output.Add(constructor(value, false, true));
				}

				if (spacing > 0f)
				{
					value += spacing;
					if (value < size)
					{
						output.Add(constructor(value, true, false));
					}
				}
			}

			do
			{
				value += step;
				if (value >= size)
				{
					break;
				}

				output.Add(constructor(value, false, true));

				if (spacing > 0f)
				{
					value += spacing;
					if (value >= size)
					{
						break;
					}

					output.Add(constructor(value, true, false));
				}
			}
			while (true);
		}

		/// <summary>
		/// Calculate lines with snap spacing.
		/// </summary>
		/// <typeparam name="T">Line type.</typeparam>
		/// <param name="size">Size.</param>
		/// <param name="margin">Margin.</param>
		/// <param name="step">Step.</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="constructor">Line constructor.</param>
		/// <param name="output">Result list.</param>
		protected virtual void CalculateLinesWithSnapSpacing<T>(float size, float margin, float step, float spacing, Func<float, bool, bool, T> constructor, List<T> output)
		{
			if (step <= 0f)
			{
				return;
			}

			var value = 0f;
			size -= margin;

			if (margin > 0f)
			{
				value += margin;

				if (value < size)
				{
					output.Add(constructor(value, true, true));
				}
			}

			do
			{
				value += step;
				if (value >= size)
				{
					break;
				}

				output.Add(constructor(value, true, true));

				if (spacing > 0)
				{
					value += spacing;
					if (value >= size)
					{
						break;
					}

					output.Add(constructor(value, true, true));
				}
			}
			while (true);
		}
	}
}