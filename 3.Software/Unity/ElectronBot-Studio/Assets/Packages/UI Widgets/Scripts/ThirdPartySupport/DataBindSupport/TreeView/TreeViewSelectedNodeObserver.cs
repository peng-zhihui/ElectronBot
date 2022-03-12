#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Observers;

	/// <summary>
	/// Observes value changes of the SelectedNode of an TreeView.
	/// </summary>
	public class TreeViewSelectedNodeObserver : ComponentDataObserver<UIWidgets.TreeView, UIWidgets.TreeNode<UIWidgets.TreeViewItem>>
	{
		/// <inheritdoc />
		protected override void AddListener(UIWidgets.TreeView target)
		{
			target.NodeSelected.AddListener(NodeSelectedTreeView);
			target.NodeDeselected.AddListener(NodeDeselectedTreeView);
		}

		/// <inheritdoc />
		protected override UIWidgets.TreeNode<UIWidgets.TreeViewItem> GetValue(UIWidgets.TreeView target)
		{
			return target.SelectedNode;
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