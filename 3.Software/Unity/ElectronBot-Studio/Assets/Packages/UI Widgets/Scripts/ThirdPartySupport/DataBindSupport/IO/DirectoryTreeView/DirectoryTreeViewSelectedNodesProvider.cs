#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedNodes of an DirectoryTreeView.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] DirectoryTreeView SelectedNodes Provider")]
	public class DirectoryTreeViewSelectedNodesProvider : ComponentDataProvider<UIWidgets.DirectoryTreeView, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.DirectoryTreeView target)
		{
			target.NodeSelected.AddListener(NodeSelectedDirectoryTreeView);
			target.NodeDeselected.AddListener(NodeDeselectedDirectoryTreeView);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>> GetValue(UIWidgets.DirectoryTreeView target)
		{
			return target.SelectedNodes;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.DirectoryTreeView target)
		{
			target.NodeSelected.RemoveListener(NodeSelectedDirectoryTreeView);
			target.NodeDeselected.RemoveListener(NodeDeselectedDirectoryTreeView);
		}

		void NodeSelectedDirectoryTreeView(UIWidgets.TreeNode<UIWidgets.FileSystemEntry> arg0)
		{
			OnTargetValueChanged();
		}

		void NodeDeselectedDirectoryTreeView(UIWidgets.TreeNode<UIWidgets.FileSystemEntry> arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif