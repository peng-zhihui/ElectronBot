namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test ListViewAutoResize.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class TestListViewAutoResize : MonoBehaviour
	{
		ListViewIcons listView;

		/// <summary>
		/// ListView.
		/// </summary>
		protected ListViewIcons ListView
		{
			get
			{
				if (listView == null)
				{
					listView = GetComponent<ListViewIcons>();
				}

				return listView;
			}
		}

		/// <summary>
		/// Test add.
		/// </summary>
		public void TestAdd()
		{
			var item = new ListViewIconsItemDescription()
			{
				Name = string.Format("test {0}", ListView.DataSource.Count.ToString()),
			};
			ListView.DataSource.Add(item);
		}

		/// <summary>
		/// Test delete.
		/// </summary>
		public void TestDelete()
		{
			var c = ListView.DataSource.Count;
			if (c > 0)
			{
				ListView.DataSource.RemoveAt(c - 1);
			}
		}

		/// <summary>
		/// Test add item with random amount of lines.
		/// </summary>
		public void TestAddRandomLines()
		{
			var line = string.Format("test {0}", ListView.DataSource.Count.ToString());
			var n = Random.Range(1, 4);

			var name = string.Empty;
			for (int i = 0; i < n; i++)
			{
				name += (i == 0) ? line : "\r\n" + line;
			}

			var item = new ListViewIconsItemDescription()
			{
				Name = name,
			};
			ListView.DataSource.Add(item);
		}
	}
}