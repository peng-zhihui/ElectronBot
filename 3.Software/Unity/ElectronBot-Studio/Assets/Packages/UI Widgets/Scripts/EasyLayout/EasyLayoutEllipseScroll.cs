namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Scroll for EasyLayout with Ellipse layout type.
	/// </summary>
	[RequireComponent(typeof(EasyLayout))]
	public class EasyLayoutEllipseScroll : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IScrollHandler, UIWidgets.ILateUpdatable
	{
#region Interactable

		/// <summary>
		/// The CanvasGroup cache.
		/// </summary>
		protected readonly List<CanvasGroup> CanvasGroupCache = new List<CanvasGroup>();

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
		/// Determines whether this widget is interactable.
		/// </summary>
		/// <returns><c>true</c> if this instance is interactable; otherwise, <c>false</c>.</returns>
		public virtual bool IsInteractable()
		{
			return GroupsAllowInteraction && Interactable;
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		protected virtual void InteractableChanged()
		{
			if (IsInteractable())
			{
				OnInteractableEnabled();
			}
			else
			{
				OnInteractableDisabled();
			}
		}

		/// <summary>
		/// What to do when widget became interactable.
		/// </summary>
		protected virtual void OnInteractableEnabled()
		{
		}

		/// <summary>
		/// What to do when widget became not interactable.
		/// </summary>
		protected virtual void OnInteractableDisabled()
		{
		}
#endregion

		[NonSerialized]
		EasyLayout layout;

		/// <summary>
		/// Layout.
		/// </summary>
		protected EasyLayout Layout
		{
			get
			{
				if (layout == null)
				{
					layout = GetComponent<EasyLayout>();
				}

				return layout;
			}
		}

		/// <summary>
		/// Is scroll horizontal or vertical?
		/// </summary>
		[SerializeField]
		public bool IsHorizontal = false;

		/// <summary>
		/// Drag sensitivity.
		/// </summary>
		[SerializeField]
		public float DragSensitivity = 0.5f;

		/// <summary>
		/// Scroll sensitivity.
		/// </summary>
		[SerializeField]
		public float ScrollSensitivity = 2f;

		/// <summary>
		/// Layout scroll.
		/// </summary>
		protected float ScrollValue
		{
			get
			{
				return Layout.EllipseSettings.AngleScroll;
			}

			set
			{
				Layout.EllipseSettings.AngleScroll = value;

				OnScrollEvent.Invoke();
			}
		}

		/// <summary>
		/// Intertia.
		/// </summary>
		[SerializeField]
		public bool Inertia = true;

		/// <summary>
		/// Time to stop.
		/// </summary>
		[SerializeField]
		public float TimeToStop = 0.5f;

		/// <summary>
		/// Animate inertia scroll with unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Scroll event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnScrollEvent = new UnityEvent();

		/// <summary>
		/// Scroll stop event.
		/// </summary>
		[SerializeField]
		public UnityEvent OnScrollStopEvent = new UnityEvent();

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

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive()
		{
			return base.IsActive() && IsInteractable() && Layout.LayoutType == LayoutTypes.Ellipse;
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

			PrevScrollValue = ScrollValue;
			CurrentScrollValue = ScrollValue;

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

			var v = IsHorizontal ? -eventData.delta.x : eventData.delta.y;
			Scroll(v * DragSensitivity);
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
			var v = IsHorizontal ? -eventData.scrollDelta.x : eventData.scrollDelta.y;
			Scroll(v * ScrollSensitivity);
		}

		/// <summary>
		/// Scroll.
		/// </summary>
		/// <param name="delta">Delta.</param>
		protected virtual void Scroll(float delta)
		{
			ScrollValue += delta;

			CurrentScrollValue += delta;
			var time_delta = GetDeltaTime();
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
			if (!Inertia)
			{
				OnScrollStopEvent.Invoke();
				return;
			}

			IntertiaVelocity = -ScrollVelocity;
			CurrentDecelerationRate = -IntertiaVelocity / TimeToStop;

			var direction = Mathf.Sign(IntertiaVelocity);
			var time_to_stop_sq = Mathf.Pow(TimeToStop, 2f);
			var distance = ((-Mathf.Abs(CurrentDecelerationRate) * time_to_stop_sq) / 2f) + (Mathf.Abs(IntertiaVelocity) * TimeToStop);

			InertiaDistance = distance;
			IntertiaVelocity = (InertiaDistance - (-Mathf.Abs(CurrentDecelerationRate) * (TimeToStop * TimeToStop) / 2f)) / TimeToStop;
			IntertiaVelocity *= direction;
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			UIWidgets.Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();
			UIWidgets.Updater.RemoveLateUpdate(this);
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
			else if (!IsDragging && (InertiaDistance > 0f))
			{
				var delta = GetDeltaTime();
				var distance = IntertiaVelocity > 0f
					? Mathf.Min(InertiaDistance, IntertiaVelocity * delta)
					: Mathf.Max(-InertiaDistance, IntertiaVelocity * delta);

				ScrollValue += distance;
				InertiaDistance -= Mathf.Abs(distance);

				if (InertiaDistance > 0f)
				{
					IntertiaVelocity += CurrentDecelerationRate * delta;
					ScrollVelocity = -IntertiaVelocity;
				}
				else
				{
					OnScrollStopEvent.Invoke();
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
		/// Get delta time.
		/// </summary>
		/// <returns>Delta time.</returns>
		protected float GetDeltaTime()
		{
			// return UnscaledTime? Time.unscaledDeltaTime: Time.deltaTime;
			return UIWidgets.UtilitiesTime.GetDeltaTime(UnscaledTime);
		}
	}
}