namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Create TreeView data from hierarchy.
	/// </summary>
	public class TreeViewDataFromHierarchy : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		public TreeView TreeView;

		/// <summary>
		/// Process the start event.
		/// </summary>
		public virtual void Start()
		{
			AllGameObjects();
		}

		/// <summary>
		/// Show all gameobjects in TreeView.
		/// </summary>
		public void AllGameObjects()
		{
			TreeView.Nodes = Hierarchy2Data();
		}

		/// <summary>
		/// Show nested gameobjects in TreeView.
		/// </summary>
		/// <param name="parent">Parent.</param>
		public void NestedGameObject(Transform parent)
		{
			TreeView.Nodes = Hierarchy2Data(parent);
		}

		ObservableList<TreeNode<TreeViewItem>> Hierarchy2Data()
		{
			var nodes = new ObservableList<TreeNode<TreeViewItem>>();
			var roots = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
			foreach (var go in roots)
			{
				var item = new TreeViewItem(go.name);
				item.Tag = go;

				var node = new TreeNode<TreeViewItem>(item);
				node.Nodes = Hierarchy2Data(go.transform);

				nodes.Add(node);
			}

			return nodes;
		}

		ObservableList<TreeNode<TreeViewItem>> Hierarchy2Data(Transform source)
		{
			var nodes = new ObservableList<TreeNode<TreeViewItem>>();

			for (int i = 0; i < source.childCount; i++)
			{
				var child = source.GetChild(i);
				var item = new TreeViewItem(child.name);
				item.Tag = child.gameObject;

				var node = new TreeNode<TreeViewItem>(item);
				node.Nodes = Hierarchy2Data(child);

				nodes.Add(node);
			}

			return nodes;
		}
	}
}