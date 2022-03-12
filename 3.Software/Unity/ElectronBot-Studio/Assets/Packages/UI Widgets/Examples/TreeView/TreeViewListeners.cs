namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TreeView listeners.
	/// </summary>
	public class TreeViewListeners : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		public TreeView Tree;

		/// <summary>
		/// Adds listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			Tree.NodeSelected.AddListener(NodeSelected);
			Tree.NodeDeselected.AddListener(NodeDeselected);
		}

		/// <summary>
		/// Handle node selected event.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void NodeSelected(TreeNode<TreeViewItem> node)
		{
			Debug.Log(string.Format("{0} selected", node.Item.Name));
		}

		/// <summary>
		/// Handle node deselected event.
		/// </summary>
		/// <param name="node">Node.</param>
		public virtual void NodeDeselected(TreeNode<TreeViewItem> node)
		{
			Debug.Log(string.Format("{0} deselected", node.Item.Name));
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (Tree != null)
			{
				Tree.NodeSelected.RemoveListener(NodeSelected);
				Tree.NodeDeselected.RemoveListener(NodeDeselected);
			}
		}
	}
}