namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test Tabs.
	/// </summary>
	public class TestTabs : MonoBehaviour
	{
		/// <summary>
		/// Tabs.
		/// </summary>
		[SerializeField]
		protected Tabs Tabs;

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			Tabs.OnTabSelect.AddListener(Test2);
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (Tabs != null)
			{
				Tabs.OnTabSelect.RemoveListener(Test2);
			}
		}

		/// <summary>
		/// Log selected tab info.
		/// </summary>
		/// <param name="index">Index.</param>
		public void Test2(int index)
		{
			Debug.Log(string.Format("Name: {0}", Tabs.SelectedTab.Name));
			Debug.Log(string.Format("Index: {0}", index.ToString()));
			Debug.Log(string.Format("Index: {0}", Array.IndexOf(Tabs.TabObjects, Tabs.SelectedTab).ToString()));
		}
	}
}