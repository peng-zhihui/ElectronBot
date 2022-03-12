#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Providers.Getters;
	using UnityEngine;

	/// <summary>
	/// Provides the SelectedNodes of an TreeView.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Getters/[DB] TreeView SelectedNodes Provider")]
	public class TreeViewSelectedNodesProvider : ComponentDataProvider<UIWidgets.TreeView, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.TreeViewItem>>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.TreeView target)
		{
			target.NodeSelected.AddListener(NodeSelectedTreeView);
			target.NodeDeselected.AddListener(NodeDeselectedTreeView);
		}

		/// <inheritdoc />
		protected override System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.TreeViewItem>> GetValue(UIWidgets.TreeView target)
		{
			return target.SelectedNodes;
		}

		/// <inheritdoc />
		protected override void RemoveListener(UIWidgets.TreeView target)
		{
			target.NodeSelected.RemoveListener(NodeSelectedTreeView);
			target.NodeDeselected.RemoveListener(NodeDeselectedTreeView);
		}

		void NodeSelectedTreeView(UIWidgets.TreeNode<UIWidgets.TreeViewItem> arg0)
		{
			OnTargetValueChanged();
		}

		void NodeDeselectedTreeView(UIWidgets.TreeNode<UIWidgets.TreeViewItem> arg0)
		{
			OnTargetValueChanged();
		}
	}
}
#endif