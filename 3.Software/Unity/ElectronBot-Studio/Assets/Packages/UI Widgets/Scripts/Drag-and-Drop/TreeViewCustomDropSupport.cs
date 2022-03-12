namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TreeViewCustom drop support.
	/// </summary>
	/// <typeparam name="TTreeView">Type of the TreeView.</typeparam>
	/// <typeparam name="TTreeViewComponent">Type of the TreeView component.</typeparam>
	/// <typeparam name="TTreeViewItem">Type of the TreeView item.</typeparam>
	public class TreeViewCustomDropSupport<TTreeView, TTreeViewComponent, TTreeViewItem> : MonoBehaviour, IDropSupport<TreeNode<TTreeViewItem>>, IDropSupport<TTreeViewItem>
		where TTreeView : TreeViewCustom<TTreeViewComponent, TTreeViewItem>
		where TTreeViewComponent : TreeViewComponentBase<TTreeViewItem>
	{
		TTreeView source;

		/// <summary>
		/// Gets the current TreeView.
		/// </summary>
		/// <value>Current TreeView.</value>
		public TTreeView Source
		{
			get
			{
				if (source == null)
				{
					source = GetComponent<TTreeView>();
				}

				return source;
			}
		}

		/// <summary>
		/// Drop position.
		/// </summary>
		[SerializeField]
		public NearestType DropPosition = NearestType.Auto;

		/// <summary>
		/// The drop indicator.
		/// </summary>
		[SerializeField]
		public ListViewDropIndicator DropIndicator;

		RectTransform rectTransform;

		/// <summary>
		/// Current RectTransform.
		/// </summary>
		protected RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Receive dropped items.
		/// </summary>
		[SerializeField]
		public bool ReceiveItems = true;

		/// <summary>
		/// Receive dropped nodes.
		/// </summary>
		[SerializeField]
		public bool ReceiveNodes = true;

		#region IDropSupport<TreeNode<TTreeViewItem>>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TreeNode<TTreeViewItem> data, PointerEventData eventData)
		{
			if (!Source.IsInteractable())
			{
				return false;
			}

			if (!ReceiveNodes)
			{
				return false;
			}

			var index = Source.GetNearestIndex(eventData, DropPosition);
			if (Source.IsValid(index))
			{
				var nearest_node = Source.DataSource[index].Node;
				var nearest_parent = nearest_node.Parent;

				if ((nearest_parent != null) && !data.CanBeParent(nearest_parent))
				{
					index = -1;
				}
			}

			ShowDropIndicator(index);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TreeNode<TTreeViewItem> data, PointerEventData eventData)
		{
			if (Source.Nodes == null)
			{
				Source.Nodes = new ObservableList<TreeNode<TTreeViewItem>>();
			}

			var index = Source.GetNearestIndex(eventData, DropPosition);
			var dropped = false;
			if (Source.IsValid(index))
			{
				var nearest_node = Source.DataSource[index].Node;
				var nearest_parent = nearest_node.Parent;
				if (nearest_parent == null)
				{
					data.Parent = null;
					Source.Nodes.Insert(index, data);
					dropped = true;
				}
				else if (data.CanBeParent(nearest_parent))
				{
					index = nearest_parent.Nodes.IndexOf(nearest_node);
					data.Parent = null;
					nearest_parent.Nodes.Insert(index, data);
					dropped = true;
				}
			}

			if (!dropped)
			{
				data.Parent = null;
				Source.Nodes.Add(data);
			}

			HideDropIndicator();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TreeNode<TTreeViewItem> data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion

		#region IDropSupport<TTreeViewItem>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TTreeViewItem data, PointerEventData eventData)
		{
			if (!Source.IsInteractable())
			{
				return false;
			}

			if (!ReceiveItems)
			{
				return false;
			}

			var index = Source.GetNearestIndex(eventData, DropPosition);
			ShowDropIndicator(index);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TTreeViewItem data, PointerEventData eventData)
		{
			if (Source.Nodes == null)
			{
				Source.Nodes = new ObservableList<TreeNode<TTreeViewItem>>();
			}

			var new_node = new TreeNode<TTreeViewItem>(data, null, true, true);
			AddNewNode(new_node, eventData);

			HideDropIndicator();
		}

		/// <summary>
		/// Add new node.
		/// </summary>
		/// <param name="node">Node.</param>
		/// <param name="eventData">Event data.</param>
		protected virtual void AddNewNode(TreeNode<TTreeViewItem> node, PointerEventData eventData)
		{
			var index = Source.GetNearestIndex(eventData, DropPosition);
			if (Source.IsValid(index))
			{
				var nearest_node = Source.DataSource[index].Node;
				var nearest_parent = nearest_node.RealParent;

				index = nearest_parent.Nodes.IndexOf(nearest_node);
				nearest_parent.Nodes.Insert(index, node);
			}
			else
			{
				Source.Nodes.Add(node);
			}
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TTreeViewItem data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ShowDropIndicator(int index)
		{
			if (DropIndicator != null)
			{
				DropIndicator.Show(index, Source);
			}
		}

		/// <summary>
		/// Hides the drop indicator.
		/// </summary>
		protected virtual void HideDropIndicator()
		{
			if (DropIndicator != null)
			{
				DropIndicator.Hide();
			}
		}
	}
}