namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Sidebar.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	[AddComponentMenu("UI/New UI Widgets/Sidebar")]
	public class Sidebar : UIBehaviourConditional, IBeginDragHandler, IEndDragHandler, IDragHandler, IStylable
	{
		#region Interactable
		[SerializeField]
		[FormerlySerializedAs("Interactable")]
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
				StopAnimations();
				if (IsOpen)
				{
					SetOpen();
				}
				else
				{
					SetClose();
				}
			}
		}
		#endregion

		/// <summary>
		/// AnimationCurve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		public AnimationCurve Curve = AnimationCurve.EaseInOut(0, 0, 1, 1);

		[SerializeField]
		[FormerlySerializedAs("Direction")]
		SidebarAxis direction = SidebarAxis.LeftToRight;

		/// <summary>
		/// Direction.
		/// </summary>
		public SidebarAxis Direction
		{
			get
			{
				return direction;
			}

			set
			{
				direction = value;
				RefreshPosition();
			}
		}

		[SerializeField]
		SidebarAnimation animationType = SidebarAnimation.Overlay;

		/// <summary>
		/// Sidebar animation type.
		/// </summary>
		/// <value>The animation.</value>
		public SidebarAnimation AnimationType
		{
			get
			{
				return animationType;
			}

			set
			{
				SetAnimation(value);
			}
		}

		/// <summary>
		/// Scale cannot be lower this value for ScaleDown animation.
		/// </summary>
		[SerializeField]
		protected float scaleDownLimit = 0f;

		/// <summary>
		/// Scale cannot be lower this value for ScaleDown animation.
		/// </summary>
		public float ScaleDownLimit
		{
			get
			{
				return scaleDownLimit;
			}

			set
			{
				scaleDownLimit = Mathf.Clamp01(value);
			}
		}

		/// <summary>
		/// AnimationType converted to int.
		/// </summary>
		protected int AnimationTypeInt
		{
			get
			{
				return (int)animationType;
			}
		}

		[SerializeField]
		bool isOpen;

		/// <summary>
		/// Gets or sets a value indicating whether this instance is open.
		/// </summary>
		/// <value><c>true</c> if this instance is open; otherwise, <c>false</c>.</value>
		public bool IsOpen
		{
			get
			{
				return isOpen;
			}

			set
			{
				isOpen = value;
				SetModal(isOpen);
				ResetPosition();
			}
		}

		[SerializeField]
		bool modal;

		/// <summary>
		/// Is sidebar should be closed with click outside block.
		/// </summary>
		public bool Modal
		{
			get
			{
				return modal;
			}

			set
			{
				modal = value;
				if (!modal)
				{
					ModalClose();
				}
			}
		}

		/// <summary>
		/// Modal background color.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("modal")]
		[Tooltip("Background color.")]
		public Color ModalColor = new Color(1f, 1f, 1f, 0f);

		[SerializeField]
		bool scrollRectSupport;

		/// <summary>
		/// Handle children ScrollRect's drag events.
		/// </summary>
		/// <value><c>true</c> if ScrollRect support; otherwise, <c>false</c>.</value>
		public bool ScrollRectSupport
		{
			get
			{
				return scrollRectSupport;
			}

			set
			{
				if (scrollRectSupport)
				{
					RemoveScrollRectHandle();
				}

				scrollRectSupport = value;

				if (scrollRectSupport)
				{
					AddScrollRectHandle();
				}
			}
		}

		/// <summary>
		/// Nested ScrollRects.
		/// </summary>
		protected List<ScrollRect> NestedScrollRects = new List<ScrollRect>();

		/// <summary>
		/// The content.
		/// </summary>
		[SerializeField]
		public RectTransform Content;

		/// <summary>
		/// Animate layout.
		/// </summary>
		[SerializeField]
		public bool AnimateWithLayout;

		/// <summary>
		/// Can animate layout?
		/// </summary>
		public bool CanAnimateWithLayout
		{
			get
			{
				return AnimateWithLayout && Content != null && ContentLayout != null;
			}
		}

		LayoutGroup сontentLayout;

		/// <summary>
		/// Content layout.
		/// </summary>
		protected LayoutGroup ContentLayout
		{
			get
			{
				if (сontentLayout == null)
				{
					сontentLayout = Content.GetComponent<LayoutGroup>();
				}

				return сontentLayout;
			}
		}

		[SerializeField]
		GameObject optionalHandle;

		/// <summary>
		/// Gets or sets the optional handle.
		/// </summary>
		/// <value>The optional handle.</value>
		public GameObject OptionalHandle
		{
			get
			{
				return optionalHandle;
			}

			set
			{
				if (optionalHandle != null)
				{
					RemoveHandleEvents(optionalHandle);
				}

				optionalHandle = value;

				if (optionalHandle != null)
				{
					AddHandleEvents(optionalHandle);
				}
			}
		}

		/// <summary>
		/// Parent canvas.
		/// </summary>
		[SerializeField]
		public RectTransform ParentCanvas;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = false;

		/// <summary>
		/// OnOpen event.
		/// </summary>
		public UnityEvent OnOpen = new UnityEvent();

		/// <summary>
		/// OnClose event.
		/// </summary>
		public UnityEvent OnClose = new UnityEvent();

		/// <summary>
		/// OnOpeningStarted event.
		/// </summary>
		public UnityEvent OnOpeningStarted = new UnityEvent();

		/// <summary>
		/// OnClosingStarted event.
		/// </summary>
		public UnityEvent OnClosingStarted = new UnityEvent();

		RectTransform sidebarRect;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>RectTransform.</value>
		protected RectTransform SidebarRect
		{
			get
			{
				if (sidebarRect == null)
				{
					sidebarRect = transform as RectTransform;
				}

				return sidebarRect;
			}
		}

		/// <summary>
		/// Open actions.
		/// </summary>
		protected Dictionary<int, Action> ActionSetOpen;

		/// <summary>
		/// Close actions.
		/// </summary>
		protected Dictionary<int, Action> ActionSetClose;

		/// <summary>
		/// Drag actions.
		/// </summary>
		protected Dictionary<int, Action<Vector2>> ActionDrag;

		/// <summary>
		/// Animate actions.
		/// </summary>
		protected Dictionary<int, Action> ActionAnimate;

		/// <summary>
		/// Set open state actions.
		/// </summary>
		protected Dictionary<int, Func<float>> ActionOpenState;

		/// <summary>
		/// Sidebar position on close state.
		/// </summary>
		protected Vector2 SidebarPosition;

		/// <summary>
		/// Layout padding.
		/// </summary>
		protected float LayoutPadding;

		/// <summary>
		/// Content position.
		/// </summary>
		protected Vector2 ContentPosition;

		/// <summary>
		/// Content size.
		/// </summary>
		protected Vector2 ContentSize;

		/// <summary>
		/// Content top left corner position.
		/// </summary>
		protected Vector2 ContentTopLeftCorner;

		/// <summary>
		/// The current animation.
		/// </summary>
		protected List<IEnumerator> Animations = new List<IEnumerator>();

		/// <summary>
		/// Get warning message.
		/// </summary>
		/// <returns>Warning message.</returns>
		protected string GetWarning()
		{
			if (Content != null && Content.gameObject == gameObject)
			{
				return "Content value cannot not be Sidebar gameobject.";
			}

			if (Curve[0].value >= Curve[Curve.length - 1].value)
			{
				return string.Format("Curve requirements: start value (current: {0}) should be less than end value (current: {1}).", Curve[0].value.ToString(), Curve[Curve.length - 1].value.ToString());
			}

			return string.Empty;
		}

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
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

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}

			InitActions();

			var warning = GetWarning();
			if (!string.IsNullOrEmpty(warning))
			{
				Debug.LogWarning(warning, this);
			}

			RefreshPosition();

			OptionalHandle = optionalHandle;

			ScrollRectSupport = scrollRectSupport;

			ResetPosition();

			if (IsOpen)
			{
				ModalOpen();
			}
		}

		/// <summary>
		/// Init actions for each animations.
		/// </summary>
		protected void InitActions()
		{
			ActionSetOpen = new Dictionary<int, Action>()
			{
				{ (int)SidebarAnimation.Overlay, OverlaySetOpen },
				{ (int)SidebarAnimation.Push, PushSetOpen },
				{ (int)SidebarAnimation.ScaleDown, ScaleDownSetOpen },
				{ (int)SidebarAnimation.SlideAlong, SlideAlongSetOpen },
				{ (int)SidebarAnimation.SlideOut, SlideOutSetOpen },
				{ (int)SidebarAnimation.Uncover, UncoverSetOpen },
				{ (int)SidebarAnimation.Resize, ResizeSetOpen },
				{ (int)SidebarAnimation.ScaleDownAndPush, ScaleDownAndPushSetOpen },
			};

			ActionSetClose = new Dictionary<int, Action>()
			{
				{ (int)SidebarAnimation.Overlay, OverlaySetClose },
				{ (int)SidebarAnimation.Push, PushSetClose },
				{ (int)SidebarAnimation.ScaleDown, ScaleDownSetClose },
				{ (int)SidebarAnimation.SlideAlong, SlideAlongSetClose },
				{ (int)SidebarAnimation.SlideOut, SlideOutSetClose },
				{ (int)SidebarAnimation.Uncover, UncoverSetClose },
				{ (int)SidebarAnimation.Resize, ResizeSetClose },
				{ (int)SidebarAnimation.ScaleDownAndPush, ScaleDownAndPushSetClose },
			};

			ActionDrag = new Dictionary<int, Action<Vector2>>()
			{
				{ (int)SidebarAnimation.Overlay, OverlayDrag },
				{ (int)SidebarAnimation.Push, PushDrag },
				{ (int)SidebarAnimation.ScaleDown, ScaleDownDrag },
				{ (int)SidebarAnimation.SlideAlong, SlideAlongDrag },
				{ (int)SidebarAnimation.SlideOut, SlideOutDrag },
				{ (int)SidebarAnimation.Uncover, UncoverDrag },
				{ (int)SidebarAnimation.Resize, ResizeDrag },
				{ (int)SidebarAnimation.ScaleDownAndPush, ScaleDownAndPushDrag },
			};

			ActionAnimate = new Dictionary<int, Action>()
			{
				{ (int)SidebarAnimation.Overlay, OverlayAnimate },
				{ (int)SidebarAnimation.Push, PushAnimate },
				{ (int)SidebarAnimation.ScaleDown, ScaleDownAnimate },
				{ (int)SidebarAnimation.SlideAlong, SlideAlongAnimate },
				{ (int)SidebarAnimation.SlideOut, SlideOutAnimate },
				{ (int)SidebarAnimation.Uncover, UncoverAnimate },
				{ (int)SidebarAnimation.Resize, ResizeAnimate },
				{ (int)SidebarAnimation.ScaleDownAndPush, ScaleDownAndPushAnimate },
			};

			ActionOpenState = new Dictionary<int, Func<float>>()
			{
				{ (int)SidebarAnimation.Overlay, OverlayOpenState },
				{ (int)SidebarAnimation.Push, PushOpenState },
				{ (int)SidebarAnimation.ScaleDown, ScaleDownOpenState },
				{ (int)SidebarAnimation.SlideAlong, SlideAlongOpenState },
				{ (int)SidebarAnimation.SlideOut, SlideOutOpenState },
				{ (int)SidebarAnimation.Uncover, UncoverOpenState },
				{ (int)SidebarAnimation.Resize, ResizeOpenState },
				{ (int)SidebarAnimation.ScaleDownAndPush, ScaleDownAndPushOpenState },
			};
		}

		/// <summary>
		/// Set sidebar and content position according to current state.
		/// </summary>
		protected void RefreshPosition()
		{
			SidebarPosition = SidebarRect.anchoredPosition;

			if (Content != null)
			{
				ContentPosition = Content.anchoredPosition;
				ContentSize = Content.rect.size;
				ContentTopLeftCorner = UtilitiesRectTransform.GetTopLeftCorner(Content);
				LayoutPadding = GetLayoutPadding();
			}
		}

		/// <summary>
		/// Add ScrollRect listeners.
		/// </summary>
		protected void AddScrollRectHandle()
		{
			GetComponentsInChildren<ScrollRect>(NestedScrollRects);

			foreach (var s in NestedScrollRects)
			{
				AddHandleEvents(s);
			}

			NestedScrollRects.Clear();
		}

		/// <summary>
		/// Remove ScrollRect listeners.
		/// </summary>
		protected void RemoveScrollRectHandle()
		{
			GetComponentsInChildren<ScrollRect>(NestedScrollRects);

			foreach (var s in NestedScrollRects)
			{
				RemoveHandleEvents(s);
			}

			NestedScrollRects.Clear();
		}

		/// <summary>
		/// Add events listeners.
		/// </summary>
		/// <param name="handleComponent">Handle component.</param>
		protected void AddHandleEvents(Component handleComponent)
		{
			AddHandleEvents(handleComponent.gameObject);
		}

		/// <summary>
		/// Add events listeners.
		/// </summary>
		/// <param name="handleObject">Handle object.</param>
		protected void AddHandleEvents(GameObject handleObject)
		{
			var handle = Utilities.GetOrAddComponent<SidebarHandle>(handleObject);
			handle.BeginDragEvent.AddListener(OnBeginDrag);
			handle.DragEvent.AddListener(OnDrag);
			handle.EndDragEvent.AddListener(OnEndDrag);
		}

		/// <summary>
		/// Remove events listeners.
		/// </summary>
		/// <param name="handleComponent">Handle component.</param>
		protected void RemoveHandleEvents(Component handleComponent)
		{
			RemoveHandleEvents(handleComponent.gameObject);
		}

		/// <summary>
		/// Remove events listeners.
		/// </summary>
		/// <param name="handleObject">Handle object.</param>
		protected void RemoveHandleEvents(GameObject handleObject)
		{
			var handle = handleObject.GetComponent<SidebarHandle>();
			if (handle != null)
			{
				handle.BeginDragEvent.RemoveListener(OnBeginDrag);
				handle.DragEvent.RemoveListener(OnDrag);
				handle.EndDragEvent.RemoveListener(OnEndDrag);
			}
		}

		/// <summary>
		/// Stop running animations.
		/// </summary>
		protected void StopAnimations()
		{
			foreach (var a in Animations)
			{
				StopCoroutine(a);
			}

			Animations.Clear();
			AnimationsFinished = 0;
		}

		/// <summary>
		/// Start prepared animations.
		/// </summary>
		protected void StartAnimations()
		{
			for (int i = 0; i < Animations.Count; i++)
			{
				StartCoroutine(Animations[i]);
			}
		}

		/// <summary>
		/// Set new animation type.
		/// </summary>
		/// <param name="anim">Animation type.</param>
		protected void SetAnimation(SidebarAnimation anim)
		{
			if (anim == animationType)
			{
				return;
			}

			StopAnimations();
			animationType = anim;

			ResetPosition();
		}

		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Sets the modal state.
		/// </summary>
		/// <param name="isOpen">Is open?</param>
		protected void SetModal(bool isOpen)
		{
			if (isOpen)
			{
				ModalOpen();
			}
			else
			{
				ModalClose();
			}
		}

		/// <summary>
		/// Open modal helper.
		/// </summary>
		protected void ModalOpen()
		{
			if (!modal)
			{
				return;
			}

			if (ModalKey.HasValue)
			{
				return;
			}

			var parent = (Content != null) ? Content.parent : transform.parent;
			ModalKey = ModalHelper.Open(this, null, ModalColor, Close, ParentCanvas);

			var modal_rt = ModalHelper.GetInstance(ModalKey.Value).transform as RectTransform;
			modal_rt.SetParent(parent, false);

			if (Content != null)
			{
				modal_rt.localPosition = Content.localPosition;
			}

			if (
				(AnimationType != SidebarAnimation.Uncover)
				&& (AnimationType != SidebarAnimation.SlideAlong)
				&& (AnimationType != SidebarAnimation.SlideOut))
			{
				transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Close modal helper.
		/// </summary>
		protected void ModalClose()
		{
			if (ModalKey.HasValue)
			{
				ModalHelper.GetInstance(ModalKey.Value).transform.SetParent(UtilitiesUI.FindTopmostCanvas(transform), false);
				ModalHelper.Close(ModalKey.Value);

				ModalKey = null;
			}
		}

		/// <summary>
		/// Resets the position.
		/// </summary>
		protected void ResetPosition()
		{
			if (isOpen)
			{
				ActionSetOpen[AnimationTypeInt]();
			}
			else
			{
				ActionSetClose[AnimationTypeInt]();
			}
		}

		/// <summary>
		/// Called by a BaseInputModule before a drag is started.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnBeginDrag(PointerEventData eventData)
		{
			// do nothing
		}

		/// <summary>
		/// Called by a BaseInputModule when a drag is ended.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnEndDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			var k = ActionOpenState[AnimationTypeInt]();

			if (k == 0f)
			{
				isOpen = false;
				ResetPosition();
				SetModal(isOpen);
				OnClose.Invoke();
				return;
			}

			if (k == 1f)
			{
				isOpen = true;
				ResetPosition();
				SetModal(isOpen);
				OnOpen.Invoke();
				return;
			}

			isOpen = k < 0.5f;
			Animate(k > 0f);
		}

		/// <summary>
		/// When dragging is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			StopAllCoroutines();

			Vector2 current_point;
			Vector2 original_point;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(SidebarRect, eventData.position, eventData.pressEventCamera, out current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(SidebarRect, eventData.pressPosition, eventData.pressEventCamera, out original_point);

			var delta = current_point - original_point;

			ActionDrag[AnimationTypeInt](delta);
			SetModal(ActionOpenState[AnimationTypeInt]() > 0);
		}

		/// <summary>
		/// Toggle this instance with animation.
		/// If animation already running state will not be toggled.
		/// </summary>
		public void Toggle()
		{
			if (IsOpen)
			{
				Close();
			}
			else
			{
				Open();
			}
		}

		/// <summary>
		/// Open this instance. with animation.
		/// If animation already running state will not be opened.
		/// </summary>
		public void Open()
		{
			Init();

			if (IsOpen)
			{
				return;
			}

			Animate(IsOpen);
		}

		/// <summary>
		/// Close this instance with animation.
		/// If animation already running state will not be closed.
		/// </summary>
		public void Close()
		{
			Init();

			if (!IsOpen)
			{
				return;
			}

			Animate(IsOpen);
		}

		/// <summary>
		/// Forcibly open this instance with animation.
		/// Already running animation will be canceled.
		/// </summary>
		public void ForciblyOpen()
		{
			Init();

			ForciblyAnimate(true);
		}

		/// <summary>
		/// Forcibly close this instance with animation.
		/// Already running animation will be canceled.
		/// </summary>
		public void ForciblyClose()
		{
			Init();

			ForciblyAnimate(false);
		}

		/// <summary>
		/// Forcibly the animate to specified state.
		/// Already running animation will be canceled.
		/// </summary>
		/// <param name="shoudBeOpen">If set to <c>true</c> state will changed to open.</param>
		protected void ForciblyAnimate(bool shoudBeOpen)
		{
			StopAnimations();

			isOpen = !shoudBeOpen;

			SetModal(isOpen);
			ActionAnimate[AnimationTypeInt]();
		}

		/// <summary>
		/// Set open state.
		/// </summary>
		public void SetOpen()
		{
			Init();
			isOpen = true;
			ResetPosition();
		}

		/// <summary>
		/// Set close state.
		/// </summary>
		public void SetClose()
		{
			Init();
			isOpen = false;
			ResetPosition();
		}

		/// <summary>
		/// Is direction is horizontal?
		/// </summary>
		/// <returns>true if direction is horizontal; otherwise, false.</returns>
		protected bool IsHorizontal()
		{
			return Direction == SidebarAxis.LeftToRight || Direction == SidebarAxis.RightToLeft;
		}

		/// <summary>
		/// Is axis is reverse?
		/// </summary>
		/// <returns>true axis is reverse; otherwise, false.</returns>
		protected bool IsReverse()
		{
			return Direction == SidebarAxis.RightToLeft || Direction == SidebarAxis.TopToBottom;
		}

		/// <summary>
		/// Animate.
		/// </summary>
		/// <param name="isOpen">Is open?</param>
		protected void Animate(bool isOpen)
		{
			StopAnimations();

			if (isOpen)
			{
				OnClosingStarted.Invoke();
			}
			else
			{
				OnOpeningStarted.Invoke();
			}

			SetModal(isOpen);

			ActionAnimate[AnimationTypeInt]();
		}

		/// <summary>
		/// Finished animations count.
		/// </summary>
		protected int AnimationsFinished;

		/// <summary>
		/// Animation ended.
		/// </summary>
		protected void AnimationEnd()
		{
			AnimationsFinished += 1;
			if (Animations.Count > AnimationsFinished)
			{
				return;
			}

			Animations.Clear();
			AnimationsFinished = 0;

			isOpen = !isOpen;
			ResetPosition();
			SetModal(isOpen);

			if (IsOpen)
			{
				OnOpen.Invoke();
			}
			else
			{
				OnClose.Invoke();
			}
		}

		/// <summary>
		/// Sidebar size along direction axis.
		/// </summary>
		/// <returns>Size along direction axis</returns>
		protected float SidebarSize()
		{
			return IsHorizontal() ? SidebarRect.rect.width : SidebarRect.rect.height;
		}

		/// <summary>
		/// Get time.
		/// </summary>
		/// <returns>Time.</returns>
		[Obsolete("Use Utilities.GetTime(UnscaledTime).")]
		protected float GetTime()
		{
			return Utilities.GetTime(UnscaledTime);
		}

		#region Base

		/// <summary>
		/// Get position for specified progress.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <param name="position">Progress, </param>
		/// <returns>Sidebar position for specified progress.</returns>
		protected float RectOpenState(RectTransform rect, Vector2 position)
		{
			var pos = IsHorizontal()
				? rect.anchoredPosition.x - position.x
				: rect.anchoredPosition.y - position.y;
			var sign = IsReverse() ? -1 : +1;

			return pos / (SidebarSize() * sign);
		}

		/// <summary>
		/// Get open position for specified progress.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <param name="position">Initial position.</param>
		/// <param name="rate">Progress rate.</param>
		/// <returns>Open position for specified progress.</returns>
		protected Vector2 RectOpenPosition(RectTransform rect, Vector2 position, float rate = 1f)
		{
			switch (Direction)
			{
				case SidebarAxis.LeftToRight:
					return new Vector2(position.x + (SidebarSize() * rate), rect.anchoredPosition.y);
				case SidebarAxis.RightToLeft:
					return new Vector2(position.x - (SidebarSize() * rate), rect.anchoredPosition.y);
				case SidebarAxis.TopToBottom:
					return new Vector2(rect.anchoredPosition.x, position.y - (SidebarSize() * rate));
				case SidebarAxis.BottomToTop:
					return new Vector2(rect.anchoredPosition.x, position.y + (SidebarSize() * rate));
				default:
					throw new NotSupportedException(string.Format("Unknown sidebar axis: {0}", EnumHelper<SidebarAxis>.ToString(Direction)));
			}
		}

		/// <summary>
		/// Set sidebar open position.
		/// </summary>
		protected void SidebarSetOpen()
		{
			SidebarRect.anchoredPosition = RectOpenPosition(SidebarRect, SidebarPosition);
		}

		/// <summary>
		/// Set content open position.
		/// </summary>
		protected void ContentSetOpen()
		{
			Content.anchoredPosition = RectOpenPosition(Content, ContentPosition);
		}

		/// <summary>
		/// Set sidebar close position.
		/// </summary>
		protected void SidebarSetClose()
		{
			SidebarRect.anchoredPosition = SidebarPosition;
		}

		/// <summary>
		/// Set content close position.
		/// </summary>
		protected void ContentSetClose()
		{
			Content.anchoredPosition = ContentPosition;
		}

		/// <summary>
		/// Handle RectTransform drag.
		/// </summary>
		/// <param name="rect">RectTrasnform.</param>
		/// <param name="closePosition">Close position.</param>
		/// <param name="openPosition">Open position.</param>
		/// <param name="delta">Drag value.</param>
		protected void RectDrag(RectTransform rect, Vector2 closePosition, Vector2 openPosition, Vector2 delta)
		{
			var min = new Vector2(Mathf.Min(closePosition.x, openPosition.x), Mathf.Min(closePosition.y, openPosition.y));
			var max = new Vector2(Mathf.Max(closePosition.x, openPosition.x), Mathf.Max(closePosition.y, openPosition.y));
			switch (Direction)
			{
				case SidebarAxis.LeftToRight:
				case SidebarAxis.RightToLeft:
					var lv = IsOpen ? openPosition.x + delta.x : closePosition.x + delta.x;
					var lx = Mathf.Clamp(lv, min.x, max.x);
					rect.anchoredPosition = new Vector2(lx, rect.anchoredPosition.y);
					break;
				case SidebarAxis.TopToBottom:
				case SidebarAxis.BottomToTop:
					var tv = IsOpen ? openPosition.y + delta.y : closePosition.y + delta.y;
					var ty = Mathf.Clamp(tv, min.y, max.y);
					rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, ty);
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown sidebar axis: {0}", EnumHelper<SidebarAxis>.ToString(Direction)));
			}
		}

		/// <summary>
		/// Handle RectTransform scale drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected virtual void ContentScaleDrag(Vector2 delta)
		{
			var rectDelta = SidebarStateValue();
			var movement = SidebarSize() - Mathf.Abs(rectDelta);
			var scale = ContentScale(movement);

			Content.localScale = new Vector3(scale, scale, 1f);
		}

		/// <summary>
		/// Handle sidebar drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void SidebarDrag(Vector2 delta)
		{
			RectDrag(SidebarRect, SidebarPosition, RectOpenPosition(SidebarRect, SidebarPosition), delta);
		}

		/// <summary>
		/// Handle content drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void ContentDrag(Vector2 delta)
		{
			RectDrag(Content, ContentPosition, RectOpenPosition(Content, ContentPosition), delta);
		}

		/// <summary>
		/// Animation coroutine.
		/// </summary>
		/// <param name="rect">RectTransform.</param>
		/// <param name="endPos">End position.</param>
		/// <param name="rectSpeed">Animation speed.</param>
		/// <returns>Coroutine.</returns>
		protected IEnumerator AnimationCoroutine(RectTransform rect, Vector2 endPos, float rectSpeed)
		{
			var animation_length = Curve[Curve.length - 1].time;
			var size = SidebarSize();

			var start_position = IsHorizontal() ? rect.anchoredPosition.x : rect.anchoredPosition.y;
			var end_position = IsHorizontal() ? endPos.x : endPos.y;
			var delta = start_position - end_position;

			var move_direction = IsReverse() ? -1 : +1;
			float movement = isOpen
				? delta
				: (size * rectSpeed * move_direction) - delta;
			var acceleration = (movement == 0f) ? 1f : (size * rectSpeed) / movement;

			if (rect == SidebarRect)
			{
				acceleration = Mathf.Abs(acceleration);
			}

			float animation_position;
			var animate_direction = isOpen ? -1 : +1;

			if ((rect == Content) && IsReverse())
			{
				acceleration *= -1;
			}

			var startTime = UtilitiesTime.GetTime(UnscaledTime);
			do
			{
				animation_position = (UtilitiesTime.GetTime(UnscaledTime) - startTime) * acceleration;
				var value = Curve.Evaluate(animation_position) * movement * animate_direction;

				rect.anchoredPosition = IsHorizontal()
					? new Vector2(start_position + value, rect.anchoredPosition.y)
					: new Vector2(rect.anchoredPosition.x, start_position + value);
				yield return null;
			}
			while (animation_position < animation_length);

			AnimationEnd();
		}

		#endregion

		#region Overlay

		/// <summary>
		/// Overlay. Set open state.
		/// </summary>
		protected void OverlaySetOpen()
		{
			SidebarSetOpen();
			ContentSetClose();
		}

		/// <summary>
		/// Overlay. Set close state.
		/// </summary>
		protected void OverlaySetClose()
		{
			SidebarSetClose();
			ContentSetClose();
		}

		/// <summary>
		/// Overlay. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void OverlayDrag(Vector2 delta)
		{
			SidebarDrag(delta);
		}

		/// <summary>
		/// Overlay. Animate.
		/// </summary>
		protected void OverlayAnimate()
		{
			Animations.Add(AnimationCoroutine(SidebarRect, SidebarPosition, 1f));
			StartAnimations();
		}

		/// <summary>
		/// Overlay. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float OverlayOpenState()
		{
			return RectOpenState(SidebarRect, SidebarPosition);
		}
		#endregion

		#region Push

		/// <summary>
		/// Push. Set open state.
		/// </summary>
		protected void PushSetOpen()
		{
			SidebarSetOpen();
			ContentSetOpen();
		}

		/// <summary>
		/// Push. Set close state.
		/// </summary>
		protected void PushSetClose()
		{
			SidebarSetClose();
			ContentSetClose();
		}

		/// <summary>
		/// Push. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void PushDrag(Vector2 delta)
		{
			SidebarDrag(delta);
			ContentDrag(delta);
		}

		/// <summary>
		/// Push. Animate.
		/// </summary>
		protected void PushAnimate()
		{
			Animations.Add(AnimationCoroutine(SidebarRect, SidebarPosition, 1f));
			Animations.Add(AnimationCoroutine(Content, ContentPosition, 1f));
			StartAnimations();
		}

		/// <summary>
		/// Push. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float PushOpenState()
		{
			return RectOpenState(SidebarRect, SidebarPosition);
		}
		#endregion

		#region Uncover

		/// <summary>
		/// Uncover. Set open state.
		/// </summary>
		protected void UncoverSetOpen()
		{
			SidebarSetOpen();
			ContentSetOpen();

			SidebarRect.SetAsFirstSibling();

			if (OptionalHandle != null)
			{
				OptionalHandle.transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Uncover. Set close state.
		/// </summary>
		protected void UncoverSetClose()
		{
			SidebarSetOpen();
			ContentSetClose();

			SidebarRect.SetAsFirstSibling();

			if (OptionalHandle != null)
			{
				OptionalHandle.transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// Uncover. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void UncoverDrag(Vector2 delta)
		{
			SidebarSetOpen();

			ContentDrag(delta);
		}

		/// <summary>
		/// Uncover. Animate.
		/// </summary>
		protected void UncoverAnimate()
		{
			SidebarSetOpen();

			Animations.Add(AnimationCoroutine(Content, ContentPosition, 1f));
			StartAnimations();
		}

		/// <summary>
		/// Uncover. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float UncoverOpenState()
		{
			return RectOpenState(Content, ContentPosition);
		}
		#endregion

		#region ScaleDown

		/// <summary>
		/// Get content scale.
		/// </summary>
		/// <returns>Content scale.</returns>
		protected float ContentScale()
		{
			return ContentScale(0);
		}

		/// <summary>
		/// Get content scale for specified progress.
		/// </summary>
		/// <param name="pos">Progress.</param>
		/// <returns>Content scale.</returns>
		protected float ContentScale(float pos)
		{
			var content_size = IsHorizontal() ? Content.rect.width : Content.rect.height;
			var rate = pos / SidebarSize();
			var min_scale = Mathf.Max(ScaleDownLimit, (content_size - (SidebarSize() * 2)) / content_size);

			return Mathf.Lerp(min_scale, 1f, rate);
		}

		/// <summary>
		/// ScaleDown. Set open state.
		/// </summary>
		protected void ScaleDownSetOpen()
		{
			SidebarSetOpen();
			ContentSetClose();

			var scale = ContentScale();
			Content.localScale = new Vector3(scale, scale, 1f);
		}

		/// <summary>
		/// ScaleDown. Set close state.
		/// </summary>
		protected void ScaleDownSetClose()
		{
			SidebarSetClose();
			ContentSetClose();

			Content.localScale = Vector3.one;
		}

		/// <summary>
		/// ScaleDown. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void ScaleDownDrag(Vector2 delta)
		{
			SidebarDrag(delta);
			ContentScaleDrag(delta);
		}

		/// <summary>
		/// ScaleDown. Animate.
		/// </summary>
		protected void ScaleDownAnimate()
		{
			Animations.Add(AnimationCoroutine(SidebarRect, SidebarPosition, 1f));
			Animations.Add(ScaleDownAnimationCoroutine());
			StartAnimations();
		}

		/// <summary>
		/// ScaleDown. Get animation coroutine.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		protected IEnumerator ScaleDownAnimationCoroutine()
		{
			var animation_length = Curve[Curve.length - 1].time;

			var scale_size = ContentScale() - 1f;
			var start_scale = Content.localScale.x;
			var delta = start_scale - 1f;

			var movement = isOpen ? delta : scale_size - delta;
			var acceleration = (movement == 0f) ? 1f : scale_size / movement;
			var animate_direction = isOpen ? -1 : +1;

			float animation_position;
			float scale;

			var start_time = UtilitiesTime.GetTime(UnscaledTime);
			do
			{
				animation_position = (UtilitiesTime.GetTime(UnscaledTime) - start_time) * acceleration;
				scale = Curve.Evaluate(animation_position) * movement * animate_direction;

				Content.localScale = new Vector3(start_scale + scale, start_scale + scale, 1f);

				yield return null;
			}
			while (animation_position < animation_length);

			AnimationEnd();
		}

		/// <summary>
		/// ScaleDown. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float ScaleDownOpenState()
		{
			return RectOpenState(SidebarRect, SidebarPosition);
		}
		#endregion

		#region SlideAlong

		/// <summary>
		/// SlideAlong. Set open state.
		/// </summary>
		protected void SlideAlongSetOpen()
		{
			SidebarSetOpen();
			ContentSetOpen();

			SidebarRect.SetAsFirstSibling();

			if (OptionalHandle != null)
			{
				OptionalHandle.transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// SlideAlong. Set close state.
		/// </summary>
		protected void SlideAlongSetClose()
		{
			SidebarRect.anchoredPosition = RectOpenPosition(SidebarRect, SidebarPosition, 0.5f);
			ContentSetClose();

			SidebarRect.SetAsFirstSibling();

			if (OptionalHandle != null)
			{
				OptionalHandle.transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// SlideAlong. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void SlideAlongDrag(Vector2 delta)
		{
			var close = RectOpenPosition(SidebarRect, SidebarPosition, 0.5f);
			var open = RectOpenPosition(SidebarRect, SidebarPosition);
			RectDrag(SidebarRect, close, open, delta * 0.5f);

			ContentDrag(delta);
		}

		/// <summary>
		/// SlideAlong. Animate.
		/// </summary>
		protected void SlideAlongAnimate()
		{
			Animations.Add(AnimationCoroutine(SidebarRect, RectOpenPosition(SidebarRect, SidebarPosition, 0.5f), 0.5f));
			Animations.Add(AnimationCoroutine(Content, ContentPosition, 1f));
			StartAnimations();
		}

		/// <summary>
		/// SlideAlong. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float SlideAlongOpenState()
		{
			return RectOpenState(Content, ContentPosition);
		}
		#endregion

		#region SlideOut

		/// <summary>
		/// SlideOut. Set open state.
		/// </summary>
		protected void SlideOutSetOpen()
		{
			SidebarSetOpen();
			ContentSetOpen();

			SidebarRect.SetAsFirstSibling();

			if (OptionalHandle != null)
			{
				OptionalHandle.transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// SlideOut. Set close state.
		/// </summary>
		protected void SlideOutSetClose()
		{
			SidebarRect.anchoredPosition = RectOpenPosition(SidebarRect, SidebarPosition, 1.5f);
			ContentSetClose();

			SidebarRect.SetAsFirstSibling();

			if (OptionalHandle != null)
			{
				OptionalHandle.transform.SetAsLastSibling();
			}
		}

		/// <summary>
		/// SlideOut. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void SlideOutDrag(Vector2 delta)
		{
			var end_position = RectOpenPosition(SidebarRect, SidebarPosition, 1.5f);
			var start_position = RectOpenPosition(SidebarRect, SidebarPosition);
			RectDrag(SidebarRect, end_position, start_position, delta * -0.5f);
			ContentDrag(delta);
		}

		/// <summary>
		/// SlideOut. Animate.
		/// </summary>
		protected void SlideOutAnimate()
		{
			var speed = isOpen ? 0.5f : -0.5f;
			Animations.Add(AnimationCoroutine(SidebarRect, RectOpenPosition(SidebarRect, SidebarPosition, 1.5f), speed));
			Animations.Add(AnimationCoroutine(Content, ContentPosition, 1f));
			StartAnimations();
		}

		/// <summary>
		/// SlideOut. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float SlideOutOpenState()
		{
			return RectOpenState(Content, ContentPosition);
		}
		#endregion

		#region Resize

		/// <summary>
		/// Resize. Set open state.
		/// </summary>
		protected void ResizeSetOpen()
		{
			SidebarSetOpen();
			if (CanAnimateWithLayout)
			{
				ResizeSetContentLayoutPadding();
			}
			else
			{
				ResizeContentRectTransform();
			}
		}

		/// <summary>
		/// Resize. Set close state.
		/// </summary>
		protected void ResizeSetClose()
		{
			SidebarSetClose();
			if (CanAnimateWithLayout)
			{
				ResizeSetContentLayoutPadding();
			}
			else
			{
				ResizeContentRectTransform();
			}
		}

		/// <summary>
		/// Resize. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void ResizeDrag(Vector2 delta)
		{
			SidebarDrag(delta);
			if (CanAnimateWithLayout)
			{
				ResizeContentLayoutDrag(delta);
			}
			else
			{
				ResizeContentDrag(delta);
			}
		}

		/// <summary>
		/// Resize. Animate.
		/// </summary>
		protected void ResizeAnimate()
		{
			Animations.Add(AnimationCoroutine(SidebarRect, SidebarPosition, 1f));
			if (CanAnimateWithLayout)
			{
				Animations.Add(ResizeWithLayoutCoroutine());
			}
			else
			{
				Animations.Add(ResizeCoroutine());
				Animations.Add(ResizeMovementCoroutine());
			}

			StartAnimations();
		}

		/// <summary>
		/// Resize. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float ResizeOpenState()
		{
			return RectOpenState(SidebarRect, SidebarPosition);
		}

		/// <summary>
		/// Resize. Get movement coroutine.
		/// </summary>
		/// <returns>Movement coroutine.</returns>
		protected IEnumerator ResizeMovementCoroutine()
		{
			var animation_length = Curve[Curve.length - 1].time;
			var sidebar_size = SidebarSize();

			var start_pos = UtilitiesRectTransform.GetTopLeftCorner(Content);
			var start_position = IsHorizontal() ? start_pos.x : start_pos.y;
			var end_position = IsHorizontal() ? ContentTopLeftCorner.x : ContentTopLeftCorner.y;
			if (!isOpen)
			{
				if (Direction == SidebarAxis.LeftToRight)
				{
					end_position += sidebar_size;
				}
				else if (Direction == SidebarAxis.TopToBottom)
				{
					end_position -= sidebar_size;
				}
			}

			var delta = end_position - start_position;
			var movement = delta;
			var acceleration = (movement == 0f) ? 1f : Mathf.Abs(sidebar_size / movement);

			float animation_position;

			yield return null;

			var startTime = UtilitiesTime.GetTime(UnscaledTime);
			do
			{
				animation_position = (UtilitiesTime.GetTime(UnscaledTime) - startTime) * acceleration;
				var value = Curve.Evaluate(animation_position) * movement;

				var pos = IsHorizontal()
					? new Vector2(start_position + value, start_pos.y)
					: new Vector2(start_pos.x, start_position + value);
				UtilitiesRectTransform.SetTopLeftCorner(Content, pos);

				yield return null;
			}
			while (animation_position < animation_length);

			AnimationEnd();
		}

		/// <summary>
		/// Resize. Get resize coroutine.
		/// </summary>
		/// <returns>Resize coroutine.</returns>
		protected IEnumerator ResizeCoroutine()
		{
			var axis = IsHorizontal() ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			var animation_length = Curve[Curve.length - 1].time;
			var sidebar_size = SidebarSize();

			var start_size = IsHorizontal() ? Content.rect.width : Content.rect.height;
			var end_size = IsHorizontal() ? ContentSize.x : ContentSize.y;
			if (!isOpen)
			{
				end_size -= sidebar_size;
			}

			var delta = end_size - start_size;
			var movement = delta;
			var acceleration = (movement == 0f) ? 1f : Mathf.Abs(sidebar_size / movement);

			float animation_position;

			yield return null;

			var startTime = UtilitiesTime.GetTime(UnscaledTime);
			do
			{
				animation_position = (UtilitiesTime.GetTime(UnscaledTime) - startTime) * acceleration;
				var value = Curve.Evaluate(animation_position) * movement;

				Content.SetSizeWithCurrentAnchors(axis, start_size + value);

				yield return null;
			}
			while (animation_position < animation_length);

			AnimationEnd();
		}

		/// <summary>
		/// Resize. Get resize layout coroutine.
		/// </summary>
		/// <returns>Resize layout coroutine.</returns>
		protected IEnumerator ResizeWithLayoutCoroutine()
		{
			var animation_length = Curve[Curve.length - 1].time;
			var sidebar_size = SidebarSize();

			var start_size = GetLayoutPadding();
			var end_size = isOpen ? 0 : sidebar_size;

			var movement = end_size - start_size;
			var acceleration = (movement == 0f) ? 1f : Mathf.Abs(sidebar_size / movement);

			float animation_position;

			yield return null;

			var startTime = UtilitiesTime.GetTime(UnscaledTime);
			do
			{
				animation_position = (UtilitiesTime.GetTime(UnscaledTime) - startTime) * acceleration;
				var value = Curve.Evaluate(animation_position) * movement;

				SetLayoutPadding(start_size + value);

				yield return null;
			}
			while (animation_position < animation_length);

			AnimationEnd();
		}

		/// <summary>
		/// Get layout padding.
		/// </summary>
		/// <returns>Layout padding.</returns>
		protected float GetLayoutPadding()
		{
			switch (Direction)
			{
				case SidebarAxis.LeftToRight:
					return LayoutUtilities.GetPaddingLeft(ContentLayout);
				case SidebarAxis.RightToLeft:
					return LayoutUtilities.GetPaddingRight(ContentLayout);
				case SidebarAxis.TopToBottom:
					return LayoutUtilities.GetPaddingTop(ContentLayout);
				case SidebarAxis.BottomToTop:
					return LayoutUtilities.GetPaddingBottom(ContentLayout);
				default:
					throw new NotSupportedException(string.Format("Unknown sidebar axis: {0}", EnumHelper<SidebarAxis>.ToString(Direction)));
			}
		}

		/// <summary>
		/// Set layout padding.
		/// </summary>
		/// <param name="padding">Layout padding</param>
		protected void SetLayoutPadding(float padding)
		{
			switch (Direction)
			{
				case SidebarAxis.LeftToRight:
					LayoutUtilities.SetPaddingLeft(ContentLayout, padding);
					break;
				case SidebarAxis.RightToLeft:
					LayoutUtilities.SetPaddingRight(ContentLayout, padding);
					break;
				case SidebarAxis.TopToBottom:
					LayoutUtilities.SetPaddingTop(ContentLayout, padding);
					break;
				case SidebarAxis.BottomToTop:
					LayoutUtilities.SetPaddingBottom(ContentLayout, padding);
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown sidebar axis: {0}", EnumHelper<SidebarAxis>.ToString(Direction)));
			}
		}

		/// <summary>
		/// Resize. Set content layout padding.
		/// </summary>
		protected void ResizeSetContentLayoutPadding()
		{
			var size = isOpen ? SidebarSize() : 0;
			SetLayoutPadding(size);
		}

		/// <summary>
		/// Resize Content RectTransform.
		/// </summary>
		protected void ResizeContentRectTransform()
		{
			var axis = IsHorizontal() ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			var size = IsHorizontal() ? ContentSize.x : ContentSize.y;
			if (isOpen)
			{
				size -= SidebarSize();
			}

			Content.SetSizeWithCurrentAnchors(axis, size);

			var pos = ContentTopLeftCorner;
			if (isOpen)
			{
				if (Direction == SidebarAxis.LeftToRight)
				{
					pos.x += SidebarSize();
				}
				else if (Direction == SidebarAxis.TopToBottom)
				{
					pos.y -= SidebarSize();
				}
			}

			UtilitiesRectTransform.SetTopLeftCorner(Content, pos);
		}

		/// <summary>
		/// Resize Content Layout with drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void ResizeContentLayoutDrag(Vector2 delta)
		{
			var new_padding = ResizeDragDelta(delta);

			SetLayoutPadding(LayoutPadding + new_padding);
		}

		/// <summary>
		/// Get size delta by cursor delta.
		/// </summary>
		/// <param name="delta">Delta.</param>
		/// <returns>Size delta.</returns>
		protected float ResizeDragDelta(Vector2 delta)
		{
			float resize_delta;
			var direction = IsOpen ? -1 : 1;
			var sidebar_size = SidebarSize();
			switch (Direction)
			{
				case SidebarAxis.LeftToRight:
					resize_delta = delta.x * direction;
					break;
				case SidebarAxis.RightToLeft:
					resize_delta = -(delta.x * direction);
					break;
				case SidebarAxis.TopToBottom:
					resize_delta = -(delta.y * direction);
					break;
				case SidebarAxis.BottomToTop:
					resize_delta = delta.y * direction;
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown sidebar axis: {0}", EnumHelper<SidebarAxis>.ToString(Direction)));
			}

			resize_delta = Mathf.Clamp(resize_delta, 0, sidebar_size);
			if (IsOpen)
			{
				resize_delta = sidebar_size - resize_delta;
			}

			return resize_delta;
		}

		/// <summary>
		/// Resize content with drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void ResizeContentDrag(Vector2 delta)
		{
			var axis = IsHorizontal() ? RectTransform.Axis.Horizontal : RectTransform.Axis.Vertical;
			var size = IsHorizontal() ? ContentSize.x : ContentSize.y;
			var sidebar_size = SidebarSize();

			var pos = ContentTopLeftCorner;
			var cur_pos = pos;

			var size_delta = ResizeDragDelta(delta);

			switch (Direction)
			{
				case SidebarAxis.LeftToRight:
					cur_pos.x = Mathf.Clamp(pos.x + size_delta, pos.x, pos.x + sidebar_size);
					break;
				case SidebarAxis.RightToLeft:
					break;
				case SidebarAxis.TopToBottom:
					cur_pos.y = Mathf.Clamp(pos.y - size_delta, pos.y - sidebar_size, pos.y);
					break;
				case SidebarAxis.BottomToTop:
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown sidebar axis: {0}", EnumHelper<SidebarAxis>.ToString(Direction)));
			}

			Content.SetSizeWithCurrentAnchors(axis, size - size_delta);

			UtilitiesRectTransform.SetTopLeftCorner(Content, cur_pos);
		}
		#endregion

		#region ScaleDownAndPush

		/// <summary>
		/// ScaleDownAndPush. Set open state.
		/// </summary>
		protected void ScaleDownAndPushSetOpen()
		{
			SidebarSetOpen();

			var scale = ContentScale();
			var rate = ScaleDownAndPushRate(scale, SidebarSize()) / SidebarSize();

			Content.anchoredPosition = RectOpenPosition(Content, ContentPosition, rate);
			Content.localScale = new Vector3(scale, scale, 1f);
		}

		/// <summary>
		/// ScaleDownAndPush. Set close state.
		/// </summary>
		protected void ScaleDownAndPushSetClose()
		{
			SidebarSetClose();
			ContentSetClose();

			Content.localScale = Vector3.one;
		}

		/// <summary>
		/// How much content should be moved with specified SidebarSize for ScaleDownAndPush animation.
		/// </summary>
		/// <returns>The scale and push rate.</returns>
		/// <param name="scale">Scale.</param>
		/// <param name="sidebarSize">How much sidebar opened.</param>
		protected float ScaleDownAndPushRate(float scale, float sidebarSize)
		{
			var content_size = IsHorizontal() ? Content.rect.width : Content.rect.height;
			return sidebarSize - ((content_size / 2) * (1 - scale));
		}

		/// <summary>
		/// How much sidebar opened.
		/// </summary>
		/// <returns>Sidebar open value.</returns>
		protected float SidebarStateValue()
		{
			var startPosition = IsHorizontal() ? SidebarRect.anchoredPosition.x : SidebarRect.anchoredPosition.y;
			var endPosition = IsHorizontal() ? SidebarPosition.x : SidebarPosition.y;
			return startPosition - endPosition;
		}

		/// <summary>
		/// Handle RectTransform scale drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected virtual void ContentScaleAndPushDrag(Vector2 delta)
		{
			var sidebar_pos = Mathf.Abs(SidebarStateValue());
			var scale = ContentScale(SidebarSize() - Mathf.Abs(sidebar_pos));

			var rate = ScaleDownAndPushRate(scale, sidebar_pos) / SidebarSize();

			Content.anchoredPosition = RectOpenPosition(Content, ContentPosition, rate);
			Content.localScale = new Vector3(scale, scale, 1f);
		}

		/// <summary>
		/// ScaleDownAndPush. Handle drag.
		/// </summary>
		/// <param name="delta">Drag value.</param>
		protected void ScaleDownAndPushDrag(Vector2 delta)
		{
			SidebarDrag(delta);
			ContentScaleAndPushDrag(delta);
		}

		/// <summary>
		/// ScaleDownAndPush. Animate.
		/// </summary>
		protected void ScaleDownAndPushAnimate()
		{
			Animations.Add(AnimationCoroutine(SidebarRect, SidebarPosition, 1f));
			Animations.Add(ScaleDownAndPushAnimationCoroutine());
			StartAnimations();
		}

		/// <summary>
		/// ScaleDownAndPush. Get sidebar open state value.
		/// </summary>
		/// <returns>Sidebar open state value.</returns>
		protected float ScaleDownAndPushOpenState()
		{
			return RectOpenState(SidebarRect, SidebarPosition);
		}

		/// <summary>
		/// ScaleDown. Get animation coroutine.
		/// </summary>
		/// <returns>Animation coroutine.</returns>
		protected IEnumerator ScaleDownAndPushAnimationCoroutine()
		{
			var animation_length = Curve[Curve.length - 1].time;

			var scale_size = ContentScale() - 1f;
			var start_scale = Content.localScale.x;
			var delta = start_scale - 1f;

			var movement = isOpen ? delta : scale_size - delta;
			var acceleration = (movement == 0f) ? 1f : scale_size / movement;
			var animate_direction = isOpen ? -1 : +1;

			float animation_position;
			float scale;

			var start_time = UtilitiesTime.GetTime(UnscaledTime);
			do
			{
				animation_position = (UtilitiesTime.GetTime(UnscaledTime) - start_time) * acceleration;
				scale = Curve.Evaluate(animation_position) * movement * animate_direction;

				var rate = ScaleDownAndPushRate(start_scale + scale, Mathf.Abs(SidebarStateValue())) / SidebarSize();

				Content.anchoredPosition = RectOpenPosition(Content, ContentPosition, rate);
				Content.localScale = new Vector3(start_scale + scale, start_scale + scale, 1f);

				yield return null;
			}
			while (animation_position < animation_length);

			AnimationEnd();
		}
		#endregion

		#if UNITY_EDITOR
		/// <summary>
		/// Validate values after changes in Inspector window.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();
			ScaleDownLimit = Mathf.Clamp01(ScaleDownLimit);

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			base.Reset();

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Sidebar.Background.ApplyTo(GetComponent<Image>());

			return false;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Sidebar.Background.GetFrom(GetComponent<Image>());

			return false;
		}
		#endregion
	}
}