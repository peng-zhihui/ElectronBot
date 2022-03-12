namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileViewSample drag support.
	/// </summary>
	[RequireComponent(typeof(TileViewComponentSample))]
	public class TileViewSampleDragSupport : ListViewCustomDragSupport<TileViewSample, TileViewComponentSample, TileViewItemSample>
	{
		/// <summary>
		/// Get data.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected override TileViewItemSample GetData(TileViewComponentSample component)
		{
			return component.Item;
		}

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected override void SetDragInfoData(TileViewItemSample data)
		{
			DragInfo.SetData(data);
		}
	}
}