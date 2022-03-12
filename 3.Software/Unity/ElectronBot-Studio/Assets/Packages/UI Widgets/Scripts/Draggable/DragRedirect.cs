namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Redirect drag events from current gameobject to specified.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	public class DragRedirect : UIBehaviour, IBeginDragHandler, IInitializePotentialDragHandler, IDragHandler, IEndDragHandler, IScrollHandler
	{
		/// <summary>
		/// Drag events will be redirected to this gameobject.
		/// </summary>
		[SerializeField]
		public GameObject RedirectTo;

		/// <summary>
		/// Mark drag event as used.
		/// </summary>
		[SerializeField]
		[Tooltip("Mark drag event as used.")]
		public bool MarkAsUsed = true;

		/// <summary>
		/// Minimal distance from start position to allow event redirect.
		/// </summary>
		[SerializeField]
		public Vector2 MinDistance = Vector2.zero;

		/// <summary>
		/// Start position.
		/// </summary>
		protected Vector2 StartPosition;

		/// <summary>
		/// Is valid distance?
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if distance exceeds min distance; otherwise false.</returns>
		protected virtual bool IsValidDistance(PointerEventData eventData)
		{
			var delta = eventData.position - StartPosition;
			return (Mathf.Abs(delta.x) >= MinDistance.x) && (Mathf.Abs(delta.y) >= MinDistance.y);
		}

		/// <summary>
		/// Gets the handlers.
		/// </summary>
		/// <returns>The handlers.</returns>
		/// <typeparam name="T">The handler type.</typeparam>
		protected T[] GetHandlers<T>()
			where T : class
		{
#if UNITY_5_0 || UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			return RedirectTo.GetComponents<T>();
#else
			var components = RedirectTo.GetComponents(typeof(T));
			var handlers = new T[components.Length];
			for (int i = 0; i < components.Length; i++)
			{
				handlers[i] = components[i] as T;
			}

			return handlers;
#endif
		}

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			StartPosition = eventData.position;

			foreach (var handler in GetHandlers<IBeginDragHandler>())
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnBeginDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag has been found but before it is valid to begin the drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			foreach (var handler in GetHandlers<IInitializePotentialDragHandler>())
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnInitializePotentialDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// When dragging is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!IsValidDistance(eventData))
			{
				return;
			}

			foreach (var handler in GetHandlers<IDragHandler>())
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!IsValidDistance(eventData))
			{
				return;
			}

			foreach (var handler in GetHandlers<IEndDragHandler>())
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnEndDrag(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnScroll event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnScroll(PointerEventData eventData)
		{
			foreach (var handler in GetHandlers<IScrollHandler>())
			{
				if (handler is DragRedirect)
				{
					continue;
				}

				eventData.Reset();
				handler.OnScroll(eventData);
			}

			if (MarkAsUsed)
			{
				eventData.Use();
			}
			else
			{
				eventData.Reset();
			}
		}
	}
}