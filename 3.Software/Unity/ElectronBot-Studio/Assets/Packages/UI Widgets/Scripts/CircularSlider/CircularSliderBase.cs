namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for CircularSliders.
	/// </summary>
	/// <typeparam name="T">Type of value.</typeparam>
	[RequireComponent(typeof(RectTransform))]
	[RequireComponent(typeof(RingEffect))]
	[DataBindSupport]
	#if UNITY_2018_4_OR_NEWER
	[ExecuteAlways]
	#else
	[ExecuteInEditMode]
	#endif
	public abstract class CircularSliderBase<T> : UIBehaviour, IStylable,
		IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerDownHandler, IPointerClickHandler, IPointerUpHandler
	{
		#region Interactable
		[SerializeField]
		bool interactable = true;

		/// <summary>
		/// Is widget interactable.
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		public bool Interactable
		{
			get
			{
				return interactable;
			}

			set
			{
				if (interactable != value)
				{
					interactable = value;
					InteractableChanged();
				}
			}
		}

		/// <summary>
		/// If the canvas groups allow interaction.
		/// </summary>
		protected bool GroupsAllowInteraction = true;

		/// <summary>
		/// The CanvasGroup cache.
		/// </summary>
		protected List<CanvasGroup> CanvasGroupCache = new List<CanvasGroup>();

		/// <summary>
		/// Process the CanvasGroupChanged event.
		/// </summary>
		protected override void OnCanvasGroupChanged()
		{
			var groupAllowInteraction = true;
			var t = transform;
			while (t != null)
			{
				t.GetComponents(CanvasGroupCache);
				var shouldBreak = false;
				foreach (var canvas_group in CanvasGroupCache)
				{
					if (!canvas_group.interactable)
					{
						groupAllowInteraction = false;
						shouldBreak = true;
					}

					shouldBreak |= canvas_group.ignoreParentGroups;
				}

				if (shouldBreak)
				{
					break;
				}

				t = t.parent;
			}

			if (groupAllowInteraction != GroupsAllowInteraction)
			{
				GroupsAllowInteraction = groupAllowInteraction;
				InteractableChanged();
			}
		}

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive()
		{
			return base.IsActive() && GroupsAllowInteraction && Interactable;
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		protected virtual void InteractableChanged()
		{
			if (!base.IsActive())
			{
				return;
			}

			OnInteractableChange(GroupsAllowInteraction && Interactable);
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected virtual void OnInteractableChange(bool interactableState)
		{
		}
		#endregion

		[SerializeField]
		DragListener handle;

		/// <summary>
		/// Handle.
		/// </summary>
		public DragListener Handle
		{
			get
			{
				return handle;
			}

			set
			{
				if (handle != null)
				{
					handle.OnDragStartEvent.RemoveListener(OnBeginDrag);
					handle.OnDragEvent.RemoveListener(OnDrag);
					handle.OnDragEndEvent.RemoveListener(OnEndDrag);
				}

				handle = value;

				if (handle != null)
				{
					handle.OnDragStartEvent.AddListener(OnBeginDrag);
					handle.OnDragEvent.AddListener(OnDrag);
					handle.OnDragEndEvent.AddListener(OnEndDrag);
				}

				UpdateTracker();
				UpdatePositions();
			}
		}

		[SerializeField]
		RectTransform arrow;

		/// <summary>
		/// Arrow.
		/// </summary>
		public RectTransform Arrow
		{
			get
			{
				return arrow;
			}

			set
			{
				arrow = value;

				UpdateTracker();
				UpdatePositions();
			}
		}

		[SerializeField]
		[Range(0, 359)]
		float startAngle = 0;

		/// <summary>
		/// Start angle.
		/// </summary>
		public float StartAngle
		{
			get
			{
				return startAngle;
			}

			set
			{
				startAngle = value;
				UpdatePositions();
			}
		}

		[SerializeField]
		T minValue;

		/// <summary>
		/// Min value.
		/// </summary>
		[DataBindField]
		public T MinValue
		{
			get
			{
				return minValue;
			}

			set
			{
				minValue = value;
				Value = this.value;
			}
		}

		[SerializeField]
		T maxValue;

		/// <summary>
		/// Max value.
		/// </summary>
		[DataBindField]
		public T MaxValue
		{
			get
			{
				return maxValue;
			}

			set
			{
				maxValue = value;
				Value = this.value;
			}
		}

		[SerializeField]
		T value;

		/// <summary>
		/// Value.
		/// </summary>
		[DataBindField]
		public T Value
		{
			get
			{
				return value;
			}

			set
			{
				var v = ClampValue(value);
				if (!EqualityComparer<T>.Default.Equals(this.value, v))
				{
					this.value = v;
					UpdatePositions();
					InvokeValueChanged(this.value);
				}
			}
		}

		[SerializeField]
		T step;

		/// <summary>
		/// Step.
		/// </summary>
		[DataBindField]
		public T Step
		{
			get
			{
				return step;
			}

			set
			{
				step = value;
				Value = this.value;
			}
		}

		/// <summary>
		/// Value changed event.
		/// </summary>
		[SerializeField]
		[DataBindEvent("Value")]
		public UnityEvent OnChange = new UnityEvent();

		/// <summary>
		/// Required delayed update.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected bool DelayedUpdate;

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		RingEffect ring;

		/// <summary>
		/// Ring effect.
		/// </summary>
		public RingEffect Ring
		{
			get
			{
				if (ring == null)
				{
					ring = GetComponent<RingEffect>();
				}

				return ring;
			}
		}

		/// <summary>
		/// Properties tracker.
		/// </summary>
		protected DrivenRectTransformTracker PropertiesTracker;

		/// <summary>
		/// Driver properties.
		/// </summary>
		protected DrivenTransformProperties DrivenProperties = DrivenTransformProperties.AnchoredPosition | DrivenTransformProperties.Anchors | DrivenTransformProperties.Pivot | DrivenTransformProperties.Rotation;

		bool isInited;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
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

			isInited = true;

			Handle = handle;
			Arrow = arrow;
			Value = value;
		}

		/// <summary>
		/// Clamp value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Value in range [MinValue, MaxValue].</returns>
		protected abstract T ClampValue(T value);

		/// <summary>
		/// Update tracker.
		/// </summary>
		protected virtual void UpdateTracker()
		{
			PropertiesTracker.Clear();

			if (Handle != null)
			{
				PropertiesTracker.Add(this, Handle.RectTransform, DrivenProperties);

				var v05 = new Vector2(0.5f, 0.5f);
				Handle.RectTransform.anchorMin = v05;
				Handle.RectTransform.anchorMax = v05;
				Handle.RectTransform.pivot = v05;
			}

			if (Arrow != null)
			{
				PropertiesTracker.Add(this, Arrow, DrivenProperties);

				Arrow.anchorMin = new Vector2(0.5f, 0.5f);
				Arrow.anchorMax = new Vector2(0.5f, 0.5f);
				Arrow.pivot = new Vector2(0, 0.5f);
				Arrow.anchoredPosition = Vector2.zero;
			}
		}

		/// <summary>
		/// Update positions.
		/// </summary>
		protected virtual void UpdatePositions()
		{
			var size = RectTransform.rect.size / 2f;
			size.x -= Ring.Thickness / 2f;
			size.y -= Ring.Thickness / 2f;

			var angle = -(Value2Angle(Value) + StartAngle) % 360;
			var rotation = Quaternion.Euler(0f, 0f, angle);

			var pos = new Vector2(
				size.x * Mathf.Cos(angle * Mathf.Deg2Rad),
				size.y * Mathf.Sin(angle * Mathf.Deg2Rad));

			if (Handle != null)
			{
				Handle.RectTransform.rotation = rotation;
				Handle.RectTransform.anchoredPosition = pos;
			}

			if (Arrow != null)
			{
				Arrow.rotation = rotation;
				Arrow.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, pos.magnitude);
			}
		}

		/// <summary>
		/// Convert angle to value.
		/// </summary>
		/// <param name="angle">Angle.</param>
		/// <returns>Value.</returns>
		public abstract T Angle2Value(float angle);

		/// <summary>
		/// Convert value to angle.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Angle.</returns>
		public abstract float Value2Angle(T value);

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			Rotate(eventData);
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			Rotate(eventData);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			Rotate(eventData);
		}

		/// <summary>
		/// Process pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
			// do nothing, without it OnPointerClick does not work
		}

		/// <summary>
		/// Process pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp(PointerEventData eventData)
		{
			// do nothing, without it OnPointerClick does not work
		}

		/// <summary>
		/// Process pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			Rotate(eventData);
		}

		/// <summary>
		/// Rotate this instance using specified event data.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void Rotate(PointerEventData eventData)
		{
			Vector2 point;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out point);

			var base_angle = -DragPoint2Angle(point);
			Value = Angle2Value(base_angle - StartAngle);
		}

		/// <summary>
		/// Convert drag point to angle.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Angle.</returns>
		protected float DragPoint2Angle(Vector2 point)
		{
			var size = RectTransform.rect.size;
			var relative = new Vector2(
				point.x + (size.x * (RectTransform.pivot.x - 0.5f)),
				point.y + (size.y * (RectTransform.pivot.y - 0.5f)));

			return Point2Angle(relative);
		}

		/// <summary>
		/// Convert point to the angle.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Angle in range [-180f, 180f].</returns>
		public static float Point2Angle(Vector2 point)
		{
			var angle_rad = Mathf.Atan2(point.y, point.x);
			var angle = angle_rad * Mathf.Rad2Deg;

			angle %= 360f;

			if (angle > 180f)
			{
				angle -= 360f;
			}

			return angle;
		}

		/// <summary>
		/// Invoke value changed events.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void InvokeValueChanged(T value)
		{
			OnChange.Invoke();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Process the update event.
		/// </summary>
		protected virtual void Update()
		{
			if (DelayedUpdate)
			{
				DelayedUpdate = false;

				UpdateTracker();
				UpdatePositions();
			}
		}

		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			value = ClampValue(value);

			DelayedUpdate = true;
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			style.CircularSlider.Ring.ApplyTo(GetComponent<Image>());

			if (Ring != null)
			{
				Ring.RingColor = style.CircularSlider.RingColor;
			}

			if (Handle != null)
			{
				style.CircularSlider.Handle.ApplyTo(Handle.GetComponent<Image>());
			}

			if (Arrow != null)
			{
				style.CircularSlider.Arrow.ApplyTo(Arrow.GetComponent<Image>());
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.CircularSlider.Ring.GetFrom(GetComponent<Image>());

			if (Ring != null)
			{
				style.CircularSlider.RingColor = Ring.RingColor;
			}

			if (Handle != null)
			{
				style.CircularSlider.Handle.GetFrom(Handle.GetComponent<Image>());
			}

			if (Arrow != null)
			{
				style.CircularSlider.Arrow.GetFrom(Arrow.GetComponent<Image>());
			}

			return true;
		}
		#endregion
	}
}