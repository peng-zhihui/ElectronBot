namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test TreeViewCheckboxes.
	/// </summary>
	public class TestTreeViewCheckboxes : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeViewCheckboxes Tree;

		/// <summary>
		/// Start this instance.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			Tree.Init();

			SetTreeNodes();

			Tree.OnNodeCheckboxChanged.AddListener(CheckboxChanged);
			Tree.NodeSelected.AddListener(NodeSelected);
			Tree.NodeDeselected.AddListener(NodeDeselected);
		}

		/// <summary>
		/// Set TreeView nodes.
		/// </summary>
		protected void SetTreeNodes()
		{
			var nodes = new ObservableList<TreeNode<TreeViewCheckboxesItem>>
			{
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 2")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 3")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 4")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 5")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 6")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 7")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 8")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 9")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 10")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 11")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 12")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 13")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 14")),
			};

			nodes[0].Nodes = new ObservableList<TreeNode<TreeViewCheckboxesItem>>
			{
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-1")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-2")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-3")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-4")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-5")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-6")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-7")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-8")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-9")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-10")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-11")),
				new TreeNode<TreeViewCheckboxesItem>(new TreeViewCheckboxesItem("Item 1-12")),
			};

			Tree.Nodes = nodes;
		}

		/// <summary>
		/// Process node selected event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void NodeSelected(TreeNode<TreeViewCheckboxesItem> node)
		{
			Debug.Log(string.Format("{0} selected", node.Item.Name));
		}

		/// <summary>
		/// Process node deselected event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void NodeDeselected(TreeNode<TreeViewCheckboxesItem> node)
		{
			Debug.Log(string.Format("{0} deselected", node.Item.Name));
		}

		/// <summary>
		/// Handle checkbox changed event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void CheckboxChanged(TreeNode<TreeViewCheckboxesItem> node)
		{
			if (node.Item.Selected)
			{
				Debug.Log(string.Format("{0} checkbox selected", node.Item.Name));
			}
			else
			{
				Debug.Log(string.Format("{0} checkbox deselected", node.Item.Name));
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (Tree != null)
			{
				Tree.OnNodeCheckboxChanged.RemoveListener(CheckboxChanged);
				Tree.NodeSelected.RemoveListener(NodeSelected);
				Tree.NodeDeselected.RemoveListener(NodeDeselected);
			}
		}
	}
}