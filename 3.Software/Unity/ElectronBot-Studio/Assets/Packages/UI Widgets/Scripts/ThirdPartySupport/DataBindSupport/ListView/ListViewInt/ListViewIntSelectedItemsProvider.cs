#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItems of an ListViewInt.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewInt SelectedItems Provider")]
	public class ListViewIntSelectedItemsProvider : ComponentDataProvider<UIWidgets.ListViewInt, System.Collections.Generic.List<int>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewInt target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectListViewInt);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewInt);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<int> GetValue(UIWidgets.ListViewInt target)
		{
			return target.SelectedItems;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewInt target)
		{
			target.OnSelectObject.RemoveListener(OnSelectObjectListViewInt);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewInt);
		}

		void OnSelectObjectListViewInt(int arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewInt(int arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif