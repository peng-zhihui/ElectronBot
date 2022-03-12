namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Snap lines.
	/// </summary>
	public class SnapLines : SnapGridBase
	{
		[SerializeField]
		List<LineX> linesX = new List<LineX>();

		ObservableList<LineX> xAxisLines;

		/// <summary>
		/// Lines at X axis.
		/// </summary>
		public ObservableList<LineX> XAxisLines
		{
			get
			{
				if (xAxisLines == null)
				{
					xAxisLines = new ObservableList<LineX>(linesX);
					xAxisLines.OnChange += UpdateLines;
				}

				return xAxisLines;
			}

			set
			{
				if (xAxisLines != null)
				{
					xAxisLines.OnChange -= UpdateLines;
				}

				xAxisLines = value;

				if (xAxisLines != null)
				{
					xAxisLines.OnChange += UpdateLines;
				}

				UpdateLines();
			}
		}

		[SerializeField]
		List<LineY> linesY = new List<LineY>();

		ObservableList<LineY> yAxisLines;

		/// <summary>
		/// Lines at Y axis.
		/// </summary>
		public ObservableList<LineY> YAxisLines
		{
			get
			{
				if (yAxisLines == null)
				{
					yAxisLines = new ObservableList<LineY>(linesY);
				}

				return yAxisLines;
			}

			set
			{
				if (yAxisLines != null)
				{
					yAxisLines.OnChange -= UpdateLines;
				}

				yAxisLines = value;

				if (yAxisLines != null)
				{
					yAxisLines.OnChange += UpdateLines;
				}

				UpdateLines();
			}
		}

		/// <inheritdoc/>
		protected override void UpdateLines()
		{
			LinesX.Clear();
			LinesX.AddRange(XAxisLines);

			LinesY.Clear();
			LinesY.AddRange(YAxisLines);

			OnLinesChanged.Invoke();
		}

		#if UNITY_EDITOR

		/// <inheritdoc/>
		protected override void OnValidate()
		{
			XAxisLines = null;
			YAxisLines = null;

			base.OnValidate();
		}

		#endif
	}
}