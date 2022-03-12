namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TreeView tag.
	/// </summary>
	public class TestTreeViewTag : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView Tree;

		/// <summary>
		/// Starts this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void Start()
		{
			// Set nodes with specified tag
			Tree.Nodes[0].Item.Tag = GameObject.Find("Test GameObject");

			// Add callbacks
			Tree.NodeSelected.AddListener(OnSelect);
			Tree.NodeDeselected.AddListener(OnDeselect);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected void OnDestroy()
		{
			Tree.NodeSelected.RemoveListener(OnSelect);
			Tree.NodeDeselected.RemoveListener(OnDeselect);
		}

		void OnSelect(TreeNode<TreeViewItem> node)
		{
			var go = node.Item.Tag as GameObject;
			if (go != null)
			{
				go.SetActive(true);
			}
		}

		void OnDeselect(TreeNode<TreeViewItem> node)
		{
			var go = node.Item.Tag as GameObject;
			if (go != null)
			{
				go.SetActive(false);
			}
		}
	}
}