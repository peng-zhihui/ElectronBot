namespace UIWidgets.Examples.Shops
{
	using System.Collections.Generic;
	using UIWidgets;

	/// <summary>
	/// JRPG order.
	/// </summary>
	public class JRPGOrder : IOrder
	{
		readonly List<IOrderLine> BaseOrderLines = new List<IOrderLine>();

		readonly int total;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.Examples.Shops.JRPGOrder"/> class.
		/// </summary>
		/// <param name="orderLines">Order lines.</param>
		public JRPGOrder(ObservableList<JRPGOrderLine> orderLines)
		{
			foreach (var line in orderLines)
			{
				if (line.Quantity == 0)
				{
					continue;
				}

				total += line.Quantity * line.Price;
				BaseOrderLines.Add(line);
			}
		}

		/// <summary>
		/// Gets the order lines.
		/// </summary>
		/// <returns>The order lines.</returns>
		public List<IOrderLine> GetOrderLines()
		{
			return BaseOrderLines;
		}

		/// <summary>
		/// Order lines count.
		/// </summary>
		/// <returns>The lines count.</returns>
		public int OrderLinesCount()
		{
			return BaseOrderLines.Count;
		}

		/// <summary>
		/// Total.
		/// </summary>
		/// <returns>Total sum.</returns>
		public int Total()
		{
			return total;
		}
	}
}