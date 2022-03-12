namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRectPage.
	/// </summary>
	public class ScrollRectPage : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, ISubmitHandler
	{
		/// <summary>
		/// The page number.
		/// </summary>
		[HideInInspector]
		public int Page;

		/// <summary>
		/// OnPageSelect event.
		/// </summary>
		[SerializeField]
		public ScrollRectPageSelect OnPageSelect = new ScrollRectPageSelect();

		/// <summary>
		/// Sets the page number.
		/// </summary>
		/// <param name="page">Page.</param>
		public virtual void SetPage(int page)
		{
			Page = page;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerClick event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			OnPageSelect.Invoke(Page);
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerDown event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerUp event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnSubmit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnSubmit(BaseEventData eventData)
		{
			OnPageSelect.Invoke(Page);
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleText styleText, Style style)
		{
			// do nothing
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleText">Style for the text.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleText styleText, Style style)
		{
			// do nothing
		}

		/// <summary>
		/// Toggle interactable or gameobject.active.
		/// </summary>
		[SerializeField]
		[Tooltip("Toggle interactable or gameobject.active.")]
		public bool ToggleInteractable;

		bool isInited;

		/// <summary>
		/// Selectable.
		/// </summary>
		protected Selectable Selectable;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			Selectable = GetComponent<Selectable>();

			isInited = true;
		}

		/// <summary>
		/// Set state.
		/// </summary>
		/// <param name="active">Active.</param>
		public virtual void SetState(bool active)
		{
			Init();

			if (ToggleInteractable && (Selectable != null))
			{
				Selectable.interactable = active;
			}
			else
			{
				gameObject.SetActive(active);
			}
		}
	}
}