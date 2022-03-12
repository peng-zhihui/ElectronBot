namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Lines drawer.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Effects/Lines Drawer")]
	public class LinesDrawer : LinesDrawerBase
	{
		[SerializeField]
		List<float> linesX = new List<float>();

		ObservableList<float> observableLinesX;

		/// <summary>
		/// Lines at X axis.
		/// </summary>
		public ObservableList<float> LinesX
		{
			get
			{
				if (observableLinesX == null)
				{
					observableLinesX = new ObservableList<float>(linesX);
					observableLinesX.OnChange += UpdateLines;
				}

				return observableLinesX;
			}

			set
			{
				if (observableLinesX != null)
				{
					observableLinesX.OnChange -= UpdateLines;
				}

				observableLinesX = value;

				if (observableLinesX != null)
				{
					observableLinesX.OnChange += UpdateLines;
				}

				UpdateLines();
			}
		}

		[SerializeField]
		List<float> linesY = new List<float>();

		ObservableList<float> observableLinesY;

		/// <summary>
		/// Lines at Y axis.
		/// </summary>
		public ObservableList<float> LinesY
		{
			get
			{
				if (observableLinesY == null)
				{
					observableLinesY = new ObservableList<float>(linesY);
				}

				return observableLinesY;
			}

			set
			{
				if (observableLinesY != null)
				{
					observableLinesY.OnChange -= UpdateLines;
				}

				observableLinesY = value;

				if (observableLinesY != null)
				{
					observableLinesY.OnChange += UpdateLines;
				}

				UpdateLines();
			}
		}

		/// <inheritdoc/>
		protected override void UpdateLines()
		{
			HorizontalLines.Clear();
			HorizontalLines.AddRange(LinesX);

			VerticalLines.Clear();
			VerticalLines.AddRange(LinesY);

			base.UpdateLines();
		}

		#if UNITY_EDITOR

		/// <inheritdoc/>
		protected override void OnValidate()
		{
			LinesX = null;
			LinesY = null;

			base.OnValidate();
		}

		#endif
	}
}