namespace UIWidgets.Examples.Shops
{
	/// <summary>
	/// IOrderLine.
	/// </summary>
	public interface IOrderLine
	{
		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		Item Item { get; set; }

		/// <summary>
		/// Gets or sets the quantity.
		/// </summary>
		/// <value>The quantity.</value>
		int Quantity { get; set; }
	}
}