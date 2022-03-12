#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItem of an ListView.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListView SelectedItem Provider")]
	public class ListViewSelectedItemProvider : ComponentDataProvider<UIWidgets.ListView, string>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListView target)
		{
			target.OnSelectString.AddListener(OnSelectStringListView);
			target.OnDeselectString.AddListener(OnDeselectStringListView);
		}

		/// <inheritdoc />
		protected override string GetValue(UIWidgets.ListView target)
		{
			return target.SelectedItem;
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