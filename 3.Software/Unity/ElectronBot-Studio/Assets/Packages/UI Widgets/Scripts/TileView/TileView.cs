namespace UIWidgets
{
	/// <summary>
	/// Alias for TileViewCustom.
	/// </summary>
	/// <typeparam name="TComponent">Component type.</typeparam>
	/// <typeparam name="TItem">Item type.</typeparam>
	public class TileView<TComponent, TItem> : TileViewCustom<TComponent, TItem>
		where TComponent : ListViewItem
	{
	}
}