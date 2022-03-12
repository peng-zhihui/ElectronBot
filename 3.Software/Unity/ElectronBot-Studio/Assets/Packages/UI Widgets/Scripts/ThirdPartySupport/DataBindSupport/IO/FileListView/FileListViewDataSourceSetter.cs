#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the DataSource of a FileListView depending on the UIWidgets.ObservableList{UIWidgets.FileSystemEntry} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] FileListView DataSource Setter")]
	public class FileListViewDataSourceSetter : ComponentSingleSetter<UIWidgets.FileListView, UIWidgets.ObservableList<UIWidgets.FileSystemEntry>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.FileListView target, UIWidgets.ObservableList<UIWidgets.FileSystemEntry> value)
		{
			target.DataSource = value;
		}
	}
}
#endif