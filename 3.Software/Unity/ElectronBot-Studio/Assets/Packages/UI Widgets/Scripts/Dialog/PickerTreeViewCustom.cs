namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Picker for the TreeViewCustom.
	/// </summary>
	/// <typeparam name="TTreeView">Type of the TreeView.</typeparam>
	/// <typeparam name="TTreeViewComponent">Type of the TreeView component.</typeparam>
	/// <typeparam name="TValue">Type of the value.</typeparam>
	/// <typeparam name="TNode">Type of the node.</typeparam>
	/// <typeparam name="TPicker">Type of the this picker.</typeparam>
	public class PickerTreeViewCustom<TTreeView, TTreeViewComponent, TValue, TNode, TPicker> : Picker<TNode, TPicker>
		where TTreeView : TreeViewCustom<TTreeViewComponent, TValue>
		where TTreeViewComponent : TreeViewComponentBase<TValue>
		where TNode : TreeNode<TValue>
		where TPicker : Picker<TNode, TPicker>
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TTreeView TreeView;

		/// <summary>
		/// OK button.
		/// </summary>
		[SerializeField]
		public Button OkButton;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(TNode defaultValue)
		{
			TreeView.SelectedIndex = Node2Index(defaultValue);

			TreeView.NodeSelected.AddListener(NodeChanged);
			TreeView.NodeDeselected.AddListener(NodeChanged);
			OkButton.onClick.AddListener(OkClick);
			NodeChanged(null);
		}

		/// <summary>
		/// Get node index.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <returns>Index.</returns>
		protected int Node2Index(TNode node)
		{
			for (int i = 0; i < TreeView.DataSource.Count; i++)
			{
				if (TreeView.DataSource[i].Node.Equals(node))
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Handle selected node event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected virtual void NodeChanged(TreeNode<TValue> node)
		{
			OkButton.interactable = TreeView.SelectedNode != null;
		}

		/// <summary>
		/// Handle OkButton click.
		/// </summary>
		public void OkClick()
		{
			var node = TreeView.SelectedNode;
			if (node == null)
			{
				return;
			}

			Selected(node as TNode);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			TreeView.NodeSelected.RemoveListener(NodeChanged);
			TreeView.NodeDeselected.RemoveListener(NodeChanged);
			OkButton.onClick.RemoveListener(OkClick);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			TreeView.SetStyle(style);

			style.Dialog.Button.ApplyTo(OkButton.gameObject);
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			TreeView.GetStyle(style);

			style.Dialog.Button.GetFrom(OkButton.gameObject);
			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}