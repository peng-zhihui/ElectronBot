namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test Tabs events.
	/// </summary>
	public class TestTabsEvents : MonoBehaviour
	{
		/// <summary>
		/// Tabs.
		/// </summary>
		[SerializeField]
		protected TabsIcons Tabs;

		int currentTabIndex;

		/// <summary>
		/// Process start.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void Start()
		{
			currentTabIndex = Array.IndexOf(Tabs.TabObjects, Tabs.SelectedTab);

			Tabs.OnTabSelect.AddListener(TabChanged);
		}

		void TabChanged(int newTabIndex)
		{
			Debug.Log(string.Format("deselected tab: {0}; index {1}", GetTabName(currentTabIndex), currentTabIndex.ToString()));
			Debug.Log(string.Format("selected tab: {0}; index {1}", GetTabName(newTabIndex), newTabIndex.ToString()));

			currentTabIndex = newTabIndex;
		}

		string GetTabName(int index)
		{
			if (index < 0 || index >= Tabs.TabObjects.Length)
			{
				return "none";
			}

			return Tabs.TabObjects[index].Name;
		}

		/// <summary>
		/// Process destroy.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			Tabs.OnTabSelect.RemoveListener(TabChanged);
		}
	}
}