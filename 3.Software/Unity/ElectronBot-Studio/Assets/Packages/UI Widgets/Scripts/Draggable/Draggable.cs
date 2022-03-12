namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Draggable restriction.
	/// </summary>
	public enum DraggableRestriction
	{
		/// <summary>
		/// Without restriction.
		/// </summary>
		None = 0,

		/// <summary>
		/// Does not allow drag outside parent.
		/// </summary>
		Strict = 1,

		/// <summary>
		/// Apply restriction after drag.
		/// </summary>
		AfterDrag = 2,
	}

	/// <summary>
	/// Draggable UI object.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Draggable")]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class Draggable : UIBehaviourConditional, ISnapGridSupport
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
		/// Is instance interactable?
		/// </summary>
		/// <returns>true if instance interactable; otherwise false</returns>
		public bool IsInteractable()
		{
			return GroupsAllowInteraction && Interactable;
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
			if (!interactableState)
			{
				EndDrag(null);
			}
		}
		#endregion

		/// <summary>
		/// The handle.
		/// </summary>
		[SerializeField]
		GameObject handle;

		DraggableHandle handleDrag;

		/// <summary>
		/// Allow horizontal movement.
		/// </summary>
		[SerializeField]
		public bool Horizontal = true;

		/// <summary>
		/// Allow vertical movement.
		/// </summary>
		[SerializeField]
		public bool Vertical = true;

		/// <summary>
		/// Drag restriction.
		/// </summary>
		public DraggableRestriction Restriction = DraggableRestriction.None;

		/// <summary>
		/// Animation curve.
		/// </summary>
		[SerializeField]
		public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 0.2f, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		public bool UnscaledTime;

		/// <summary>
		/// Handle to drag.
		/// </summary>
		public GameObject Handle
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

		[SerializeField]
		[Header("Snap Settings")]
		List<SnapGridBase> snapGrids;

		/// <summary>
		/// SnapGrids.
		/// </summary>
		public List<SnapGridBase> SnapGrids
		{
			get
			{
				return snapGrids;
			}

			set
			{
				snapGrids = value;
			}
		}

		[SerializeField]
		Vector2 snapDistance = new Vector2(10f, 10f);

		/// <summary>
		/// Snap distance.
		/// </summary>
		public Vector2 SnapDistance
		{
			get
			{
				return snapDistance;
			}

			set
			{
				snapDistance = value;
			}
		}

		/// <summary>
		/// Snap mode.
		/// </summary>
		[SerializeField]
		public SnapDragMode SnapMode = SnapDragMode.OnDrag;

		/// <summary>
		/// Restore position on end drag if the snap is not applicable.
		/// </summary>
		[SerializeField]
		[Tooltip("Restore position on end drag if the snap is not applicable.")]
		public bool RestoreIfNotSnap = false;

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>RectTransform.</value>
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

		/// <summary>
		/// Is target is self?
		/// </summary>
		protected bool IsTargetSelf;

		RectTransform target;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>RectTransform.</value>
		public RectTransform Target
		{
			get
			{
				if (target == null)
				{
					target = transform as RectTransform;
				}

				return target;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				IsTargetSelf = value.GetInstanceID() == transform.GetInstanceID();
				target = value;

				if (!IsTargetSelf)
				{
					var le = Utilities.GetOrAddComponent<LayoutElement>(this);
					le.ignoreLayout = true;

					RectTransform.SetParent(target.parent, false);

					CopyRectTransformValues();
				}
			}
		}

		/// <summary>
		/// Start resize event.
		/// </summary>
		[SerializeField]
		public DraggableEvent OnStartDrag = new DraggableEvent();

		/// <summary>
		/// During resize event.
		/// </summary>
		public DraggableEvent OnDrag = new DraggableEvent();

		/// <summary>
		/// End resize event.
		/// </summary>
		[SerializeField]
		public DraggableEvent OnEndDrag = new DraggableEvent();

		/// <summary>
		/// Snap event.
		/// </summary>
		[SerializeField]
		public DraggableSnapEvent OnSnap = new DraggableSnapEvent();

		/// <summary>
		/// Snap event on end drag.
		/// </summary>
		[SerializeField]
		public DraggableSnapEvent OnEndSnap = new DraggableSnapEvent();

		/// <summary>
		/// Is instance dragged?
		/// </summary>
		protected bool IsDrag;

		/// <summary>
		/// The animation.
		/// </summary>
		protected IEnumerator Animation;

		/// <summary>
		/// Start position.
		/// </summary>
		protected Vector3 StartPosition;

		/// <summary>
		/// Starts this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Init();
		}

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			Target = gameObject.transform as RectTransform;
			SetHandle(handle != null ? handle : gameObject);
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			RemoveListeners();
		}

		/// <summary>
		/// Sets the handle.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetHandle(GameObject value)
		{
			if (handle != null)
			{
				RemoveListeners();
				Destroy(handleDrag);
			}

			handle = value;
			handleDrag = Utilities.GetOrAddComponent<DraggableHandle>(handle);
			AddListeners();
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected void AddListeners()
		{
			if (handleDrag != null)
			{
				handleDrag.OnBeginDragEvent.AddListener(BeginDrag);
				handleDrag.OnDragEvent.AddListener(Drag);
				handleDrag.OnEndDragEvent.AddListener(EndDrag);
			}
		}

		/// <summary>
		/// Remove listeners,
		/// </summary>
		protected void RemoveListeners()
		{
			if (handleDrag != null)
			{
				handleDrag.OnBeginDragEvent.RemoveListener(BeginDrag);
				handleDrag.OnDragEvent.RemoveListener(Drag);
				handleDrag.OnEndDragEvent.RemoveListener(EndDrag);
			}
		}

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void BeginDrag(PointerEventData eventData)
		{
			if (Animation != null)
			{
				StopCoroutine(Animation);
			}

			if (!IsActive())
			{
				return;
			}

			IsDrag = true;
			InitDrag();

			OnStartDrag.Invoke(this);
		}

		/// <summary>
		/// Init drag.
		/// </summary>
		public virtual void InitDrag()
		{
			StartPosition = Target.localPosition;
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void Drag(PointerEventData eventData)
		{
			if (!IsDrag)
			{
				return;
			}

			if (eventData.used)
			{
				return;
			}

			eventData.Use();

			Drag(eventData, false);

			OnDrag.Invoke(this);
		}

		/// <summary>
		/// Perform drag.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="isEnd">Is end drag?</param>
		public virtual void Drag(PointerEventData eventData, bool isEnd)
		{
			Vector2 current_point;
			Vector2 original_point;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.position, eventData.pressEventCamera, out current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(Target, eventData.pressPosition, eventData.pressEventCamera, out original_point);

			var base_delta = current_point - original_point;
			Drag(base_delta, isEnd);
		}

		/// <summary>
		/// Perform drag.
		/// </summary>
		/// <param name="delta">Delta.</param>
		/// <param name="isEnd">Is end drag?</param>
		public virtual void Drag(Vector2 delta, bool isEnd = false)
		{
			delta.y *= -1f;

			var angle_rad = Target.localRotation.eulerAngles.z * Mathf.Deg2Rad;
			var drag_delta = new Vector2(
				(delta.x * Mathf.Cos(angle_rad)) + (delta.y * Mathf.Sin(angle_rad)),
				(delta.x * Mathf.Sin(angle_rad)) - (delta.y * Mathf.Cos(angle_rad)));

			if (!Horizontal)
			{
				drag_delta.x = 0;
			}

			if (!Vertical)
			{
				drag_delta.y = 0;
			}

			var new_position = new Vector3(
				StartPosition.x + drag_delta.x,
				StartPosition.y + drag_delta.y,
				StartPosition.z);

			if (Restriction == DraggableRestriction.Strict)
			{
				new_position = RestrictPosition(new_position);
			}

			Target.localPosition = new_position;

			var apply_snap = (SnapDragMode.OnDrag == SnapMode)
				|| (isEnd && (SnapDragMode.OnEndDrag == SnapMode));
			if (apply_snap && (SnapGrids != null))
			{
				var snap = SnapGridBase.Snap(SnapGrids, SnapDistance, Target);
				var restore_position = isEnd && RestoreIfNotSnap && !snap.Snapped;
				if (restore_position)
				{
					Target.localPosition = StartPosition;
				}
				else
				{
					Target.localPosition += new Vector3(snap.Delta.x, snap.Delta.y, 0);
				}
				
				if (isEnd)
				{
					OnEndSnap.Invoke(this, snap);
				}
				else
				{
					OnSnap.Invoke(this, snap);
				}
			}

			CopyRectTransformValues();
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void EndDrag(PointerEventData eventData)
		{
			if (!IsDrag)
			{
				return;
			}

			if (Restriction == DraggableRestriction.AfterDrag)
			{
				Animation = AnimationCoroutine();
				StartCoroutine(Animation);
			}
			else
			{
				Drag(eventData, true);
			}

			IsDrag = false;

			OnEndDrag.Invoke(this);
		}

		/// <summary>
		/// Animation coroutine.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected IEnumerator AnimationCoroutine()
		{
			var start_pos = Target.localPosition;
			var end_pos = RestrictPosition(Target.localPosition);
			if (start_pos != end_pos)
			{
				var animation_length = Curve[Curve.length - 1].time;
				var startTime = UtilitiesTime.GetTime(UnscaledTime);
				var animation_position = 0f;

				do
				{
					animation_position = UtilitiesTime.GetTime(UnscaledTime) - startTime;
					var value = Curve.Evaluate(animation_position);
					Target.localPosition = Vector3.Lerp(start_pos, end_pos, value);
					CopyRectTransformValues();

					yield return null;
				}
				while (animation_position < animation_length);

				Target.localPosition = end_pos;
				CopyRectTransformValues();
			}
		}

		/// <summary>
		/// Restrict the position.
		/// </summary>
		/// <returns>The position.</returns>
		/// <param name="pos">Position.</param>
		protected virtual Vector3 RestrictPosition(Vector3 pos)
		{
			var parent = Target.parent as RectTransform;
			var parent_pivot = parent.pivot;
			var parent_size = parent.rect.size;
			var target_size = Target.rect.size;
			var target_pivot = Target.pivot;

			var min_x = -(parent_size.x * parent_pivot.x) + (target_size.x * target_pivot.x);
			var max_x = (parent_size.x * (1f - parent_pivot.x)) - (target_size.x * (1f - target_pivot.x));

			var min_y = -(parent_size.y * parent_pivot.y) + (target_size.y * target_pivot.y);
			var max_y = (parent_size.y * (1f - parent_pivot.y)) - (target_size.y * (1f - target_pivot.y));

			return new Vector3(
				Mathf.Clamp(pos.x, min_x, max_x),
				Mathf.Clamp(pos.y, min_y, max_y),
				pos.z);
		}

		/// <summary>
		/// Copy RectTransform values.
		/// </summary>
		protected void CopyRectTransformValues()
		{
			if (!IsTargetSelf)
			{
				UtilitiesRectTransform.CopyValues(Target, RectTransform);
			}
		}
	}
}