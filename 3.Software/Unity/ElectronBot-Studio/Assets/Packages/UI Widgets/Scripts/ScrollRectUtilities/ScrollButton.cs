namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Scroll button.
	/// </summary>
	public class ScrollButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler
	{
		/// <summary>
		/// OnClick event.
		/// </summary>
		public UnityEvent OnClick = new UnityEvent();

		/// <summary>
		/// OnDown.
		/// </summary>
		public UnityEvent OnDown = new UnityEvent();

		/// <summary>
		/// OnUp.
		/// </summary>
		public UnityEvent OnUp = new UnityEvent();

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			OnClick.Invoke();
		}

		/// <summary>
		/// Process the pointer down event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			OnDown.Invoke();
		}

		/// <summary>
		/// Process the pointer up event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			OnUp.Invoke();
		}
	}
}