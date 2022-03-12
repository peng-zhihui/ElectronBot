namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ScrollListener.
	/// </summary>
	public class ScrollListener : MonoBehaviour, IScrollHandler
	{
		/// <summary>
		/// Scroll event.
		/// </summary>
		public PointerUnityEvent ScrollEvent = new PointerUnityEvent();

		/// <summary>
		/// Handle scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnScroll(PointerEventData eventData)
		{
			ScrollEvent.Invoke(eventData);
		}
	}
}