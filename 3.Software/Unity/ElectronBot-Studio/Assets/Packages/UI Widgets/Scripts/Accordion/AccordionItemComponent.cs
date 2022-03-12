namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// OnClick listener.
	/// </summary>
	public class AccordionItemComponent : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISubmitHandler
	{
		/// <summary>
		/// Item event.
		/// </summary>
		public class ItemEvent : UnityEvent<AccordionItem>
		{
		}

		/// <summary>
		/// Item.
		/// </summary>
		public AccordionItem Item;

		/// <summary>
		/// What to do when the event system send a pointer click event.
		/// </summary>
		public UnityEvent OnClick = new UnityEvent();

		/// <summary>
		/// What to do when the event system send a pointer click event.
		/// </summary>
		public ItemEvent OnItemClick = new ItemEvent();

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			OnClick.Invoke();
			OnItemClick.Invoke(Item);
		}

		/// <summary>
		/// Process the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Process the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Process the submit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			OnClick.Invoke();
		}
	}
}