namespace UIWidgets
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Layout with items at any line.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	public class TrackLayoutAnyLine<TData, TPoint> : TrackLayout<TData, TPoint>
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Set order for the specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="temp">Temp list.</param>
		/// <param name="used">Temp list for the used items.</param>
		protected override void Layout(List<TData> items, List<TData> temp, List<TData> used)
		{
			EnsureSingleFixedOrder(items);

			var order = 0;
			temp.AddRange(items);
			while (temp.Count > 0)
			{
				var index = SetOrder(temp, order);
				if (index > -1)
				{
					temp.RemoveAt(index);
				}

				order += 1;
			}
		}

		/// <summary>
		/// Set specified order to the item with order equal or more preferred order.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="preferredOrder">Preferred order of the item.</param>
		/// <returns>Index of the item.</returns>
		protected virtual int SetOrder(List<TData> items, int preferredOrder)
		{
			if (items.Count == 0)
			{
				return -1;
			}

			// try to find item with same order and enabled fixedOrder
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				if ((item.Order == preferredOrder) && item.FixedOrder)
				{
					return i;
				}
			}

			// try to find any item with disabled fixedOrder
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				if (!item.FixedOrder)
				{
					item.Order = preferredOrder;
					return i;
				}
			}

			return -1;
		}
	}
}