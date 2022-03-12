namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Changed drag event to work only with one direction.
	/// </summary>
	public class DragOneDirection : UIBehaviour, IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		Vector2 startPosition = Vector2.zero;

		bool dragging;

		bool isHorizontal;

		/// <summary>
		/// Dead zone before
		/// </summary>
		[SerializeField]
		public float DeadZone = 20f;

		/// <summary>
		/// Process begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (!IsActive())
			{
				return;
			}

			startPosition = eventData.position;
		}

		/// <summary>
		/// Process drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (!IsActive())
			{
				return;
			}

			var current = eventData.position;

			if (dragging)
			{
				eventData.position = isHorizontal
					? new Vector2(current.x, startPosition.y)
					: new Vector2(startPosition.x, current.y);
			}
			else
			{
				var delta = current - startPosition;
				dragging = delta.magnitude > DeadZone;
				isHorizontal = Mathf.Abs(delta.x) > Mathf.Abs(delta.y);

				eventData.position = startPosition;
			}
		}

		/// <summary>
		/// Process end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			dragging = false;
		}

		/// <summary>
		/// Process initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
		}
	}
}