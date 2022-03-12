namespace UIWidgets.Examples.Shops
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TraderListView drag support.
	/// </summary>
	[RequireComponent(typeof(TraderListViewComponent))]
	public class TraderListViewDragSupport : ListViewCustomDragSupport<TraderListView, TraderListViewComponent, JRPGOrderLine>
	{
		/// <summary>
		/// Get data from specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>Data.</returns>
		protected override JRPGOrderLine GetData(TraderListViewComponent component)
		{
			return component.OrderLine;
		}

		/// <summary>
		/// Set data for DragInfo component.
		/// </summary>
		/// <param name="data">Data.</param>
		protected override void SetDragInfoData(JRPGOrderLine data)
		{
			DragInfo.SetData(data);
		}
	}
}