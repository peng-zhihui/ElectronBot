namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Test TreeView.
	/// </summary>
	public class TestTreeView : MonoBehaviour
	{
		/// <summary>
		/// TreeView.
		/// </summary>
		[SerializeField]
		public TreeView Tree;

		/// <summary>
		/// Nodes.
		/// </summary>
		protected ObservableList<TreeNode<TreeViewItem>> Nodes;

		/// <summary>
		/// UINodes.
		/// </summary>
		protected ObservableList<TreeNode<TreeViewItem>> UINodes;

		/// <summary>
		/// Default nodes.
		/// </summary>
		protected Dictionary<string, ObservableList<TreeNode<TreeViewItem>>> DefaultNodes;

		/// <summary>
		/// Initializes a new instance of the <see cref="TestTreeView"/> class.
		/// </summary>
		public TestTreeView()
		{
			var config = new List<int>() { 10, 4, 5, 5, };

			Nodes = GenerateTreeNodes(config, isExpanded: false);
		}

		/// <summary>
		/// Process selected nodes.
		/// </summary>
		public void ProcessSelectedNodes()
		{
			foreach (var node in Tree.SelectedNodes)
			{
				// do something with selected node
				Debug.Log(node.Item.Name);

				var component = Tree.GetItemComponent(node.Index);

				// not visible component will be null
				if (component != null)
				{
					// do something with component
					// component.ToggleEye();
					Debug.Log(component);
				}
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Tree.Init();

			Tree.Nodes = Nodes;

			// Tree.OnSelect.AddListener(ProcessSelectedNode);
			// SetComparison();
		}

		/// <summary>
		/// Handle selected node event.
		/// Deselect node if match some condition.
		/// </summary>
		/// <param name="node">Node.</param>
		protected virtual void ProcessSelectedNode(TreeNode<TreeViewItem> node)
		{
			if (node.TotalNodesCount > 1)
			{
				Tree.Deselect(node.Index);
			}
		}

		/// <summary>
		/// Log selected node.
		/// </summary>
		public void TestSelect()
		{
			Debug.Log(Tree.SelectedIndex.ToString());
			Debug.Log(Tree.SelectedItem.Node.Item.Name);
		}

		/// <summary>
		/// Add listener.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public void TestSelectAll()
		{
			Tree.NodeDeselected.AddListener(OnNodeDeselected);
		}

		/// <summary>
		/// Scroll to node.
		/// </summary>
		public void ScrollToNode()
		{
			var target = Tree.Nodes[9].Nodes[2].Nodes[1].Nodes[3];
			Tree.ScrollToAnimated(target);
			Tree.Select(target);
		}

		/// <summary>
		/// Is now handle selected node?
		/// </summary>
		protected bool handleSelected;

		/// <summary>
		/// Handle node deselected event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void OnNodeDeselected(TreeNode<TreeViewItem> node)
		{
			if (handleSelected)
			{
				return;
			}

			handleSelected = true;

			Tree.DeselectNodeWithSubnodes(node);

			handleSelected = false;
		}

		/// <summary>
		/// Handle node selected event.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void OnNodeSelected(TreeNode<TreeViewItem> node)
		{
			if (handleSelected)
			{
				return;
			}

			handleSelected = true;

			Tree.SelectNodeWithSubnodes(node);

			handleSelected = false;
		}

		/// <summary>
		/// Select fist node with sub-nodes and measure how much time it's takes.
		/// </summary>
		public void SelectFirstNodeWithSubnodes()
		{
			Tree.SelectNodeWithSubnodes(Nodes[0]);
		}

		/// <summary>
		/// Select first sub-node from second node.
		/// </summary>
		public void SelectNode()
		{
			Tree.Select(Nodes[1].Nodes[0]);
		}

		/// <summary>
		/// Select node with sub-nodes.
		/// </summary>
		public void SelectNodeWithSubnodes()
		{
			Tree.SelectNodeWithSubnodes(Nodes[1].Nodes[1]);
		}

		/// <summary>
		/// Select node and scroll to it.
		/// </summary>
		public void TestSelectAndScrollTo()
		{
			SelectAndScrollTo(Nodes[2].Nodes[1].Nodes[1]);
		}

		/// <summary>
		/// Select node and scroll to it.
		/// </summary>
		/// <param name="node">Node.</param>
		public void SelectAndScrollTo(TreeNode<TreeViewItem> node)
		{
			// expand parent nodes
			Tree.ExpandParentNodes(node);

			// select node
			Tree.Select(node);

			// scroll to node immediately
			// Tree.ScrollTo(Tree.SelectedIndex);

			// scroll to node animated
			Tree.ScrollToAnimated(Tree.SelectedIndex);
		}

		/// <summary>
		/// Use duplicates.
		/// </summary>
		public void UseDuplicates()
		{
			Nodes.OnChange += UpdateUINodes;

			UpdateUINodes();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Nodes != null)
			{
				Nodes.OnChange -= UpdateUINodes;
			}
		}

		void UpdateUINodes()
		{
			UINodes = DuplicateNodes(Nodes);

			Tree.Nodes = UINodes;
		}

		ObservableList<TreeNode<T>> DuplicateNodes<T>(ObservableList<TreeNode<T>> source)
		{
			var result = new ObservableList<TreeNode<T>>();

			foreach (var node in source)
			{
				result.Add(new TreeNode<T>(
					node.Item,
					(node.Nodes == null) ? null : DuplicateNodes(node.Nodes),
					node.IsExpanded,
					node.IsVisible));
			}

			return result;
		}

		/// <summary>
		/// Set node sort.
		/// </summary>
		public void SetComparison()
		{
			Nodes[0].Nodes.Comparison = comparisonDesc;
			Nodes.Comparison = comparisonDesc;
		}

		/// <summary>
		/// Test find node.
		/// </summary>
		/// <param name="id">Node id.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required")]
		public void TestFindNode(int id)
		{
			var node = Tree.FindNode(x => x.Item.Value == id);
			Debug.Log((node != null) ? node.Item.Name : "Node not found");
		}

		readonly Dictionary<int, TreeNode<TreeViewItem>> cache = new Dictionary<int, TreeNode<TreeViewItem>>();

		void Nodes2Cache(ObservableList<TreeNode<TreeViewItem>> nodes)
		{
			if (nodes == null)
			{
				return;
			}

			foreach (var node in nodes)
			{
				cache[node.Item.Value] = node;

				Nodes2Cache(node.Nodes);
			}
		}

		/// <summary>
		/// Create cache.
		/// </summary>
		public void CreateCache()
		{
			cache.Clear();
			Nodes2Cache(Tree.Nodes);
		}

		/// <summary>
		/// Get node from cache by id.
		/// </summary>
		/// <param name="id">Node id.</param>
		public void GetFromCache(int id)
		{
			var node = cache[id];
			Debug.Log((node != null) ? node.Item.Name : "Node not found");
		}

		/// <summary>
		/// Add subnodes.
		/// </summary>
		public void AddSubNodes()
		{
			if (Nodes.Count == 0)
			{
				return;
			}

			// get parent node
			var node = Nodes[0];

			// or find parent node by name
			/* var node = nodes.Find(x => x.Item.Name = "Node 2");*/

			if (node.Nodes == null)
			{
				node.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
			}

			var new_item1 = new TreeViewItem("Subnode 1");
			var new_node1 = new TreeNode<TreeViewItem>(new_item1);

			var new_item2 = new TreeViewItem("Subnode 2");
			var new_node2 = new TreeNode<TreeViewItem>(new_item2);

			var new_item3 = new TreeViewItem("Subnode 3");
			var new_node3 = new TreeNode<TreeViewItem>(new_item3);

			node.Nodes.BeginUpdate();

			node.Nodes.Add(new_node1);
			node.Nodes.Add(new_node2);
			node.Nodes.Add(new_node3);

			node.Nodes.EndUpdate();
		}

		/// <summary>
		/// Add node and scroll to it.
		/// </summary>
		public void ScrollToNewNode()
		{
			var new_item = new TreeViewItem("New node");
			var new_node = new TreeNode<TreeViewItem>(new_item);

			// nodes[0].Nodes.Add(new_node);
			Nodes.Add(new_node);

			// nodes.Insert(0, new_node);
			// ScrollToNode(new_node);
			ScrollToNodeAnimated(new_node);
		}

		/// <summary>
		/// Scroll to specified node.
		/// </summary>
		/// <param name="node">Node.</param>
		public void ScrollToNode(TreeNode<TreeViewItem> node)
		{
			// find index of node, DataSource contains list of visible nodes
			var index = Tree.Node2Index(node);

			// if node exists and visible
			if (index != -1)
			{
				// scroll to node
				Tree.ScrollTo(index);
			}
		}

		/// <summary>
		/// Scroll to node animated.
		/// </summary>
		/// <param name="node">Node.</param>
		public void ScrollToNodeAnimated(TreeNode<TreeViewItem> node)
		{
			// expand node, so DataSource contains list of visible nodes
			Tree.ExpandParentNodes(node);

			// get node index
			var index = Tree.Node2Index(node);

			// if node exists and visible
			if (index != -1)
			{
				// scroll to node
				Tree.ScrollToAnimated(index);
			}
		}

		/// <summary>
		/// Log selected nodes.
		/// </summary>
		public void GetSelectedNodes()
		{
			Debug.Log(Tree.SelectedIndex.ToString());
			Debug.Log(UtilitiesCollections.List2String(Tree.SelectedIndices));
			var selectedNodes = Tree.SelectedNodes;
			if (selectedNodes != null)
			{
				foreach (var node in selectedNodes)
				{
					Debug.Log(node.Item.Name);
				}
			}
		}

		/// <summary>
		/// Get node path.
		/// </summary>
		public void GetNodePath()
		{
			var path = Nodes[0].Nodes[0].Nodes[0].Path;
			foreach (var node in path)
			{
				Debug.Log(node.Item.Name);
			}
		}

		/// <summary>
		/// Select nodes.
		/// </summary>
		public void SelectNodes()
		{
			if ((Nodes.Count == 0) || (Nodes[0].Nodes.Count == 0))
			{
				return;
			}

			// replace on find node "Node 1 - 1"
			var parent_node = Nodes[0].Nodes[0];
			var children = new List<TreeNode<TreeViewItem>>();
			GetChildrenNodes(parent_node, children);

			// add children to selected nodes
			var selected_nodes = Tree.SelectedNodes;
			selected_nodes.AddRange(children);
			Tree.SelectedNodes = selected_nodes;

			// select only children
			// Tree.SelectedNodes = children;
		}

		/// <summary>
		/// Deselect nodes.
		/// </summary>
		public void DeselectNodes()
		{
			if ((Nodes.Count == 0) || (Nodes[0].Nodes.Count == 0))
			{
				return;
			}

			// replace on find node "Node 1 - 1"
			var parent_node = Nodes[0].Nodes[0];
			var children = new List<TreeNode<TreeViewItem>>();
			GetChildrenNodes(parent_node, children);

			// remove children from selected nodes
			var selected_nodes = Tree.SelectedNodes;
			foreach (var node in children)
			{
				selected_nodes.Remove(node);
			}

			Tree.SelectedNodes = selected_nodes;

			// deselect all
			// Tree.SelectedNodes = new List<TreeNode<TreeViewItem>>();
		}

		void GetChildrenNodes(TreeNode<TreeViewItem> node, List<TreeNode<TreeViewItem>> children)
		{
			if (node.Nodes == null)
			{
				return;
			}

			children.AddRange(node.Nodes);
			foreach (var n in node.Nodes)
			{
				GetChildrenNodes(n, children);
			}
		}

		/// <summary>
		/// Only one node can be selected at once.
		/// </summary>
		public void SetOnlyOnSelectable()
		{
			Tree.MultipleSelect = false;
		}

		/// <summary>
		/// Multiple nodes can be selected at once.
		/// </summary>
		public void SetMultipleSelectable()
		{
			Tree.MultipleSelect = true;
		}

		/// <summary>
		/// Compare nodes by Name in ascending order.
		/// </summary>
		protected Comparison<TreeNode<TreeViewItem>> comparisonAsc = (x, y) =>
		{
			return UtilitiesCompare.Compare(x.Item.LocalizedName ?? x.Item.Name, y.Item.LocalizedName ?? y.Item.Name);
		};

		/// <summary>
		/// Compare nodes by Name in descending order.
		/// </summary>
		protected Comparison<TreeNode<TreeViewItem>> comparisonDesc = (x, y) =>
		{
			return -UtilitiesCompare.Compare(x.Item.LocalizedName ?? x.Item.Name, y.Item.LocalizedName ?? y.Item.Name);
		};

		/// <summary>
		/// Sort in ascending order.
		/// </summary>
		public void SortAsc()
		{
			Nodes.BeginUpdate();
			ApplyNodesSort(Nodes, comparisonAsc);
			Nodes.EndUpdate();
		}

		/// <summary>
		/// Sort in descending order.
		/// </summary>
		public void SortDesc()
		{
			Nodes.BeginUpdate();
			ApplyNodesSort(Nodes, comparisonDesc);
			Nodes.EndUpdate();
		}

		/// <summary>
		/// Apply nodes sort.
		/// </summary>
		/// <typeparam name="T">Node type.</typeparam>
		/// <param name="nodes">Nodes.</param>
		/// <param name="comparison">Nodes comparison.</param>
		public static void ApplyNodesSort<T>(ObservableList<TreeNode<T>> nodes, Comparison<TreeNode<T>> comparison)
		{
			nodes.Sort(comparison);
			foreach (var node in nodes)
			{
				if (node.Nodes != null)
				{
					ApplyNodesSort(node.Nodes, comparison);
				}
			}
		}

		/// <summary>
		/// Remove node by name.
		/// </summary>
		/// <param name="name">Name.</param>
		public void TestRemove(string name)
		{
			RemoveByName(Nodes, name);
		}

		/// <summary>
		/// Remove node by name.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="name">Name.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required")]
		public static void RemoveByName(ObservableList<TreeNode<TreeViewItem>> nodes, string name)
		{
			Remove(nodes, x => x.Item.Name == name);
		}

		/// <summary>
		/// Remove node if it's match specified function.
		/// </summary>
		/// <typeparam name="T">Node type.</typeparam>
		/// <param name="nodes">Nodes.</param>
		/// <param name="match">Match function.</param>
		/// <returns>true if node removed; otherwise, false.</returns>
		public static bool Remove<T>(ObservableList<TreeNode<T>> nodes, Predicate<TreeNode<T>> match)
		{
			var findedNode = nodes.Find(match);
			if (findedNode != null)
			{
				findedNode.Parent = null;

				// this.nodes.Add(findedNode as TreeNode<TreeViewItem>);
				return true;
			}

			foreach (var node in nodes)
			{
				if (node.Nodes == null)
				{
					continue;
				}

				if (Remove(node.Nodes, match))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Test node position.
		/// </summary>
		/// <param name="name">Name.</param>
		public void TestReorder(string name)
		{
			ChangePositionByName(Nodes, name, 0);
		}

		/// <summary>
		/// Change node position.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="name">Node name.</param>
		/// <param name="position">New position.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required")]
		public static void ChangePositionByName(ObservableList<TreeNode<TreeViewItem>> nodes, string name, int position)
		{
			ChangePosition(nodes, x => x.Item.Name == name, position);
		}

		/// <summary>
		/// Change node position.
		/// </summary>
		/// <typeparam name="T">Node type.</typeparam>
		/// <param name="nodes">Nodes.</param>
		/// <param name="match">Node match.</param>
		/// <param name="position">New position.</param>
		/// <returns>true if position changed; otherwise, false.</returns>
		public static bool ChangePosition<T>(ObservableList<TreeNode<T>> nodes, Predicate<TreeNode<T>> match, int position)
		{
			var findedNode = nodes.Find(match);
			if (findedNode != null)
			{
				nodes.BeginUpdate();
				nodes.Remove(findedNode);
				nodes.Insert(position, findedNode);
				nodes.EndUpdate();
				return true;
			}

			foreach (var node in nodes)
			{
				if (node.Nodes == null)
				{
					continue;
				}

				if (ChangePosition(node.Nodes, match, position))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Show tree with 10000 nodes.
		/// </summary>
		public void Test10K()
		{
			var config = new List<int>() { 10, 10, 10, 10 };
			Nodes = GenerateTreeNodes(config, isExpanded: true);

			Tree.Nodes = Nodes;
		}

		/// <summary>
		/// Icon for nodes.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("icon")]
		protected Sprite Icon;

		/// <summary>
		/// Show tree with long nodes names.
		/// </summary>
		public void LongNames()
		{
			var config = new List<int>() { 2, 2, 2, 2, 2, 2, 2, 2, 2 };
			Nodes = GenerateTreeNodes(config, isExpanded: true, icon: Icon);

			Tree.Nodes = Nodes;
		}

		/// <summary>
		/// Set nodes by key.
		/// </summary>
		/// <param name="k">Key.</param>
		public void PerformanceCheck(string k)
		{
			if (DefaultNodes == null)
			{
				DefaultNodes = new Dictionary<string, ObservableList<TreeNode<TreeViewItem>>>();

				var config1k = new List<int>() { 10, 10, 10 };
				DefaultNodes.Add("1k", GenerateTreeNodes(config1k, isExpanded: true));

				var config5k = new List<int>() { 5, 10, 10, 10 };
				DefaultNodes.Add("5k", GenerateTreeNodes(config5k, isExpanded: true));

				var config10k = new List<int>() { 10, 10, 10, 10 };
				DefaultNodes.Add("10k", GenerateTreeNodes(config10k, isExpanded: true));

				var config50k = new List<int>() { 5, 10, 10, 10, 10 };
				DefaultNodes.Add("50k", GenerateTreeNodes(config50k, isExpanded: true));
			}

			Nodes = DefaultNodes[k];
			Tree.Nodes = DefaultNodes[k];
		}

		/// <summary>
		/// Set tree nodes.
		/// </summary>
		public void SetTreeNodes()
		{
			Tree.Nodes = Nodes;

			Nodes.BeginUpdate();

			var test_item = new TreeViewItem("added");
			var test_node = new TreeNode<TreeViewItem>(test_item);
			Nodes.Add(test_node);
			Nodes[1].IsVisible = false;
			Nodes[2].Nodes[1].IsVisible = false;

			Nodes.EndUpdate();
		}

		/// <summary>
		/// Add node.
		/// </summary>
		public void AddNode()
		{
			var test_item = new TreeViewItem("New node");
			var test_node = new TreeNode<TreeViewItem>(test_item);
			Nodes.Add(test_node);
		}

		/// <summary>
		/// Toggle node.
		/// </summary>
		public void ToggleNode()
		{
			Nodes[0].Nodes[0].IsExpanded = !Nodes[0].Nodes[0].IsExpanded;
		}

		/// <summary>
		/// Expand all nodes.
		/// </summary>
		public void ExpandeAllNodes()
		{
			ToggleNodes(Nodes, true);
		}

		/// <summary>
		/// Change node names.
		/// </summary>
		public void ChangeNodesName()
		{
			Nodes[0].Item.Name = "Node renamed from code";
			Nodes[0].Nodes[1].Item.Name = "Another node renamed from code";
		}

		/// <summary>
		/// Reset filter.
		/// </summary>
		public void ResetFilter()
		{
			Nodes.BeginUpdate();

			foreach (var node in Nodes)
			{
				SetVisible(node);
			}

			Nodes.EndUpdate();
		}

		void SetVisible(TreeNode<TreeViewItem> node)
		{
			if (node.Nodes != null)
			{
				foreach (var n in node.Nodes)
				{
					SetVisible(n);
				}
			}

			node.IsVisible = true;
		}

		/// <summary>
		/// Toggle nodes state.
		/// </summary>
		/// <param name="nodes">Nodes list.</param>
		/// <param name="isExpanded">Expanded state.</param>
		public void ToggleNodes(ObservableList<TreeNode<TreeViewItem>> nodes, bool isExpanded)
		{
			if ((nodes == null) || (nodes.Count == 0))
			{
				return;
			}

			nodes.BeginUpdate();

			foreach (var node in nodes)
			{
				node.IsExpanded = isExpanded;
				ToggleNodes(node.Nodes, isExpanded);
			}

			nodes.EndUpdate();
		}

		/// <summary>
		/// Filter nodes by name.
		/// </summary>
		/// <param name="nameContains">Name.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Required")]
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Required")]
		public void Filter(string nameContains)
		{
			Nodes.BeginUpdate();

			SampleFilter(Nodes, x => UtilitiesCompare.Contains(x.Name, nameContains));

			Nodes.EndUpdate();
		}

		/// <summary>
		/// Clear nodes.
		/// </summary>
		public void Clear()
		{
			Nodes = new ObservableList<TreeNode<TreeViewItem>>();
			Tree.Nodes = Nodes;
		}

		/// <summary>
		/// Sample filter.
		/// Set node.IsVisible if it's match condition or it's subnodes match condition.
		/// </summary>
		/// <param name="nodes">Nodes.</param>
		/// <param name="filterFunc">Match function.</param>
		/// <returns>true if any node match condition; otherwise, false.</returns>
		protected static bool SampleFilter(ObservableList<TreeNode<TreeViewItem>> nodes, Func<TreeViewItem, bool> filterFunc)
		{
			var result = false;

			foreach (var node in nodes)
			{
				var have_visible_children = (node.Nodes != null) && SampleFilter(node.Nodes, filterFunc);
				node.IsVisible = have_visible_children || filterFunc(node.Item);

				result |= node.IsVisible;
			}

			return result;
		}

		/// <summary>
		/// Generate nodes.
		/// </summary>
		/// <param name="items">Depth list.</param>
		/// <param name="nameStartsWith">Start part of name.</param>
		/// <param name="isExpanded">Is nodes expanded?</param>
		/// <param name="icon">Icon</param>
		/// <param name="start">Start index in the depth list.</param>
		/// <returns>Nodes.</returns>
		public static ObservableList<TreeNode<TreeViewItem>> GenerateTreeNodes(List<int> items, string nameStartsWith = "Node ", bool isExpanded = true, Sprite icon = null, int start = 0)
		{
			var count = items[start];
			var result = new ObservableList<TreeNode<TreeViewItem>>(true, count);

			result.BeginUpdate();

			for (int i = 0; i < count; i++)
			{
				var item_name = string.Format("{0}{1}", nameStartsWith, (i + 1).ToString());
				var item = new TreeViewItem(item_name, null);
				var nodes = items.Count > (start + 1)
					? GenerateTreeNodes(items, string.Format("{0} - ", item_name), isExpanded, icon, start + 1)
					: null;

				result.Add(new TreeNode<TreeViewItem>(item, nodes, isExpanded));
			}

			result.EndUpdate();

			return result;
		}

		/// <summary>
		/// Reload scene.
		/// </summary>
		public virtual void ReloadScene()
		{
			#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
			#else
			Application.LoadLevel(Application.loadedLevel);
			#endif
		}
	}
}