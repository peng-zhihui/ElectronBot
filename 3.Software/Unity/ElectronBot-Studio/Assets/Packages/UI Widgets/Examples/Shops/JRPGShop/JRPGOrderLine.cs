namespace UIWidgets.Examples.Shops
{
	using System;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// JRPG order line.
	/// </summary>
	[Serializable]
	public class JRPGOrderLine : IOrderLine
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
		/// The price.
		/// </summary>
		[SerializeField]
		public int Price;

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
		/// Initializes a new instance of the <see cref="UIWidgets.Examples.Shops.JRPGOrderLine"/> class.
		/// </summary>
		/// <param name="newItem">New item.</param>
		/// <param name="newPrice">New price.</param>
		public JRPGOrderLine(Item newItem, int newPrice)
		{
			item = newItem;
			Price = newPrice;
		}

		/// <summary>
		/// Is it playlist? Otherwise it's song.
		/// </summary>
		public bool IsPlaylist;
	}
}