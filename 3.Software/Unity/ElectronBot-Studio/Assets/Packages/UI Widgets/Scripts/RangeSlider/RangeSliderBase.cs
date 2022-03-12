namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Range slider type.
	/// </summary>
	public enum RangeSliderType
	{
		/// <summary>
		/// The allow handle overlay.
		/// </summary>
		AllowHandleOverlay = 0,

		/// <summary>
		/// The disable handle overlay.
		/// </summary>
		DisableHandleOverlay = 1,
	}

	/// <summary>
	/// Range slider base.
	/// </summary>
	/// <typeparam name="T">Type of slider value.</typeparam>
	[DataBindSupport]
	public abstract class RangeSliderBase<T> : UIWidgetsBehaviour, IPointerClickHandler, IStylable, IValidateable
		where T : struct
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
			return base.IsActive() && GroupsAllowInteraction && Interactable && isInited;
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		protected virtual void InteractableChanged()
		{
			if (!base.IsActive() || !isInited)
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
			if (HandleMin != null)
			{
				HandleMin.interactable = interactableState;
			}

			if (HandleMax != null)
			{
				HandleMax.interactable = interactableState;
			}
		}
		#endregion

		/// <summary>
		/// OnChangeEvent.
		/// </summary>
		[Serializable]
		public class OnChangeEvent : UnityEvent<T, T>
		{
		}

		[SerializeField]
		RangeSliderType type = RangeSliderType.AllowHandleOverlay;

		/// <summary>
		/// Gets or sets the slider type.
		/// </summary>
		/// <value>The type.</value>
		public RangeSliderType Type
		{
			get
			{
				return type;
			}

			set
			{
				type = value;
				UpdateMinHandle();
				UpdateMaxHandle();
				UpdateFill();
			}
		}

		/// <summary>
		/// The minimum value.
		/// </summary>
		[SerializeField]
		protected T valueMin;

		/// <summary>
		/// Gets or sets the minimum value.
		/// </summary>
		/// <value>The minimum value.</value>
		[DataBindField]
		public T ValueMin
		{
			get
			{
				return valueMin;
			}

			set
			{
				if (!EqualityComparer<T>.Default.Equals(valueMin, InBoundsMin(value)))
				{
					valueMin = InBoundsMin(value);
					UpdateMinHandle();
					UpdateFill();
					OnValuesChanged.Invoke(valueMin, valueMax);
					OnChange.Invoke();
				}
			}
		}

		/// <summary>
		/// The maximum value.
		/// </summary>
		[SerializeField]
		protected T valueMax;

		/// <summary>
		/// Gets or sets the maximum value.
		/// </summary>
		/// <value>The maximum value.</value>
		[DataBindField]
		public T ValueMax
		{
			get
			{
				return valueMax;
			}

			set
			{
				if (!EqualityComparer<T>.Default.Equals(valueMax, InBoundsMax(value)))
				{
					valueMax = InBoundsMax(value);
					UpdateMaxHandle();
					UpdateFill();
					OnValuesChanged.Invoke(valueMin, valueMax);
					OnChange.Invoke();
				}
			}
		}

		/// <summary>
		/// The step.
		/// </summary>
		[SerializeField]
		protected T step;

		/// <summary>
		/// Gets or sets the step.
		/// </summary>
		/// <value>The step.</value>
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
			}
		}

		/// <summary>
		/// The minimum limit.
		/// </summary>
		[SerializeField]
		protected T limitMin;

		/// <summary>
		/// Gets or sets the minimum limit.
		/// </summary>
		/// <value>The minimum limit.</value>
		[DataBindField]
		public T LimitMin
		{
			get
			{
				return limitMin;
			}

			set
			{
				limitMin = value;
				ValueMin = valueMin;
				ValueMax = valueMax;
			}
		}

		/// <summary>
		/// The maximum limit.
		/// </summary>
		[SerializeField]
		protected T limitMax;

		/// <summary>
		/// Gets or sets the maximum limit.
		/// </summary>
		/// <value>The maximum limit.</value>
		[DataBindField]
		public T LimitMax
		{
			get
			{
				return limitMax;
			}

			set
			{
				limitMax = value;
				ValueMin = valueMin;
				ValueMax = valueMax;
			}
		}

		/// <summary>
		/// The handle minimum.
		/// </summary>
		[SerializeField]
		protected RangeSliderHandle handleMin;

		/// <summary>
		/// The handle minimum rect.
		/// </summary>
		protected RectTransform handleMinRect;

		/// <summary>
		/// Gets the handle minimum rect.
		/// </summary>
		/// <value>The handle minimum rect.</value>
		public RectTransform HandleMinRect
		{
			get
			{
				if (handleMin != null && handleMinRect == null)
				{
					handleMinRect = handleMin.transform as RectTransform;
				}

				return handleMinRect;
			}
		}

		/// <summary>
		/// Gets or sets the handle minimum.
		/// </summary>
		/// <value>The handle minimum.</value>
		public RangeSliderHandle HandleMin
		{
			get
			{
				return handleMin;
			}

			set
			{
				handleMin = value;
				handleMinRect = null;
				handleMin.IsHorizontal = IsHorizontal;
				handleMin.PositionLimits = MinPositionLimits;
				handleMin.PositionChanged = UpdateMinValue;
				handleMin.OnSubmit = SelectMaxHandle;
				handleMin.Increase = IncreaseMin;
				handleMin.Decrease = DecreaseMin;

				HandleMinRect.anchorMin = new Vector2(0, 0);
				HandleMinRect.anchorMax = IsHorizontal() ? new Vector2(0, 1) : new Vector2(1, 0);
			}
		}

		/// <summary>
		/// The handle max.
		/// </summary>
		[SerializeField]
		protected RangeSliderHandle handleMax;

		/// <summary>
		/// The handle max rect.
		/// </summary>
		protected RectTransform handleMaxRect;

		/// <summary>
		/// Gets the handle maximum rect.
		/// </summary>
		/// <value>The handle maximum rect.</value>
		public RectTransform HandleMaxRect
		{
			get
			{
				if (handleMax != null && handleMaxRect == null)
				{
					handleMaxRect = handleMax.transform as RectTransform;
				}

				return handleMaxRect;
			}
		}

		/// <summary>
		/// Gets or sets the handle max.
		/// </summary>
		/// <value>The handle max.</value>
		public RangeSliderHandle HandleMax
		{
			get
			{
				return handleMax;
			}

			set
			{
				handleMax = value;
				handleMaxRect = null;
				handleMax.IsHorizontal = IsHorizontal;
				handleMax.PositionLimits = MaxPositionLimits;
				handleMax.PositionChanged = UpdateMaxValue;
				handleMax.OnSubmit = SelectMinHandle;
				handleMax.Increase = IncreaseMax;
				handleMax.Decrease = DecreaseMax;

				HandleMaxRect.anchorMin = new Vector2(0, 0);
				HandleMaxRect.anchorMax = IsHorizontal() ? new Vector2(0, 1) : new Vector2(1, 0);
			}
		}

		/// <summary>
		/// The usable range rect.
		/// </summary>
		[SerializeField]
		protected RectTransform UsableRangeRect;

		/// <summary>
		/// The fill rect.
		/// </summary>
		[SerializeField]
		protected RectTransform FillRect;

		/// <summary>
		/// The range slider rect.
		/// </summary>
		protected RectTransform rangeSliderRect;

		/// <summary>
		/// Gets the handle maximum rect.
		/// </summary>
		/// <value>The handle maximum rect.</value>
		public RectTransform RangeSliderRect
		{
			get
			{
				if (rangeSliderRect == null)
				{
					rangeSliderRect = transform as RectTransform;
				}

				return rangeSliderRect;
			}
		}

		/// <summary>
		/// OnValuesChanged event.
		/// </summary>
		[DataBindEvent("ValueMin", "ValueMax")]
		[FormerlySerializedAs("OnValuesChange")]
		public OnChangeEvent OnValuesChanged = new OnChangeEvent();

		/// <summary>
		/// OnValuesChanged event.
		/// </summary>
		[Obsolete("Renamed to OnValuesChanged.")]
		public OnChangeEvent OnValuesChange
		{
			get
			{
				return OnValuesChanged;
			}
		}

		/// <summary>
		/// OnChange event.
		/// </summary>
		public UnityEvent OnChange = new UnityEvent();

		/// <summary>
		/// Whole number of steps.
		/// </summary>
		public bool WholeNumberOfSteps;

		bool isInited;

		void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			HandleMin = handleMin;
			HandleMax = handleMax;
			UpdateMinHandle();
			UpdateMaxHandle();
			UpdateFill();
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Init();
		}

		/// <summary>
		/// Sets the values.
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public void SetValue(T min, T max)
		{
			var oldValueMin = valueMin;
			var oldValueMax = valueMax;

			valueMin = min;
			valueMax = max;

			valueMin = InBoundsMin(valueMin);
			valueMax = InBoundsMax(valueMax);

			var min_equals = EqualityComparer<T>.Default.Equals(oldValueMin, valueMin);
			var max_equals = EqualityComparer<T>.Default.Equals(oldValueMax, valueMax);

			if (!min_equals || !max_equals)
			{
				UpdateMinHandle();
				UpdateMaxHandle();
				UpdateFill();

				OnValuesChanged.Invoke(valueMin, valueMax);
				OnChange.Invoke();
			}
		}

		/// <summary>
		/// Sets the limits.
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public void SetLimit(T min, T max)
		{
			var oldValueMin = valueMin;
			var oldValueMax = valueMax;

			limitMin = min;
			limitMax = max;

			valueMin = InBoundsMin(valueMin);
			valueMax = InBoundsMax(valueMax);

			var min_equals = EqualityComparer<T>.Default.Equals(oldValueMin, valueMin);
			var max_equals = EqualityComparer<T>.Default.Equals(oldValueMax, valueMax);

			if (!min_equals || !max_equals)
			{
				UpdateMinHandle();
				UpdateMaxHandle();
				UpdateFill();

				OnValuesChanged.Invoke(valueMin, valueMax);
				OnChange.Invoke();
			}
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public virtual bool IsHorizontal()
		{
			return true;
		}

		/// <summary>
		/// Returns size of usable rect.
		/// </summary>
		/// <returns>The size.</returns>
		protected float FillSize()
		{
			var result = IsHorizontal() ? UsableRangeRect.rect.width : UsableRangeRect.rect.height;
			if (type == RangeSliderType.DisableHandleOverlay)
			{
				result -= IsHorizontal() ? HandleMinRect.rect.width : HandleMinRect.rect.height;
			}

			return result;
		}

		/// <summary>
		/// Minimum size of the handle.
		/// </summary>
		/// <returns>The handle size.</returns>
		protected float MinHandleSize()
		{
			if (IsHorizontal())
			{
				return HandleMinRect.rect.width;
			}
			else
			{
				return HandleMinRect.rect.height;
			}
		}

		/// <summary>
		/// Maximum size of the handle.
		/// </summary>
		/// <returns>The handle size.</returns>
		protected float MaxHandleSize()
		{
			if (IsHorizontal())
			{
				return HandleMaxRect.rect.width;
			}
			else
			{
				return HandleMaxRect.rect.height;
			}
		}

		/// <summary>
		/// Updates the minimum value.
		/// </summary>
		/// <param name="delta">Delta.</param>
		protected void UpdateMinValue(float delta)
		{
			var pos = GetHandlePosition(HandleMinRect) + delta;
			ValueMin = PositionToValue(pos - GetHandleStartPoint());
		}

		/// <summary>
		/// Updates the maximum value.
		/// </summary>
		/// <param name="delta">Delta.</param>
		protected void UpdateMaxValue(float delta)
		{
			var pos = GetHandlePosition(HandleMaxRect) + delta;
			ValueMax = PositionToValue(pos - GetHandleStartPoint());
		}

		/// <summary>
		/// Value to position.
		/// </summary>
		/// <returns>Position.</returns>
		/// <param name="value">Value.</param>
		protected abstract float ValueToPosition(T value);

		/// <summary>
		/// Position to value.
		/// </summary>
		/// <returns>Value.</returns>
		/// <param name="position">Position.</param>
		protected abstract T PositionToValue(float position);

		/// <summary>
		/// Gets the start point.
		/// </summary>
		/// <returns>The start point.</returns>
		protected float GetStartPoint()
		{
			var result = IsHorizontal() ? -UsableRangeRect.sizeDelta.x / 2f : UsableRangeRect.sizeDelta.y / 2f;
			if (type == RangeSliderType.DisableHandleOverlay)
			{
				result += IsHorizontal() ? HandleMinRect.rect.width : HandleMinRect.rect.height;
			}

			return result;
		}

		/// <summary>
		/// Get handle start point.
		/// </summary>
		/// <returns>Handle start point.</returns>
		protected float GetHandleStartPoint()
		{
			return IsHorizontal() ? -UsableRangeRect.sizeDelta.x / 2f : -UsableRangeRect.sizeDelta.y / 2f;
		}

		/// <summary>
		/// Position range for minimum handle.
		/// </summary>
		/// <returns>The position limits.</returns>
		protected abstract Vector2 MinPositionLimits();

		/// <summary>
		/// Position range for maximum handle.
		/// </summary>
		/// <returns>The position limits.</returns>
		protected abstract Vector2 MaxPositionLimits();

		/// <summary>
		/// Fit value to bounds.
		/// </summary>
		/// <returns>Value in bounds.</returns>
		/// <param name="value">Value.</param>
		protected abstract T InBounds(T value);

		/// <summary>
		/// Fit minimum value to bounds.
		/// </summary>
		/// <returns>Value in bounds.</returns>
		/// <param name="value">Value.</param>
		protected abstract T InBoundsMin(T value);

		/// <summary>
		/// Fit maximum value to bounds.
		/// </summary>
		/// <returns>Value in bounds.</returns>
		/// <param name="value">Value.</param>
		protected abstract T InBoundsMax(T value);

		/// <summary>
		/// Increases the minimum value.
		/// </summary>
		protected abstract void IncreaseMin();

		/// <summary>
		/// Decreases the minimum value.
		/// </summary>
		protected abstract void DecreaseMin();

		/// <summary>
		/// Increases the maximum value.
		/// </summary>
		protected abstract void IncreaseMax();

		/// <summary>
		/// Decreases the maximum value.
		/// </summary>
		protected abstract void DecreaseMax();

		/// <summary>
		/// Updates the minimum handle.
		/// </summary>
		protected void UpdateMinHandle()
		{
			UpdateHandle(HandleMinRect, valueMin);
		}

		/// <summary>
		/// Updates the maximum handle.
		/// </summary>
		protected void UpdateMaxHandle()
		{
			UpdateHandle(HandleMaxRect, valueMax);
		}

		/// <summary>
		/// Updates the fill.
		/// </summary>
		protected void UpdateFill()
		{
			var size_delta = FillRect.sizeDelta;
			if (IsHorizontal())
			{
				FillRect.anchorMin = new Vector2(HandleMinRect.anchorMin.x, 0.5f);
				FillRect.anchorMax = new Vector2(HandleMaxRect.anchorMin.x, 0.5f);
				FillRect.anchoredPosition = Vector2.zero;
				FillRect.sizeDelta = new Vector2(0, size_delta.y);
			}
			else
			{
				FillRect.anchorMin = new Vector2(0.5f, HandleMinRect.anchorMin.y);
				FillRect.anchorMax = new Vector2(0.5f, HandleMaxRect.anchorMin.y);
				FillRect.anchoredPosition = Vector2.zero;
				FillRect.sizeDelta = new Vector2(size_delta.x, 0);
			}
		}

		/// <summary>
		/// Updates the handle.
		/// </summary>
		/// <param name="handle">Handle transform.</param>
		/// <param name="value">Value.</param>
		protected void UpdateHandle(RectTransform handle, T value)
		{
			var pos = handle.anchoredPosition;
			var anchorMin = handle.anchorMin;
			var anchorMax = handle.anchorMax;
			var size = (transform as RectTransform).rect.size;

			if (IsHorizontal())
			{
				if (size[0] == 0f)
				{
					return;
				}

				var rate = ((type == RangeSliderType.DisableHandleOverlay) && (handle == HandleMinRect))
					? 1.5f
					: 0.5f;
				anchorMin.x = (ValueToPosition(value) + (handle.rect.width * (handle.pivot.x - rate))) / size[0];
				anchorMax.x = anchorMin.x;
				pos.x = 0;
			}
			else
			{
				if (size[1] == 0f)
				{
					return;
				}

				var rate = ((type == RangeSliderType.DisableHandleOverlay) && (handle == HandleMinRect))
					? 0.0f
					: -1.0f;
				anchorMin.y = (ValueToPosition(value) + (handle.rect.height * (handle.pivot.y - rate))) / size[1];
				anchorMax.y = anchorMin.y;
				pos.y = 0;
			}

			handle.anchoredPosition = pos;
			handle.anchorMin = anchorMin;
			handle.anchorMax = anchorMax;
		}

		/// <summary>
		/// Selects the minimum handle.
		/// </summary>
		void SelectMinHandle()
		{
			if ((EventSystem.current != null) && (!EventSystem.current.alreadySelecting))
			{
				EventSystem.current.SetSelectedGameObject(handleMin.gameObject);
			}
		}

		/// <summary>
		/// Selects the max handle.
		/// </summary>
		void SelectMaxHandle()
		{
			if ((EventSystem.current != null) && (!EventSystem.current.alreadySelecting))
			{
				EventSystem.current.SetSelectedGameObject(handleMax.gameObject);
			}
		}

		/// <summary>
		/// Convert handle anchor value to position.
		/// </summary>
		/// <returns>Position.</returns>
		/// <param name="handle">Handle.</param>
		protected float GetHandlePosition(RectTransform handle)
		{
			var size = (transform as RectTransform).rect.size;
			if (IsHorizontal())
			{
				return handle.anchorMin.x * size[0];
			}
			else
			{
				return handle.anchorMin.y * size[1];
			}
		}

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerClick(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			Vector2 position;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(UsableRangeRect, eventData.position, eventData.pressEventCamera, out position))
			{
				return;
			}

			position -= UsableRangeRect.rect.position;

			var new_min_position = (IsHorizontal() ? position.x : position.y) + GetHandleStartPoint();
			var new_max_position = new_min_position;

			var min_position = GetHandlePosition(HandleMinRect);
			var max_position = GetHandlePosition(HandleMaxRect);

			var handle_size = MinHandleSize();
			if (type == RangeSliderType.DisableHandleOverlay)
			{
				max_position += IsHorizontal() ? handle_size : -handle_size;
			}

			var delta_min = new_min_position - min_position;
			var delta_max = max_position - new_max_position;
			if ((delta_min - handle_size) < delta_max)
			{
				UpdateMinValue(delta_min);
			}
			else
			{
				UpdateMaxValue(-delta_max);
			}
		}

		/// <summary>
		/// Enable this instance.
		/// </summary>
		protected override void OnEnable()
		{
			UpdateMinHandle();
			UpdateMaxHandle();
			UpdateFill();
		}

		/// <summary>
		/// Handle RectTransform dimensions change event.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			UpdateMinHandle();
			UpdateMaxHandle();
			UpdateFill();
		}

		/// <summary>
		/// Convert value to range [0, 1] based on limits.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>Clamped value.</returns>
		public abstract float Value01(float value);

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		public virtual void Validate()
		{
			valueMin = InBoundsMin(valueMin);
			valueMax = InBoundsMax(valueMax);

			if (handleMin != null && handleMax != null && UsableRangeRect != null && FillRect != null)
			{
				UpdateMinHandle();
				UpdateMaxHandle();
				UpdateFill();
			}
		}

		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			valueMin = InBoundsMin(valueMin);
			valueMax = InBoundsMax(valueMax);
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			var slider_style = IsHorizontal() ? style.RangeSliderHorizontal : style.RangeSliderVertical;

			slider_style.Background.ApplyTo(GetComponent<Image>());

			if (UsableRangeRect != null)
			{
				slider_style.UsableRange.ApplyTo(UsableRangeRect.GetComponent<Image>());
			}

			if (FillRect != null)
			{
				slider_style.Fill.ApplyTo(FillRect.GetComponent<Image>());
			}

			if (handleMin != null)
			{
				slider_style.HandleMin.ApplyTo(handleMin.GetComponent<Image>());

				handleMin.transition = slider_style.HandleMinTransition;
				handleMin.colors = slider_style.HandleMinColors;
				handleMin.spriteState = slider_style.HandleMinSprites;
				handleMin.animationTriggers = slider_style.HandleMinAnimation;
			}

			if (handleMax != null)
			{
				slider_style.HandleMax.ApplyTo(handleMax.GetComponent<Image>());

				handleMax.transition = slider_style.HandleMaxTransition;
				handleMax.colors = slider_style.HandleMaxColors;
				handleMax.spriteState = slider_style.HandleMaxSprites;
				handleMax.animationTriggers = slider_style.HandleMaxAnimation;
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			var slider_style = IsHorizontal() ? style.RangeSliderHorizontal : style.RangeSliderVertical;

			slider_style.Background.GetFrom(GetComponent<Image>());

			if (UsableRangeRect != null)
			{
				slider_style.UsableRange.GetFrom(UsableRangeRect.GetComponent<Image>());
			}

			if (FillRect != null)
			{
				slider_style.Fill.GetFrom(FillRect.GetComponent<Image>());
			}

			if (handleMin != null)
			{
				slider_style.HandleMin.GetFrom(handleMin.GetComponent<Image>());

				slider_style.HandleMinTransition = handleMin.transition;
				slider_style.HandleMinColors = handleMin.colors;
				slider_style.HandleMinSprites = handleMin.spriteState;
				slider_style.HandleMinAnimation = handleMin.animationTriggers;
			}

			if (handleMax != null)
			{
				slider_style.HandleMax.GetFrom(handleMax.GetComponent<Image>());

				slider_style.HandleMaxTransition = handleMax.transition;
				slider_style.HandleMaxColors = handleMax.colors;
				slider_style.HandleMaxSprites = handleMax.spriteState;
				slider_style.HandleMaxAnimation = handleMax.animationTriggers;
			}

			return true;
		}
		#endregion
	}
}