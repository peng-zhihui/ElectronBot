namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// TableHeader cell.
	/// </summary>
	public class TableHeaderCell : MonoBehaviour,
		IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		/// <summary>
		/// OnInitializePotentialDrag event.
		/// </summary>
		public PointerUnityEvent OnInitializePotentialDragEvent = new PointerUnityEvent();

		/// <summary>
		/// OnBeginDrag event.
		/// </summary>
		public PointerUnityEvent OnBeginDragEvent = new PointerUnityEvent();

		/// <summary>
		/// OnDrag event.
		/// </summary>
		public PointerUnityEvent OnDragEvent = new PointerUnityEvent();

		/// <summary>
		/// OnEndDrag event.
		/// </summary>
		public PointerUnityEvent OnEndDragEvent = new PointerUnityEvent();

		/// <summary>
		/// Called by a BaseInputModule when a drag has been found but before it is valid to begin the drag.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			OnInitializePotentialDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			OnBeginDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// When draging is occuring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			OnDragEvent.Invoke(eventData);
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			OnEndDragEvent.Invoke(eventData);
		}
	}
}