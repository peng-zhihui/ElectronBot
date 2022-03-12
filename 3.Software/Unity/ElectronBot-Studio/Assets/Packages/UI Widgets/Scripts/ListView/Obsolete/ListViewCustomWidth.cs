namespace UIWidgets
{
	/// <summary>
	/// Base class for custom ListView for items with variable widths.
	/// </summary>
	/// <typeparam name="TComponent">Type of DefaultItem component.</typeparam>
	/// <typeparam name="TItem">Type of item.</typeparam>
	public class ListViewCustomWidth<TComponent, TItem> : ListViewCustomSize<TComponent, TItem>
		where TComponent : ListViewItem
	{
	}
}