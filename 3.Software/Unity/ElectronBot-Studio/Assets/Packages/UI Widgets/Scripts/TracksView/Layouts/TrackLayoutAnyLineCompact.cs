namespace UIWidgets
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Layout with compact order and items at any line.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	public class TrackLayoutAnyLineCompact<TData, TPoint> : TrackLayout<TData, TPoint>
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
				var index = SetOrder(temp, used, order);
				if (index > -1)
				{
					used.Add(temp[index]);
					temp.RemoveAt(index);
				}
				else
				{
					used.Clear();
					order += 1;
				}
			}
		}

		/// <summary>
		/// Set specified order to the item with order equal or more preferred order and not intersected with any item in the used list.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="used">List of items with setted order.</param>
		/// <param name="preferredOrder">Preferred order of the item.</param>
		/// <returns>Index of the item.</returns>
		protected int SetOrder(List<TData> items, List<TData> used, int preferredOrder)
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

			// try to find any item with disabled fixedOrder and not intersected with usedItems
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				if ((!item.FixedOrder) && CanBeWithItems(item, used))
				{
					item.Order = preferredOrder;
					return i;
				}
			}

			return -1;
		}
	}
}