namespace UIWidgets
{
	using System;
	using System.Collections;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Tooltip.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Tooltip.png
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Tooltip")]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[Obsolete("Replaced with generic Tooltip and TooltipViewer")]
	public class Tooltip : MonoBehaviour,
		IPointerEnterHandler, IPointerExitHandler,
		ISelectHandler, IDeselectHandler,
		IStylable
	{
		/// <summary>
		/// Tooltip object.
		/// </summary>
		[SerializeField]
		protected GameObject tooltipObject;

		/// <summary>
		/// Bring to front tooltip object.
		/// </summary>
		[SerializeField]
		public bool BringToFront = true;

		/// <summary>
		/// Is tooltip object at front.
		/// </summary>
		[NonSerialized]
		protected bool IsAtFront;

		/// <summary>
		/// Seconds before tooltip shown after pointer enter.
		/// </summary>
		[SerializeField]
		public float ShowDelay = 0.3f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// The tooltip object.
		/// </summary>
		public GameObject TooltipObject
		{
			get
			{
				return tooltipObject;
			}

			set
			{
				tooltipObject = value;

				if (tooltipObject != null)
				{
					TooltipParent = tooltipObject.transform.parent as RectTransform;
				}
			}
		}

		/// <summary>
		/// Anchored position.
		/// </summary>
		[HideInInspector]
		protected Vector2? anchoredPosition;

		/// <summary>
		/// Canvas transform.
		/// </summary>
		[SerializeField]
		protected RectTransform ParentCanvas;

		/// <summary>
		/// Tooltip parent object.
		/// </summary>
		[HideInInspector]
		protected RectTransform TooltipParent;

		/// <summary>
		/// Show event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnShow = new UnityEvent();

		/// <summary>
		/// Hide event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnHide = new UnityEvent();

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
		protected virtual void Init()
		{
			TooltipObject = tooltipObject;

			if (TooltipObject != null)
			{
				if (ParentCanvas == null)
				{
					ParentCanvas = UtilitiesUI.FindTopmostCanvas(TooltipParent);
				}

				TooltipObject.SetActive(false);
			}
		}

		/// <summary>
		/// Current coroutine.
		/// </summary>
		protected IEnumerator currentCoroutine;

		/// <summary>
		/// Show coroutine.
		/// </summary>
		/// <returns>Coroutine.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		protected virtual IEnumerator ShowCoroutine()
		{
			yield return UtilitiesTime.Wait(ShowDelay, UnscaledTime);

			Init();

			if ((ParentCanvas != null) && BringToFront && (!IsAtFront))
			{
				IsAtFront = true;
				anchoredPosition = (TooltipObject.transform as RectTransform).anchoredPosition;
				TooltipParent = tooltipObject.transform.parent as RectTransform;
				TooltipObject.transform.SetParent(ParentCanvas);
			}

			TooltipObject.SetActive(true);
			OnShow.Invoke();
		}

		/// <summary>
		/// Show this tooltip.
		/// </summary>
		public void Show()
		{
			if (TooltipObject == null)
			{
				return;
			}

			currentCoroutine = ShowCoroutine();
			StartCoroutine(currentCoroutine);
		}

		/// <summary>
		/// Hide coroutine.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected virtual IEnumerator HideCoroutine()
		{
			if (currentCoroutine != null)
			{
				StopCoroutine(currentCoroutine);
			}

			if (TooltipObject != null)
			{
				TooltipObject.SetActive(false);
				OnHide.Invoke();
				yield return null;

				if (IsAtFront)
				{
					IsAtFront = false;
					TooltipObject.transform.SetParent(TooltipParent);
					if (anchoredPosition != null)
					{
						(TooltipObject.transform as RectTransform).anchoredPosition = (Vector2)anchoredPosition;
					}
				}
			}
		}

		/// <summary>
		/// Hide this tooltip.
		/// </summary>
		public void Hide()
		{
			StartCoroutine(HideCoroutine());
		}

		/// <summary>
		/// Process the pointer enter event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			Show();
		}

		/// <summary>
		/// Process the pointer exit event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			Hide();
		}

		/// <summary>
		/// Process the select event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnSelect(BaseEventData eventData)
		{
			Show();
		}

		/// <summary>
		/// Process the deselect event.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		public void OnDeselect(BaseEventData eventData)
		{
			Hide();
		}

		/// <summary>
		/// Place the tooltip in its correct hierarchy and position.
		/// </summary>
		protected virtual void OnEnable()
		{
			if (IsAtFront)
			{
				IsAtFront = false;
				TooltipObject.transform.SetParent(TooltipParent);
				if (anchoredPosition != null)
				{
					(TooltipObject.transform as RectTransform).anchoredPosition = anchoredPosition.Value;
				}
			}
		}

		/// <summary>
		/// Disable the tooltip automatically.
		/// </summary>
		protected virtual void OnDisable()
		{
			if (IsAtFront && (TooltipObject != null))
			{
				TooltipObject.SetActive(false);
				OnHide.Invoke();
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			var stylable = Compatibility.GetComponent<IStylable>(tooltipObject);
			if (stylable != null)
			{
				stylable.SetStyle(style);
			}
			else
			{
				style.Tooltip.Background.ApplyTo(tooltipObject);
				style.Tooltip.Text.ApplyTo(tooltipObject.transform.Find("Text"));
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			var stylable = Compatibility.GetComponent<IStylable>(tooltipObject);
			if (stylable != null)
			{
				stylable.GetStyle(style);
			}
			else
			{
				style.Tooltip.Background.GetFrom(tooltipObject);
				style.Tooltip.Text.GetFrom(tooltipObject.transform.Find("Text"));
			}

			return true;
		}
		#endregion

		#if UNITY_EDITOR
		/// <summary>
		/// Create tooltip object.
		/// </summary>
		protected virtual void CreateTooltipObject()
		{
			TooltipObject = UtilitiesEditor.CreateWidgetFromPrefab(PrefabsMenu.Instance.Tooltip);
			TooltipObject.transform.SetParent(transform);

			var tooltipRectTransform = TooltipObject.transform as RectTransform;

			tooltipRectTransform.anchorMin = new Vector2(1, 1);
			tooltipRectTransform.anchorMax = new Vector2(1, 1);
			tooltipRectTransform.pivot = new Vector2(1, 0);

			tooltipRectTransform.anchoredPosition = new Vector2(0, 0);
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected virtual void Reset()
		{
			if (TooltipObject == null)
			{
				CreateTooltipObject();
			}

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}

		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif
	}
}