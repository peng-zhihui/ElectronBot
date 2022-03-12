namespace UIWidgets.Examples.Shops
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Prices.
	/// </summary>
	public static class Prices
	{
		static readonly Dictionary<string, int> BasePrices = new Dictionary<string, int>()
		{
			{ "Stick", 5 },
			{ "Sword", 100 },
			{ "Short Sword", 75 },
			{ "Long Sword", 120 },
			{ "Knife", 50 },
			{ "Dagger", 80 },
			{ "Hammer", 150 },
			{ "Shield", 70 },
			{ "Leather Armor", 200 },
			{ "Ring", 20 },
			{ "Bow", 100 },
			{ "Crossbow", 120 },

			// another items
			{ "HP Potion", 10 },
			{ "Mana Potion", 10 },
			{ "HP UP", 1000 },
			{ "Mana UP", 1000 },

			// more items
			{ "Wood", 10 },
			{ "Wheat", 10 },
			{ "Fruits", 20 },
			{ "Sugar", 70 },
			{ "Metal", 10 },
			{ "Cotton", 20 },
			{ "Silver", 300 },
			{ "Gold", 500 },
			{ "Cocoa", 160 },
			{ "Coffee", 140 },
			{ "Tobacco", 120 },
		};

		/// <summary>
		/// Gets the price.
		/// </summary>
		/// <returns>The price.</returns>
		/// <param name="item">Item.</param>
		/// <param name="priceFactor">Price factor.</param>
		public static int GetPrice(Item item, float priceFactor)
		{
			if (!BasePrices.ContainsKey(item.Name))
			{
				return 1;
			}

			return Mathf.Max(1, Mathf.RoundToInt(BasePrices[item.Name] * priceFactor));
		}
	}
}