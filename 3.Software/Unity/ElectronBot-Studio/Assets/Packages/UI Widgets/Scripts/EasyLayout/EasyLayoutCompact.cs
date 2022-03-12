namespace EasyLayoutNS
{
	using UnityEngine;

	/// <summary>
	/// Compact layout group.
	/// </summary>
	public class EasyLayoutCompact : EasyLayoutCompactOrGrid
	{
		/// <summary>
		/// Constraint type.
		/// </summary>
		protected CompactConstraints Constraint;

		/// <summary>
		/// Row align.
		/// </summary>
		protected HorizontalAligns RowAlign;

		/// <summary>
		/// Inner align.
		/// </summary>
		protected InnerAligns InnerAlign;

		/// <inheritdoc/>
		protected override bool IsCompact
		{
			get
			{
				return true;
			}
		}

		/// <inheritdoc/>
		public override void LoadSettings(EasyLayout layout)
		{
			base.LoadSettings(layout);

			Constraint = layout.CompactConstraint;
			RowAlign = layout.RowAlign;
			InnerAlign = layout.InnerAlign;
		}

		/// <summary>
		/// Group the specified elements.
		/// </summary>
		protected override void Group()
		{
			if (ElementsGroup.Count == 0)
			{
				return;
			}

			if (IsHorizontal)
			{
				GroupHorizontal();
			}
			else
			{
				GroupVertical();
			}

			var rows = ElementsGroup.Rows;
			var columns = ElementsGroup.Columns;

			if ((Constraint == CompactConstraints.MaxRowCount) && (rows > ConstraintCount))
			{
				ElementsGroup.Clear();
				GroupByRows();
			}
			else if ((Constraint == CompactConstraints.MaxColumnCount) && (columns > ConstraintCount))
			{
				ElementsGroup.Clear();
				GroupByColumns();
			}

			if (!TopToBottom)
			{
				ElementsGroup.BottomToTop();
			}

			if (RightToLeft)
			{
				ElementsGroup.RightToLeft();
			}
		}

		void GroupHorizontal()
		{
			var base_length = MainAxisSize;
			var length = base_length;
			var spacing = Spacing.x;

			int row = 0;
			int column = 0;
			for (int i = 0; i < ElementsGroup.Count; i++)
			{
				var element = ElementsGroup[i];
				if (column == 0)
				{
					length -= element.AxisSize;
				}
				else if (length >= (element.AxisSize + spacing))
				{
					length -= element.AxisSize + spacing;
				}
				else
				{
					length = base_length - element.AxisSize;

					row++;
					column = 0;
				}

				ElementsGroup.SetPosition(i, row, column);
				column++;
			}
		}

		void GroupVertical()
		{
			var base_length = MainAxisSize;
			var length = base_length;
			var spacing = Spacing.y;

			int row = 0;
			int column = 0;

			for (int i = 0; i < ElementsGroup.Count; i++)
			{
				var element = ElementsGroup[i];
				if (row == 0)
				{
					length -= element.AxisSize;
				}
				else if (length >= (element.AxisSize + spacing))
				{
					length -= element.AxisSize + spacing;
				}
				else
				{
					length = base_length - element.AxisSize;

					column++;
					row = 0;
				}

				ElementsGroup.SetPosition(i, row, column);
				row++;
			}
		}

		/// <summary>
		/// Calculate sizes of the elements.
		/// </summary>
		protected override void CalculateSizes()
		{
			if ((ChildrenWidth == ChildrenSize.ShrinkOnOverflow) && (ChildrenHeight == ChildrenSize.ShrinkOnOverflow))
			{
				ShrinkOnOverflow();
			}
			else
			{
				if (IsHorizontal)
				{
					if (ChildrenWidth == ChildrenSize.FitContainer)
					{
						ResizeWidthToFit(false);
					}
					else if (ChildrenWidth == ChildrenSize.SetPreferredAndFitContainer)
					{
						ResizeWidthToFit(true);
					}
					else if (ChildrenWidth == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkWidthToFit();
					}

					if (ChildrenHeight == ChildrenSize.FitContainer)
					{
						ResizeRowHeightToFit(false);
					}
					else if (ChildrenHeight == ChildrenSize.SetPreferredAndFitContainer)
					{
						ResizeRowHeightToFit(true);
					}
					else if (ChildrenHeight == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkRowHeightToFit();
					}
				}
				else
				{
					if (ChildrenWidth == ChildrenSize.FitContainer)
					{
						ResizeColumnWidthToFit(false);
					}
					else if (ChildrenWidth == ChildrenSize.SetPreferredAndFitContainer)
					{
						ResizeColumnWidthToFit(true);
					}
					else if (ChildrenWidth == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkColumnWidthToFit();
					}

					if (ChildrenHeight == ChildrenSize.FitContainer)
					{
						ResizeHeightToFit(false);
					}
					else if (ChildrenHeight == ChildrenSize.SetPreferredAndFitContainer)
					{
						ResizeHeightToFit(true);
					}
					else if (ChildrenHeight == ChildrenSize.ShrinkOnOverflow)
					{
						ShrinkHeightToFit();
					}
				}
			}
		}

		void ResizeHeightToFit(bool increaseOnly)
		{
			var height = InternalSize.y;
			for (int column = 0; column < ElementsGroup.Columns; column++)
			{
				ResizeToFit(height, ElementsGroup.GetColumn(column), Spacing.y, RectTransform.Axis.Vertical, increaseOnly);
			}
		}

		void ShrinkHeightToFit()
		{
			var height = InternalSize.y;
			for (int column = 0; column < ElementsGroup.Columns; column++)
			{
				ShrinkToFit(height, ElementsGroup.GetColumn(column), Spacing.y, RectTransform.Axis.Vertical);
			}
		}

		/// <summary>
		/// Calculate size of the group.
		/// </summary>
		/// <param name="isHorizontal">ElementsGroup are in horizontal order?</param>
		/// <param name="spacing">Spacing.</param>
		/// <param name="padding">Padding,</param>
		/// <returns>Size.</returns>
		protected override GroupSize CalculateGroupSize(bool isHorizontal, Vector2 spacing, Vector2 padding)
		{
			return ElementsGroup.Size(spacing, padding);
		}

		/// <summary>
		/// Get aligned width.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="maxWidth">Maximum width.</param>
		/// <param name="cellMaxSize">Max size of the cell.</param>
		/// <param name="emptyWidth">Width of the empty space.</param>
		/// <returns>Aligned width.</returns>
		protected override Vector2 GetAlignByWidth(LayoutElementInfo element, float maxWidth, Vector2 cellMaxSize, float emptyWidth)
		{
			return new Vector2(
				emptyWidth * RowAligns[(int)RowAlign],
				(cellMaxSize.y - element.Height) * InnerAligns[(int)InnerAlign]);
		}

		/// <summary>
		/// Get aligned height.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="maxHeight">Maximum height.</param>
		/// <param name="cellMaxSize">Max size of the cell.</param>
		/// <param name="emptyHeight">Height of the empty space.</param>
		/// <returns>Aligned height.</returns>
		protected override Vector2 GetAlignByHeight(LayoutElementInfo element, float maxHeight, Vector2 cellMaxSize, float emptyHeight)
		{
			return new Vector2(
				(cellMaxSize.x - element.Width) * InnerAligns[(int)InnerAlign],
				emptyHeight * RowAligns[(int)RowAlign]);
		}
	}
}