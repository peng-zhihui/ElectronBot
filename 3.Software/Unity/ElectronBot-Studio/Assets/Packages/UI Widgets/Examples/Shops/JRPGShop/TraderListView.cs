namespace UIWidgets.Examples.Shops
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;

	/// <summary>
	/// TraderListView sort fields.
	/// </summary>
	public enum TraderListViewSortFields
	{
		/// <summary>
		/// Item name.
		/// </summary>
		ItemName = 0,

		/// <summary>
		/// Item available.
		/// </summary>
		ItemAvailable = 1,

		/// <summary>
		/// Price.
		/// </summary>
		Price = 2,

		/// <summary>
		/// Quantity.
		/// </summary>
		[Obsolete("Renamed to Quantity.")]
		Count = 3,

		/// <summary>
		/// Quantity.
		/// </summary>
		Quantity = 3,
	}

	/// <summary>
	/// Trader list view.
	/// </summary>
	public class TraderListView : ListViewCustom<TraderListViewComponent, JRPGOrderLine>
	{
		TraderListViewSortFields currentSortField = TraderListViewSortFields.ItemName;

		Dictionary<int, Comparison<JRPGOrderLine>> sortComparers;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			sortComparers = new Dictionary<int, Comparison<JRPGOrderLine>>()
			{
				{ (int)TraderListViewSortFields.ItemName, ItemNameComparer },
				{ (int)TraderListViewSortFields.ItemAvailable, ItemAvailableComparer },
				{ (int)TraderListViewSortFields.Price, PriceComparer },
				{ (int)TraderListViewSortFields.Quantity, QuantityComparer },
			};

			base.Init();
		}

		/// <summary>
		/// Toggle sort.
		/// </summary>
		/// <param name="field">Sort field.</param>
		public void ToggleSort(TraderListViewSortFields field)
		{
			if (field == currentSortField)
			{
				DataSource.Reverse();
			}
			else if (sortComparers.ContainsKey((int)field))
			{
				currentSortField = field;

				DataSource.Sort(sortComparers[(int)field]);
			}
		}

		#region used in Button.OnClick()

		/// <summary>
		/// Sort by Item name.
		/// </summary>
		public void SortByItemName()
		{
			ToggleSort(TraderListViewSortFields.ItemName);
		}

		/// <summary>
		/// Sort by Item available.
		/// </summary>
		public void SortByItemAvailable()
		{
			ToggleSort(TraderListViewSortFields.ItemAvailable);
		}

		/// <summary>
		/// Sort by Price.
		/// </summary>
		public void SortByPrice()
		{
			ToggleSort(TraderListViewSortFields.Price);
		}

		/// <summary>
		/// Sort by Quantity.
		/// </summary>
		[Obsolete("Renamed to SortByQuantity().")]
		public void SortByCount()
		{
			SortByQuantity();
		}

		/// <summary>
		/// Sort by Quantity.
		/// </summary>
		public void SortByQuantity()
		{
			ToggleSort(TraderListViewSortFields.Quantity);
		}

		#endregion

		#region Items comparers

		/// <summary>
		/// Item name comparer.
		/// </summary>
		/// <param name="x">First JRPGOrderLine.</param>
		/// <param name="y">Second JRPGOrderLine.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		protected static int ItemNameComparer(JRPGOrderLine x, JRPGOrderLine y)
		{
			return UtilitiesCompare.Compare(x.Item.Name, y.Item.Name);
		}

		/// <summary>
		/// Item available comparer.
		/// </summary>
		/// <param name="x">First JRPGOrderLine.</param>
		/// <param name="y">Second JRPGOrderLine.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		protected static int ItemAvailableComparer(JRPGOrderLine x, JRPGOrderLine y)
		{
			if (x.Item.Quantity == y.Item.Quantity)
			{
				return 0;
			}

			if (x.Item.Quantity == -1)
			{
				return 1;
			}

			if (y.Item.Quantity == -1)
			{
				return -1;
			}

			return x.Item.Quantity.CompareTo(y.Item.Quantity);
		}

		/// <summary>
		/// Price comparer.
		/// </summary>
		/// <param name="x">First JRPGOrderLine.</param>
		/// <param name="y">Second JRPGOrderLine.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		protected static int PriceComparer(JRPGOrderLine x, JRPGOrderLine y)
		{
			return x.Price.CompareTo(y.Price);
		}

		/// <summary>
		/// Quantity comparer.
		/// </summary>
		/// <param name="x">First JRPGOrderLine.</param>
		/// <param name="y">Second JRPGOrderLine.</param>
		/// <returns>A 32-bit signed integer that indicates whether X precedes, follows, or appears in the same position in the sort order as the Y parameter.</returns>
		protected static int QuantityComparer(JRPGOrderLine x, JRPGOrderLine y)
		{
			return x.Quantity.CompareTo(y.Quantity);
		}
		#endregion
	}
}