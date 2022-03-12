namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Splitter type.
	/// </summary>
	public enum SplitterType
	{
		/// <summary>
		/// Horizontal.
		/// </summary>
		Horizontal = 0,

		/// <summary>
		/// Vertical.
		/// </summary>
		Vertical = 1,
	}

	/// <summary>
	/// Splitter mode.
	/// </summary>
	public enum SplitterMode
	{
		/// <summary>
		/// Auto mode. Use previous and next siblings in hierarchy.
		/// </summary>
		Auto = 0,

		/// <summary>
		/// Manual mode. Use specified targets to resize.
		/// </summary>
		Manual = 1,
	}

	/// <summary>
	/// Splitter.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Interactions/Splitter")]
	[DisallowMultipleComponent]
	[RequireComponent(typeof(RectTransform))]
	public class Splitter : UIBehaviour,
		IInitializePotentialDragHandler, IBeginDragHandler, IEndDragHandler, IDragHandler,
		IPointerEnterHandler, IPointerExitHandler, ILateUpdatable
	{
		/// <summary>
		/// Allow resize.
		/// </summary>
		[Obsolete("Replaced with Interactable.")]
		public bool AllowResize
		{
			get
			{
				return Interactable;
			}

			set
			{
				Interactable = value;
			}
		}

		#region Interactable
		[SerializeField]
		[FormerlySerializedAs("allowResize")]
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
				if (IsCursorOver)
				{
					IsCursorOver = false;

					if (!processDrag)
					{
						ResetCursor();
					}
				}
			}
		}
		#endregion

		/// <summary>
		/// The type.
		/// </summary>
		public SplitterType Type = SplitterType.Vertical;

		/// <summary>
		/// Is need to update RectTransform on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateRectTransforms = true;

		/// <summary>
		/// Is need to update LayoutElement on Resize.
		/// </summary>
		[SerializeField]
		public bool UpdateLayoutElements = true;

		/// <summary>
		/// The current camera. For Screen Space - Overlay let it empty.
		/// </summary>
		[NonSerialized]
		protected Camera CurrentCamera;

		/// <summary>
		/// Cursors.
		/// </summary>
		[SerializeField]
		public Cursors Cursors;

		/// <summary>
		/// The cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D CursorTexture;

		/// <summary>
		/// The cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 CursorHotSpot = new Vector2(16, 16);

		/// <summary>
		/// The default cursor texture.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Texture2D DefaultCursorTexture;

		/// <summary>
		/// The default cursor hot spot.
		/// </summary>
		[SerializeField]
		[Obsolete("Replaced with Cursors and UICursor.Cursors.")]
		public Vector2 DefaultCursorHotSpot;

		/// <summary>
		/// Start resize event.
		/// </summary>
		public SplitterResizeEvent OnStartResize = new SplitterResizeEvent();

		/// <summary>
		/// During resize event.
		/// </summary>
		public SplitterResizeEvent OnResize = new SplitterResizeEvent();

		/// <summary>
		/// End resize event.
		/// </summary>
		public SplitterResizeEvent OnEndResize = new SplitterResizeEvent();

		RectTransform rectTransform;

		/// <summary>
		/// Gets the RectTransform.
		/// </summary>
		/// <value>The RectTransform.</value>
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
		/// Mode.
		/// </summary>
		[SerializeField]
		protected SplitterMode Mode = SplitterMode.Auto;

		/// <summary>
		/// Previous object.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("leftTarget")]
		protected RectTransform PreviousObject;

		/// <summary>
		/// Next object.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("rightTarget")]
		protected RectTransform NextObject;

		LayoutElement previousLayoutElement;

		/// <summary>
		/// LayoutElement of the previous target.
		/// </summary>
		protected LayoutElement PreviousLayoutElement
		{
			get
			{
				if ((previousLayoutElement == null) || (previousLayoutElement.gameObject != PreviousObject.gameObject))
				{
					previousLayoutElement = Utilities.GetOrAddComponent<LayoutElement>(PreviousObject);
				}

				return previousLayoutElement;
			}
		}

		LayoutElement nextLayoutElement;

		/// <summary>
		/// LayoutElement of the next target.
		/// </summary>
		protected LayoutElement NextLayoutElement
		{
			get
			{
				if ((nextLayoutElement == null) || (nextLayoutElement.gameObject != NextObject.gameObject))
				{
					nextLayoutElement = Utilities.GetOrAddComponent<LayoutElement>(NextObject);
				}

				return nextLayoutElement;
			}
		}

		/// <summary>
		/// Preferred size of the previous element.
		/// </summary>
		protected Vector2 PreviousPreferredSize;

		/// <summary>
		/// Minimal size of the previous element.
		/// </summary>
		protected Vector2 PreviousMinSize;

		/// <summary>
		/// Preferred size of the next element.
		/// </summary>
		protected Vector2 NextPreferredSize;

		/// <summary>
		/// Minimal size of the next element.
		/// </summary>
		protected Vector2 NextMinSize;

		Vector2 summarySize;

		bool processDrag;

		List<Splitter> nestedSplitters = new List<Splitter>();

		bool isInited;

		bool cursorChanged;

		/// <summary>
		/// Is cursor over?
		/// </summary>
		protected bool IsCursorOver;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();
			Init();
		}

		/// <summary>
		/// Process the initialize potential drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnInitializePotentialDrag(PointerEventData eventData)
		{
			LayoutUtilities.UpdateLayout(transform.parent.GetComponent<LayoutGroup>());
			transform.parent.GetComponentsInChildren<Splitter>(nestedSplitters);

			foreach (var splitter in nestedSplitters)
			{
				InitSizes(splitter);
			}

			nestedSplitters.Clear();
		}

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

#pragma warning disable 0618
			if (DefaultCursorTexture != null)
			{
				UICursor.ObsoleteWarning();
			}
#pragma warning restore 0618
		}

		/// <summary>
		/// Init splitter sizes.
		/// </summary>
		/// <param name="splitter">Splitter.</param>
		protected virtual void InitSizes(Splitter splitter)
		{
			splitter.InitSizes();
		}

		/// <summary>
		/// Init sizes.
		/// </summary>
		protected void InitSizes()
		{
			var index = transform.GetSiblingIndex();

			if (index == 0 || transform.parent.childCount == (index + 1))
			{
				return;
			}

			if (Mode == SplitterMode.Auto)
			{
				PreviousObject = transform.parent.GetChild(index - 1) as RectTransform;
				NextObject = transform.parent.GetChild(index + 1) as RectTransform;
			}

			PreviousLayoutElement.preferredWidth = PreviousObject.rect.width;
			PreviousLayoutElement.preferredHeight = PreviousObject.rect.height;

			NextLayoutElement.preferredWidth = NextObject.rect.width;
			NextLayoutElement.preferredHeight = NextObject.rect.height;
		}

		/// <summary>
		/// Can change cursor?
		/// </summary>
		protected bool CanChangeCursor
		{
			get
			{
				return UICursor.CanSet(this) && CompatibilityInput.MousePresent && IsCursorOver;
			}
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerEnter event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			IsCursorOver = true;
			CurrentCamera = eventData.pressEventCamera;
		}

		/// <summary>
		/// Called by a BaseInputModule when an OnPointerExit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			IsCursorOver = false;

			if (!processDrag)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Reset cursor.
		/// </summary>
		protected void ResetCursor()
		{
			cursorChanged = false;
			UICursor.Reset(this);
		}

		/// <summary>
		/// Get cursor.
		/// </summary>
		/// <returns>Cursor.</returns>
		protected virtual Cursors.Cursor GetCursor()
		{
			if (Cursors != null)
			{
				return IsHorizontal() ? Cursors.NorthSouthArrow : Cursors.EastWestArrow;
			}

			if (UICursor.Cursors != null)
			{
				return IsHorizontal() ? UICursor.Cursors.NorthSouthArrow : UICursor.Cursors.EastWestArrow;
			}

			return default(Cursors.Cursor);
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
		/// Update cursor.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (!IsActive())
			{
				return;
			}

			if (!CanChangeCursor)
			{
				return;
			}

			if (CompatibilityInput.IsMouseLeftButtonPressed)
			{
				return;
			}

			if (processDrag)
			{
				return;
			}

			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, CompatibilityInput.MousePosition, CurrentCamera, out point))
			{
				return;
			}

			var rect = RectTransform.rect;
			if (rect.Contains(point))
			{
				cursorChanged = true;
				UICursor.Set(this, GetCursor());
			}
			else if (cursorChanged)
			{
				ResetCursor();
			}
		}

		/// <summary>
		/// Process the begin drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnBeginDrag(PointerEventData eventData)
		{
			if (!IsActive())
			{
				return;
			}

			Vector2 point;
			processDrag = false;

			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out point))
			{
				return;
			}

			var index = transform.GetSiblingIndex();

			if (index == 0 || transform.parent.childCount == (index + 1))
			{
				return;
			}

			UICursor.Set(this, GetCursor());
			cursorChanged = true;

			processDrag = true;

			if (Mode == SplitterMode.Auto)
			{
				PreviousObject = transform.parent.GetChild(index - 1) as RectTransform;
				NextObject = transform.parent.GetChild(index + 1) as RectTransform;
			}

			var previous_rect = PreviousObject.rect;
			PreviousLayoutElement.preferredWidth = previous_rect.width;
			PreviousLayoutElement.preferredHeight = previous_rect.height;
			PreviousPreferredSize = previous_rect.size;

			var next_rect = NextObject.rect;
			NextLayoutElement.preferredWidth = next_rect.width;
			NextLayoutElement.preferredHeight = next_rect.height;
			NextPreferredSize = next_rect.size;

			summarySize = new Vector2(PreviousPreferredSize.x + NextPreferredSize.x, PreviousPreferredSize.y + NextPreferredSize.y);

			OnStartResize.Invoke(this);
		}

		/// <summary>
		/// Process the end drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnEndDrag(PointerEventData eventData)
		{
			ResetCursor();

			if (processDrag)
			{
				processDrag = false;

				OnEndResize.Invoke(this);
			}
		}

		/// <summary>
		/// Process the drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (!processDrag)
			{
				return;
			}

			Vector2 current_point;
			Vector2 original_point;

			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.position, eventData.pressEventCamera, out current_point);
			RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, eventData.pressPosition, eventData.pressEventCamera, out original_point);

			var delta = current_point - original_point;

			if (UpdateRectTransforms)
			{
				PerformUpdateRectTransforms(delta);
			}

			if (UpdateLayoutElements)
			{
				PerformUpdateLayoutElements(delta);
			}

			OnResize.Invoke(this);
		}

		/// <summary>
		/// Is horizontal direction?
		/// </summary>
		/// <returns>true if direction is horizontal; otherwise false.</returns>
		protected bool IsHorizontal()
		{
			return Type == SplitterType.Horizontal;
		}

		/// <summary>
		/// Update RectTransform sizes.
		/// </summary>
		/// <param name="delta">Size delta.</param>
		protected void PerformUpdateRectTransforms(Vector2 delta)
		{
			if (!IsHorizontal())
			{
				float previous_width;
				float next_width;

				if (delta.x > 0)
				{
					previous_width = Mathf.Min(PreviousPreferredSize.x + delta.x, summarySize.x - NextLayoutElement.minWidth);
					next_width = summarySize.x - previous_width;
				}
				else
				{
					next_width = Mathf.Min(NextPreferredSize.x - delta.x, summarySize.x - PreviousLayoutElement.minWidth);
					previous_width = summarySize.x - next_width;
				}

				PreviousObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, previous_width);
				NextObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, next_width);
			}
			else
			{
				float previous_height;
				float next_height;

				delta.y *= -1;
				if (delta.y > 0)
				{
					previous_height = Mathf.Min(PreviousPreferredSize.y + delta.y, summarySize.y - NextLayoutElement.minHeight);
					next_height = summarySize.y - previous_height;
				}
				else
				{
					next_height = Mathf.Min(NextPreferredSize.y - delta.y, summarySize.y - PreviousLayoutElement.minHeight);
					previous_height = summarySize.y - next_height;
				}

				PreviousObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, previous_height);
				NextObject.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, next_height);
			}
		}

		/// <summary>
		/// Update LayouElement sizes.
		/// </summary>
		/// <param name="delta">Size delta.</param>
		protected void PerformUpdateLayoutElements(Vector2 delta)
		{
			if (!IsHorizontal())
			{
				if (delta.x > 0)
				{
					PreviousLayoutElement.preferredWidth = Mathf.Min(PreviousPreferredSize.x + delta.x, summarySize.x - NextLayoutElement.minWidth);
					NextLayoutElement.preferredWidth = summarySize.x - PreviousLayoutElement.preferredWidth;
				}
				else
				{
					NextLayoutElement.preferredWidth = Mathf.Min(NextPreferredSize.x - delta.x, summarySize.x - PreviousLayoutElement.minWidth);
					PreviousLayoutElement.preferredWidth = summarySize.x - NextLayoutElement.preferredWidth;
				}
			}
			else
			{
				delta.y *= -1;
				if (delta.y > 0)
				{
					PreviousLayoutElement.preferredHeight = Mathf.Min(PreviousPreferredSize.y + delta.y, summarySize.y - NextLayoutElement.minHeight);
					NextLayoutElement.preferredHeight = summarySize.y - PreviousLayoutElement.preferredHeight;
				}
				else
				{
					NextLayoutElement.preferredHeight = Mathf.Min(NextPreferredSize.y - delta.y, summarySize.y - PreviousLayoutElement.minHeight);
					PreviousLayoutElement.preferredHeight = summarySize.y - NextLayoutElement.preferredHeight;
				}
			}
		}

		#if UNITY_EDITOR

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			CursorsDPISelector.Require(this);

			base.Reset();
		}

		#endif
	}
}