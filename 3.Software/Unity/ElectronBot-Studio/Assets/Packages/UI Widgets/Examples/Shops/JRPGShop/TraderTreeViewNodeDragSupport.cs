namespace UIWidgets.Examples.Shops
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TraderTreeViewNode drag support.
	/// </summary>
	[RequireComponent(typeof(TraderTreeViewComponent))]
	public class TraderTreeViewNodeDragSupport : TreeViewCustomNodeDragSupport<TraderTreeViewComponent, TraderListViewComponent, JRPGOrderLine>
	{
		/// <summary>
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		public override bool CanDrag(PointerEventData eventData)
		{
			return !Node.Item.IsPlaylist;
		}
	}
}