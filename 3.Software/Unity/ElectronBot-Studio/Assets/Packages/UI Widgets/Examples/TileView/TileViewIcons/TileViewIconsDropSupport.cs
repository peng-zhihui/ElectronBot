namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TileViewIcons drop support.
	/// </summary>
	[RequireComponent(typeof(TileViewIcons))]
	public class TileViewIconsDropSupport : ListViewCustomDropSupport<TileViewIcons, ListViewIconsItemComponent, ListViewIconsItemDescription>, IDropSupport<TreeNode<TreeViewItem>>
	{
		#region IDropSupport<TreeNode<TreeViewItem>>

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public bool CanReceiveDrop(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			var result = data.Nodes == null || data.Nodes.Count == 0;

			if (result)
			{
				var index = ListView.GetNearestIndex(eventData);
				ShowDropIndicator(index);
			}

			return result;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void Drop(TreeNode<TreeViewItem> data, PointerEventData eventData)
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

			// remove node from tree
			data.Parent = null;

			HideDropIndicator();
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public void DropCanceled(TreeNode<TreeViewItem> data, PointerEventData eventData)
		{
			HideDropIndicator();
		}
		#endregion
	}
}