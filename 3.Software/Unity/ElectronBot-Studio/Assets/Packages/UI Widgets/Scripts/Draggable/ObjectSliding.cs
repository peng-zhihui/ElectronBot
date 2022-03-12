namespace UIWidgets
{
	using System.Collections;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Object sliding direction.
	/// </summary>
	public enum ObjectSlidingDirection
	{
		/// <summary>
		/// Horizontal direction.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical direction.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Allow to drag objects between specified positions.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Object Sliding")]
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	public class ObjectSliding : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
	{
		#region Interactable
		[SerializeField]
		bool interactable = true;

		/// <summary>
		/// Is the Sidebar eligible for interaction?
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
				SetEndPosition(false);
			}
		}
		#endregion

		/// <summary>
		/// Allowed positions.
		/// </summary>
		public List<float> Positions = new List<float>();

		/// <summary>
		/// Slide direction.
		/// </summary>
		public ObjectSlidingDirection Direction = ObjectSlidingDirection.Horizontal;

		/// <summary>
		/// Movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		public AnimationCurve Movement = AnimationCurve.EaseInOut(0, 0, 1, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
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

		ScrollRect parentScrollRect;

		/// <summary>
		/// Parent ScrollRect.
		/// </summary>
		protected ScrollRect ParentScrollRect
		{
			get
			{
				if (parentScrollRect == null)
				{
					parentScrollRect = GetComponentInParent<ScrollRect>();
				}

				return parentScrollRect;
			}
		}

		/// <summary>
		/// The current animation.
		/// </summary>
		protected IEnumerator CurrentAnimation;

		/// <summary>
		/// Is animation running?
		/// </summary>
		protected bool IsAnimationRunning;

		/// <summary>
		/// Is drag allowed?
		/// </summary>
		protected bool AllowDrag;

		/// <summary>
		/// Start position.
		/// </summary>
		protected Vector3 StartPosition;

		/// <summary>
		/// Is direction is horizontal?
		/// </summary>
		/// <returns>true if direction is horizontal; otherwise, false.</returns>
		protected bool IsHorizontal()
		{
			return Direction == ObjectSlidingDirection.Horizontal;
		}

		/// <summary>
		/// Get end position.
		/// </summary>
		/// <returns>End position.</returns>
		protected float GetEndPosition()
		{
			var cur_position = IsHorizontal() ? RectTransform.anchoredPosition.x : RectTransform.anchoredPosition.y;

			var nearest_position = Positions[0];

			for (int i = 1; i < Positions.Count; i++)
			{
				if (Mathf.Abs(Positions[i] - cur_position) < Mathf.Abs(nearest_position - cur_position))
				{
					nearest_position = Positions[i];
				}
			}

			return nearest_position;
		}

		/// <summary>
		/// Handle begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			if (IsAnimationRunning)
			{
				IsAnimationRunning = false;
				if (CurrentAnimation != null)
				{
					StopCoroutine(CurrentAnimation);
				}
			}

			Vector2 currrent_position;
			Vector2 original_position;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, null, out currrent_position);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out original_position);
			StartPosition = RectTransform.localPosition;

			var delta = currrent_position - original_position;
			AllowDrag = (IsHorizontal() && (Mathf.Abs(delta.x) > Mathf.Abs(delta.y)))
				|| (!IsHorizontal() && (Mathf.Abs(delta.y) > Mathf.Abs(delta.x)));

			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnBeginDrag(eventData);
				}
			}
		}

		/// <summary>
		/// Handle drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnDrag(eventData);
				}

				return;
			}

			if (eventData.used)
			{
				return;
			}

			eventData.Use();

			Vector2 current_position;
			Vector2 original_position;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out current_position);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out original_position);

			var position = new Vector3(
				IsHorizontal() ? StartPosition.x + (current_position.x - original_position.x) : StartPosition.x,
				!IsHorizontal() ? StartPosition.y + (current_position.y - original_position.y) : StartPosition.y,
				StartPosition.z);

			RectTransform.localPosition = position;
		}

		/// <summary>
		/// Handle end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			if (!AllowDrag)
			{
				if (ParentScrollRect != null)
				{
					ParentScrollRect.OnEndDrag(eventData);
				}

				return;
			}

			SetEndPosition(true);
		}

		/// <summary>
		/// Set end position.
		/// </summary>
		/// <param name="animate">Animate.</param>
		protected virtual void SetEndPosition(bool animate)
		{
			if (Positions.Count == 0)
			{
				return;
			}

			var end_position = GetEndPosition();
			if (animate)
			{
				IsAnimationRunning = true;
				var start_position = IsHorizontal() ? RectTransform.anchoredPosition.x : RectTransform.anchoredPosition.y;
				CurrentAnimation = RunAnimation(IsHorizontal(), start_position, end_position, UnscaledTime);
				StartCoroutine(CurrentAnimation);
			}
			else
			{
				RectTransform.anchoredPosition = IsHorizontal()
					? new Vector2(end_position, RectTransform.anchoredPosition.y)
					: new Vector2(RectTransform.anchoredPosition.x, end_position);
			}
		}

		/// <summary>
		/// Runs the animation.
		/// </summary>
		/// <returns>The animation.</returns>
		/// <param name="isHorizontal">If set to <c>true</c> is horizontal.</param>
		/// <param name="startPosition">Start position.</param>
		/// <param name="endPosition">End position.</param>
		/// <param name="unscaledTime">If set to <c>true</c> use unscaled time.</param>
		protected virtual IEnumerator RunAnimation(bool isHorizontal, float startPosition, float endPosition, bool unscaledTime)
		{
			float delta;

			var animation_length = Movement[Movement.length - 1].time;
			var start_time = UtilitiesTime.GetTime(unscaledTime);
			do
			{
				delta = UtilitiesTime.GetTime(unscaledTime) - start_time;
				var value = Movement.Evaluate(delta);

				var position = startPosition + ((endPosition - startPosition) * value);
				RectTransform.anchoredPosition = isHorizontal
					? new Vector2(position, RectTransform.anchoredPosition.y)
					: new Vector2(RectTransform.anchoredPosition.x, position);

				yield return null;
			}
			while (delta < animation_length);

			RectTransform.anchoredPosition = isHorizontal
				? new Vector2(endPosition, RectTransform.anchoredPosition.y)
				: new Vector2(RectTransform.anchoredPosition.x, endPosition);

			IsAnimationRunning = false;
		}
	}
}