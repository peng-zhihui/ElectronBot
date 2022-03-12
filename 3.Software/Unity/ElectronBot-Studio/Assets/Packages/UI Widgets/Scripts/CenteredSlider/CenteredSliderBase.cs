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
	/// Centered slider base class (zero at center, positive and negative parts have different scales).
	/// </summary>
	/// <typeparam name="T">Type of sliver value.</typeparam>
	[DataBindSupport]
	public abstract class CenteredSliderBase<T> : UIBehaviourConditional, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IStylable, IValidateable
		where T : struct
	{
		/// <summary>
		/// OnChangeEvent
		/// </summary>
		[Serializable]
		public class OnChangeEvent : UnityEvent<T>
		{
		}

		/// <summary>
		/// Value.
		/// </summary>
		[SerializeField]
		protected T sliderValue;

		/// <summary>
		/// Gets or sets the value.
		/// </summary>
		/// <value>Value.</value>
		[DataBindField]
		public T Value
		{
			get
			{
				return sliderValue;
			}

			set
			{
				SetValue(value);
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
				SetValue(sliderValue);
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
				SetValue(sliderValue);
			}
		}

		/// <summary>
		/// The use value limits.
		/// </summary>
		[SerializeField]
		protected bool useValueLimits;

		/// <summary>
		/// Gets or sets use value limits.
		/// </summary>
		/// <value><c>true</c> if use value limits; otherwise, <c>false</c>.</value>
		[DataBindField]
		public bool UseValueLimits
		{
			get
			{
				return useValueLimits;
			}

			set
			{
				useValueLimits = value;
				SetValue(sliderValue);
			}
		}

		/// <summary>
		/// The value minimum limit.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("useValueLimits")]
		protected T valueMin;

		/// <summary>
		/// Gets or sets the value minimum limit.
		/// </summary>
		/// <value>The value minimum limit.</value>
		[DataBindField]
		public T ValueMin
		{
			get
			{
				return valueMin;
			}

			set
			{
				valueMin = value;
				SetValue(sliderValue);
			}
		}

		/// <summary>
		/// The value maximum limit.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("useValueLimits")]
		protected T valueMax;

		/// <summary>
		/// Gets or sets the value maximum limit.
		/// </summary>
		/// <value>The maximum limit.</value>
		[DataBindField]
		public T ValueMax
		{
			get
			{
				return valueMax;
			}

			set
			{
				valueMax = value;
				SetValue(sliderValue);
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
		/// Whole number of steps.
		/// </summary>
		[DataBindField]
		public bool WholeNumberOfSteps = false;

		/// <summary>
		/// The handle.
		/// </summary>
		[SerializeField]
		protected RangeSliderHandle handle;

		/// <summary>
		/// The handle rect.
		/// </summary>
		protected RectTransform handleRect;

		/// <summary>
		/// Gets the handle rect.
		/// </summary>
		/// <value>The handle rect.</value>
		public RectTransform HandleRect
		{
			get
			{
				if (handle != null && handleRect == null)
				{
					handleRect = handle.transform as RectTransform;
				}

				return handleRect;
			}
		}

		/// <summary>
		/// Gets or sets the handle.
		/// </summary>
		/// <value>The handle.</value>
		public RangeSliderHandle Handle
		{
			get
			{
				return handle;
			}

			set
			{
				SetHandle(value);
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
		/// OnValuesChange event.
		/// </summary>
		[DataBindEvent("Value")]
		[FormerlySerializedAs("OnValuesChange")]
		public OnChangeEvent OnValueChanged = new OnChangeEvent();

		/// <summary>
		/// OnValuesChange event.
		/// </summary>
		[Obsolete("Renamed to OnValueChange.")]
		public OnChangeEvent OnValuesChange
		{
			get
			{
				return OnValuesChange;
			}
		}

		/// <summary>
		/// OnChange event.
		/// </summary>
		public UnityEvent OnChange = new UnityEvent();

		/// <summary>
		/// Is init called?
		/// </summary>
		bool isInitCalled;

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			if (isInitCalled)
			{
				return;
			}

			isInitCalled = true;

			SetHandle(handle);
			UpdateHandle();
			UpdateFill();
		}

		/// <summary>
		/// Implementation of a callback that is sent if an associated RectTransform has it's dimensions changed.
		/// </summary>
		protected override void OnRectTransformDimensionsChange()
		{
			UpdateHandle();
			UpdateFill();
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerDown event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerUp event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerUp(PointerEventData eventData)
		{
		}

		/// <summary>
		/// Sets the value.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetValue(T value)
		{
			var v = InBounds(value);
			if (!EqualityComparer<T>.Default.Equals(sliderValue, v))
			{
				sliderValue = v;

				UpdateHandle();
				OnValueChanged.Invoke(Value);
				OnChange.Invoke();
			}
		}

		/// <summary>
		/// Sets the handle.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetHandle(RangeSliderHandle value)
		{
			handle = value;
			handle.RectTransform = UsableRangeRect;
			handle.IsHorizontal = IsHorizontal;
			handle.PositionLimits = PositionLimits;
			handle.PositionChanged = UpdateValue;
			handle.Increase = Increase;
			handle.Decrease = Decrease;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			Init();
		}

		/// <summary>
		/// Sets the limits.
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public void SetLimit(T min, T max)
		{
			// set limits to skip InBounds check
			limitMin = min;
			limitMax = max;

			// set limits with InBounds check and update handle's positions
			LimitMin = limitMin;
			LimitMax = limitMax;
		}

		/// <summary>
		/// Sets the value limits.
		/// </summary>
		/// <param name="min">Minimum.</param>
		/// <param name="max">Max.</param>
		public void SetValueLimit(T min, T max)
		{
			// set limits to skip InBounds check
			valueMin = min;
			valueMax = max;

			// set limits with InBounds check and update handle's positions
			ValueMin = valueMin;
			ValueMax = valueMax;
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
		protected float RangeSize()
		{
			return IsHorizontal() ? UsableRangeRect.rect.width : UsableRangeRect.rect.height;
		}

		/// <summary>
		/// Size of the handle.
		/// </summary>
		/// <returns>The handle size.</returns>
		protected float HandleSize()
		{
			return IsHorizontal() ? HandleRect.rect.width : HandleRect.rect.height;
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
		/// Updates the minimum value.
		/// </summary>
		/// <param name="position">Position.</param>
		protected void UpdateValue(float position)
		{
			var pos = GetHandlePosition(HandleRect) + position;

			var size = UsableRangeRect.rect.size;
			pos += IsHorizontal() ? size.x / 2f : -size.y / 2f;

			Value = PositionToValue(pos);
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
			var delta = UsableRangeRect.sizeDelta;
			return IsHorizontal() ? -delta.x / 2f : -delta.y / 2f;
		}

		/// <summary>
		/// Position range for minimum handle.
		/// </summary>
		/// <returns>The position limits.</returns>
		protected abstract Vector2 PositionLimits();

		/// <summary>
		/// Fit value to bounds.
		/// </summary>
		/// <returns>Value.</returns>
		/// <param name="value">Value to fit.</param>
		protected abstract T InBounds(T value);

		/// <summary>
		/// Increases the minimum value.
		/// </summary>
		protected abstract void Increase();

		/// <summary>
		/// Decreases the minimum value.
		/// </summary>
		protected abstract void Decrease();

		/// <summary>
		/// Updates the handle.
		/// </summary>
		protected void UpdateHandle()
		{
			var new_position = HandleRect.anchoredPosition;
			if (IsHorizontal())
			{
				new_position.x = ValueToPosition(Value) + (HandleRect.rect.width * (HandleRect.pivot.x - 0.5f));
			}
			else
			{
				new_position.y = ValueToPosition(Value) + (HandleRect.rect.height * (HandleRect.pivot.y - 0.5f));
			}

			HandleRect.anchoredPosition = new_position;

			UpdateFill();
		}

		/// <summary>
		/// Determines whether this instance is positive value.
		/// </summary>
		/// <returns><c>true</c> if this instance is positive value; otherwise, <c>false</c>.</returns>
		protected abstract bool IsPositiveValue();

		/// <summary>
		/// Updates the fill size.
		/// </summary>
		protected virtual void UpdateFill()
		{
			FillRect.anchorMin = new Vector2(0.5f, 0.5f);
			FillRect.anchorMax = new Vector2(0.5f, 0.5f);
			if (IsHorizontal())
			{
				if (IsPositiveValue())
				{
					FillRect.pivot = new Vector2(0.0f, 0.5f);
					FillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, HandleRect.localPosition.x - UsableRangeRect.localPosition.x);
				}
				else
				{
					FillRect.pivot = new Vector2(1.0f, 0.5f);
					FillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, UsableRangeRect.localPosition.x - HandleRect.localPosition.x);
				}
			}
			else
			{
				if (IsPositiveValue())
				{
					FillRect.pivot = new Vector2(0.5f, 0.0f);
					FillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, HandleRect.localPosition.y - UsableRangeRect.localPosition.y);
				}
				else
				{
					FillRect.pivot = new Vector2(0.5f, 1.0f);
					FillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, UsableRangeRect.localPosition.y - HandleRect.localPosition.y);
				}
			}
		}

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			Vector2 position;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(UsableRangeRect, eventData.position, eventData.pressEventCamera, out position);

			UpdateValue(IsHorizontal() ? position.x : position.y);
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		public virtual void Validate()
		{
			if (handle != null && UsableRangeRect != null && FillRect != null)
			{
				UpdateHandle();
			}
		}

		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			sliderValue = InBounds(sliderValue);
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			var slider_style = IsHorizontal() ? style.CenteredSliderHorizontal : style.CenteredSliderVertical;

			slider_style.Background.ApplyTo(GetComponent<Image>());

			if (UsableRangeRect != null)
			{
				slider_style.UsableRange.ApplyTo(UsableRangeRect.GetComponent<Image>());
			}

			if (FillRect != null)
			{
				slider_style.Fill.ApplyTo(FillRect.GetComponent<Image>());
			}

			if (handle != null)
			{
				slider_style.Handle.ApplyTo(handle.GetComponent<Image>());
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			var slider_style = IsHorizontal() ? style.CenteredSliderHorizontal : style.CenteredSliderVertical;

			slider_style.Background.GetFrom(GetComponent<Image>());

			if (UsableRangeRect != null)
			{
				slider_style.UsableRange.GetFrom(UsableRangeRect.GetComponent<Image>());
			}

			if (FillRect != null)
			{
				slider_style.Fill.GetFrom(FillRect.GetComponent<Image>());
			}

			if (handle != null)
			{
				slider_style.Handle.GetFrom(handle.GetComponent<Image>());
			}

			return true;
		}
		#endregion
	}
}