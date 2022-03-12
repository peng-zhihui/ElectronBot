namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TableList.
	/// </summary>
	[RequireComponent(typeof(TableList))]
	public class TableListTest : MonoBehaviour
	{
		/// <summary>
		/// The table.
		/// </summary>
		[NonSerialized]
		public TableList Table;

		/// <summary>
		/// The header.
		/// </summary>
		[SerializeField]
		public TableHeader Header;

		/// <summary>
		/// The header cell.
		/// </summary>
		[SerializeField]
		public TableListCell HeaderCell;

		/// <summary>
		/// The row cell.
		/// </summary>
		[SerializeField]
		public TableListCell RowCell;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public void Start()
		{
			Table = GetComponent<TableList>();

			Table.Init();
			Table.DataSource = Generate(100, 10);

			Header.Refresh();
		}

		/// <summary>
		/// Generate test data.
		/// </summary>
		/// <param name="rows">Rows.</param>
		/// <param name="columns">Columns.</param>
		/// <returns>Test list.</returns>
		protected static ObservableList<List<int>> Generate(int rows, int columns)
		{
			var data = new ObservableList<List<int>>();
			for (int i = 0; i < rows; i++)
			{
				var row = new List<int>(columns);
				for (int j = 0; j < columns; j++)
				{
					row.Add((columns * i) + j);
				}

				data.Add(row);
			}

			return data;
		}

		int columns = 2;

		/// <summary>
		/// Add the column.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		public void AddColumn(string columnName)
		{
			columns += 1;

			foreach (var component in Table.GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				AddCellToRow(component);
			}

			Table.ComponentsColoring();

			AddCellToHeader(string.Format("{0} {1}", columnName, columns.ToString()));
		}

		/// <summary>
		/// Add the cell to row.
		/// </summary>
		/// <param name="row">Row.</param>
		protected void AddCellToRow(TableListComponent row)
		{
			var cell = Compatibility.Instantiate(RowCell);
			cell.transform.SetParent(row.transform, false);
			cell.gameObject.SetActive(true);

			row.TextAdapterComponents.Add(cell.TextAdapter);
			row.UpdateView();
		}

		/// <summary>
		/// Add the cell to header.
		/// </summary>
		/// <param name="columnName">Column name.</param>
		protected void AddCellToHeader(string columnName)
		{
			var cell = Compatibility.Instantiate(HeaderCell);
			cell.TextAdapter.text = columnName;
			Header.AddCell(cell.gameObject);
		}

		int cellToDelete;

		/// <summary>
		/// Removes the column.
		/// </summary>
		/// <param name="index">Index.</param>
		public void RemoveColumn(int index)
		{
			cellToDelete = index;

			foreach (var component in Table.GetComponentsEnumerator(PoolEnumeratorMode.All))
			{
				RemoveCellFromRow(component);
			}

			RemoveCellFromHeader(index);
		}

		/// <summary>
		/// Removes the cell from row.
		/// </summary>
		/// <param name="row">Row.</param>
		protected void RemoveCellFromRow(TableListComponent row)
		{
			var cell = row.transform.GetChild(cellToDelete);
			cell.gameObject.SetActive(false);
			cell.transform.SetParent(null, false);

			row.TextAdapterComponents.RemoveAt(cellToDelete);
			row.UpdateView();
		}

		/// <summary>
		/// Removes the cell from header.
		/// </summary>
		/// <param name="index">Index.</param>
		protected void RemoveCellFromHeader(int index)
		{
			var cell = Header.transform.GetChild(index);
			Header.RemoveCell(cell.gameObject);
		}
	}
}