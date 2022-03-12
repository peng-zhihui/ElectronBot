namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test Tabs creation.
	/// </summary>
	public class TestTabsCreate : MonoBehaviour
	{
		/// <summary>
		/// The tabs.
		/// </summary>
		[SerializeField]
		protected Tabs Tabs;

		/// <summary>
		/// The template.
		/// </summary>
		[SerializeField]
		public GameObject Template;

		/// <summary>
		/// Create tabs.
		/// </summary>
		/// <param name="count">Tabs count.</param>
		public void SetTabs(int count)
		{
			ClearTabs();
			Tabs.TabObjects = CreateTabs(count);
		}

		/// <summary>
		/// Clears the tabs.
		/// </summary>
		public void ClearTabs()
		{
			foreach (var tab in Tabs.TabObjects)
			{
				Destroy(tab.TabObject);
			}

			Tabs.TabObjects = Compatibility.EmptyArray<Tab>();
		}

		Tab[] CreateTabs(int count)
		{
			var tabs = new Tab[count];

			for (int i = 0; i < count; i++)
			{
				var tab_name = string.Format("Tab {0}", (i + 1).ToString());
				tabs[i] = new Tab()
				{
					Name = tab_name,
					TabObject = CreateTabObject(tab_name),
				};
			}

			return tabs;
		}

		GameObject CreateTabObject(string tabName)
		{
			var result = Compatibility.Instantiate(Template);
			result.name = tabName;
			result.transform.SetParent(Template.transform.parent, false);
			result.SetActive(true);
			result.GetComponentInChildren<TextAdapter>().text = tabName;

			return result.gameObject;
		}
	}
}