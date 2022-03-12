namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewButtons navigation.
	/// </summary>
	public class ListViewButtonNavigation : MonoBehaviour, IMoveHandler
	{
		/// <summary>
		/// Button index.
		/// </summary>
		[SerializeField]
		protected int ButtonIndex;

		/// <summary>
		/// Current item.
		/// </summary>
		[SerializeField]
		protected ListViewButtonsComponent Item;

		/// <summary>
		/// Process move event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnMove(AxisEventData eventData)
		{
			GameObject target = null;
			var lv = Item.Owner as ListViewButtons;

			switch (eventData.moveDir)
			{
				case MoveDirection.Left:
					target = Item.GetPrevButton(ButtonIndex);
					break;
				case MoveDirection.Right:
					target = Item.GetNextButton(ButtonIndex);
					break;
				case MoveDirection.Up:
					var prev_index = Item.Index - 1;
					if (!lv.IsValid(prev_index))
					{
						break;
					}

					if (!lv.IsVisible(prev_index, 1f))
					{
						lv.ScrollTo(prev_index);
					}

					var prev_item = lv.GetItemComponent(prev_index);
					if (prev_item != null)
					{
						target = prev_item.GetFirstButton();
					}

					break;
				case MoveDirection.Down:
					var next_index = Item.Index + 1;
					if (!lv.IsValid(next_index))
					{
						break;
					}

					if (!lv.IsVisible(next_index, 1f))
					{
						lv.ScrollTo(next_index);
					}

					var next_item = lv.GetItemComponent(Item.Index + 1);
					if (next_item != null)
					{
						target = next_item.GetFirstButton();
					}

					break;
				default:
					target = null;
					break;
			}

			if (target != null)
			{
				eventData.selectedObject = target;
			}
		}
	}
}