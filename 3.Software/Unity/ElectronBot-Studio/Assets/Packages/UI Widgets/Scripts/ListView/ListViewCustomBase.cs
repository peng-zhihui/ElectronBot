namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using System.Threading;
	using EasyLayoutNS;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for custom ListViews.
	/// </summary>
	public abstract class ListViewCustomBase : ListViewBase, IAutoScroll
	{
		/// <summary>
		/// Virtualization.
		/// </summary>
		[SerializeField]
		protected bool virtualization = true;

		/// <summary>
		/// Virtualization.
		/// </summary>
		public bool Virtualization
		{
			get
			{
				return virtualization;
			}

			set
			{
				if (virtualization != value)
				{
					virtualization = value;
					UpdateView();
				}
			}
		}

		/// <summary>
		/// ListView display type.
		/// </summary>
		[SerializeField]
		protected ListViewType listType = ListViewType.ListViewWithFixedSize;

		/// <summary>
		/// ListView display type.
		/// </summary>
		public abstract ListViewType ListType
		{
			get;
			set;
		}

		/// <summary>
		/// If data source setted?
		/// </summary>
		protected bool DataSourceSetted;

		/// <summary>
		/// Is data source changed?
		/// </summary>
		protected bool IsDataSourceChanged;

		/// <summary>
		/// Destroy instances of the previous DefaultItem when replacing DefaultItem.
		/// </summary>
		[SerializeField]
		[Tooltip("Destroy instances of the previous DefaultItem when replacing DefaultItem.")]
		protected bool destroyDefaultItemsCache = true;

		/// <summary>
		/// Destroy instances of the previous DefaultItem when replacing DefaultItem.
		/// </summary>
		public abstract bool DestroyDefaultItemsCache
		{
			get;
			set;
		}

		[SerializeField]
		[FormerlySerializedAs("Sort")]
		bool sort = true;

		/// <summary>
		/// Sort items.
		/// Deprecated. Replaced with DataSource.Comparison.
		/// </summary>
		[Obsolete("Replaced with DataSource.Comparison.")]
		public virtual bool Sort
		{
			get
			{
				return sort;
			}

			set
			{
				sort = value;
				if (sort && isListViewCustomInited)
				{
					UpdateItems();
				}
			}
		}

		/// <summary>
		/// Disable ScrollRect if ListView is not interactable.
		/// </summary>
		[SerializeField]
		[Tooltip("Disable ScrollRect if not interactable.")]
		protected bool disableScrollRect = false;

		/// <summary>
		/// Disable ScrollRect if not interactable.
		/// </summary>
		public bool DisableScrollRect
		{
			get
			{
				return disableScrollRect;
			}

			set
			{
				if (disableScrollRect != value)
				{
					disableScrollRect = value;
					ToggleScrollRect();
				}
			}
		}

		/// <summary>
		/// The displayed indices.
		/// </summary>
		protected List<int> DisplayedIndices = new List<int>();

		/// <summary>
		/// Gets the first displayed index.
		/// </summary>
		/// <value>The first displayed index.</value>
		[Obsolete("Renamed to DisplayedIndexFirst.")]
		public int DisplayedIndicesFirst
		{
			get
			{
				return DisplayedIndices.Count > 0 ? DisplayedIndices[0] : -1;
			}
		}

		/// <summary>
		/// Gets the last displayed index.
		/// </summary>
		/// <value>The last displayed index.</value>
		[Obsolete("Renamed to DisplayedIndexLast.")]
		public int DisplayedIndicesLast
		{
			get
			{
				return DisplayedIndices.Count > 0 ? DisplayedIndices[DisplayedIndices.Count - 1] : -1;
			}
		}

		/// <summary>
		/// Gets the first displayed index.
		/// </summary>
		/// <value>The first displayed index.</value>
		public int DisplayedIndexFirst
		{
			get
			{
				return DisplayedIndices.Count > 0 ? DisplayedIndices[0] : -1;
			}
		}

		/// <summary>
		/// Gets the last displayed index.
		/// </summary>
		/// <value>The last displayed index.</value>
		public int DisplayedIndexLast
		{
			get
			{
				return DisplayedIndices.Count > 0 ? DisplayedIndices[DisplayedIndices.Count - 1] : -1;
			}
		}

		/// <summary>
		/// If enabled scroll limited to last item.
		/// </summary>
		[SerializeField]
		[Obsolete("Use ScrollRect.MovementType = Clamped instead.")]
		public bool LimitScrollValue = false;

		#region Coloring fields

		/// <summary>
		/// Allow items coloring.
		/// </summary>
		[SerializeField]
		protected bool allowColoring = true;

		/// <summary>
		/// Allow items coloring.
		/// </summary>
		public bool AllowColoring
		{
			get
			{
				return allowColoring;
			}

			set
			{
				if (allowColoring != value)
				{
					allowColoring = value;
					ComponentsColoring(true);
				}
			}
		}

		/// <summary>
		/// Default background color.
		/// </summary>
		[SerializeField]
		protected Color defaultBackgroundColor = Color.white;

		/// <summary>
		/// Default color.
		/// </summary>
		[SerializeField]
		protected Color defaultColor = Color.black;

		/// <summary>
		/// Default background color.
		/// </summary>
		public Color DefaultBackgroundColor
		{
			get
			{
				return defaultBackgroundColor;
			}

			set
			{
				defaultBackgroundColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// Default text color.
		/// </summary>
		public Color DefaultColor
		{
			get
			{
				return defaultColor;
			}

			set
			{
				defaultColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// Highlighted background color.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("HighlightedBackgroundColor")]
		protected Color highlightedBackgroundColor = new Color(203, 230, 244, 255);

		/// <summary>
		/// Color of background on pointer over.
		/// </summary>
		public Color HighlightedBackgroundColor
		{
			get
			{
				return highlightedBackgroundColor;
			}

			set
			{
				highlightedBackgroundColor = value;
				ComponentsHighlightedColoring();
			}
		}

		/// <summary>
		/// Color of text on pointer text.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("HighlightedColor")]
		protected Color highlightedColor = Color.black;

		/// <summary>
		/// Color of background on pointer over.
		/// </summary>
		public Color HighlightedColor
		{
			get
			{
				return highlightedColor;
			}

			set
			{
				highlightedColor = value;
				ComponentsHighlightedColoring();
			}
		}

		/// <summary>
		/// Selected background color.
		/// </summary>
		[SerializeField]
		protected Color selectedBackgroundColor = new Color(53, 83, 227, 255);

		/// <summary>
		/// Selected color.
		/// </summary>
		[SerializeField]
		protected Color selectedColor = Color.black;

		/// <summary>
		/// Background color of selected item.
		/// </summary>
		public Color SelectedBackgroundColor
		{
			get
			{
				return selectedBackgroundColor;
			}

			set
			{
				selectedBackgroundColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// Text color of selected item.
		/// </summary>
		public Color SelectedColor
		{
			get
			{
				return selectedColor;
			}

			set
			{
				selectedColor = value;
				ComponentsColoring(true);
			}
		}

		/// <summary>
		/// Highlight item under pointer.
		/// </summary>
		[SerializeField]
		public bool FixHighlightItemUnderPointer = true;

		/// <summary>
		/// How long a color transition should take.
		/// </summary>
		[SerializeField]
		public float FadeDuration = 0f;
		#endregion

		/// <summary>
		/// The ScrollRect.
		/// </summary>
		[SerializeField]
		protected ScrollRect scrollRect;

		/// <summary>
		/// Gets or sets the ScrollRect.
		/// </summary>
		/// <value>The ScrollRect.</value>
		public ScrollRect ScrollRect
		{
			get
			{
				return scrollRect;
			}

			set
			{
				SetScrollRect(value);
			}
		}

		/// <summary>
		/// The size of the ScrollRect.
		/// </summary>
		protected Vector2 ScrollRectSize;

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		protected ListViewDirection direction = ListViewDirection.Vertical;

		/// <summary>
		/// Set content size fitter settings?
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("_setContentSizeFitter")]
		protected bool setContentSizeFitter = true;

		/// <summary>
		/// The set ContentSizeFitter parameters according direction.
		/// </summary>
		public bool SetContentSizeFitter
		{
			get
			{
				return setContentSizeFitter;
			}

			set
			{
				setContentSizeFitter = value;

				UpdateLayoutBridgeContentSizeFitter();
			}
		}

		/// <summary>
		/// Update LayoutBridge ContentSizeFitter.
		/// </summary>
		protected abstract void UpdateLayoutBridgeContentSizeFitter();

		/// <summary>
		/// Gets or sets the direction.
		/// </summary>
		/// <value>The direction.</value>
		public ListViewDirection Direction
		{
			get
			{
				return direction;
			}

			set
			{
				SetDirection(value);
			}
		}

		/// <summary>
		/// The layout.
		/// </summary>
		protected EasyLayout layout;

		/// <summary>
		/// Gets the layout.
		/// </summary>
		/// <value>The layout.</value>
		public EasyLayout Layout
		{
			get
			{
				if (layout == null)
				{
					layout = Container.GetComponent<EasyLayout>();
				}

				return layout;
			}
		}

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected ILayoutBridge layoutBridge;

		/// <summary>
		/// LayoutBridge.
		/// </summary>
		protected abstract ILayoutBridge LayoutBridge
		{
			get;
		}

		/// <summary>
		/// Require EasyLayout.
		/// </summary>
		protected virtual bool RequireEasyLayout
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Scroll use unscaled time.
		/// </summary>
		[SerializeField]
		public bool ScrollUnscaledTime = true;

		/// <summary>
		/// Scroll movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		public AnimationCurve ScrollMovement = AnimationCurve.EaseInOut(0, 0, 0.25f, 1);

		/// <summary>
		/// The scroll coroutine.
		/// </summary>
		protected IEnumerator ScrollCoroutine;

		/// <summary>
		/// The main thread.
		/// </summary>
		protected Thread MainThread;

		/// <summary>
		/// Gets a value indicating whether this instance is executed in main thread.
		/// </summary>
		/// <value><c>true</c> if this instance is executed in main thread; otherwise, <c>false</c>.</value>
		protected bool IsMainThread
		{
			get
			{
				return MainThread != null && MainThread.Equals(Thread.CurrentThread);
			}
		}

		/// <summary>
		/// Is DefaultItem implements IViewData{TItem}.
		/// </summary>
		protected bool CanSetData;

		/// <summary>
		/// Center the list items if all items visible.
		/// </summary>
		[SerializeField]
		[Tooltip("Center the list items if all items visible.")]
		protected bool centerTheItems;

		/// <summary>
		/// Center the list items if all items visible.
		/// </summary>
		public virtual bool CenterTheItems
		{
			get
			{
				return centerTheItems;
			}

			set
			{
				centerTheItems = value;
				UpdateView();
			}
		}

		/// <summary>
		/// List should be looped.
		/// </summary>
		[SerializeField]
		protected bool loopedList = false;

		/// <summary>
		/// List can be looped.
		/// </summary>
		/// <value><c>true</c> if list can be looped; otherwise, <c>false</c>.</value>
		public virtual bool LoopedList
		{
			get
			{
				return loopedList;
			}

			set
			{
				loopedList = value;
			}
		}

		/// <summary>
		/// Precalculate item sizes.
		/// Disabling this option increase performance with huge lists of items with variable sizes and decrease scroll precision.
		/// </summary>
		[SerializeField]
		public bool PrecalculateItemSizes = true;

		/// <summary>
		/// Header.
		/// </summary>
		[SerializeField]
		protected TableHeader header;

		/// <summary>
		/// Header.
		/// </summary>
		public TableHeader Header
		{
			get
			{
				return header;
			}

			set
			{
				if (header != value)
				{
					if (header != null)
					{
						header.List = null;
					}

					header = value;

					if (header != null)
					{
						header.List = this;

						if (isListViewCustomInited)
						{
							header.Refresh();
						}
					}
				}
			}
		}

		/// <summary>
		/// Maximal count of the visible items.
		/// </summary>
		public abstract int MaxVisibleItems
		{
			get;
		}

		/// <summary>
		/// The size of the DefaultItem.
		/// </summary>
		protected Vector2 ItemSize;

		/// <summary>
		/// Is ListViewCustom inited?
		/// </summary>
		[NonSerialized]
		protected bool isListViewCustomInited = false;

		#region ComponentsHighlightedColoring

		readonly List<RaycastResult> raycastResults = new List<RaycastResult>();

		readonly List<int> highlightedIndices = new List<int>(2);

		readonly List<int> prevHighlightedIndices = new List<int>(2);

		/// <summary>
		/// Highlighted indices.
		/// </summary>
		public ReadOnlyCollection<int> HighlightedIndices
		{
			get
			{
				FindHighlightedIndices();

				return highlightedIndices.AsReadOnly();
			}
		}

		/// <summary>
		/// Find highlighted indices.
		/// </summary>
		protected virtual void FindHighlightedIndices()
		{
			prevHighlightedIndices.Clear();
			prevHighlightedIndices.AddRange(highlightedIndices);
			highlightedIndices.Clear();

			if (EventSystem.current != null)
			{
				var item_under_navigation = FindItemInHierarchy(EventSystem.current.currentSelectedGameObject);
				if (item_under_navigation != null)
				{
					// temporary disabled, probably unnecessary
					// highlightedIndices.Add(item_under_navigation.Index);
				}
			}

			var item_under_pointer = FindItemUnderPointer();
			if ((item_under_pointer != null) && (!highlightedIndices.Contains(item_under_pointer.Index)))
			{
				highlightedIndices.Add(item_under_pointer.Index);
			}
		}

		/// <summary>
		/// Apply changed highlighted colors.
		/// </summary>
		protected virtual void ComponentsHighlightedColoring()
		{
			if (!isListViewCustomInited)
			{
				return;
			}

			if (!FixHighlightItemUnderPointer)
			{
				return;
			}

			FindHighlightedIndices();

			foreach (var index in prevHighlightedIndices)
			{
				if (highlightedIndices.Contains(index))
				{
					continue;
				}

				var item = GetComponent(index);
				if (item == null)
				{
					continue;
				}

				if (allowColoring)
				{
					Coloring(item);
				}

				if (IsSelected(index))
				{
					item.StateSelected();
				}
				else
				{
					item.StateDefault();
				}
			}

			foreach (var index in highlightedIndices)
			{
				var item = GetComponent(index);
				if (item == null)
				{
					continue;
				}

				if (allowColoring)
				{
					HighlightColoring(item);
				}

				item.StateHighlighted();
			}
		}

		PointerEventData ItemUnderPointerEventData;
		EventSystem ItemUnderPointerEventSystem;

		/// <summary>
		/// Find item under pointer.
		/// </summary>
		/// <returns>Item.</returns>
		protected ListViewItem FindItemUnderPointer()
		{
			if (EventSystem.current != null)
			{
				var es = EventSystem.current;
				if ((ItemUnderPointerEventData == null) || (ItemUnderPointerEventSystem != es))
				{
					ItemUnderPointerEventSystem = es;
					ItemUnderPointerEventData = new PointerEventData(es);
				}

				ItemUnderPointerEventData.position = CompatibilityInput.MousePosition;

				EventSystem.current.RaycastAll(ItemUnderPointerEventData, raycastResults);
			}

			foreach (var raycastResult in raycastResults)
			{
				if (!raycastResult.isValid)
				{
					continue;
				}

				var item = raycastResult.gameObject.GetComponent<ListViewItem>();
				if ((item != null) && (item.Owner != null) && (item.Owner.GetInstanceID() == GetInstanceID()) && Items.Contains(item))
				{
					return item;
				}
			}

			raycastResults.Clear();

			return null;
		}

		/// <summary>
		/// Find item in hierarchy.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		/// <returns>Item.</returns>
		protected virtual ListViewItem FindItemInHierarchy(GameObject go)
		{
			if (go == null)
			{
				return null;
			}

			var current_transform = go.transform;
			if (!current_transform.IsChildOf(Container))
			{
				return null;
			}

			var container_id = Container.GetInstanceID();
			while ((current_transform.parent != null) && (current_transform.parent.GetInstanceID() != container_id))
			{
				current_transform = current_transform.parent;
			}

			return (current_transform.parent == null) ? null : current_transform.GetComponent<ListViewItem>();
		}
		#endregion

		/// <inheritdoc/>
		protected override void UpdateComponents<TItemBase>(List<TItemBase> newItems)
		{
			items.Clear();
			#if CSHARP_7_OR_LATER
			items.AddRange(newItems);
			#else
			foreach (var item in newItems)
			{
				items.Add(item);
			}
			#endif
		}

		/// <summary>
		/// Toggle ScrollRect state.
		/// </summary>
		protected void ToggleScrollRect()
		{
			if (ScrollRect != null)
			{
				ScrollRect.enabled = !DisableScrollRect || IsInteractable();
			}
		}

		/// <summary>
		/// What to do when widget became interactable.
		/// </summary>
		protected override void OnInteractableEnabled()
		{
			ToggleScrollRect();
		}

		/// <summary>
		/// What to do when widget became not interactable.
		/// </summary>
		protected override void OnInteractableDisabled()
		{
			ToggleScrollRect();
		}

		/// <summary>
		/// Sets the direction.
		/// </summary>
		/// <param name="newDirection">New direction.</param>
		/// <param name="updateView">Update view.</param>
		protected abstract void SetDirection(ListViewDirection newDirection, bool updateView = true);

		/// <summary>
		/// Updates the view.
		/// </summary>
		public abstract void UpdateView();

		/// <summary>
		/// Set ScrollRect.
		/// </summary>
		/// <param name="newScrollRect">New ScrollRect.</param>
		protected abstract void SetScrollRect(ScrollRect newScrollRect);

		/// <summary>
		/// Update ScrollRect size.
		/// </summary>
		protected void UpdateScrollRectSize()
		{
			if (scrollRect == null)
			{
				return;
			}

			ScrollRectSize = (scrollRect.transform as RectTransform).rect.size;
			ScrollRectSize.x = float.IsNaN(ScrollRectSize.x) ? 1f : Mathf.Max(ScrollRectSize.x, 1f);
			ScrollRectSize.y = float.IsNaN(ScrollRectSize.y) ? 1f : Mathf.Max(ScrollRectSize.y, 1f);
		}

		/// <summary>
		/// Get secondary scroll position (for the cross direction).
		/// </summary>
		/// <param name="index">Index.</param>
		/// <returns>Secondary scroll position.</returns>
		protected virtual float GetScrollPositionSecondary(int index)
		{
			var current_position = ContainerAnchoredPosition;

			return IsHorizontal() ? current_position.y : current_position.x;
		}

		/// <summary>
		/// Check currently selected GameObject.
		/// </summary>
		/// <param name="position">Scroll position.</param>
		protected void SelectableCheck(Vector2 position)
		{
			SelectableCheck();
		}

		/// <summary>
		/// Select new selectable GameObject if needed.
		/// </summary>
		/// <param name="position">Scroll position.</param>
		protected void SelectableSet(Vector2 position)
		{
			SelectableSet();
		}

		/// <summary>
		/// Check currently selected GameObject.
		/// </summary>
		protected abstract void SelectableCheck();

		/// <summary>
		/// Select new selectable GameObject if needed.
		/// </summary>
		protected abstract void SelectableSet();

		/// <summary>
		/// Gets the layout margin.
		/// </summary>
		/// <returns>The layout margin.</returns>
		public override Vector4 GetLayoutMargin()
		{
			return LayoutBridge.GetMarginSize();
		}

		/// <summary>
		/// Gets the spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacing()
		{
			return LayoutBridge.GetSpacing();
		}

		/// <summary>
		/// Gets the horizontal spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacingX()
		{
			return LayoutBridge.GetSpacingX();
		}

		/// <summary>
		/// Gets the vertical spacing between items.
		/// </summary>
		/// <returns>The item spacing.</returns>
		public override float GetItemSpacingY()
		{
			return LayoutBridge.GetSpacingY();
		}

		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public override bool IsHorizontal()
		{
			return direction == ListViewDirection.Horizontal;
		}

		#region AutoScroll

		/// <summary>
		/// Auto scroll area.
		/// </summary>
		[SerializeField]
		[Tooltip("Distance from the ScrollRect borders where auto scroll enabled.")]
		public float AutoScrollArea = 40f;

		/// <summary>
		/// Auto scroll speed.
		/// </summary>
		[SerializeField]
		public float AutoScrollSpeed = 200f;

		/// <summary>
		/// Scroll coroutine.
		/// </summary>
		protected Coroutine AutoScrollCoroutine;

		/// <summary>
		/// Auto scroll direction.
		/// </summary>
		protected int AutoScrollDirection;

		/// <summary>
		/// Auto scroll EventData for callback.
		/// </summary>
		protected PointerEventData AutoScrollEventData;

		/// <summary>
		/// Auto scroll callback.
		/// </summary>
		protected Action<PointerEventData> AutoScrollCallback;

		/// <summary>
		/// Stop auto-scroll.
		/// </summary>
		public virtual void AutoScrollStop()
		{
			if (AutoScrollCoroutine == null)
			{
				return;
			}

			AutoScrollDirection = 0;
			StopCoroutine(AutoScrollCoroutine);

			AutoScrollCoroutine = null;
			AutoScrollCallback = null;
			AutoScrollEventData = null;
		}

		/// <summary>
		/// Start scroll when pointer is near ScrollRect border.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="callback">Callback.</param>
		/// <returns>true if auto scroll started; otherwise false.</returns>
		public virtual bool AutoScrollStart(PointerEventData eventData, Action<PointerEventData> callback)
		{
			var target = ScrollRect.transform as RectTransform;

			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out point))
			{
				AutoScrollStop();
				return false;
			}

			var rect_start = target.rect;
			var rect_end = target.rect;

			if (IsHorizontal())
			{
				rect_start.width = AutoScrollArea;

				rect_end.position = new Vector2(rect_end.position.x + rect_end.width - AutoScrollArea, rect_end.position.y);
				rect_end.width = AutoScrollArea;
			}
			else
			{
				rect_start.position = new Vector2(rect_start.position.x, rect_start.position.y + rect_start.height - AutoScrollArea);
				rect_start.height = AutoScrollArea;

				rect_end.height = AutoScrollArea;
			}

			var new_direction = 0;
			if (rect_start.Contains(point))
			{
				new_direction = -1;
			}
			else if (rect_end.Contains(point))
			{
				new_direction = +1;
			}

			if (new_direction != AutoScrollDirection)
			{
				AutoScrollStop();

				if (new_direction != 0)
				{
					AutoScrollDirection = new_direction;
					AutoScrollEventData = eventData;
					AutoScrollCallback = callback;
					AutoScrollCoroutine = StartCoroutine(AutoScroll());
				}
			}

			return AutoScrollDirection != 0;
		}

		/// <summary>
		/// Auto scroll.
		/// </summary>
		/// <returns>Coroutine.</returns>
		protected abstract IEnumerator AutoScroll();
		#endregion

		/// <summary>
		/// Start ListView in editor mode.
		/// </summary>
		public virtual void InEditorStart()
		{
#if UNITY_EDITOR
			Init();
#endif
		}

		/// <summary>
		/// Stop ListView in editor mode.
		/// </summary>
		public virtual void InEditorStop()
		{
#if UNITY_EDITOR
			OnDestroy();
			isListViewCustomInited = false;
#endif
		}

		/// <summary>
		/// Restart ListView in editor mode.
		/// </summary>
		public virtual void InEditorRestart()
		{
#if UNITY_EDITOR
			InEditorStop();
			InEditorStart();
#endif
		}
	}
}