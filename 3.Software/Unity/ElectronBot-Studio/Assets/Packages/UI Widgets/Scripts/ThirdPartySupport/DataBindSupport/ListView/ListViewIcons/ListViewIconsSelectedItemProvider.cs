#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedItem of an ListViewIcons.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] ListViewIcons SelectedItem Provider")]
	public class ListViewIconsSelectedItemProvider : ComponentDataProvider<UIWidgets.ListViewIcons, UIWidgets.ListViewIconsItemDescription>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewIcons target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectListViewIcons);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewIcons);
		}

		/// <inheritdoc />
		protected override UIWidgets.ListViewIconsItemDescription GetValue(UIWidgets.ListViewIcons target)
		{
			return target.SelectedItem;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.ListViewIcons target)
		{
			target.OnSelectObject.RemoveListener(OnSelectObjectListViewIcons);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectListViewIcons);
		}

		void OnSelectObjectListViewIcons(int arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectListViewIcons(int arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif