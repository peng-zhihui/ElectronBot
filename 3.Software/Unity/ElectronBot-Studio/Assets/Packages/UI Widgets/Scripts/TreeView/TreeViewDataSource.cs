namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// TreeViewDataSource.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Collections/TreeView DataSource")]
	[RequireComponent(typeof(TreeView))]
	[DisallowMultipleComponent]
	public class TreeViewDataSource : MonoBehaviour
	{
		/// <summary>
		/// The data.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TreeViewDataSourceItem> Data = new List<TreeViewDataSourceItem>();

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;
			SetDataSource();
		}

		/// <summary>
		/// Sets the data source.
		/// </summary>
		public virtual void SetDataSource()
		{
			var tree = GetComponent<TreeView>();
			tree.Init();

			var nodes = new ObservableList<TreeNode<TreeViewItem>>();
			List2Tree(nodes);
			tree.Nodes = nodes;
		}

		/// <summary>
		/// Convert flat list to tree.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		public virtual void List2Tree(ObservableList<TreeNode<TreeViewItem>> nodes)
		{
			TreeNode<TreeViewItem> last_node = null;
			for (int i = 0; i < Data.Count; i++)
			{
				var item = Data[i];
				item.IsVisible = true;

				if (item.Depth == 0)
				{
					last_node = Item2Node(item);
					nodes.Add(last_node);
				}
				else if (item.Depth == (Data[i - 1].Depth + 1))
				{
					var current_node = Item2Node(item);
					last_node.Nodes.Add(current_node);

					last_node = current_node;
				}
				else if (item.Depth <= Data[i - 1].Depth)
				{
					var n = Data[i - 1].Depth + 1 - item.Depth;

					for (int j = 0; j < n; j++)
					{
						last_node = last_node.Parent;
					}

					var current_node = Item2Node(item);
					last_node.Nodes.Add(current_node);

					last_node = current_node;
				}
				else
				{
					// Debug.LogWarning("Unknown case");
				}
			}
		}

		/// <summary>
		/// Convert item to node.
		/// </summary>
		/// <returns>The node.</returns>
		/// <param name="item">Item.</param>
		protected virtual TreeNode<TreeViewItem> Item2Node(TreeViewDataSourceItem item)
		{
			var nodeItem = new TreeViewItem(item.Name, item.Icon)
			{
				Value = item.Value,
				Tag = item.Tag,
			};

			return new TreeNode<TreeViewItem>(nodeItem, new ObservableList<TreeNode<TreeViewItem>>(), item.IsExpanded, item.IsVisible);
		}
	}
}