namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Click events listener.
	/// </summary>
	public class ClickListener : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
	{
		/// <summary>
		/// Pointer click event.
		/// </summary>
		public PointerUnityEvent ClickEvent = new PointerUnityEvent();

		/// <summary>
		/// Pointer down event.
		/// </summary>
		public PointerUnityEvent DownEvent = new PointerUnityEvent();

		/// <summary>
		/// Pointer up event.
		/// </summary>
		public PointerUnityEvent UpEvent = new PointerUnityEvent();

		/// <summary>
		/// Double click event.
		/// </summary>
		public PointerUnityEvent DoubleClickEvent = new PointerUnityEvent();

		/// <summary>
		/// Registered IPointerClickHandler callback.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			ClickEvent.Invoke(eventData);

			if ((eventData.clickCount == 2) && (eventData.button == PointerEventData.InputButton.Left))
			{
				DoubleClickEvent.Invoke(eventData);
			}
		}

		/// <summary>
		/// Registered IPointerDownHandler callback.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
			DownEvent.Invoke(eventData);
		}

		/// <summary>
		/// Registered IPointerUpHandler callback.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp(PointerEventData eventData)
		{
			UpEvent.Invoke(eventData);
		}
	}
}