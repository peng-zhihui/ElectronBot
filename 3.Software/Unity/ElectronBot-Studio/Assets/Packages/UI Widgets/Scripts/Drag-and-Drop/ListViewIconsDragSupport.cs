namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// ListViewIcons drag support.
	/// </summary>
	[RequireComponent(typeof(ListViewIconsItemComponent))]
	public class ListViewIconsDragSupport : ListViewCustomDragSupport<ListViewIcons, ListViewIconsItemComponent, ListViewIconsItemDescription>
	{
		/// <inheritdoc/>
		protected override ListViewIconsItemDescription GetData(ListViewIconsItemComponent component)
		{
			return component.Item;
		}

		/// <inheritdoc/>
		protected override void SetDragInfoData(ListViewIconsItemDescription data)
		{
			DragInfo.SetData(data);
		}
	}
}