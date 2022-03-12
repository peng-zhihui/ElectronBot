namespace UIWidgets.Examples.Shops
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TreeViewNode drop support.
	/// Receive drops from TreeView and ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(TraderTreeViewComponent))]
	public class TraderTreeViewNodeDropSupport : MonoBehaviour, IDropSupport<TreeNode<JRPGOrderLine>>, IDropSupport<JRPGOrderLine>
	{
		/// <summary>
		/// The drop indicator.
		/// </summary>
		[SerializeField]
		protected ListViewDropIndicator DropIndicator;

		TraderTreeViewComponent source;

		/// <summary>
		/// Gets the current TraderTreeViewComponent.
		/// </summary>
		/// <value>Current TraderTreeViewComponent.</value>
		public TraderTreeViewComponent Source
		{
			get
			{
				if (source == null)
				{
					source = GetComponent<TraderTreeViewComponent>();
				}

				return source;
			}
		}

		/// <summary>
		/// Gets the playlist node.
		/// </summary>
		/// <returns>The playlist node.</returns>
		/// <param name="node">Node.</param>
		protected static TreeNode<JRPGOrderLine> GetPlaylistNode(TreeNode<JRPGOrderLine> node)
		{
			if (node.Item.IsPlaylist)
			{
				return node;
			}

			return node.Parent;
		}

		/// <summary>
		/// Add the node.
		/// </summary>
		/// <param name="node">Node.</param>
		protected void AddNode(TreeNode<JRPGOrderLine> node)
		{
			var parent = GetPlaylistNode(Source.Node);

			if (parent.Nodes == null)
			{
				parent.Nodes = new ObservableList<TreeNode<JRPGOrderLine>>();
			}

			// calculate insert position
			var index = Source.Node.Item.IsPlaylist ? -1 : Source.Node.Parent.Nodes.IndexOf(Source.Node);
			if (index != -1)
			{
				parent.Nodes.Insert(index, node);
			}
			else
			{
				parent.Nodes.Add(node);
			}
		}

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		protected virtual void ShowDropIndicator()
		{
			if (DropIndicator != null)
			{
				var index = Source.Node.Item.IsPlaylist
					? Source.Index + Source.Node.AllUsedNodesCount + 1
					: Source.Index;
				DropIndicator.Show(index, Source.Owner);
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

		#region IDropSupport<TreeNode<JRPGOrderLine>>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TreeNode<JRPGOrderLine> data, PointerEventData eventData)
		{
			var result = data.CanBeParent(Source.Node);

			if (result)
			{
				ShowDropIndicator();
			}

			return result;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TreeNode<JRPGOrderLine> data, PointerEventData eventData)
		{
			// remove from parent node, othwerwise it will be duplicated
			data.Parent.Nodes.Remove(data);

			AddNode(data);

			HideDropIndicator();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TreeNode<JRPGOrderLine> data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion

		#region IDropSupport<ListViewIconsItemDescription>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(JRPGOrderLine data, PointerEventData eventData)
		{
			ShowDropIndicator();

			return true;
		}

		/// <summary>
		/// OnDrop event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnDrop = new UnityEvent();

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(JRPGOrderLine data, PointerEventData eventData)
		{
			// create new node and item
			var newItem = new JRPGOrderLine(data.Item, data.Price)
			{
				Quantity = data.Quantity,
				IsPlaylist = data.IsPlaylist,
			};

			var newNode = new TreeNode<JRPGOrderLine>(newItem);

			AddNode(newNode);

			HideDropIndicator();

			// do something after new song added
			// some code

			// or invoke event
			OnDrop.Invoke();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(JRPGOrderLine data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion
	}
}