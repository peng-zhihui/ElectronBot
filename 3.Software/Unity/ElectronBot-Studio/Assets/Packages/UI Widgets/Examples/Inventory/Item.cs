namespace UIWidgets.Examples.Inventory
{
	using System;
	using UnityEngine;

	/// <summary>
	/// Item.
	/// </summary>
	[Serializable]
	public class Item
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// Color.
		/// </summary>
		[SerializeField]
		public Color Color;

		/// <summary>
		/// Weight.
		/// </summary>
		[SerializeField]
		public float Weight;

		/// <summary>
		/// Price.
		/// </summary>
		[SerializeField]
		public int Price;
	}
}