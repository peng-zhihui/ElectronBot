namespace UIWidgets.Examples.Inventory
{
	/// <summary>
	/// Item drag data.
	/// </summary>
	public struct ItemDragData
	{
		/// <summary>
		/// Item.
		/// </summary>
		public readonly Item Item;

		/// <summary>
		/// Inventory index.
		/// </summary>
		public readonly int InventoryIndex;

		/// <summary>
		/// Slot index.
		/// </summary>
		public readonly int SlotIndex;

		/// <summary>
		/// Empty instance.
		/// </summary>
		public static ItemDragData Empty
		{
			get
			{
				return new ItemDragData(null, -1, -1);
			}
		}

		private ItemDragData(Item item, int inventoryIndex, int slotIndex)
		{
			Item = item;
			InventoryIndex = inventoryIndex;
			SlotIndex = slotIndex;
		}

		/// <summary>
		/// Create inventory item drag data instance.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="inventoryIndex">Inventory index.</param>
		public static ItemDragData Inventory(Item item, int inventoryIndex)
		{
			return new ItemDragData(item, inventoryIndex, -1);
		}

		/// <summary>
		/// Create slot item drag data instance.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="inventoryIndex">Inventory index.</param>
		/// <param name="slotIndex">Slot index.</param>
		public static ItemDragData Slot(Item item, int slotIndex)
		{
			return new ItemDragData(item, -1, slotIndex);
		}

		/// <summary>
		/// Is instance and target have the same position.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>true if instance and target have the same position; otherwise false.</returns>
		public bool SamePosition(ItemDragData target)
		{
			if ((InventoryIndex > -1) && (target.InventoryIndex > -1))
			{
				return InventoryIndex == target.InventoryIndex;
			}

			if ((SlotIndex > -1) && (target.SlotIndex > -1))
			{
				return SlotIndex == target.SlotIndex;
			}

			return false;
		}
	}
}