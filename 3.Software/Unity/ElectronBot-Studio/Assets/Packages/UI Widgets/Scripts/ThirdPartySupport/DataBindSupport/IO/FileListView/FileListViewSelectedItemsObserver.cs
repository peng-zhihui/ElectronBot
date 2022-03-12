#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItems of an FileListView.
	/// </summary>
	public class FileListViewSelectedItemsObserver : ComponentDataObserver<UIWidgets.FileListView, System.Collections.Generic.List<UIWidgets.FileSystemEntry>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.FileListView target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectFileListView);
			target.OnDeselectObject.AddListener(OnDeselectObjectFileListView);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<UIWidgets.FileSystemEntry> GetValue(UIWidgets.FileListView target)
		{
			return target.SelectedItems;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.FileListView target)
		{
			target.OnSelectObject.RemoveListener(OnSelectObjectFileListView);
			target.OnDeselectObject.RemoveListener(OnDeselectObjectFileListView);
		}

		void OnSelectObjectFileListView(int arg0)
		{
			OnTargetValueChanged();
		}

		void OnDeselectObjectFileListView(int arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif