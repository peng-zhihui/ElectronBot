namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Bring to front UI object on click.
	/// Use carefully: it change hierarchy. Objects under layout control will be at another positions.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Bring To Front")]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class BringToFront : MonoBehaviour, IPointerDownHandler
	{
		/// <summary>
		/// Bring to front UI object with parents.
		/// </summary>
		[SerializeField]
		public bool WithParents = false;

		/// <summary>
		/// Process the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			ToFront();
		}

		/// <summary>
		/// Bring to front UI object.
		/// </summary>
		public virtual void ToFront()
		{
			ToFront(transform);
		}

		/// <summary>
		/// Bring to front the specified object.
		/// </summary>
		/// <param name="obj">Object.</param>
		void ToFront(Transform obj)
		{
			obj.SetAsLastSibling();
			if (WithParents && (obj.parent != null))
			{
				ToFront(obj.parent);
			}
		}
	}
}