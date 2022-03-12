namespace UIWidgets.Examples
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Table cell.
	/// </summary>
	public class TableCell : MonoBehaviour, IPointerUpHandler, IPointerDownHandler, IPointerClickHandler
	{
		/// <summary>
		/// CellClicked event.
		/// </summary>
		public UnityEvent CellClicked = new UnityEvent();

		/// <summary>
		/// Handle pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			CellClicked.Invoke();
		}

		/// <summary>
		/// Handle pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			// required for the OnPointerClick event
		}

		/// <summary>
		/// Handle pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			// required for the OnPointerClick event
		}
	}
}