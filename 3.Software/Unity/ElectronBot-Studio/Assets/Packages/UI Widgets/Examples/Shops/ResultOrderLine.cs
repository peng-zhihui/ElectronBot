namespace UIWidgets.Examples.Shops
{
	/// <summary>
	/// Result order line.
	/// </summary>
	public class ResultOrderLine : IOrderLine
	{
		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public Item Item
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the quantity.
		/// </summary>
		/// <value>The quantity.</value>
		public int Quantity
		{
			get;
			set;
		}
	}
}