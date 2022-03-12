namespace UIWidgets.Examples.Shops
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Harbor order line.
	/// </summary>
	[Serializable]
	public class HarborOrderLine : IOrderLine
	{
		[SerializeField]
		Item item;

		/// <summary>
		/// Gets or sets the item.
		/// </summary>
		/// <value>The item.</value>
		public Item Item
		{
			get
			{
				return item;
			}

			set
			{
				item = value;
			}
		}

		/// <summary>
		/// The buy price.
		/// </summary>
		[SerializeField]
		public int BuyPrice;

		/// <summary>
		/// The sell price.
		/// </summary>
		[SerializeField]
		public int SellPrice;

		/// <summary>
		/// The buy quantity.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("BuyCount")]
		public int BuyQuantity;

		/// <summary>
		/// The buy quantity.
		/// </summary>
		[Obsolete("Renamed to BuyQuantity.")]
		public int BuyCount
		{
			get
			{
				return BuyQuantity;
			}

			set
			{
				BuyQuantity = value;
			}
		}

		/// <summary>
		/// The sell quantity.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("SellCount")]
		public int SellQuantity;

		/// <summary>
		/// The buy quantity.
		/// </summary>
		[Obsolete("Renamed to SellQuantity.")]
		public int SellCount
		{
			get
			{
				return SellQuantity;
			}

			set
			{
				SellQuantity = value;
			}
		}

		[SerializeField]
		[FormerlySerializedAs("count")]
		int quantity;

		/// <summary>
		/// Gets or sets the quantity.
		/// </summary>
		/// <value>The quantity.</value>
		public int Quantity
		{
			get
			{
				return quantity;
			}

			set
			{
				quantity = value;
			}
		}

		/// <summary>
		/// Gets or sets the quantity.
		/// </summary>
		/// <value>The quantity.</value>
		[Obsolete("Renamed to Quantity.")]
		public int Count
		{
			get
			{
				return quantity;
			}

			set
			{
				quantity = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.Examples.Shops.HarborOrderLine"/> class.
		/// </summary>
		/// <param name="newItem">New item.</param>
		/// <param name="buyPrice">Buy price.</param>
		/// <param name="sellPrice">Sell price.</param>
		/// <param name="buyQuantity">Buy quantity.</param>
		/// <param name="sellQuantity">Sell quantity.</param>
		public HarborOrderLine(Item newItem, int buyPrice, int sellPrice, int buyQuantity, int sellQuantity)
		{
			item = newItem;
			BuyPrice = buyPrice;
			SellPrice = sellPrice;
			BuyQuantity = buyQuantity;
			SellQuantity = sellQuantity;
		}
	}
}