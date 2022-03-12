namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the ScrollBlock.
	/// </summary>
	[RequireComponent(typeof(EasyLayoutNS.EasyLayout))]
	public abstract class ScrollBlockBase : UIBehaviourConditional, IStylable, ILateUpdatable,
		IBeginDragHandler, IDragHandler, IEndDragHandler, IInitializePotentialDragHandler, IScrollHandler
	{
		/// <summary>
		/// Do nothing.
		/// </summary>
		public static void DoNothing()
		{
		}

		/// <summary>
		/// Default function to get value.
		/// </summary>
		/// <param name="step">Step.</param>
		/// <returns>Value.</returns>
		public static string DefaultValue(int step)
		{
			return string.Format("Index: {0}", step.ToString());
		}

		/// <summary>
		/// Default function to check is interactable.
		/// </summary>
		/// <returns>true.</returns>
		protected static bool DefaultInteractable()
		{
			return true;
		}

		/// <summary>
		/// Default function to check is action allowed.
		/// </summary>
		/// <returns>true.</returns>
		protected static bool DefaultAllow()
		{
			return true;
		}

		/// <summary>
		/// Action to increase the value.
		/// </summary>
		public Action Increase = DoNothing;

		/// <summary>
		/// Action to decrease the value.
		/// </summary>
		public Action Decrease = DoNothing;

		/// <summary>
		/// Function to check is increase allowed.
		/// </summary>
		public Func<bool> AllowIncrease = DefaultAllow;

		/// <summary>
		/// Function to check is decrease allowed.
		/// </summary>
		public Func<bool> AllowDecrease = DefaultAllow;

		/// <summary>
		/// Convert index to the displayed string.
		/// </summary>
		public Func<int, string> Value = DefaultValue;

		/// <summary>
		/// Is interactable?
		/// </summary>
		public Func<bool> IsInteractable = DefaultInteractable;

		/// <summary>
		/// Size of the DefaultItem.
		/// </summary>
		public Vector2 DefaultItemSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Layout.
		/// </summary>
		[NonSerialized]
		protected EasyLayoutBridge Layout;

		/// <summary>
		/// Count of the visible items.
		/// </summary>
		public abstract int Count
		{
			get;
		}

		/// <summary>
		/// Is horizontal scroll?
		/// </summary>
		[SerializeField]
		protected bool IsHorizontal;

		/// <summary>
		/// Scroll sensitivity.
		/// </summary>
		[SerializeField]
		public float ScrollSensitivity = 15f;

		/// <summary>
		/// Layout internal padding.
		/// </summary>
		public float Padding
		{
			get
			{
				return Layout.GetFiller().x;
			}

			set
			{
				var padding = ClampPadding(value);
				Layout.SetFiller(padding, 0f);
			}
		}

		/// <summary>
		/// Animate inertia scroll with unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Auto-scroll to center after scroll/drag.
		/// </summary>
		[SerializeField]
		public bool AlwaysCenter = true;

		/// <summary>
		/// Time to stop.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("AlwaysCenter")]
		public float TimeToStop = 0.5f;

		/// <summary>
		/// Velocity.
		/// </summary>
		[NonSerialized]
		protected float ScrollVelocity;

		/// <summary>
		/// Inertia velocity.
		/// </summary>
		[NonSerialized]
		protected float IntertiaVelocity;

		/// <summary>
		/// Current deceleration rate.
		/// </summary>
		[NonSerialized]
		protected float CurrentDecelerationRate;

		/// <summary>
		/// Inertia distance.
		/// </summary>
		[NonSerialized]
		protected float InertiaDistance;

		/// <summary>
		/// Is drag event occurring?
		/// </summary>
		[NonSerialized]
		protected bool IsDragging;

		/// <summary>
		/// Is scrolling occurring?
		/// </summary>
		[NonSerialized]
		protected bool IsScrolling;

		/// <summary>
		/// Previous scroll value.
		/// </summary>
		[NonSerialized]
		protected float PrevScrollValue;

		/// <summary>
		/// Current scroll value.
		/// </summary>
		[NonSerialized]
		protected float CurrentScrollValue;

		RectTransform rectTransform;

		/// <summary>
		/// Current RectTransformn.
		/// </summary>
		protected RectTransform RectTransform
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

		/// <summary>
		/// Median index of the components.
		/// </summary>
		protected abstract int ComponentsMedian
		{
			get;
		}

		/// <summary>
		/// Distance to center.
		/// </summary>
		public float DistanceToCenter
		{
			get
			{
				return GetCenter() - Padding;
			}
		}

		/// <summary>
		/// Is inited?
		/// </summary>
		protected bool IsInited
		{
			get;
			private set;
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			if (IsInited)
			{
				return;
			}

			IsInited = true;

			UpdateLayout();

			var resizer = Utilities.GetOrAddComponent<ResizeListener>(this);
			resizer.OnResizeNextFrame.AddListener(Resize);

			Resize();

			AlignComponents();
		}

		/// <summary>
		/// Clamp padding value.
		/// </summary>
		/// <param name="padding">Padding.</param>
		/// <returns>Clamped value.</returns>
		protected float ClampPadding(float padding)
		{
			var current = Layout.GetFiller().x;
			var delta = current - padding;
			if ((delta < 0) && (!AllowDecrease()))
			{
				return current;
			}

			if ((delta > 0) && (!AllowIncrease()))
			{
				return current;
			}

			var size = ItemFullSize();
			var center = GetCenter();
			if (Mathf.Round(padding) >= (-size / 2f))
			{
				var d = Mathf.Round(padding) - center;
				var n = Mathf.RoundToInt(d / size);
				if (n > 0)
				{
					for (int i = 0; i < n; i++)
					{
						if (!AllowDecrease())
						{
							break;
						}

						padding -= size;
						Decrease();
					}

					UpdateView();
				}
			}
			else if (Mathf.Round(padding) < (-size / 2f))
			{
				var d = center - Mathf.Round(padding);
				var n = Mathf.RoundToInt(d / size);
				if (n > 0)
				{
					for (int i = 0; i < n; i++)
					{
						if (!AllowIncrease())
						{
							break;
						}

						padding += size;
						Increase();
					}

					UpdateView();
				}
			}

			return padding;
		}

		/// <summary>
		/// Update the layout.
		/// </summary>
		protected abstract void UpdateLayout();

		/// <summary>
		/// Container size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ContainerSize()
		{
			return (IsHorizontal ? RectTransform.rect.width : RectTransform.rect.height) - Layout.GetFullMargin();
		}

		/// <summary>
		/// Content size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ContentSize()
		{
			return (ItemFullSize() * Count) - Layout.GetSpacing();
		}

		/// <summary>
		/// Item size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ItemSize()
		{
			return IsHorizontal ? DefaultItemSize.x : DefaultItemSize.y;
		}

		/// <summary>
		/// Item size with spacing.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ItemFullSize()
		{
			return ItemSize() + Layout.GetSpacing();
		}

		/// <summary>
		/// Calculate the maximum count of the visible components.
		/// </summary>
		/// <returns>Maximum count of the visible components.</returns>
		protected int CalculateMax()
		{
			var result = Mathf.CeilToInt((ContainerSize() + Layout.GetSpacing()) / ItemFullSize()) + 1;
			if (result < 0)
			{
				result = 0;
			}

			if ((result % 2) == 0)
			{
				result += 1;
			}

			return result;
		}

		/// <summary>
		/// Is component is null?
		/// </summary>
		/// <param name="component">Component.</param>
		/// <returns>true if component is null; otherwise, false.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Reviewed.")]
		protected virtual bool IsNullComponent(ScrollBlockItem component)
		{
			return component == null;
		}

		/// <summary>
		/// Process RectTransform resize.
		/// </summary>
		protected abstract void Resize();

		/// <summary>
		/// Update view.
		/// </summary>
		public abstract void UpdateView();

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive()
		{
			return base.IsActive() && IsInited && IsInteractable();
		}

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (!IsActive())
			{
				return;
			}

			IsDragging = true;

			PrevScrollValue = Padding;
			CurrentScrollValue = Padding;

			StopInertia();
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsDragging)
			{
				return;
			}

			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			if (!IsActive())
			{
				return;
			}

			StopInertia();
			var scroll_delta = IsHorizontal ? eventData.delta.x : -eventData.delta.y;
			Scroll(scroll_delta);
		}

		/// <summary>
		/// Process scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnScroll(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			IsScrolling = true;
			var scroll_delta = IsHorizontal ? eventData.scrollDelta.x : -eventData.scrollDelta.y;
			Scroll(scroll_delta * ScrollSensitivity);
		}

		/// <summary>
		/// Scroll.
		/// </summary>
		/// <param name="delta">Delta.</param>
		protected virtual void Scroll(float delta)
		{
			Padding += delta;

			CurrentScrollValue += delta;
			var time_delta = UtilitiesTime.DefaultGetDeltaTime(UnscaledTime);
			var new_velocity = (PrevScrollValue - CurrentScrollValue) / time_delta;
			ScrollVelocity = Mathf.Lerp(ScrollVelocity, new_velocity, time_delta * 10);
			PrevScrollValue = CurrentScrollValue;
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!IsDragging)
			{
				return;
			}

			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return;
			}

			IsDragging = false;
			InitIntertia();
		}

		/// <summary>
		/// Init inertia.
		/// </summary>
		protected virtual void InitIntertia()
		{
			IntertiaVelocity = -ScrollVelocity;
			CurrentDecelerationRate = -IntertiaVelocity / TimeToStop;

			var direction = Mathf.Sign(IntertiaVelocity);
			var time_to_stop_sq = Mathf.Pow(TimeToStop, 2f);
			var distance = ((-Mathf.Abs(CurrentDecelerationRate) * time_to_stop_sq) / 2f) + (Mathf.Abs(IntertiaVelocity) * TimeToStop);
			InertiaDistance = ClampDistance(distance, direction);
			IntertiaVelocity = (InertiaDistance - (-Mathf.Abs(CurrentDecelerationRate) * (TimeToStop * TimeToStop) / 2f)) / TimeToStop;
			IntertiaVelocity *= direction;
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Late update.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (IsScrolling)
			{
				IsScrolling = false;
				InitIntertia();
			}
			else if (!IsDragging && (InertiaDistance > 0f) && AlwaysCenter)
			{
				var delta = UtilitiesTime.DefaultGetDeltaTime(UnscaledTime);
				var distance = IntertiaVelocity > 0f
					? Mathf.Min(InertiaDistance, IntertiaVelocity * delta)
					: Mathf.Max(-InertiaDistance, IntertiaVelocity * delta);

				Padding += distance;
				InertiaDistance -= Mathf.Abs(distance);

				if (InertiaDistance > 0f)
				{
					IntertiaVelocity += CurrentDecelerationRate * delta;
					ScrollVelocity = -IntertiaVelocity;
				}
				else
				{
					StopInertia();
				}
			}
		}

		/// <summary>
		/// Stop inertia.
		/// </summary>
		protected void StopInertia()
		{
			CurrentDecelerationRate = 0f;
			InertiaDistance = 0f;
		}

		/// <summary>
		/// Clamp distance to stop right at value.
		/// </summary>
		/// <param name="distance">Distance.</param>
		/// <param name="direction">Scroll direction.</param>
		/// <returns>Clamped distance.</returns>
		protected float ClampDistance(float distance, float direction)
		{
			var extra = (GetCenter() - Padding) * direction;
			var steps = Mathf.Round((Mathf.Abs(distance) - extra) / ItemFullSize());
			var new_distance = (steps * ItemFullSize()) + extra;
			return new_distance;
		}

		/// <summary>
		/// Get center.
		/// </summary>
		/// <returns>Center.</returns>
		protected float GetCenter()
		{
			return -(ContentSize() - ContainerSize()) / 2f;
		}

		/// <summary>
		/// Align components.
		/// </summary>
		protected void AlignComponents()
		{
			Padding = GetCenter();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			var resizer = GetComponent<ResizeListener>();
			if (resizer != null)
			{
				resizer.OnResizeNextFrame.RemoveListener(Resize);
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag has been found but before it is valid to begin the drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnInitializePotentialDrag(PointerEventData eventData)
		{
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public abstract bool SetStyle(Style style);

		/// <inheritdoc/>
		public abstract bool GetStyle(Style style);
		#endregion
	}
}