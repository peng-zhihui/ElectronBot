namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TileViewSample drop support.
	/// </summary>
	[RequireComponent(typeof(TileViewSample))]
	public class TileViewSampleDropSupport : ListViewCustomDropSupport<TileViewSample, TileViewComponentSample, TileViewItemSample>
	{
	}
}