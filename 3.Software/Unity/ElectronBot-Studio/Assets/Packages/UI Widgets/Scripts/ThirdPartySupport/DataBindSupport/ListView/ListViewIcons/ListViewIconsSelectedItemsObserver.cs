#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItems of an ListViewIcons.
	/// </summary>
	public class ListViewIconsSelectedItemsObserver : ComponentDataObserver<UIWidgets.ListViewIcons, System.Collections.Generic.List<UIWidgets.ListViewIconsItemDescription>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.ListViewIcons target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectListViewIcons);
			target.OnDeselectObject.AddListener(OnDeselectObjectListViewIcons);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<UIWidgets.ListViewIconsItemDescription> GetValue(UIWidgets.ListViewIcons target)
		{
			return target.SelectedItems;
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