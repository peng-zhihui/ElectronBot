#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedItem of an FileListView.
	/// </summary>
	public class FileListViewSelectedItemObserver : ComponentDataObserver<UIWidgets.FileListView, UIWidgets.FileSystemEntry>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.FileListView target)
		{
			target.OnSelectObject.AddListener(OnSelectObjectFileListView);
			target.OnDeselectObject.AddListener(OnDeselectObjectFileListView);
		}

		/// <inheritdoc />
		protected override UIWidgets.FileSystemEntry GetValue(UIWidgets.FileListView target)
		{
			return target.SelectedItem;
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