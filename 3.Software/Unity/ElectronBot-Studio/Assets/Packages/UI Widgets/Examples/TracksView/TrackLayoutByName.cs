namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;

	/// <summary>
	/// Layout with compact order and items at any line grouped by name.
	/// </summary>
	public class TrackLayoutByName : TrackLayoutAnyLineCompact<TrackData, DateTime>
	{
		readonly List<TrackData> resetFixedOrder = new List<TrackData>();

		/// <summary>
		/// Set order for the specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="temp">Temp list.</param>
		/// <param name="used">Temp list for the used items.</param>
		protected override void Layout(List<TrackData> items, List<TrackData> temp, List<TrackData> used)
		{
			EnsureSingleFixedOrder(items);

			// find items with FixedOrder
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];
				if (item.FixedOrder)
				{
					temp.Add(item);
				}
			}

			// set same order for the items with same name
			for (int temp_index = 0; temp_index < temp.Count; temp_index++)
			{
				var fixed_item = temp[temp_index];
				for (int item_index = 0; item_index < items.Count; item_index++)
				{
					var item = items[item_index];
					if ((!ReferenceEquals(item, fixed_item)) && (item.Name == fixed_item.Name))
					{
						item.Order = fixed_item.Order;
						item.FixedOrder = true;
						resetFixedOrder.Add(item);
					}
				}
			}

			temp.Clear();

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

			// reset fixedOrder
			for (int i = 0; i < resetFixedOrder.Count; i++)
			{
				resetFixedOrder[i].FixedOrder = false;
			}
		}

		/// <summary>
		/// Can be item placed along with the specified items.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="items">Items.</param>
		/// <returns>true if items can be places together; otherwise false.</returns>
		protected override bool CanBeWithItems(TrackData item, List<TrackData> items)
		{
			var valid_name = (items.Count == 0) || (items[0].Name == item.Name);

			return valid_name && !IsIntersect(items, item.StartPoint, item.EndPoint);
		}
	}
}