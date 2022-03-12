namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TabsIcons.
	/// </summary>
	public class TestTabsIcons : MonoBehaviour
	{
		/// <summary>
		/// Tabs.
		/// </summary>
		[SerializeField]
		protected TabsIcons Tabs;

		/// <summary>
		/// TileView template.
		/// </summary>
		[SerializeField]
		protected TileViewIcons TileViewTemplate;

		static ObservableList<ListViewIconsItemDescription> GenerateRandomData(string tabName)
		{
			var result = new ObservableList<ListViewIconsItemDescription>();

			for (int i = 0; i < UnityEngine.Random.Range(0, 4); i++)
			{
				var item = new ListViewIconsItemDescription()
				{
					Name = string.Format("{0} : item {1}", tabName, (i + 1).ToString()),
				};
				result.Add(item);
			}

			return result;
		}

		/// <summary>
		/// Show tabs with data.
		/// </summary>
		public void ShowTabsWithData()
		{
			SetRandomTabs();

			// display only tabs where tileview have items
			var tabs_to_display = Array.FindAll(Tabs.TabObjects, x => x.TabObject.GetComponent<TileViewIcons>().DataSource.Count > 0);

			// destroy tab objects with zero items
			foreach (var x in Tabs.TabObjects)
			{
				if (x.TabObject.GetComponent<TileViewIcons>().DataSource.Count == 0)
				{
					Destroy(x.TabObject);
					x.TabObject = null;
				}
			}

			if (tabs_to_display.Length > 0)
			{
				Tabs.TabObjects = tabs_to_display;
			}
			else
			{
				// repeat if all tabs empty
				ShowTabsWithData();
			}
		}

		/// <summary>
		/// Show all tabs.
		/// </summary>
		public void ShowTabsAll()
		{
			SetRandomTabs();

			// disable tabs where tileview have zero items
			foreach (var x in Tabs.TabObjects)
			{
				if (x.TabObject.GetComponent<TileViewIcons>().DataSource.Count > 0)
				{
					Tabs.EnableTab(x);
				}
				else
				{
					Tabs.DisableTab(x);
				}
			}

			// select first tab with items if exists
			var first_tab = Array.Find(Tabs.TabObjects, x => x.TabObject.GetComponent<TileViewIcons>().DataSource.Count > 0);
			if (first_tab != null)
			{
				Tabs.SelectTab(first_tab);
			}
		}

		void SetRandomTabs()
		{
			if (Tabs.gameObject.activeInHierarchy)
			{
				// destroy existing tileviews
				foreach (var tab in Tabs.TabObjects)
				{
					Destroy(tab.TabObject);
				}
			}
			else
			{
				// enable tabs
				Tabs.gameObject.SetActive(true);
			}

			// create tabs with random data
			Tabs.TabObjects = CreateTabs(5);
		}

		// create tabs
		TabIcons[] CreateTabs(int count)
		{
			var tabs = new TabIcons[count];

			for (int i = 0; i < count; i++)
			{
				var tab_name = string.Format("Tab {0}", (i + 1).ToString());
				tabs[i] = new TabIcons()
				{
					Name = tab_name,
					TabObject = CreateTabObject(tab_name),
				};
			}

			return tabs;
		}

		GameObject CreateTabObject(string tabName)
		{
			var result = Compatibility.Instantiate(TileViewTemplate);
			result.transform.SetParent(TileViewTemplate.transform.parent, false);
			result.gameObject.SetActive(true);
			result.DataSource = GenerateRandomData(tabName);

			return result.gameObject;
		}
	}
}