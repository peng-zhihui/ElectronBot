#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItems of an ListView.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListView SelectedItems Provider")]
	public class ListViewSelectedItemsProvider : ComponentDataProvider<UIWidgets.ListView, System.Collections.Generic.List<string>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListView target)
		{
			target.OnSelectString.AddListener(OnSelectStringListView);
			target.OnDeselectString.AddListener(OnDeselectStringListView);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<string> GetValue(UIWidgets.ListView target)
		{
			return target.SelectedItems;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListView target)
		{
			target.OnSelectString.RemoveListener(OnSelectStringListView);
			target.OnDeselectString.RemoveListener(OnDeselectStringListView);
		}

		void OnSelectStringListView(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}

		void OnDeselectStringListView(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif