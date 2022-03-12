namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// DirectoryTreeViewComponent
	/// </summary>
	public class DirectoryTreeViewComponent : TreeViewComponentBase<FileSystemEntry>
	{
		/// <summary>
		/// Current item.
		/// </summary>
		[HideInInspector]
		public FileSystemEntry Item;

		/// <summary>
		/// Sets the data.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="depth">Depth.</param>
		public override void SetData(TreeNode<FileSystemEntry> node, int depth)
		{
			Node = node;
			base.SetData(Node, depth);

			UpdateView();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			TextAdapter.text = Node.Item.DisplayName;
		}
	}
}