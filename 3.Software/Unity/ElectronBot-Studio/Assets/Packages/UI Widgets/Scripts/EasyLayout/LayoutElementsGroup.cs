namespace EasyLayoutNS
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Layout elements group.
	/// </summary>
	public class LayoutElementsGroup
	{
		List<LayoutElementInfo> elements;

		int rows;

		int columns;

		List<LayoutElementInfo> rowElements = new List<LayoutElementInfo>();

		List<LayoutElementInfo> columnElements = new List<LayoutElementInfo>();

		/// <summary>
		/// Elements.
		/// </summary>
		public List<LayoutElementInfo> Elements
		{
			get
			{
				return elements;
			}
		}

		/// <summary>
		/// Get element by index.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Element,</returns>
		public LayoutElementInfo this[int index]
		{
			get
			{
				return elements[index];
			}
		}

		/// <summary>
		/// Rows.
		/// </summary>
		public int Rows
		{
			get
			{
				return rows + 1;
			}
		}

		/// <summary>
		/// Columns.
		/// </summary>
		public int Columns
		{
			get
			{
				return columns + 1;
			}
		}

		/// <summary>
		/// Elements count.
		/// </summary>
		public int Count
		{
			get
			{
				return elements.Count;
			}
		}

		/// <summary>
		/// Set elements.
		/// </summary>
		/// <param name="newElements">Elements.</param>
		public void SetElements(List<LayoutElementInfo> newElements)
		{
			elements = newElements;

			Clear();
		}

		/// <summary>
		/// Clear.
		/// </summary>
		public void Clear()
		{
			rows = -1;
			columns = -1;
			foreach (var element in elements)
			{
				element.Row = -1;
				element.Column = -1;
			}
		}

		/// <summary>
		/// Set position of the element.
		/// </summary>
		/// <param name="index">Index of the element.</param>
		/// <param name="row">Row.</param>
		/// <param name="column">Column.</param>
		public void SetPosition(int index, int row, int column)
		{
			SetPosition(elements[index], row, column);
		}

		/// <summary>
		/// Set position of the element.
		/// </summary>
		/// <param name="element">Element.</param>
		/// <param name="row">Row.</param>
		/// <param name="column">Column.</param>
		public void SetPosition(LayoutElementInfo element, int row, int column)
		{
			element.Row = row;
			element.Column = column;

			rows = Mathf.Max(rows, row);
			columns = Mathf.Max(columns, column);
		}

		/// <summary>
		/// Get elements at row.
		/// </summary>
		/// <param name="row">Row.</param>
		/// <returns>Elements.</returns>
		public List<LayoutElementInfo> GetRow(int row)
		{
			rowElements.Clear();

			foreach (var elem in elements)
			{
				if (elem.Row == row)
				{
					rowElements.Add(elem);
				}
			}

			return rowElements;
		}

		/// <summary>
		/// Get elements at column.
		/// </summary>
		/// <param name="column">Column.</param>
		/// <returns>Elements.</returns>
		public List<LayoutElementInfo> GetColumn(int column)
		{
			columnElements.Clear();

			foreach (var elem in elements)
			{
				if (elem.Column == column)
				{
					columnElements.Add(elem);
				}
			}

			return columnElements;
		}

		/// <summary>
		/// Get target position in the group.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Position.</returns>
		public EasyLayoutPosition GetElementPosition(RectTransform target)
		{
			var target_id = target.GetInstanceID();
			foreach (var element in elements)
			{
				if (element.Rect.GetInstanceID() == target_id)
				{
					return new EasyLayoutPosition(element.Row, element.Column);
				}
			}

			return new EasyLayoutPosition(-1, -1);
		}

		/// <summary>
		/// Change elements order to bottom to top.
		/// </summary>
		public void BottomToTop()
		{
			foreach (var element in elements)
			{
				element.Row = rows - element.Row;
			}
		}

		/// <summary>
		/// Change elements order to right to left.
		/// </summary>
		public void RightToLeft()
		{
			for (int i = 0; i < Rows; i++)
			{
				var row = GetRow(i);
				foreach (var element in row)
				{
					element.Column = (row.Count - 1) - element.Column;
				}
			}
		}

		/// <summary>
		/// Get size.
		/// </summary>
		/// <param name="spacing">Spacing.</param>
		/// <param name="padding">Padding.</param>
		/// <returns>Size.</returns>
		public GroupSize Size(Vector2 spacing, Vector2 padding)
		{
			var width = HorizontalSize(spacing.x, padding.x);
			var height = VerticalSize(spacing.y, padding.y);

			return new GroupSize(width, height);
		}

		GroupSize HorizontalSize(float spacing, float padding)
		{
			var size = default(GroupSize);

			for (int i = 0; i < Rows; i++)
			{
				var block = GetRow(i);
				var block_size = new GroupSize(((block.Count - 1) * spacing) + padding, 0f);
				foreach (var element in block)
				{
					block_size.Width += element.Width;
					block_size.MinWidth += element.MinWidth;
					block_size.PreferredWidth += element.PreferredWidth;
				}

				size.Max(block_size);
			}

			return size;
		}

		GroupSize VerticalSize(float spacing, float padding)
		{
			var size = default(GroupSize);

			for (int i = 0; i < Columns; i++)
			{
				var block = GetColumn(i);
				var block_size = new GroupSize(0f, ((block.Count - 1) * spacing) + padding);
				foreach (var element in block)
				{
					block_size.Height += element.Height;
					block_size.MinHeight += element.MinHeight;
					block_size.PreferredHeight += element.PreferredHeight;
				}

				size.Max(block_size);
			}

			return size;
		}

		/// <summary>
		/// Compare elements by row and column.
		/// </summary>
		protected static System.Comparison<LayoutElementInfo> LayoutElementInfoComparison = Comparison;

		/// <summary>
		/// Sort elements.
		/// </summary>
		public void Sort()
		{
			Elements.Sort(LayoutElementInfoComparison);
		}

		/// <summary>
		/// Compare elements by row and column.
		/// </summary>
		/// <param name="x">First element.</param>
		/// <param name="y">Second element.</param>
		/// <returns>Result of the comparison.</returns>
		protected static int Comparison(LayoutElementInfo x, LayoutElementInfo y)
		{
			var row_comparison = x.Row.CompareTo(y.Row);

			if (row_comparison == 0)
			{
				return x.Column.CompareTo(y.Column);
			}

			return row_comparison;
		}
	}
}