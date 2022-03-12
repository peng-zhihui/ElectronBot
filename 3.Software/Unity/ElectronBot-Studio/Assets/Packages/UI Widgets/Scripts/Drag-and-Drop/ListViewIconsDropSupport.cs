namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewIcons drop support.
	/// Receive drops from TreeView and ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(ListViewIcons))]
	public class ListViewIconsDropSupport : ListViewCustomDropSupport<ListViewIcons, ListViewIconsItemComponent, ListViewIconsItemDescription>, IDropSupport<TreeNode<TreeViewItem>>
	{
		#region IDropSupport<TreeNode<TreeViewItem>>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual bool CanReceiveDrop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			if (!ReceiveNodes)
			{
				return false;
			}

			var has_subnodes = (data.Nodes != null) && (data.Nodes.Count > 0);
			if (ReceiveOnlyEmptyNode && has_subnodes)
			{
				return false;
			}

			var index = ListView.GetNearestIndex(eventData);
			ShowDropIndicator(index);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void Drop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			var index = ListView.GetNearestIndex(eventData);

			var item = new ListViewIconsItemDescription()
			{
				Name = data.Item.Name,
				LocalizedName = data.Item.LocalizedName,
				Icon = data.Item.Icon,
				Value = data.Item.Value,
			};

			AddItem(item, index);

			if (DeleteNodeAfterDrop)
			{
				// remove node from tree
				data.Parent = null;
			}

			HideDropIndicator();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void DropCanceled(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion
	}
}