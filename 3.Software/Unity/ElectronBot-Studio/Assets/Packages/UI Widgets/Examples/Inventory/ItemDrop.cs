namespace UIWidgets.Examples.Inventory
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Item drop.
	/// </summary>
	[RequireComponent(typeof(ItemView))]
	public class ItemDrop : MonoBehaviour, IDropSupport<ItemDragData>
	{
		/// <summary>
		/// Current ListView.
		/// </summary>
		[SerializeField]
		public InventoryController Controller;

		/// <summary>
		/// Target.
		/// </summary>
		protected ItemView target;

		/// <summary>
		/// Target.
		/// </summary>
		public ItemView Target
		{
			get
			{
				if (target == null)
				{
					target = GetComponent<ItemView>();
				}

				return target;
			}
		}

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual bool CanReceiveDrop(ItemDragData data, PointerEventData eventData)
		{
			if (Target == null)
			{
				return false;
			}

			if (!Controller.CanSwap(data, Target.GetDragData()))
			{
				return false;
			}

			ShowDropIndicator(data);

			return true;
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void Drop(ItemDragData data, PointerEventData eventData)
		{
			HideDropIndicator();

			Controller.Swap(data, Target.GetDragData());
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void DropCanceled(ItemDragData data, PointerEventData eventData)
		{
			HideDropIndicator();
		}

		/// <summary>
		/// Shows the drop indicator.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void ShowDropIndicator(ItemDragData data)
		{
		}

		/// <summary>
		/// Hides the drop indicator.
		/// </summary>
		protected virtual void HideDropIndicator()
		{
		}
	}
}