namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using UnityEngine;

	/// <summary>
	/// Tree node.
	/// </summary>
	/// <typeparam name="TItem">Type of node item.</typeparam>
	public class TreeNode<TItem> : IObservable, INotifyPropertyChanged, IEquatable<TreeNode<TItem>>
	{
		/// <summary>
		/// Occurs when on change.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Occurs when a property value changes.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// The pause observation.
		/// </summary>
		public bool PauseObservation;

		[SerializeField]
		bool isVisible = true;

		/// <summary>
		/// Is node visible?
		/// </summary>
		/// <value><c>true</c> if this node is visible; otherwise, <c>false</c>.</value>
		public bool IsVisible
		{
			get
			{
				return isVisible;
			}

			set
			{
				if (isVisible != value)
				{
					isVisible = value;
					NotifyPropertyChanged("IsVisible");
				}
			}
		}

		[SerializeField]
		bool isExpanded;

		/// <summary>
		/// Is node expanded?
		/// </summary>
		/// <value><c>true</c> if this node is expanded; otherwise, <c>false</c>.</value>
		public bool IsExpanded
		{
			get
			{
				return isExpanded;
			}

			set
			{
				if (isExpanded != value)
				{
					isExpanded = value;
					NotifyPropertyChanged("IsExpanded");
				}
			}
		}

		[SerializeField]
		TItem item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public TItem Item
		{
			get
			{
				return item;
			}

			set
			{
				item = value;
				NotifyPropertyChanged("Item");
			}
		}

		[SerializeField]
		ObservableList<TreeNode<TItem>> nodes;

		/// <summary>
		/// Gets or sets the nodes.
		/// </summary>
		/// <value>The nodes.</value>
		public ObservableList<TreeNode<TItem>> Nodes
		{
			get
			{
				return nodes;
			}

			set
			{
				if (nodes != null)
				{
					nodes.OnChange -= NotifyPropertyChanged;
					nodes.OnCollectionChange -= CollectionChanged;

					for (int i = nodes.Count - 1; i >= 0; i--)
					{
						nodes[i].Parent = null;
					}
				}

				nodes = value;

				if (nodes != null)
				{
					CollectionChanged();

					nodes.OnChange += NotifyPropertyChanged;
					nodes.OnCollectionChange += CollectionChanged;
				}

				NotifyPropertyChanged("Nodes");
			}
		}

		/// <summary>
		/// Gets the total nodes count.
		/// </summary>
		/// <value>The total nodes count.</value>
		public int TotalNodesCount
		{
			get
			{
				if (nodes == null)
				{
					return 1;
				}

				var total = 1;

				for (int i = 0; i < nodes.Count; i++)
				{
					total += nodes[i].TotalNodesCount;
				}

				return total;
			}
		}

		/// <summary>
		/// The used nodes count.
		/// </summary>
		public int UsedNodesCount;

		/// <summary>
		/// Gets all used nodes count.
		/// </summary>
		/// <value>All used nodes count.</value>
		public int AllUsedNodesCount
		{
			get
			{
				if (!isVisible)
				{
					return 0;
				}

				if (!isExpanded)
				{
					return 0 + UsedNodesCount;
				}

				if (nodes == null)
				{
					return 0 + UsedNodesCount;
				}

				var total = UsedNodesCount;

				for (int i = 0; i < nodes.Count; i++)
				{
					total += nodes[i].AllUsedNodesCount;
				}

				return total;
			}
		}

		WeakReference parent;

		/// <summary>
		/// Gets or sets the parent (excluding TreeView.RootNode).
		/// </summary>
		/// <value>The parent.</value>
		public TreeNode<TItem> Parent
		{
			get
			{
				var result = RealParent;
				if ((result != null) && !IsNull(result.Item))
				{
					return result;
				}

				return null;
			}

			set
			{
				SetParentValue(value);
			}
		}

		/// <summary>
		/// Gets the real parent (including TreeView.RootNode).
		/// </summary>
		/// <value>The real parent.</value>
		public TreeNode<TItem> RealParent
		{
			get
			{
				if ((parent != null) && parent.IsAlive)
				{
					return parent.Target as TreeNode<TItem>;
				}

				return null;
			}
		}

		/// <summary>
		/// Gets the all parent node, current parent node in first.
		/// </summary>
		/// <value>The path.</value>
		public List<TreeNode<TItem>> Path
		{
			get
			{
				var result = new List<TreeNode<TItem>>();
				var current_parent = Parent;

				while (current_parent != null)
				{
					result.Add(current_parent);
					current_parent = current_parent.Parent;
				}

				var last = result.Count - 1;
				if ((last >= 0) && IsNull(result[last].Item))
				{
					result.RemoveAt(last);
				}

				return result;
			}
		}

		/// <summary>
		/// Gets the root node. Use object.ReferenceEquals to check if nodes belong to same tree.
		/// </summary>
		/// <value>The root node.</value>
		public TreeNode<TItem> RootNode
		{
			get
			{
				var current_parent = RealParent;

				while (current_parent != null)
				{
					if (current_parent.RealParent == null)
					{
						break;
					}

					current_parent = current_parent.RealParent;
				}

				return current_parent;
			}
		}

		/// <summary>
		/// The index in TreeView, used for optimization purpose.
		/// </summary>
		public int Index = -1;

		/// <summary>
		/// Determines whether this instance is parent of node the specified node.
		/// </summary>
		/// <returns><c>true</c> if this instance is parent of node the specified node; otherwise, <c>false</c>.</returns>
		/// <param name="node">Node.</param>
		public bool IsParentOfNode(TreeNode<TItem> node)
		{
			if (node == null)
			{
				return false;
			}

			var nodeParent = node.Parent;
			while (nodeParent != null)
			{
				if (nodeParent == this)
				{
					return true;
				}

				nodeParent = nodeParent.Parent;
			}

			return false;
		}

		/// <summary>
		/// Determines whether this instance can be child of the specified newParent.
		/// </summary>
		/// <returns><c>true</c> if this instance can be child of the specified newParent; otherwise, <c>false</c>.</returns>
		/// <param name="newParent">New parent.</param>
		public bool CanBeParent(TreeNode<TItem> newParent)
		{
			if (newParent == null)
			{
				return false;
			}

			if (this == newParent)
			{
				return false;
			}

			return !IsParentOfNode(newParent);
		}

		void SetParentValue(TreeNode<TItem> newParent)
		{
			var old_parent = RealParent;

			if (old_parent == newParent)
			{
				return;
			}

			if (newParent != null)
			{
				if (newParent == this)
				{
					throw new ArgumentException("Node cannot be own parent.");
				}

				if (IsParentOfNode(newParent))
				{
					throw new ArgumentException("Own child node cannot be parent node.");
				}
			}

			if (old_parent != null)
			{
				old_parent.nodes.OnCollectionChange -= old_parent.CollectionChanged;
				old_parent.nodes.Remove(this);
				old_parent.nodes.OnCollectionChange += old_parent.CollectionChanged;
			}

			if (newParent != null)
			{
				parent = new WeakReference(newParent);

				if (newParent.nodes == null)
				{
					newParent.nodes = new ObservableList<TreeNode<TItem>>();

					newParent.nodes.OnChange += newParent.NotifyPropertyChanged;
					newParent.nodes.OnCollectionChange += newParent.CollectionChanged;
				}

				newParent.nodes.OnCollectionChange -= newParent.CollectionChanged;
				newParent.nodes.Add(this);
				newParent.nodes.OnCollectionChange += newParent.CollectionChanged;
			}
			else
			{
				parent = null;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeNode{TItem}"/> class.
		/// </summary>
		/// <param name="nodeItem">Node item.</param>
		/// <param name="nodeNodes">Node nodes.</param>
		/// <param name="nodeIsExpanded">If set to <c>true</c> node is expanded.</param>
		/// <param name="nodeIsVisible">If set to <c>true</c> node is visible.</param>
		public TreeNode(
			TItem nodeItem,
			ObservableList<TreeNode<TItem>> nodeNodes = null,
			bool nodeIsExpanded = false,
			bool nodeIsVisible = true)
		{
			item = nodeItem;
			nodes = nodeNodes;

			isExpanded = nodeIsExpanded;
			isVisible = nodeIsVisible;

			if (nodes != null)
			{
				nodes.OnChange += NotifyPropertyChanged;
				nodes.OnCollectionChange += CollectionChanged;
				CollectionChanged();
			}
		}

		/// <summary>
		/// TItem is value type.
		/// </summary>
		private static readonly bool IsValueType;

		static TreeNode()
		{
			IsValueType = typeof(TItem).IsValueType;
		}

		/// <summary>
		/// Check if item is null.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>true if item is null; otherwise false.</returns>
		protected static bool IsNull(TItem item)
		{
			if (IsValueType)
			{
				return false;
			}

			return item == null;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeNode{TItem}"/> class.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="serializedNodes">List of serialized nodes.</param>
		public TreeNode(TreeNodeSerialized<TItem> node, List<TreeNodeSerialized<TItem>> serializedNodes)
		{
			PauseObservation = node.PauseObservation;
			isVisible = node.IsVisible;
			isExpanded = node.IsExpanded;
			item = node.Item;

			nodes = new ObservableList<TreeNode<TItem>>();
			for (int i = 0; i < node.SubNodesCount; i++)
			{
				nodes.Add(new TreeNode<TItem>(serializedNodes[node.FirstSubNodeIndex + i], serializedNodes));
			}

			nodes.OnChange += NotifyPropertyChanged;
			nodes.OnCollectionChange += CollectionChanged;
		}

		/// <summary>
		/// Finalizes an instance of the <see cref="TreeNode{TItem}"/> class.
		/// </summary>
		~TreeNode()
		{
			if (nodes != null)
			{
				nodes.OnChange -= NotifyPropertyChanged;
				nodes.OnCollectionChange -= CollectionChanged;
			}
		}

		void CollectionChanged()
		{
			if ((nodes == null) || (nodes.Count == 0))
			{
				return;
			}

			nodes.BeginUpdate();

			foreach (var node in nodes)
			{
				var node_parent = node.Parent;

				if (node_parent != this)
				{
					if ((node_parent != null) && (node_parent.Nodes != null))
					{
						if (ReferenceEquals(nodes, node_parent.Nodes))
						{
							node_parent.Nodes = new ObservableList<TreeNode<TItem>>();
						}
						else
						{
							node_parent.Nodes.Remove(node);
						}
					}

					node.parent = new WeakReference(this);
				}
			}

			nodes.EndUpdate();
		}

		void NotifyPropertyChanged()
		{
			NotifyPropertyChanged("Nodes");
		}

		void NotifyPropertyChanged(string propertyName)
		{
			if (PauseObservation)
			{
				return;
			}

			var c_handlers = OnChange;
			if (c_handlers != null)
			{
				c_handlers();
			}

			var handlers = PropertyChanged;
			if (handlers != null)
			{
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		/// <summary>
		/// Remove this node from tree.
		/// </summary>
		public void RemoveFromTree()
		{
			Parent = null;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
		public override bool Equals(object other)
		{
			if (other is TreeNode<TItem>)
			{
				return Equals((TreeNode<TItem>)other);
			}

			return false;
		}

		/// <summary>
		/// Determines whether the specified object is equal to the current object.
		/// </summary>
		/// <param name="other">The object to compare with the current object.</param>
		/// <returns><c>true</c> if the specified object is equal to the current object; otherwise, <c>false</c>.</returns>
		public virtual bool Equals(TreeNode<TItem> other)
		{
			return ReferenceEquals(this, other);
		}

		/// <summary>
		/// Returns true if the nodes items are equal, false otherwise.
		/// </summary>
		/// <param name="a">The first object.</param>
		/// <param name="b">The second object.</param>
		/// <returns>true if the objects equal; otherwise, false.</returns>
		public static bool operator ==(TreeNode<TItem> a, TreeNode<TItem> b)
		{
			if (ReferenceEquals(a, null))
			{
				return ReferenceEquals(b, null);
			}

			return a.Equals(b);
		}

		/// <summary>
		/// Returns true if the nodes items are not equal, false otherwise.
		/// </summary>
		/// <param name="a">The first object.</param>
		/// <param name="b">The second object.</param>
		/// <returns>true if the objects not equal; otherwise, false.</returns>
		public static bool operator !=(TreeNode<TItem> a, TreeNode<TItem> b)
		{
			return !(a == b);
		}

		/// <summary>
		/// Serves as a hash function for a particular type.
		/// </summary>
		/// <returns>A hash code for this instance that is suitable for use in hashing algorithms and data structures such as a hash table.</returns>
		public override int GetHashCode()
		{
			return nodes != null ? nodes.GetHashCode() : 0;
		}

		/// <summary>
		/// Serialize the specified nodes.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <returns>Flat list of nodes.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Reviewed.")]
		public static List<TreeNodeSerialized<TItem>> Serialize(ObservableList<TreeNode<TItem>> nodes)
		{
			if (nodes == null)
			{
				return new List<TreeNodeSerialized<TItem>>();
			}

			var serializedNodes = new List<TreeNodeSerialized<TItem>>();
			Serialize(nodes, serializedNodes, 0);

			return serializedNodes;
		}

		/// <summary>
		/// Serialize the specified nodes.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="serializedNodes">List of serialized nodes.</param>
		/// <param name="depth">Depth.</param>
		/// <returns>Flat list of nodes.</returns>
		protected static List<TreeNodeSerialized<TItem>> Serialize(ObservableList<TreeNode<TItem>> nodes, List<TreeNodeSerialized<TItem>> serializedNodes, int depth)
		{
			foreach (var node in nodes)
			{
				serializedNodes.Add(new TreeNodeSerialized<TItem>(node, serializedNodes.Count + 1, depth));
				if (node.Nodes != null)
				{
					Serialize(node.Nodes, serializedNodes, depth + 1);
				}
			}

			return serializedNodes;
		}

		/// <summary>
		/// De-serialize the specified nodes.
		/// </summary>
		/// <param name="serializedNodes">Serialized nodes.</param>
		/// <returns>De-serialized nodes.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Reviewed.")]
		public static ObservableList<TreeNode<TItem>> Deserialize(List<TreeNodeSerialized<TItem>> serializedNodes)
		{
			var result = new ObservableList<TreeNode<TItem>>();

			if (serializedNodes == null)
			{
				return result;
			}

			foreach (var serialized_node in serializedNodes)
			{
				if (serialized_node.Depth == 0)
				{
					result.Add(new TreeNode<TItem>(serialized_node, serializedNodes));
				}
			}

			return result;
		}

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			return string.Format("TreeNode<{0}>(Item = {1})", UtilitiesEditor.GetFriendlyTypeName(typeof(TItem)), IsNull(item) ? "null" : item.ToString());
		}
	}
}