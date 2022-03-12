namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Test DirectoryTreeView.
	/// </summary>
	public class TestDirectoryTreeView : MonoBehaviour
	{
		/// <summary>
		/// DirectoryTreeView.
		/// </summary>
		[SerializeField]
		protected DirectoryTreeView DirectoryTree;

		/// <summary>
		/// Select specified directory.
		/// </summary>
		/// <param name="path">Directory.</param>
		public void SelectDirectory(string path)
		{
			var selected = DirectoryTree.SelectDirectory(path);
			Debug.Log(selected ? "directory selected" : "directory not found");
		}

		/// <summary>
		/// Set specified root directory.
		/// </summary>
		/// <param name="path">Root directory.</param>
		public void SetRoot(string path)
		{
			DirectoryTree.RootDirectory = path;
		}

		/// <summary>
		/// Get selected node.
		/// </summary>
		public void GetSelected()
		{
			var node = DirectoryTree.SelectedNode;
			if (node != null)
			{
				Debug.Log(node.Item.FullName);
			}
		}
	}
}