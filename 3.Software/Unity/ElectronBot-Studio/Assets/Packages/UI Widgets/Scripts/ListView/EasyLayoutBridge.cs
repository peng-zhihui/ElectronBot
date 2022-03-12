namespace UIWidgets
{
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Bridge to EasyLayout class.
	/// </summary>
	public class EasyLayoutBridge : ILayoutBridge
	{
		bool isHorizontal;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is horizontal.
		/// </summary>
		/// <value><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</value>
		public bool IsHorizontal
		{
			get
			{
				return isHorizontal;
			}

			set
			{
				isHorizontal = value;
				UpdateDirection();
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this <see cref="UIWidgets.EasyLayoutBridge"/> update content size fitter.
		/// </summary>
		/// <value><c>true</c> if update content size fitter; otherwise, <c>false</c>.</value>
		public bool UpdateContentSizeFitter
		{
			get;
			set;
		}

		/// <summary>
		/// Layout.
		/// </summary>
		protected EasyLayout Layout;

		/// <summary>
		/// Default item.
		/// </summary>
		protected RectTransform DefaultItem;

		/// <summary>
		/// Fitter.
		/// </summary>
		protected ContentSizeFitter Fitter;

		/// <summary>
		/// Control layout RectTransform settings.
		/// </summary>
		protected bool ControlRectTrasform;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.EasyLayoutBridge"/> class.
		/// </summary>
		/// <param name="layout">Layout.</param>
		/// <param name="defaultItem">Default item.</param>
		/// <param name="updateContentSizeFitter">Update ContentSizeFitter on direction change.</param>
		/// <param name="controlRectTransform">Control layout RectTransform settings.</param>
		public EasyLayoutBridge(EasyLayout layout, RectTransform defaultItem, bool updateContentSizeFitter = true, bool controlRectTransform = true)
		{
			Layout = layout;
			DefaultItem = defaultItem;
			UpdateContentSizeFitter = updateContentSizeFitter;
			ControlRectTrasform = controlRectTransform;

			Fitter = Layout.GetComponent<ContentSizeFitter>();
		}

		void UpdateDirection()
		{
			if (UpdateContentSizeFitter)
			{
				if (Fitter != null)
				{
					Fitter.horizontalFit = IsHorizontal ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
					Fitter.verticalFit = !IsHorizontal ? ContentSizeFitter.FitMode.PreferredSize : ContentSizeFitter.FitMode.Unconstrained;
				}
			}

			if (Layout.LayoutType == LayoutTypes.Grid)
			{
				if (Layout.GridConstraint != GridConstraints.Flexible)
				{
					Layout.GridConstraint = isHorizontal ? GridConstraints.FixedRowCount : GridConstraints.FixedColumnCount;
				}
			}

			if (ControlRectTrasform)
			{
				var layout_rect_transform = Layout.transform as RectTransform;
				layout_rect_transform.pivot = new Vector2(0, 1);
				if (isHorizontal)
				{
					layout_rect_transform.anchorMin = new Vector2(0, 0);
					layout_rect_transform.anchorMax = new Vector2(0, 1);
				}
				else
				{
					layout_rect_transform.anchorMin = new Vector2(0, 1);
					layout_rect_transform.anchorMax = new Vector2(1, 1);
				}

				layout_rect_transform.sizeDelta = new Vector2(0, 0);
			}
		}

		/// <summary>
		/// Updates the layout.
		/// </summary>
		public void UpdateLayout()
		{
			Layout.UpdateLayout();

			if (Fitter != null)
			{
				Fitter.SetLayoutHorizontal();
				Fitter.SetLayoutVertical();
			}
		}

		/// <summary>
		/// Sets the filler.
		/// </summary>
		/// <param name="first">First.</param>
		/// <param name="last">Last.</param>
		public void SetFiller(float first, float last)
		{
			var padding = IsHorizontal
				? new Padding(first, last, 0, 0)
				: new Padding(0, 0, first, last);

			Layout.PaddingInner = padding;
		}

		/// <summary>
		/// Get filler.
		/// </summary>
		/// <returns>Filler value.</returns>
		public Vector2 GetFiller()
		{
			return IsHorizontal
				? new Vector2(Layout.PaddingInner.Left, Layout.PaddingInner.Right)
				: new Vector2(Layout.PaddingInner.Top, Layout.PaddingInner.Bottom);
		}

		/// <summary>
		/// Gets the size of the item.
		/// </summary>
		/// <returns>The item size.</returns>
		public Vector2 GetItemSize()
		{
			var rect = DefaultItem.rect;
			var width = Mathf.Max(rect.width, LayoutUtility.GetPreferredWidth(DefaultItem));
			var height = Mathf.Max(rect.height, LayoutUtility.GetPreferredHeight(DefaultItem));

			return new Vector2(width, height);
		}

		/// <summary>
		/// Gets the left or top margin.
		/// </summary>
		/// <returns>The margin.</returns>
		public float GetMargin()
		{
			return IsHorizontal ? Layout.GetMarginLeft() : Layout.GetMarginTop();
		}

		/// <summary>
		/// Return MarginX or MarginY depend of direction.
		/// </summary>
		/// <returns>The full margin.</returns>
		public float GetFullMargin()
		{
			return IsHorizontal ? GetFullMarginX() : GetFullMarginY();
		}

		/// <summary>
		/// Gets sum of left and right margin.
		/// </summary>
		/// <returns>The margin.</returns>
		public float GetFullMarginX()
		{
			return Layout.GetMarginLeft() + Layout.GetMarginRight();
		}

		/// <summary>
		/// Gets sum of top and bottom margin.
		/// </summary>
		/// <returns>The full margin.</returns>
		public float GetFullMarginY()
		{
			return Layout.GetMarginTop() + Layout.GetMarginBottom();
		}

		/// <summary>
		/// Gets the size of the margin.
		/// </summary>
		/// <returns>The margin size.</returns>
		public Vector4 GetMarginSize()
		{
			return new Vector4(Layout.GetMarginLeft(), Layout.GetMarginRight(), Layout.GetMarginTop(), Layout.GetMarginBottom());
		}

		/// <summary>
		/// Gets the spacing between items.
		/// </summary>
		/// <returns>The spacing.</returns>
		public float GetSpacing()
		{
			return IsHorizontal ? Layout.Spacing.x : Layout.Spacing.y;
		}

		/// <summary>
		/// Gets the horizontal spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		public float GetSpacingX()
		{
			return Layout.Spacing.x;
		}

		/// <summary>
		/// Gets the vertical spacing.
		/// </summary>
		/// <returns>The spacing.</returns>
		public float GetSpacingY()
		{
			return Layout.Spacing.y;
		}

		/// <summary>
		/// Blocks constraint count.
		/// </summary>
		/// <param name="blocks">Default blocks count.</param>
		/// <returns>The constraint.</returns>
		public int BlocksConstraint(int blocks)
		{
			if (Layout.LayoutType == LayoutTypes.Staggered)
			{
				if (Layout.StaggeredSettings.FixedBlocksCount)
				{
					return Mathf.Min(1, Layout.StaggeredSettings.BlocksCount);
				}
			}

			return blocks;
		}

		/// <summary>
		/// Columns constraint count.
		/// </summary>
		/// <param name="columns">Columns count.</param>
		/// <returns>The constraint.</returns>
		public int ColumnsConstraint(int columns)
		{
			if (Layout.LayoutType == LayoutTypes.Compact)
			{
				if (Layout.CompactConstraint == CompactConstraints.MaxColumnCount)
				{
					var c = Mathf.Max(1, Layout.CompactConstraintCount);
					return Mathf.Min(c, columns);
				}
			}
			else if (Layout.LayoutType == LayoutTypes.Grid)
			{
				if (Layout.GridConstraint == GridConstraints.FixedColumnCount)
				{
					return Mathf.Max(1, Layout.GridConstraintCount);
				}
			}

			return columns;
		}

		/// <summary>
		/// Rows the constraint count.
		/// </summary>
		/// <param name="rows">Rows count.</param>
		/// <returns>The constraint.</returns>
		public int RowsConstraint(int rows)
		{
			if (Layout.LayoutType == LayoutTypes.Compact)
			{
				if (Layout.CompactConstraint == CompactConstraints.MaxRowCount)
				{
					var r = Mathf.Max(1, Layout.CompactConstraintCount);
					return Mathf.Min(r, rows);
				}
			}
			else if (Layout.LayoutType == LayoutTypes.Grid)
			{
				if (Layout.GridConstraint == GridConstraints.FixedRowCount)
				{
					return Mathf.Max(1, Layout.GridConstraintCount);
				}
			}

			return rows;
		}
	}
}