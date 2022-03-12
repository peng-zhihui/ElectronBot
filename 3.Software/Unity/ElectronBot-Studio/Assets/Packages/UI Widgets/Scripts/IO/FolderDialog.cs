namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Folder dialog.
	/// </summary>
	public class FolderDialog : Picker<string, FolderDialog>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public DirectoryTreeView DirectoryTreeView;

		/// <summary>
		/// OK button.
		/// </summary>
		[SerializeField]
		public Button OkButton;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(string defaultValue)
		{
			DirectoryTreeView.SelectDirectory(defaultValue);

			DirectoryTreeView.NodeSelected.AddListener(NodeChanged);
			DirectoryTreeView.NodeDeselected.AddListener(NodeChanged);
			OkButton.onClick.AddListener(OkClick);
			NodeChanged(null);
		}

		/// <summary>
		/// Handle selected node event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected virtual void NodeChanged(TreeNode<FileSystemEntry> node)
		{
			OkButton.interactable = DirectoryTreeView.SelectedNode != null;
		}

		/// <summary>
		/// Handle OkButton click.
		/// </summary>
		public void OkClick()
		{
			var node = DirectoryTreeView.SelectedNode;
			if (node == null)
			{
				return;
			}

			Selected(node.Item.FullName);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			DirectoryTreeView.NodeSelected.RemoveListener(NodeChanged);
			DirectoryTreeView.NodeDeselected.RemoveListener(NodeChanged);
			OkButton.onClick.RemoveListener(OkClick);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			DirectoryTreeView.SetStyle(style);

			style.Dialog.Button.ApplyTo(OkButton.gameObject);
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			DirectoryTreeView.GetStyle(style);

			style.Dialog.Button.GetFrom(OkButton.gameObject);
			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}