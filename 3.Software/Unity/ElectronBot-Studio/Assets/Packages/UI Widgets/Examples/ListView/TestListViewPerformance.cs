namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test ListView performance.
	/// </summary>
	public class TestListViewPerformance : MonoBehaviour
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("lv")]
		protected ListViewString ListView;

		/// <summary>
		/// ListViewIcons.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("lvi")]
		protected ListViewIcons ListViewIcons;

		/// <summary>
		/// Clear ListViewIcons.
		/// </summary>
		public void ListClear()
		{
			ListViewIcons.DestroyGameObjects = true;
			ListViewIcons.DataSource.Clear();
		}

		/// <summary>
		/// Set new list to ListView with specified items count.
		/// </summary>
		/// <param name="n">Items count.</param>
		public void TestN(int n)
		{
			ListView.DataSource = UtilitiesCollections.CreateList(n, x => x.ToString("00000"));
		}

		/// <summary>
		/// Set new list with 2 items.
		/// </summary>
		public void Test2()
		{
			TestN(2);
		}

		/// <summary>
		/// Set new list with 5 items.
		/// </summary>
		public void Test5()
		{
			TestN(5);
		}

		/// <summary>
		/// Set new list with 10 items.
		/// </summary>
		public void Test10()
		{
			TestN(10);
		}

		/// <summary>
		/// Set new list with 100 items.
		/// </summary>
		public void Test100()
		{
			TestN(100);
		}

		/// <summary>
		/// Set new list with 1000 items.
		/// </summary>
		public void Test1000()
		{
			TestN(1000);
		}

		/// <summary>
		/// Set new list with 10000 items.
		/// </summary>
		public void Test10000()
		{
			TestN(10000);
		}

		/// <summary>
		/// Set new list to ListViewIcons with specified items count.
		/// </summary>
		/// <param name="n">Items count.</param>
		public void TestiN(int n)
		{
			ListViewIcons.DataSource = UtilitiesCollections.CreateList(n, x => new ListViewIconsItemDescription()
			{
				Name = x.ToString("00000"),
			});
		}

		/// <summary>
		/// Set new list with 2 items.
		/// </summary>
		public void Testi2()
		{
			TestiN(2);
		}

		/// <summary>
		/// Set new list with 5 items.
		/// </summary>
		public void Testi5()
		{
			TestiN(5);
		}

		/// <summary>
		/// Set new list with 1000 items.
		/// </summary>
		public void Testi1000()
		{
			TestiN(1000);
		}

		/// <summary>
		/// Set new list with 10000 items.
		/// </summary>
		public void Testi10000()
		{
			TestiN(10000);
		}
	}
}