namespace UIWidgets.Examples
{
	using UnityEngine;

	/// <summary>
	/// Test SimpleTable.
	/// </summary>
	public class SimpleTableTest : MonoBehaviour
	{
		/// <summary>
		/// SimpleTable.
		/// </summary>
		public SimpleTable Table;

		/// <summary>
		/// Add item.
		/// </summary>
		public void Add()
		{
			Table.DataSource.Add(new SimpleTableItem() { Field1 = "Value 1", Field2 = "Value 2", Field3 = "Value 3" });
		}

		/// <summary>
		/// Remove item.
		/// </summary>
		public void Remove()
		{
			Table.DataSource.RemoveAt(0);
		}

		/// <summary>
		/// Add item at start.
		/// </summary>
		public void AddAtStart()
		{
			var item = new SimpleTableItem() { Field1 = "First row 1", Field2 = "First row 2", Field3 = "First row 3" };
			Table.DataSource.Insert(0, item);
		}
	}
}