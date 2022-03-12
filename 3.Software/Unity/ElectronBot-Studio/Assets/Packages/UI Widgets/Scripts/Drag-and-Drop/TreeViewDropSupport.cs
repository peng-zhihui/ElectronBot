namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TreeView drop support.
	/// Receive drops from TreeView and ListViewIcons.
	/// </summary>
	[RequireComponent(typeof(TreeView))]
	public class TreeViewDropSupport : TreeViewCustomDropSupport<TreeView, TreeViewComponent, TreeViewItem>, IDropSupport<ListViewIconsItemDescription>
	{
		#region IDropSupport<ListViewIconsItemDescription>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual bool CanReceiveDrop(ListViewIconsItemDescription data, PointerEventData eventData)
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
		public virtual void Drop(ListViewIconsItemDescription data, PointerEventData eventData)
		{
			if (Source.Nodes == null)
			{
				Source.Nodes = new ObservableList<TreeNode<TreeViewItem>>();
			}

			var new_item = new TreeViewItem(data.Name, data.Icon)
			{
				LocalizedName = data.LocalizedName,
				Value = data.Value,
			};
			var new_node = new TreeNode<TreeViewItem>(new_item, null, true, true);
			AddNewNode(new_node, eventData);

			HideDropIndicator();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void DropCanceled(ListViewIconsItemDescription data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion
	}
}