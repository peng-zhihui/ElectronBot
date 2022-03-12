namespace UIWidgets.Examples.Shops
{
	using System.Collections.Generic;

	/// <summary>
	/// IOrder.
	/// </summary>
	public interface IOrder
	{
		/// <summary>
		/// Gets the order lines.
		/// </summary>
		/// <returns>The order lines.</returns>
		List<IOrderLine> GetOrderLines();

		/// <summary>
		/// Order lines count.
		/// </summary>
		/// <returns>The lines count.</returns>
		int OrderLinesCount();

		/// <summary>
		/// Total.
		/// </summary>
		/// <returns>Total sum.</returns>
		int Total();
	}
}