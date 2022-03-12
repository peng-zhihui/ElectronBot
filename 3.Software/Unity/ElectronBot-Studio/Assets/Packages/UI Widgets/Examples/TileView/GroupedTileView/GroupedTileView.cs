namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// GroupedTileView
	/// </summary>
	public class GroupedTileView : ListViewCustom<GroupedTileViewComponent, Photo>
	{
		/// <summary>
		/// Grouped data.
		/// </summary>
		public GroupedPhotos GroupedData = new GroupedPhotos();

		bool isGroupedListViewInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isGroupedListViewInited)
			{
				return;
			}

			isGroupedListViewInited = true;

			base.Init();

			GroupedData.GroupComparison = (x, y) => x.Created.CompareTo(y.Created);
			GroupedData.Data = DataSource;

			GroupedData.EmptyGroupItem = new Photo() { IsGroup = true, IsEmpty = true };
			GroupedData.EmptyItem = new Photo() { IsEmpty = true };
			GroupedData.ItemsPerBlock = ListRenderer.GetItemsPerBlock();
		}

		/// <inheritdoc/>
		public override void UpdateItems()
		{
			base.UpdateItems();

			GroupedData.ItemsPerBlock = ListRenderer.GetItemsPerBlock();
		}

		/// <inheritdoc/>
		public override void Resize()
		{
			base.Resize();

			GroupedData.ItemsPerBlock = ListRenderer.GetItemsPerBlock();
		}
	}
}