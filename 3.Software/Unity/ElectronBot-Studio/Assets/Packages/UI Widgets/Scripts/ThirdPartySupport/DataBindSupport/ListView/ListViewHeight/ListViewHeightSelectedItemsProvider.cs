#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItems of an ListViewHeight.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewHeight SelectedItems Provider")]
	public class ListViewHeightSelectedItemsProvider : ComponentDataProvider<UIWidgets.ListViewHeight, System.Collections.Generic.List<string>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewHeight target)
		{
			target.OnSelectString.AddListener(OnSelectStringListViewHeight);
			target.OnDeselectString.AddListener(OnDeselectStringListViewHeight);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<string> GetValue(UIWidgets.ListViewHeight target)
		{
			return target.SelectedItems;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewHeight target)
		{
			target.OnSelectString.RemoveListener(OnSelectStringListViewHeight);
			target.OnDeselectString.RemoveListener(OnDeselectStringListViewHeight);
		}

		void OnSelectStringListViewHeight(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}

		void OnDeselectStringListViewHeight(int arg0, string arg1)
		{
			OnTargetValueChanged();
		}
	}
}
#endif