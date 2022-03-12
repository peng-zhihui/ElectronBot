namespace UIWidgets.Examples.Inventory
{
	using UnityEngine;

	/// <summary>
	/// Inventory controller.
	/// </summary>
	public class InventoryController : MonoBehaviour
	{
		/// <summary>
		/// Inventory view.
		/// </summary>
		[SerializeField]
		protected InventoryView InventoryView;

		/// <summary>
		/// Slot views.
		/// </summary>
		[SerializeField]
		protected ItemView[] SlotViews = new ItemView[6];

		/// <summary>
		/// Tooltip.
		/// </summary>
		[SerializeField]
		protected InventoryTooltip Tooltip;

		/// <summary>
		/// Items.
		/// </summary>
		protected ObservableList<Item> Items = new ObservableList<Item>()
		{
			// 00-09
			new Item() { Name = "Item 1", Color = new Color(0.9f, 0.9f, 0.9f), Price = 100, Weight = 10f },
			new Item() { Name = "Item 2", Color = new Color(0.9f, 0.5f, 0.9f), Price = 200, Weight = 20f },
			new Item() { Name = "Item 3", Color = new Color(0.9f, 0.9f, 0.5f), Price = 300, Weight = 30f },
			null,
			null,
			new Item() { Name = "Item 4", Color = new Color(0.5f, 0.9f, 0.9f), Price = 400, Weight = 40f },
			null,
			null,
			null,
			new Item() { Name = "Item 5", Color = new Color(0.5f, 0.5f, 0.9f), Price = 500, Weight = 50f },

			// 10-19
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,
			null,

			// 20-29
			null,
			null,
			null,
			new Item() { Name = "Item 6", Color = new Color(0.5f, 0.5f, 0.5f), Price = 600, Weight = 60f },
			null,
			null,
			null,
			null,
			null,
			null,
		};

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected virtual void Start()
		{
			InventoryView.DataSource = Items;

			for (int i = 0; i < SlotViews.Length; i++)
			{
				SlotViews[i].SlotIndex = i;
			}

			InventoryView.OnSelectObject.AddListener(SetTooltipSelected);
			InventoryView.Select(0);
		}

		void SetTooltipSelected(int index)
		{
			if (Tooltip == null)
			{
				return;
			}

			Tooltip.SetSelected(InventoryView.DataSource[index]);
		}

		/// <summary>
		/// Check if items can be swapped.
		/// </summary>
		/// <param name="dragged">Dragged item.</param>
		/// <param name="dropTarget">Target item.</param>
		/// <returns>true if items can be swapped; otherwise false.</returns>
		public bool CanSwap(ItemDragData dragged, ItemDragData dropTarget)
		{
			return !dragged.SamePosition(dropTarget);
		}

		/// <summary>
		/// Swap items.
		/// </summary>
		/// <param name="dragged">Dragged item.</param>
		/// <param name="dropTarget">Target item.</param>
		public void Swap(ItemDragData dragged, ItemDragData dropTarget)
		{
			Items.BeginUpdate();

			if (dropTarget.InventoryIndex > -1)
			{
				Items[dropTarget.InventoryIndex] = dragged.Item;
			}
			else if (dropTarget.SlotIndex > -1)
			{
				SlotViews[dropTarget.SlotIndex].SetData(dragged.Item);
			}

			if (dragged.InventoryIndex > -1)
			{
				Items[dragged.InventoryIndex] = dropTarget.Item;
			}
			else if(dragged.SlotIndex > -1)
			{
				SlotViews[dragged.SlotIndex].SetData(dropTarget.Item);
			}

			Items.EndUpdate();
		}
	}
}