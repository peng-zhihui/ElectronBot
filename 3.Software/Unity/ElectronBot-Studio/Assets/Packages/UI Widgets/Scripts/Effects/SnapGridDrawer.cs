namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Grid drawer.
	/// </summary>
	[RequireComponent(typeof(SnapGridBase))]
	[AddComponentMenu("UI/New UI Widgets/Effects/Snap Grid Drawer")]
	public class SnapGridDrawer : LinesDrawerBase
	{
		/// <summary>
		/// Grid.
		/// </summary>
		protected SnapGridBase Grid;

		[SerializeField]
		bool includeBorders;

		/// <summary>
		/// Draw borders.
		/// </summary>
		public bool IncludeBorders
		{
			get
			{
				return includeBorders;
			}

			set
			{
				if (includeBorders != value)
				{
					includeBorders = value;
					UpdateMaterial();
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			if (Grid == null)
			{
				Grid = GetComponent<SnapGridBase>();
			}

			if (Grid != null)
			{
				// ensure only one listener
				Grid.OnLinesChanged.RemoveListener(UpdateLines);
				Grid.OnLinesChanged.AddListener(UpdateLines);
			}

			base.OnEnable();
		}

		/// <inheritdoc/>
		protected override void OnDisable()
		{
			base.OnDisable();

			if (Grid != null)
			{
				Grid.OnLinesChanged.RemoveListener(UpdateLines);
			}
		}

		/// <inheritdoc/>
		protected override void UpdateLines()
		{
			HorizontalLines.Clear();
			VerticalLines.Clear();

			if (Grid != null)
			{
				Grid.GetLinesX(IncludeBorders, HorizontalLines);
				Grid.GetLinesY(IncludeBorders, VerticalLines);
			}

			base.UpdateLines();
		}

		#if UNITY_EDITOR

		/// <inheritdoc/>
		protected override void OnValidate()
		{
			base.OnValidate();

			OnEnable();
		}
		#endif
	}
}