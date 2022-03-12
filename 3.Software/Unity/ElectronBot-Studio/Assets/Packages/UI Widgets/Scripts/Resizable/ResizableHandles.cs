namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Handles for the Resizable component.
	/// </summary>
	[DisallowMultipleComponent]
	[RequireComponent(typeof(Resizable))]
	[AddComponentMenu("UI/New UI Widgets/Interactions/Resizable Handles")]
	public class ResizableHandles : UIBehaviourConditional
	{
		/// <summary>
		/// ResizableHandle.
		/// </summary>
		protected class ResizableHandle
		{
			/// <summary>
			/// Owner.
			/// </summary>
			protected ResizableHandles Owner;

			/// <summary>
			/// Regions.
			/// </summary>
			protected Resizable.Regions Regions;

			/// <summary>
			/// Handle.
			/// </summary>
			protected DragListener Handle;

			/// <summary>
			/// RectTransform.
			/// </summary>
			protected RectTransform RectTransform;

			/// <summary>
			/// Is currently dragged?
			/// </summary>
			protected bool IsDrag;

			/// <summary>
			/// Initializes a new instance of the <see cref="ResizableHandle"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="regions">Regions.</param>
			/// <param name="handle">Handle.</param>
			public ResizableHandle(ResizableHandles owner, Resizable.Regions regions, DragListener handle)
			{
				Owner = owner;
				Regions = regions;
				Handle = handle;
				RectTransform = handle.transform as RectTransform;

				Handle.OnDragStartEvent.AddListener(StartDrag);
				Handle.OnDragEvent.AddListener(Drag);
				Handle.OnDragEndEvent.AddListener(EndDrag);
			}

			/// <summary>
			/// Process begin drag event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			protected virtual void StartDrag(PointerEventData eventData)
			{
				if (!Owner.IsActive())
				{
					return;
				}

				IsDrag = true;
				Owner.Target.InitResize();

				Owner.OnStartResize.Invoke(Owner.Target);
			}

			/// <summary>
			/// Process end drag event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			protected virtual void EndDrag(PointerEventData eventData)
			{
				IsDrag = false;

				Owner.OnEndResize.Invoke(Owner.Target);
			}

			/// <summary>
			/// Process drag event.
			/// </summary>
			/// <param name="eventData">Event data.</param>
			protected virtual void Drag(PointerEventData eventData)
			{
				if (!IsDrag)
				{
					return;
				}

				Vector2 current_point;
				Vector2 original_point;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out current_point);
				RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out original_point);

				var delta = current_point - original_point;
				Owner.Target.Resize(Regions, delta);
				Owner.OnResize.Invoke(Owner.Target, Regions, delta);
			}

			/// <summary>
			/// Process destroy event.
			/// </summary>
			public void Destroy()
			{
				if (Handle != null)
				{
					Handle.OnDragStartEvent.RemoveListener(StartDrag);
					Handle.OnDragEvent.RemoveListener(Drag);
					Handle.OnDragEndEvent.RemoveListener(EndDrag);
				}

				Owner = null;
				Handle = null;
				RectTransform = null;
			}
		}

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
		}
		#endregion

		/// <summary>
		/// Use own handles or handles from the specified source.
		/// </summary>
		[SerializeField]
		public bool OwnHandles = true;

		/// <summary>
		/// Handles source.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles", false)]
		public ResizableHandles HandlesSource;

		/// <summary>
		/// Top left drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener topLeft;

		/// <summary>
		/// Top left drag handle.
		/// </summary>
		public DragListener TopLeft
		{
			get
			{
				return topLeft;
			}

			set
			{
				topLeft = value;
				CreateHandle(ref topLeftHandle, new Resizable.Regions() { Top = true, Left = true }, topLeft);
			}
		}

		/// <summary>
		/// Top center drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener topCenter;

		/// <summary>
		/// Top center drag handle.
		/// </summary>
		public DragListener TopCenter
		{
			get
			{
				return topCenter;
			}

			set
			{
				topCenter = value;
				CreateHandle(ref topCenterHandle, new Resizable.Regions() { Top = true, }, topCenter);
			}
		}

		/// <summary>
		/// Top right drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener topRight;

		/// <summary>
		/// Top right drag handle.
		/// </summary>
		public DragListener TopRight
		{
			get
			{
				return topRight;
			}

			set
			{
				topRight = value;
				CreateHandle(ref topRightHandle, new Resizable.Regions() { Top = true, Right = true }, topRight);
			}
		}

		/// <summary>
		/// Middle left drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener middleLeft;

		/// <summary>
		/// Middle left drag handle.
		/// </summary>
		public DragListener MiddleLeft
		{
			get
			{
				return middleLeft;
			}

			set
			{
				middleLeft = value;
				CreateHandle(ref middleLeftHandle, new Resizable.Regions() { Left = true }, middleLeft);
			}
		}

		/// <summary>
		/// Middle right drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener middleRight;

		/// <summary>
		/// Middle right drag handle.
		/// </summary>
		public DragListener MiddleRight
		{
			get
			{
				return middleRight;
			}

			set
			{
				middleRight = value;
				CreateHandle(ref middleRightHandle, new Resizable.Regions() { Right = true }, middleRight);
			}
		}

		/// <summary>
		/// Bottom left drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener bottomLeft;

		/// <summary>
		/// Bottom left drag handle.
		/// </summary>
		public DragListener BottomLeft
		{
			get
			{
				return bottomLeft;
			}

			set
			{
				bottomLeft = value;
				CreateHandle(ref bottomLeftHandle, new Resizable.Regions() { Bottom = true, Left = true }, bottomLeft);
			}
		}

		/// <summary>
		/// Bottom center drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener bottomCenter;

		/// <summary>
		/// Bottom center drag handle.
		/// </summary>
		public DragListener BottomCenter
		{
			get
			{
				return bottomCenter;
			}

			set
			{
				bottomCenter = value;
				CreateHandle(ref bottomCenterHandle, new Resizable.Regions() { Bottom = true, }, bottomCenter);
			}
		}

		/// <summary>
		/// Bottom right drag handle.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("OwnHandles")]
		protected DragListener bottomRight;

		/// <summary>
		/// Bottom right drag handle.
		/// </summary>
		public DragListener BottomRight
		{
			get
			{
				return bottomRight;
			}

			set
			{
				bottomRight = value;
				CreateHandle(ref bottomRightHandle, new Resizable.Regions() { Bottom = true, Right = true }, bottomRight);
			}
		}

		/// <summary>
		/// Top left handle.
		/// </summary>
		protected ResizableHandle topLeftHandle;

		/// <summary>
		/// Top center handle.
		/// </summary>
		protected ResizableHandle topCenterHandle;

		/// <summary>
		/// Top right handle.
		/// </summary>
		protected ResizableHandle topRightHandle;

		/// <summary>
		/// Middle left handle.
		/// </summary>
		protected ResizableHandle middleLeftHandle;

		/// <summary>
		/// Middle right handle.
		/// </summary>
		protected ResizableHandle middleRightHandle;

		/// <summary>
		/// Bottom left handle.
		/// </summary>
		protected ResizableHandle bottomLeftHandle;

		/// <summary>
		/// Bottom center handle.
		/// </summary>
		protected ResizableHandle bottomCenterHandle;

		/// <summary>
		/// Bottom right handle.
		/// </summary>
		protected ResizableHandle bottomRightHandle;

		/// <summary>
		/// Target.
		/// </summary>
		protected Resizable Target;

		bool isInited;

		/// <summary>
		/// On start resize event.
		/// </summary>
		[SerializeField]
		public ResizableEvent OnStartResize = new ResizableEvent();

		/// <summary>
		/// On resize event.
		/// </summary>
		[SerializeField]
		public ResizableDeltaEvent OnResize = new ResizableDeltaEvent();

		/// <summary>
		/// On end resize event.
		/// </summary>
		[SerializeField]
		public ResizableEvent OnEndResize = new ResizableEvent();

		/// <summary>
		/// Starts this instance.
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

			Target = GetComponent<Resizable>();
			Target.OnResizeDirectionsChanged.AddListener(SetHandlesState);
			Target.Interactable = false;

			TopLeft = topLeft;
			TopCenter = topCenter;
			TopRight = topRight;

			MiddleLeft = middleLeft;
			MiddleRight = middleRight;

			BottomLeft = bottomLeft;
			BottomCenter = bottomCenter;
			BottomRight = bottomRight;

			SetHandlesState(Target);
		}

		/// <summary>
		/// Set handles state.
		/// </summary>
		/// <param name="resizable">Resizable component.</param>
		protected void SetHandlesState(Resizable resizable)
		{
			var directions = resizable.ResizeDirections;
			SetHandleState(TopLeft, directions.TopLeft);
			SetHandleState(TopCenter, directions.Top);
			SetHandleState(TopRight, directions.TopRight);

			SetHandleState(MiddleLeft, directions.Left);
			SetHandleState(MiddleRight, directions.Right);

			SetHandleState(BottomLeft, directions.BottomLeft);
			SetHandleState(BottomCenter, directions.Bottom);
			SetHandleState(BottomRight, directions.BottomRight);
		}

		/// <summary>
		/// Set handles state.
		/// </summary>
		/// <param name="handle">Handle.</param>
		/// <param name="state">State.</param>
		protected virtual void SetHandleState(DragListener handle, bool state)
		{
			if (handle != null)
			{
				handle.gameObject.SetActive(state);
			}
		}

		/// <summary>
		/// Interactable state of the handles source.
		/// </summary>
		protected bool HandlesSourceInteractable;

		/// <summary>
		/// Get source handles.
		/// </summary>
		public void GetSourceHandles()
		{
			HandlesSourceInteractable = HandlesSource.interactable;
			HandlesSource.interactable = false;
			SetHandlesParent(HandlesSource, transform);

			TopLeft = HandlesSource.TopLeft;
			TopCenter = HandlesSource.TopCenter;
			TopRight = HandlesSource.TopRight;

			MiddleLeft = HandlesSource.MiddleLeft;
			MiddleRight = HandlesSource.MiddleRight;

			BottomLeft = HandlesSource.BottomLeft;
			BottomCenter = HandlesSource.BottomCenter;
			BottomRight = HandlesSource.BottomRight;

			SetHandlesState(Target);
		}

		/// <summary>
		/// Release source handles.
		/// </summary>
		public void ReleaseSourceHandles()
		{
			HandlesSource.interactable = HandlesSourceInteractable;
			HandlesSource.Init();
			SetHandlesParent(HandlesSource, HandlesSource.transform);
			HandlesSource.SetHandlesState(HandlesSource.Target);
		}

		/// <summary>
		/// Set new parent to the handles.
		/// </summary>
		/// <param name="handles">Handles.</param>
		/// <param name="parent">Parent.</param>
		protected static void SetHandlesParent(ResizableHandles handles, Transform parent)
		{
			SetHandleParent(handles.TopLeft, parent);
			SetHandleParent(handles.TopCenter, parent);
			SetHandleParent(handles.TopRight, parent);

			SetHandleParent(handles.MiddleLeft, parent);
			SetHandleParent(handles.MiddleRight, parent);

			SetHandleParent(handles.BottomLeft, parent);
			SetHandleParent(handles.BottomCenter, parent);
			SetHandleParent(handles.BottomRight, parent);
		}

		/// <summary>
		/// Set new parent to the handle.
		/// </summary>
		/// <param name="handle">Handle.</param>
		/// <param name="parent">Parent.</param>
		protected static void SetHandleParent(DragListener handle, Transform parent)
		{
			if (handle != null)
			{
				handle.transform.SetParent(parent, false);
			}
		}

		/// <summary>
		/// Enable handles.
		/// </summary>
		public void EnableHandles()
		{
			Init();

			SetHandlesState(Target);
		}

		/// <summary>
		/// Disable handles.
		/// </summary>
		public void DisableHandles()
		{
			DisableHandle(topLeft);
			DisableHandle(topCenter);
			DisableHandle(topRight);

			DisableHandle(middleLeft);
			DisableHandle(middleRight);

			DisableHandle(bottomLeft);
			DisableHandle(bottomCenter);
			DisableHandle(bottomRight);
		}

		/// <summary>
		/// Disable handle.
		/// </summary>
		/// <param name="handle">Handle.</param>
		public virtual void DisableHandle(DragListener handle)
		{
			if (handle != null)
			{
				handle.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Create handle.
		/// </summary>
		/// <param name="handle">Handle to create.</param>
		/// <param name="regions">Regions.</param>
		/// <param name="drag">Drag handle.</param>
		protected virtual void CreateHandle(ref ResizableHandle handle, Resizable.Regions regions, DragListener drag)
		{
			DestroyHandle(ref handle);

			if (drag == null)
			{
				return;
			}

			handle = new ResizableHandle(this, regions, drag);
		}

		/// <summary>
		/// Destroy handle.
		/// </summary>
		/// <param name="handle">Handle.</param>
		protected virtual void DestroyHandle(ref ResizableHandle handle)
		{
			if (handle == null)
			{
				return;
			}

			handle.Destroy();
			handle = null;
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (Target != null)
			{
				Target.OnResizeDirectionsChanged.RemoveListener(SetHandlesState);
			}

			DestroyHandle(ref topLeftHandle);
			DestroyHandle(ref topCenterHandle);
			DestroyHandle(ref topRightHandle);

			DestroyHandle(ref middleLeftHandle);
			DestroyHandle(ref middleRightHandle);

			DestroyHandle(ref bottomLeftHandle);
			DestroyHandle(ref bottomCenterHandle);
			DestroyHandle(ref bottomRightHandle);

			base.OnDestroy();
		}
	}
}