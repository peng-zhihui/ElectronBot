#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Nodes of a DirectoryTreeView depending on the UIWidgets.ObservableList{UIWidgets.TreeNode{UIWidgets.FileSystemEntry}} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] DirectoryTreeView Nodes Setter")]
	public class DirectoryTreeViewNodesSetter : ComponentSingleSetter<UIWidgets.DirectoryTreeView, UIWidgets.ObservableList<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.DirectoryTreeView target, UIWidgets.ObservableList<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>> value)
		{
			target.Nodes = value;
		}
	}
}
#endif