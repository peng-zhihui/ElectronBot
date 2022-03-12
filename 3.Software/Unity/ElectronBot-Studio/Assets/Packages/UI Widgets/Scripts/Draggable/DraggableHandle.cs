namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Draggable handle.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DraggableHandle : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
	{
		/// <summary>
		/// Begin drag event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnBeginDragEvent = new PointerUnityEvent();

		/// <summary>
		/// During drag event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnDragEvent = new PointerUnityEvent();

		/// <summary>
		/// End drag event.
		/// </summary>
		[SerializeField]
		public PointerUnityEvent OnEndDragEvent = new PointerUnityEvent();

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			OnBeginDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			OnDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			OnEndDragEvent.Invoke(eventData);
		}
	}
}