namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using EasyLayoutNS;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// ScrollRect Paginator.
	/// </summary>
	public class ScrollRectPaginator : MonoBehaviour, IStylable, IUpgradeable
	{
		/// <summary>
		/// ScrollRect for pagination.
		/// </summary>
		[SerializeField]
		protected ScrollRect ScrollRect;

		/// <summary>
		/// DefaultPage template.
		/// </summary>
		[SerializeField]
		protected RectTransform DefaultPage;

		/// <summary>
		/// ScrollRectPage component of DefaultPage.
		/// </summary>
		protected ScrollRectPage SRDefaultPage;

		/// <summary>
		/// ActivePage.
		/// </summary>
		[SerializeField]
		protected RectTransform ActivePage;

		/// <summary>
		/// ScrollRectPage component of ActivePage.
		/// </summary>
		protected ScrollRectPage SRActivePage;

		/// <summary>
		/// The previous page.
		/// </summary>
		[SerializeField]
		protected RectTransform PrevPage;

		/// <summary>
		/// ScrollRectPage component of PrevPage.
		/// </summary>
		protected ScrollRectPage SRPrevPage;

		/// <summary>
		/// The next page.
		/// </summary>
		[SerializeField]
		protected RectTransform NextPage;

		/// <summary>
		/// ScrollRectPage component of NextPage.
		/// </summary>
		protected ScrollRectPage SRNextPage;

		/// <summary>
		/// The direction.
		/// </summary>
		[SerializeField]
		public PaginatorDirection Direction = PaginatorDirection.Auto;

		/// <summary>
		/// The type of the page size.
		/// </summary>
		[SerializeField]
		protected PageSizeType pageSizeType = PageSizeType.Auto;

		/// <summary>
		/// Space between pages.
		/// </summary>
		[SerializeField]
		protected float pageSpacing = 0f;

		/// <summary>
		/// Space between pages.
		/// </summary>
		public float PageSpacing
		{
			get
			{
				return pageSpacing;
			}

			set
			{
				pageSpacing = value;

				RecalculatePages();
			}
		}

		/// <summary>
		/// Minimal drag distance to fast scroll to next slide.
		/// </summary>
		[SerializeField]
		public float FastDragDistance = 30f;

		/// <summary>
		/// Max drag time to fast scroll to next slide.
		/// </summary>
		[SerializeField]
		public float FastDragTime = 0.5f;

		/// <summary>
		/// Gets or sets the type of the page size.
		/// </summary>
		/// <value>The type of the page size.</value>
		public virtual PageSizeType PageSizeType
		{
			get
			{
				return pageSizeType;
			}

			set
			{
				pageSizeType = value;
				RecalculatePages();
			}
		}

		/// <summary>
		/// The size of the page.
		/// </summary>
		[SerializeField]
		protected float pageSize;

		/// <summary>
		/// Gets or sets the size of the page.
		/// </summary>
		/// <value>The size of the page.</value>
		public virtual float PageSize
		{
			get
			{
				return pageSize;
			}

			set
			{
				pageSize = value;
				RecalculatePages();
			}
		}

		int pages;

		/// <summary>
		/// Gets or sets the pages count.
		/// </summary>
		/// <value>The pages.</value>
		public virtual int Pages
		{
			get
			{
				return pages;
			}

			protected set
			{
				pages = Mathf.Max(1, value);
				UpdatePageButtons();
			}
		}

		/// <summary>
		/// The current page number.
		/// </summary>
		[SerializeField]
		protected int currentPage;

		/// <summary>
		/// Gets or sets the current page number.
		/// </summary>
		/// <value>The current page.</value>
		public int CurrentPage
		{
			get
			{
				return currentPage;
			}

			set
			{
				GoToPage(value);
			}
		}

#pragma warning disable 0649
		[SerializeField]
		[FormerlySerializedAs("ForceScrollOnPage")]
		bool forceScrollOnPage;
#pragma warning restore 0649

		/// <summary>
		/// The force scroll position to page.
		/// </summary>
		[Obsolete("Replace with ForcedPosition.")]
		public bool ForceScrollOnPage
		{
			get
			{
				return ForcedPosition == PaginatorPagePosition.OnStart;
			}

			set
			{
				ForcedPosition = value ? PaginatorPagePosition.OnStart : PaginatorPagePosition.None;
			}
		}

		/// <summary>
		/// Scroll to specified page position after user drag or scroll.
		/// </summary>
		public PaginatorPagePosition ForcedPosition = PaginatorPagePosition.None;

		/// <summary>
		/// Use animation.
		/// </summary>
		[SerializeField]
		public bool Animation = true;

		/// <summary>
		/// Movement curve.
		/// </summary>
		[SerializeField]
		[Tooltip("Requirements: start value should be less than end value; Recommended start value = 0; end value = 1;")]
		[FormerlySerializedAs("Curve")]
		public AnimationCurve Movement = AnimationCurve.EaseInOut(0, 0, 1, 1);

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// OnPageSelect event.
		/// </summary>
		[SerializeField]
		public ScrollRectPageSelect OnPageSelect = new ScrollRectPageSelect();

		/// <summary>
		/// OnMovement event.
		/// Params:
		/// - page
		/// - relative distance between this page and next page in range 0..1
		/// </summary>
		[SerializeField]
		public PaginatorMovement OnMovement = new PaginatorMovement();

		/// <summary>
		/// The default pages.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ScrollRectPage> DefaultPages = new List<ScrollRectPage>();

		/// <summary>
		/// The default pages cache.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ScrollRectPage> DefaultPagesCache = new List<ScrollRectPage>();

		/// <summary>
		/// Change the last page size to full-page size.
		/// </summary>
		[SerializeField]
		protected bool lastPageFullSize = true;

		/// <summary>
		/// Change the last page size to full-page size.
		/// </summary>
		public bool LastPageFullSize
		{
			get
			{
				return lastPageFullSize;
			}

			set
			{
				lastPageFullSize = value;
				UpdateLastPageMargin();
			}
		}

		/// <summary>
		/// ScrollRect.content layout.
		/// </summary>
		protected EasyLayout Layout;

		/// <summary>
		/// Default margin value.
		/// </summary>
		protected float DefaultMargin;

		/// <summary>
		/// The current animation.
		/// </summary>
		protected IEnumerator currentAnimation;

		/// <summary>
		/// Is animation running?
		/// </summary>
		protected bool isAnimationRunning;

		/// <summary>
		/// Is dragging ScrollRect?
		/// </summary>
		protected bool isDragging;

		/// <summary>
		/// The cursor position at drag start.
		/// </summary>
		protected Vector2 CursorStartPosition;

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
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

			LayoutRebuilder.ForceRebuildLayoutImmediate(ScrollRect.content);

			var resizeListener = Utilities.GetOrAddComponent<ResizeListener>(ScrollRect);
			resizeListener.OnResizeNextFrame.AddListener(RecalculatePages);

			var contentResizeListener = Utilities.GetOrAddComponent<ResizeListener>(ScrollRect.content);
			contentResizeListener.OnResizeNextFrame.AddListener(RecalculatePages);

			var dragListener = Utilities.GetOrAddComponent<DragListener>(ScrollRect);
			dragListener.OnDragStartEvent.AddListener(OnScrollRectDragStart);
			dragListener.OnDragEvent.AddListener(OnScrollRectDrag);
			dragListener.OnDragEndEvent.AddListener(OnScrollRectDragEnd);

			ScrollRect.onValueChanged.AddListener(OnScrollRectValueChanged);

			var scroll_listener = Utilities.GetOrAddComponent<ScrollListener>(ScrollRect);
			scroll_listener.ScrollEvent.AddListener(ContainerScroll);

			if (DefaultPage != null)
			{
				SRDefaultPage = Utilities.GetOrAddComponent<ScrollRectPage>(DefaultPage);
				SRDefaultPage.gameObject.SetActive(false);
			}

			if (ActivePage != null)
			{
				SRActivePage = Utilities.GetOrAddComponent<ScrollRectPage>(ActivePage);
			}

			if (PrevPage != null)
			{
				SRPrevPage = Utilities.GetOrAddComponent<ScrollRectPage>(PrevPage);
				SRPrevPage.SetPage(0);
				SRPrevPage.OnPageSelect.AddListener(Prev);
			}

			if (NextPage != null)
			{
				SRNextPage = Utilities.GetOrAddComponent<ScrollRectPage>(NextPage);
				SRNextPage.OnPageSelect.AddListener(Next);
			}

			Layout = ScrollRect.content.GetComponent<EasyLayout>();
			if (Layout != null)
			{
				DefaultMargin = IsHorizontal() ? Layout.MarginInner.Right : Layout.MarginInner.Bottom;
			}

			RecalculatePages();
			GoToPage(currentPage, true);
		}

		/// <summary>
		/// Get ScrollRect.
		/// </summary>
		/// <returns>ScrollRect</returns>
		public ScrollRect GetScrollRect()
		{
			return ScrollRect;
		}

		/// <summary>
		/// Invoke OnMovement event.
		/// </summary>
		protected virtual void MovementInvoke()
		{
			var position = Mathf.Round(GetCalculatedPosition());
			var page_size = GetPageSize();
			var prev_page = Mathf.FloorToInt(position / page_size);
			var ratio = (position - (page_size * prev_page)) / page_size;

			OnMovement.Invoke(prev_page, ratio);
		}

		/// <summary>
		/// Update layout margin to change the last page size to full-page size.
		/// </summary>
		protected virtual void UpdateLastPageMargin()
		{
			if (!LastPageFullSize)
			{
				return;
			}

			if (Layout == null)
			{
				Debug.LogWarning("LastPageFullSize requires EasyLayout component at the ScrollRect.content gameobject.");
				return;
			}

			var margin = LastPageFullSize ? GetLastPageMargin() : 0;

			var layout_margin = Layout.MarginInner;
			if (IsHorizontal())
			{
				layout_margin.Right = margin + DefaultMargin;
			}
			else
			{
				layout_margin.Bottom = margin + DefaultMargin;
			}

			Layout.MarginInner = layout_margin;
		}

		/// <summary>
		/// Get margin to change the last page size to full-page size.
		/// </summary>
		/// <returns>Margin.</returns>
		protected virtual float GetLastPageMargin()
		{
			var list_view_size = IsHorizontal() ? ScrollRect.content.rect.width : ScrollRect.content.rect.height;
			list_view_size -= IsHorizontal() ? Layout.MarginFullHorizontal : Layout.MarginFullVertical;

			var page_size = PageSpacing;
			if (PageSizeType == PageSizeType.Fixed)
			{
				page_size += PageSize;
			}
			else if (IsHorizontal())
			{
				page_size += (ScrollRect.transform as RectTransform).rect.width;
			}
			else
			{
				page_size += (ScrollRect.transform as RectTransform).rect.height;
			}

			var last_page_size = list_view_size % page_size;
			var margin = last_page_size > 0.1f
				? page_size - last_page_size - PageSpacing
				: 0f;

			return margin;
		}

		/// <summary>
		/// Handle ScrollRect scroll event.
		/// Open previous or next page depend of scroll direction.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void ContainerScroll(PointerEventData eventData)
		{
			if (ForcedPosition == PaginatorPagePosition.None)
			{
				UpdateObjects(GetPage());
				return;
			}

			var direction = (Mathf.Abs(eventData.scrollDelta.x) > Mathf.Abs(eventData.scrollDelta.y))
				? eventData.scrollDelta.x
				: eventData.scrollDelta.y;
			if (direction > 0)
			{
				Next();
			}
			else
			{
				Prev();
			}
		}

		/// <summary>
		/// Determines whether the specified pageComponent is null.
		/// </summary>
		/// <returns><c>true</c> if the specified pageComponent is null; otherwise, <c>false</c>.</returns>
		/// <param name="pageComponent">Page component.</param>
		protected static bool IsNullComponent(object pageComponent)
		{
			return pageComponent == null;
		}

		/// <summary>
		/// Updates the page buttons.
		/// </summary>
		protected virtual void UpdatePageButtons()
		{
			if (SRDefaultPage == null)
			{
				return;
			}

			DefaultPages.RemoveAll(IsNullComponent);

			if (DefaultPages.Count == Pages)
			{
				return;
			}

			if (DefaultPages.Count < Pages)
			{
				DefaultPagesCache.RemoveAll(IsNullComponent);

				for (int i = DefaultPages.Count; i < Pages; i++)
				{
					AddComponent(i);
				}

				if (SRNextPage != null)
				{
					SRNextPage.SetPage(Pages - 1);
					SRNextPage.transform.SetAsLastSibling();
				}
			}
			else
			{
				for (int i = Pages; i < DefaultPages.Count; i++)
				{
					DefaultPages[i].gameObject.SetActive(false);
					DefaultPagesCache.Add(DefaultPages[i]);
				}

				DefaultPages.RemoveRange(Pages, DefaultPages.Count - Pages);

				if (SRNextPage != null)
				{
					SRNextPage.SetPage(Pages - 1);
				}
			}

			LayoutUtilities.UpdateLayout(DefaultPage.parent.GetComponent<LayoutGroup>());
		}

		/// <summary>
		/// Adds page the component.
		/// </summary>
		/// <param name="page">Page.</param>
		protected virtual void AddComponent(int page)
		{
			ScrollRectPage component;
			if (DefaultPagesCache.Count > 0)
			{
				component = DefaultPagesCache[DefaultPagesCache.Count - 1];
				DefaultPagesCache.RemoveAt(DefaultPagesCache.Count - 1);
			}
			else
			{
				component = Compatibility.Instantiate(SRDefaultPage);
				component.transform.SetParent(SRDefaultPage.transform.parent, false);

				component.OnPageSelect.AddListener(GoToPage);

				Utilities.FixInstantiated(SRDefaultPage, component);
			}

			component.transform.SetAsLastSibling();
			component.gameObject.SetActive(true);
			component.SetPage(page);

			DefaultPages.Add(component);
		}

		/// <summary>
		/// Determines whether direction is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public virtual bool IsHorizontal()
		{
			if (Direction == PaginatorDirection.Horizontal)
			{
				return true;
			}

			if (Direction == PaginatorDirection.Vertical)
			{
				return false;
			}

			if (ScrollRect.horizontal)
			{
				return true;
			}

			if (ScrollRect.vertical)
			{
				return false;
			}

			var rect = ScrollRect.content.rect;

			return rect.width >= rect.height;
		}

		/// <summary>
		/// Gets the size of the page.
		/// </summary>
		/// <returns>The page size.</returns>
		protected virtual float GetPageSize()
		{
			if (PageSizeType == PageSizeType.Fixed)
			{
				return PageSize + PageSpacing;
			}

			if (IsHorizontal())
			{
				return (ScrollRect.transform as RectTransform).rect.width + PageSpacing;
			}
			else
			{
				return (ScrollRect.transform as RectTransform).rect.height + PageSpacing;
			}
		}

		/// <summary>
		/// Get ScrollRect size.
		/// </summary>
		/// <returns>Size.</returns>
		protected float ScrollRectSize()
		{
			var size = (ScrollRect.transform as RectTransform).rect.size;

			return IsHorizontal() ? size.x : size.y;
		}

		/// <summary>
		/// Go to next page.
		/// </summary>
		/// <param name="x">Unused.</param>
		void Next(int x)
		{
			Next();
		}

		/// <summary>
		/// Go to previous page.
		/// </summary>
		/// <param name="x">Unused.</param>
		void Prev(int x)
		{
			Prev();
		}

		/// <summary>
		/// Go to the next page.
		/// </summary>
		public virtual void Next()
		{
			if (CurrentPage == (Pages - 1))
			{
				return;
			}

			CurrentPage += 1;
		}

		/// <summary>
		/// Go to the previous page.
		/// </summary>
		public virtual void Prev()
		{
			if (CurrentPage == 0)
			{
				return;
			}

			CurrentPage -= 1;
		}

		/// <summary>
		/// Go to the first page.
		/// </summary>
		public virtual void FirstPage()
		{
			CurrentPage = 0;
		}

		/// <summary>
		/// Go to the last page.
		/// </summary>
		public virtual void LastPage()
		{
			if (Pages > 0)
			{
				return;
			}

			CurrentPage = Pages - 1;
		}

		/// <summary>
		/// Can be dragged?
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>true if drag allowed; otherwise, false.</returns>
		protected virtual bool IsValidDrag(PointerEventData eventData)
		{
			if (!gameObject.activeInHierarchy)
			{
				return false;
			}

			if (eventData.button != PointerEventData.InputButton.Left)
			{
				return false;
			}

			if (!ScrollRect.IsActive())
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Happens when ScrollRect OnDragStart event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnScrollRectDragStart(PointerEventData eventData)
		{
			if (!IsValidDrag(eventData))
			{
				return;
			}

			DragDelta = Vector2.zero;

			isDragging = true;

			CursorStartPosition = Vector2.zero;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(ScrollRect.transform as RectTransform, eventData.position, eventData.pressEventCamera, out CursorStartPosition);

			DragStarted = UtilitiesTime.GetTime(UnscaledTime);

			StopAnimation();
		}

		/// <summary>
		/// The drag delta.
		/// </summary>
		protected Vector2 DragDelta = Vector2.zero;

		/// <summary>
		/// Time when drag started.
		/// </summary>
		protected float DragStarted;

		/// <summary>
		/// Happens when ScrollRect OnDrag event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnScrollRectDrag(PointerEventData eventData)
		{
			MovementInvoke();

			if (!IsValidDrag(eventData))
			{
				return;
			}

			Vector2 current_cursor;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(ScrollRect.transform as RectTransform, eventData.position, eventData.pressEventCamera, out current_cursor))
			{
				return;
			}

			DragDelta = current_cursor - CursorStartPosition;
		}

		/// <summary>
		/// Happens when ScrollRect OnDragEnd event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnScrollRectDragEnd(PointerEventData eventData)
		{
			if (!IsValidDrag(eventData))
			{
				return;
			}

			isDragging = false;
			if (ForcedPosition != PaginatorPagePosition.None)
			{
				ScrollChanged();
			}
		}

		/// <summary>
		/// Happens when ScrollRect onValueChanged event occurs.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void OnScrollRectValueChanged(Vector2 value)
		{
			if (isAnimationRunning || !gameObject.activeInHierarchy || isDragging)
			{
				return;
			}

			if (ForcedPosition != PaginatorPagePosition.None)
			{
				ScrollChanged();
			}

			MovementInvoke();
		}

		/// <summary>
		/// Get current page.
		/// </summary>
		/// <returns>Page.</returns>
		protected virtual int GetPage()
		{
			var position = GetCalculatedPosition();
			var page = Mathf.RoundToInt(position / GetPageSize());
			if (page >= Pages)
			{
				page = Pages - 1;
			}

			return page;
		}

		/// <summary>
		/// Handle scroll changes.
		/// </summary>
		protected virtual void ScrollChanged()
		{
			if (!gameObject.activeInHierarchy)
			{
				return;
			}

			var distance = Mathf.Abs(IsHorizontal() ? DragDelta.x : DragDelta.y);
			var time = UtilitiesTime.GetTime(UnscaledTime) - DragStarted;

			var is_fast = (distance >= FastDragDistance) && (time <= FastDragTime);
			if (is_fast)
			{
				var direction = IsHorizontal() ? DragDelta.x : -DragDelta.y;
				DragDelta = Vector2.zero;

				if (direction == 0f)
				{
					return;
				}

				var page = direction < 0 ? CurrentPage + 1 : CurrentPage - 1;
				GoToPage(page, true);
			}
			else
			{
				GoToPage(GetPage(), true);

				DragDelta = Vector2.zero;
				DragStarted = 0f;
			}
		}

		/// <summary>
		/// Gets the size of the content.
		/// </summary>
		/// <returns>The content size.</returns>
		public virtual float GetContentSize()
		{
			return IsHorizontal() ? ScrollRect.content.rect.width : ScrollRect.content.rect.height;
		}

		/// <summary>
		/// Go to page without animation.
		/// </summary>
		/// <param name="page">Page.</param>
		public virtual void SetPage(int page)
		{
			page = Mathf.Clamp(page, 0, Pages - 1);

			StopAnimation();
			ScrollRect.StopMovement();

			SetPosition(Page2Position(page), IsHorizontal());

			UpdateObjects(page);

			currentPage = page;

			OnPageSelect.Invoke(currentPage);
		}

		/// <summary>
		/// Recalculate the pages count.
		/// </summary>
		protected virtual void RecalculatePages()
		{
			SetScrollRectMaxDrag();

			Pages = Mathf.Max(1, Mathf.CeilToInt(GetContentSize() / GetPageSize()));

			UpdateLastPageMargin();

			if (currentPage >= Pages)
			{
				GoToPage(Pages - 1);
			}
			else if (ForcedPosition != PaginatorPagePosition.None)
			{
				SetPage(CurrentPage);
			}
		}

		/// <summary>
		/// Set ScrollRect max drag value.
		/// </summary>
		protected virtual void SetScrollRectMaxDrag()
		{
			var scrollRectDrag = ScrollRect as ScrollRectRestrictedDrag;
			if (scrollRectDrag != null)
			{
				if (IsHorizontal())
				{
					scrollRectDrag.MaxDrag.x = GetPageSize();
					scrollRectDrag.MaxDrag.y = 0;
				}
				else
				{
					scrollRectDrag.MaxDrag.x = 0;
					scrollRectDrag.MaxDrag.y = GetPageSize();
				}
			}
		}

		/// <summary>
		/// Go to page.
		/// </summary>
		/// <param name="page">Page.</param>
		protected virtual void GoToPage(int page)
		{
			GoToPage(page, false);
		}

		/// <summary>
		/// Gets the page position.
		/// </summary>
		/// <returns>The page position.</returns>
		/// <param name="page">Page.</param>
		protected virtual float Page2Position(int page)
		{
			var result = page * GetPageSize();

			var delta = ScrollRectSize() - GetPageSize();
			switch (ForcedPosition)
			{
				case PaginatorPagePosition.None:
				case PaginatorPagePosition.OnStart:
					break;
				case PaginatorPagePosition.OnCenter:
					result += delta / 2f;
					break;
				case PaginatorPagePosition.OnEnd:
					result += delta;
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown forced position: {0}", EnumHelper<PaginatorPagePosition>.ToString(ForcedPosition)));
			}

			return result;
		}

		/// <summary>
		/// Stop animation.
		/// </summary>
		public virtual void StopAnimation()
		{
			if (!isAnimationRunning)
			{
				return;
			}

			isAnimationRunning = false;
			if (currentAnimation != null)
			{
				StopCoroutine(currentAnimation);
				currentAnimation = null;

				var position = Page2Position(currentPage);
				SetPosition(position, IsHorizontal());

				ScrollRectRestore();
			}
		}

		/// <summary>
		/// Go to page.
		/// </summary>
		/// <param name="page">Page.</param>
		/// <param name="forceUpdate">If set to <c>true</c> force update.</param>
		protected virtual void GoToPage(int page, bool forceUpdate)
		{
			page = Mathf.Clamp(page, 0, Pages - 1);
			if ((currentPage == page) && (!forceUpdate))
			{
				UpdateObjects(page);
				return;
			}

			StopAnimation();

			var end_position = Page2Position(page);

			if (GetPosition() == end_position)
			{
				UpdateObjects(page);
				return;
			}

			ScrollRect.StopMovement();

			if (Animation)
			{
				StartAnimation(end_position);
			}
			else
			{
				SetPosition(end_position, IsHorizontal());
			}

			UpdateObjects(page);

			currentPage = page;

			OnPageSelect.Invoke(currentPage);
		}

		/// <summary>
		/// Start animation.
		/// </summary>
		/// <param name="target">Target position.</param>
		protected virtual void StartAnimation(float target)
		{
			isAnimationRunning = true;
			ScrollRectStop();
			currentAnimation = RunAnimation(IsHorizontal(), GetPosition(), target, UnscaledTime);
			StartCoroutine(currentAnimation);
		}

		/// <summary>
		/// Saved ScrollRect.horizontal value.
		/// </summary>
		protected bool ScrollRectHorizontal;

		/// <summary>
		/// Saved ScrollRect.vertical value.
		/// </summary>
		protected bool ScrollRectVertical;

		/// <summary>
		/// Save ScrollRect state and disable scrolling.
		/// </summary>
		protected virtual void ScrollRectStop()
		{
			ScrollRectHorizontal = ScrollRect.horizontal;
			ScrollRectVertical = ScrollRect.vertical;

			ScrollRect.horizontal = false;
			ScrollRect.vertical = false;
		}

		/// <summary>
		/// Restore ScrollRect state.
		/// </summary>
		protected virtual void ScrollRectRestore()
		{
			ScrollRect.horizontal = ScrollRectHorizontal;
			ScrollRect.vertical = ScrollRectVertical;
		}

		/// <summary>
		/// Set ScrollRect content position.
		/// </summary>
		/// <returns>Position.</returns>
		public virtual float GetPosition()
		{
			return IsHorizontal() ? -ScrollRect.content.anchoredPosition.x : ScrollRect.content.anchoredPosition.y;
		}

		/// <summary>
		/// Get position with PaginatorPagePosition included.
		/// </summary>
		/// <returns>Position.</returns>
		protected virtual float GetCalculatedPosition()
		{
			var position = GetPosition();
			var delta = ScrollRectSize() - GetPageSize();
			switch (ForcedPosition)
			{
				case PaginatorPagePosition.None:
				case PaginatorPagePosition.OnStart:
					break;
				case PaginatorPagePosition.OnCenter:
					position -= (IsHorizontal() ? -delta : delta) / 2f;
					break;
				case PaginatorPagePosition.OnEnd:
					position -= IsHorizontal() ? -delta : delta;
					break;
				default:
					throw new NotSupportedException(string.Format("Unknown forced position: {0}", EnumHelper<PaginatorPagePosition>.ToString(ForcedPosition)));
			}

			return position;
		}

		/// <summary>
		/// Set ScrollRect content position.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="isHorizontal">Is horizontal direction.</param>
		protected virtual void SetPosition(float position, bool isHorizontal)
		{
			ScrollRect.content.anchoredPosition = isHorizontal
				? new Vector2(-position, ScrollRect.content.anchoredPosition.y)
				: new Vector2(ScrollRect.content.anchoredPosition.x, position);

			MovementInvoke();
		}

		/// <summary>
		/// Set ScrollRect content position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void SetPosition(float position)
		{
			SetPosition(position, IsHorizontal());
		}

		/// <summary>
		/// Update objects.
		/// </summary>
		/// <param name="page">Page.</param>
		protected virtual void UpdateObjects(int page)
		{
			if (page >= Pages)
			{
				page = Pages - 1;
			}

			if (SRDefaultPage != null)
			{
				foreach (var dp in DefaultPages)
				{
					dp.gameObject.SetActive(true);
				}

				DefaultPages[page].gameObject.SetActive(false);
				SRActivePage.SetPage(page);
				SRActivePage.transform.SetSiblingIndex(DefaultPages[page].transform.GetSiblingIndex());
			}

			if (SRPrevPage != null)
			{
				SRPrevPage.SetState(IsPrevAvailable(page));
			}

			if (SRNextPage != null)
			{
				SRNextPage.SetState(IsNextAvailable(page));
			}
		}

		/// <summary>
		/// Is previous page available?
		/// </summary>
		/// <param name="page">Current page.</param>
		/// <returns>true if previous page available; otherwise false.</returns>
		protected virtual bool IsPrevAvailable(int page)
		{
			return (page != 0) && (Pages > 0);
		}

		/// <summary>
		/// Is next page available?
		/// </summary>
		/// <param name="page">Current page.</param>
		/// <returns>true if next page available; otherwise false.</returns>
		protected virtual bool IsNextAvailable(int page)
		{
			return (page != (Pages - 1)) && (Pages > 0);
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

				SetPosition(position, isHorizontal);

				yield return null;
			}
			while (delta < animation_length);

			SetPosition(endPosition, isHorizontal);

			ScrollRectRestore();

			isAnimationRunning = false;
			currentAnimation = null;
		}

		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="page">Page.</param>
		protected virtual void RemoveCallback(ScrollRectPage page)
		{
			page.OnPageSelect.RemoveListener(GoToPage);
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			DefaultPages.RemoveAll(IsNullComponent);
			foreach (var p in DefaultPages)
			{
				RemoveCallback(p);
			}

			DefaultPagesCache.RemoveAll(IsNullComponent);
			foreach (var p in DefaultPagesCache)
			{
				RemoveCallback(p);
			}

			if (ScrollRect != null)
			{
				var scroll_listener = ScrollRect.GetComponent<ScrollListener>();
				if (scroll_listener != null)
				{
					scroll_listener.ScrollEvent.RemoveListener(ContainerScroll);
				}

				var dragListener = ScrollRect.GetComponent<DragListener>();
				if (dragListener != null)
				{
					dragListener.OnDragStartEvent.RemoveListener(OnScrollRectDragStart);
					dragListener.OnDragEvent.RemoveListener(OnScrollRectDrag);
					dragListener.OnDragEndEvent.RemoveListener(OnScrollRectDragEnd);
				}

				var resizeListener = ScrollRect.GetComponent<ResizeListener>();
				if (resizeListener != null)
				{
					resizeListener.OnResizeNextFrame.RemoveListener(RecalculatePages);
				}

				if (ScrollRect.content != null)
				{
					var contentResizeListener = ScrollRect.content.GetComponent<ResizeListener>();
					if (contentResizeListener != null)
					{
						contentResizeListener.OnResizeNextFrame.RemoveListener(RecalculatePages);
					}
				}

				ScrollRect.onValueChanged.RemoveListener(OnScrollRectValueChanged);
			}

			if (SRPrevPage != null)
			{
				SRPrevPage.OnPageSelect.RemoveListener(Prev);
			}

			if (SRNextPage != null)
			{
				SRNextPage.OnPageSelect.RemoveListener(Next);
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (DefaultPage != null)
			{
				style.Paginator.DefaultBackground.ApplyTo(DefaultPage.GetComponent<Image>());
				Utilities.GetOrAddComponent<ScrollRectPage>(DefaultPage).SetStyle(style.Paginator.DefaultText, style);
			}

			if (ActivePage != null)
			{
				style.Paginator.ActiveBackground.ApplyTo(ActivePage.GetComponent<Image>());
				Utilities.GetOrAddComponent<ScrollRectPage>(ActivePage).SetStyle(style.Paginator.ActiveText, style);
			}

			for (int i = 0; i < DefaultPages.Count; i++)
			{
				var page = DefaultPages[i];
				style.Paginator.DefaultBackground.ApplyTo(page.GetComponent<Image>());
				Utilities.GetOrAddComponent<ScrollRectPage>(page).SetStyle(style.Paginator.DefaultText, style);
			}

			for (int i = 0; i < DefaultPagesCache.Count; i++)
			{
				var page = DefaultPagesCache[i];
				style.Paginator.DefaultBackground.ApplyTo(page.GetComponent<Image>());
				Utilities.GetOrAddComponent<ScrollRectPage>(page).SetStyle(style.Paginator.DefaultText, style);
			}

			if (PrevPage != null)
			{
				style.Paginator.DefaultBackground.ApplyTo(PrevPage.GetComponent<Image>());

				style.Paginator.DefaultText.ApplyTo(PrevPage.transform.Find("Text"));
			}

			if (NextPage != null)
			{
				style.Paginator.DefaultBackground.ApplyTo(NextPage.GetComponent<Image>());

				style.Paginator.DefaultText.ApplyTo(NextPage.transform.Find("Text"));
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (DefaultPage != null)
			{
				style.Paginator.DefaultBackground.GetFrom(DefaultPage.GetComponent<Image>());
				Utilities.GetOrAddComponent<ScrollRectPage>(DefaultPage).GetStyle(style.Paginator.DefaultText, style);
			}

			if (ActivePage != null)
			{
				style.Paginator.ActiveBackground.GetFrom(ActivePage.GetComponent<Image>());
				Utilities.GetOrAddComponent<ScrollRectPage>(ActivePage).GetStyle(style.Paginator.ActiveText, style);
			}

			if (PrevPage != null)
			{
				style.Paginator.DefaultBackground.GetFrom(PrevPage.GetComponent<Image>());

				style.Paginator.DefaultText.GetFrom(PrevPage.transform.Find("Text"));
			}

			if (NextPage != null)
			{
				style.Paginator.DefaultBackground.GetFrom(NextPage.GetComponent<Image>());

				style.Paginator.DefaultText.GetFrom(NextPage.transform.Find("Text"));
			}

			return true;
		}
		#endregion

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif

		[SerializeField]
		[HideInInspector]
		int version = 0;

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public virtual void Upgrade()
		{
			if (version == 0)
			{
				if (forceScrollOnPage)
				{
					ForcedPosition = PaginatorPagePosition.OnStart;
				}

				version = 1;
			}
		}
	}
}