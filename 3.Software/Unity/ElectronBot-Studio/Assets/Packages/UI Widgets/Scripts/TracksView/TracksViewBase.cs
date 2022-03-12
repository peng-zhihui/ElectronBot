namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for TracksView.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	public abstract class TracksViewBase<TData, TPoint> : UIBehaviourConditional, IUpdatable
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
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

		[SerializeField]
		[HideInInspector]
		ObservableList<Track<TData, TPoint>> tracks = new ObservableList<Track<TData, TPoint>>();

		/// <summary>
		/// Tracks.
		/// </summary>
		public ObservableList<Track<TData, TPoint>> Tracks
		{
			get
			{
				Init();

				return tracks;
			}

			set
			{
				Init();

				if (tracks != value)
				{
					tracks.OnChange -= UpdateView;
					tracks = value;
					tracks.OnChange += UpdateView;

					UpdateView();
				}
			}
		}

		[SerializeField]
		ScrollRect trackDataView;

		/// <summary>
		/// Tracks data View.
		/// </summary>
		public ScrollRect TracksDataView
		{
			get
			{
				return trackDataView;
			}

			set
			{
				if (trackDataView != value)
				{
					DisableDataView(trackDataView);

					trackDataView = value;

					EnableDataView(trackDataView);

					UpdateView();
				}
			}
		}

		[SerializeField]
		ScrollRect tracksNamesView;

		/// <summary>
		/// Tracks header view.
		/// </summary>
		public ScrollRect TracksNamesView
		{
			get
			{
				return tracksNamesView;
			}

			set
			{
				if (tracksNamesView != value)
				{
					DisableTracksView(tracksNamesView);

					tracksNamesView = value;

					EnableTracksView(tracksNamesView);

					UpdateView();
				}
			}
		}

		[SerializeField]
		ScrollBlockBase pointNamesView;

		/// <summary>
		/// Points names view.
		/// </summary>
		public ScrollBlockBase PointsNamesView
		{
			get
			{
				return pointNamesView;
			}

			set
			{
				if (pointNamesView != value)
				{
					DisablePointsView(pointNamesView);

					pointNamesView = value;
					pointNamesView.Init();

					EnablePointsView(pointNamesView);

					ChangeDefaultItemResizable();

					UpdateView();
				}
			}
		}

		/// <summary>
		/// First visible point.
		/// </summary>
		public TPoint VisibleStart
		{
			get;
			protected set;
		}

		/// <summary>
		/// Last visible point.
		/// </summary>
		public TPoint VisibleEnd
		{
			get;
			protected set;
		}

		/// <summary>
		/// Value at center of the PointsNamesView.
		/// </summary>
		[SerializeField]
		protected TPoint valueAtCenter;

		/// <summary>
		/// Value at center of the PointsNamesView.
		/// </summary>
		public TPoint ValueAtCenter
		{
			get
			{
				return valueAtCenter;
			}

			set
			{
				if (valueAtCenter.CompareTo(value) != 0)
				{
					valueAtCenter = value;

					PointsNamesView.UpdateView();
					UpdateView();
				}
			}
		}

		/// <summary>
		/// Space between data.
		/// </summary>
		[SerializeField]
		public float ItemsSpacing = 5f;

		/// <summary>
		/// Space between tracks.
		/// </summary>
		[SerializeField]
		public float TracksSpacing;

		/// <summary>
		/// Allow to drag data outside of the TrackDataView.
		/// </summary>
		[Tooltip("Allow to drag data outside of the TrackDataView")]
		public bool AllowDragOutside = true;

		/// <summary>
		/// Set minimal order of the items.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("setMinimalOrder")]
		protected bool itemsToTop = true;

		/// <summary>
		/// Set minimal order of the items.
		/// </summary>
		public bool ItemsToTop
		{
			get
			{
				return itemsToTop;
			}

			set
			{
				if (itemsToTop != value)
				{
					itemsToTop = value;

					if (isInited)
					{
						Layout = null;
						UpdateView();
					}
				}
			}
		}

		/// <summary>
		/// Compact items layout.
		/// </summary>
		[SerializeField]
		protected bool compact = true;

		/// <summary>
		/// Compact items layout.
		/// </summary>
		public bool Compact
		{
			get
			{
				return compact;
			}

			set
			{
				if (compact != value)
				{
					compact = value;

					if (isInited)
					{
						Layout = null;
						UpdateView();
					}
				}
			}
		}

		/// <summary>
		/// Allow temporary intersection during drag: overlapped data will be moved to another line.
		/// </summary>
		[SerializeField]
		[Tooltip("Allow temporary intersection during drag.")]
		public bool AllowIntersection = true;

		/// <summary>
		/// DefaultItem size.
		/// </summary>
		public Vector2 DefaultItemSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// DataHeader size,
		/// </summary>
		[NonSerialized]
		protected Vector2 DataHeaderSize;

		/// <summary>
		/// Layout.
		/// </summary>
		protected TrackLayout<TData, TPoint> Layout;

		/// <summary>
		/// Start this instance.
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
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			tracks.OnChange += UpdateView;
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			DisableDataView(TracksDataView);
			DisableTracksView(TracksNamesView);
			DisablePointsView(PointsNamesView);

			if (tracks != null)
			{
				tracks.OnChange -= UpdateView;
				tracks = null;
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Get string representation of ValueAtCenter at specified distance.
		/// </summary>
		/// <param name="distance">Distance.</param>
		/// <returns>String representation of value at specified distance.</returns>
		protected abstract string Value2Text(int distance);

		/// <summary>
		/// Change value on specified delta.
		/// </summary>
		/// <param name="delta">Delta.</param>
		/// <returns>New value.</returns>
		protected abstract TPoint ChangeValue(int delta);

		/// <summary>
		/// Increase ValueAtCenter at 1.
		/// </summary>
		protected virtual void Increase()
		{
			ValueAtCenter = ChangeValue(+1);
		}

		/// <summary>
		/// Decrease ValueAtCenter at 1.
		/// </summary>
		protected virtual void Decrease()
		{
			ValueAtCenter = ChangeValue(-1);
		}

		/// <summary>
		/// Set track settings.
		/// </summary>
		/// <param name="track">Track.</param>
		protected virtual void SetTrackSettings(Track<TData, TPoint> track)
		{
			track.SeparateGroups = true;
			track.Layout = Layout;
			track.ItemsToTop = ItemsToTop;
		}

		/// <summary>
		/// Update views.
		/// </summary>
		protected virtual void UpdateView()
		{
			if (Layout == null)
			{
				Layout = GetLayout();
			}

			foreach (var track in Tracks)
			{
				SetTrackSettings(track);
			}

			var half = (PointsNamesView.Count / 2) + 1;

			VisibleStart = ChangeValue(-half);
			VisibleEnd = ChangeValue(half);

			UpdateDataView();
			UpdateTracksView();
		}

		/// <summary>
		/// Get layout for the tracks.
		/// </summary>
		/// <returns>Layout function.</returns>
		protected virtual TrackLayout<TData, TPoint> GetLayout()
		{
			if (ItemsToTop)
			{
				if (Compact)
				{
					return new TrackLayoutTopLineCompact<TData, TPoint>();
				}
				else
				{
					return new TrackLayoutTopLine<TData, TPoint>();
				}
			}
			else
			{
				if (Compact)
				{
					return new TrackLayoutAnyLineCompact<TData, TPoint>();
				}
				else
				{
					return new TrackLayoutAnyLine<TData, TPoint>();
				}
			}
		}

		/// <summary>
		/// Update view on scroll.
		/// </summary>
		protected virtual void UpdateViewScroll()
		{
			var half = (PointsNamesView.Count / 2) + 1;
			var new_start = ChangeValue(-half);
			var new_end = ChangeValue(half);

			var is_changed = (VisibleStart.CompareTo(new_start) != 0) || (VisibleEnd.CompareTo(new_end) != 0);
			if (is_changed)
			{
				VisibleStart = new_start;
				VisibleEnd = new_end;

				UpdateDataView();
			}
			else
			{
				UpdateDataViewPositions();
			}
		}

		/// <summary>
		/// Update data view.
		/// </summary>
		protected abstract void UpdateDataView();

		/// <summary>
		/// Update data view positions.
		/// </summary>
		protected abstract void UpdateDataViewPositions();

		/// <summary>
		/// Update tracks view.
		/// </summary>
		protected abstract void UpdateTracksView();

		/// <summary>
		/// Get track height.
		/// </summary>
		/// <param name="track">Track.</param>
		/// <returns>Height.</returns>
		protected virtual float TrackHeight(Track<TData, TPoint> track)
		{
			return (track.MaxItemsAtSamePoint * (DefaultItemSize.y + ItemsSpacing)) - ItemsSpacing;
		}

		/// <summary>
		/// Convert point to base value.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Base value.</returns>
		protected abstract float Point2Base(TPoint point);

		/// <summary>
		/// Convert base value to point.
		/// </summary>
		/// <param name="baseValue">Base value.</param>
		/// <returns>Point.</returns>
		protected abstract TPoint Base2Point(float baseValue);

		/// <summary>
		/// Convert point to position.
		/// </summary>
		/// <param name="point">Point.</param>
		/// <returns>Position.</returns>
		public virtual float Point2Position(TPoint point)
		{
			return (Point2Base(point) * DataHeaderSize.x) - PointsNamesView.DistanceToCenter - (DataHeaderSize.x / 2f);
		}

		/// <summary>
		/// Convert position to point.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <returns>Point.</returns>
		public virtual TPoint Position2Point(float position)
		{
			position += PointsNamesView.DistanceToCenter;
			position += DataHeaderSize.x / 2f;

			return Base2Point(position / DataHeaderSize.x);
		}

		/// <summary>
		/// Temporary list.
		/// Used by TrackIntersection() method.
		/// </summary>
		protected List<TData> TempList = new List<TData>();

		/// <summary>
		/// Check if target item has intersection with items in the track within range and order.
		/// </summary>
		/// <param name="track">Track.</param>
		/// <param name="start">Start point.</param>
		/// <param name="end">End item.</param>
		/// <param name="order">New order of the target item.</param>
		/// <param name="target">Target item. Will be ignored if presents in the items list.</param>
		/// <returns>true if any items has intersection; otherwise false.</returns>
		public virtual bool TrackIntersection(Track<TData, TPoint> track, TPoint start, TPoint end, int order, TData target)
		{
			var is_new = !track.Data.Contains(target);
			if ((!is_new) && (target.Order != order))
			{
				return false;
			}

			GetPossibleIntersections(track.Data, order, target, TempList);

			var result = ListIntersection(TempList, start, end, order, target);

			TempList.Clear();

			return result;
		}

		/// <summary>
		/// Get possible intersections with the target.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="order">Order.</param>
		/// <param name="target">Target.</param>
		/// <param name="output">List of the possible intersections,</param>
		protected virtual void GetPossibleIntersections(ObservableList<TData> items, int order, TData target, List<TData> output)
		{
			foreach (var item in items)
			{
				if ((item.Order == order) && !ReferenceEquals(item, target))
				{
					output.Add(item);
				}
			}
		}

		/// <summary>
		/// Check if target item has intersection with items within range and order.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="start">Start point.</param>
		/// <param name="end">End item.</param>
		/// <param name="order">New order of the target item.</param>
		/// <param name="target">Target item. Will be ignored if presents in the items list.</param>
		/// <returns>true if any items has intersection with specified points; otherwise false.</returns>
		protected virtual bool ListIntersection(List<TData> items, TPoint start, TPoint end, int order, TData target)
		{
			foreach (var item in items)
			{
				if (ItemIntersection(item, start, end))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Is item has intersection with specified range?
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="start">Start point.</param>
		/// <param name="end">End range.</param>
		/// <returns>true if item has intersection; otherwise false.</returns>
		protected bool ItemIntersection(TData item, TPoint start, TPoint end)
		{
			// item.Start in [start..end)
			var start_intersect = (item.StartPoint.CompareTo(start) >= 0)
				&& (item.StartPoint.CompareTo(end) < 0);

			// item.End in [start..end)
			var end_intersect = (item.EndPoint.CompareTo(start) > 0)
				&& (item.EndPoint.CompareTo(end) <= 0);

			// item.Start <= start and Item.end >= end
			var full_intersect = (item.StartPoint.CompareTo(start) <= 0)
				&& (item.EndPoint.CompareTo(end) > 0);

			return start_intersect || end_intersect || full_intersect;
		}

		/// <summary>
		/// Convert position based event data to point.
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="camera">Camera.</param>
		/// <returns>Point.</returns>
		public virtual TPoint Position2Point(Vector2 position, Camera camera)
		{
			Vector2 point;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(TracksDataView.transform as RectTransform, position, camera, out point);

			return Position2Point(point.x);
		}

		/// <summary>
		/// Get minimal width of the item.
		/// </summary>
		/// <returns>Minimal width.</returns>
		protected abstract float GetItemMinWidth();

		/// <summary>
		/// Get width of the point names header.
		/// </summary>
		/// <returns>Width of the point names header.</returns>
		protected virtual float GetPointHeaderWidth()
		{
			PointsNamesView.Init();
			return PointsNamesView.DefaultItemSize.x;
		}

		/// <summary>
		/// Change settings of DefaultItem.Resizable component.
		/// </summary>
		protected abstract void ChangeDefaultItemResizable();

		/// <summary>
		/// Process resize event.
		/// </summary>
		protected virtual void OnResize()
		{
			PointsNamesView.Init();
			DataHeaderSize = PointsNamesView.DefaultItemSize;

			UpdateView();
		}

		/// <summary>
		/// Enable data view.
		/// </summary>
		/// <param name="dataView">Data view.</param>
		protected virtual void EnableDataView(ScrollRect dataView)
		{
			if (dataView == null)
			{
				return;
			}

			dataView.content.pivot = new Vector2(0.5f, 1f);
			dataView.inertia = false;
			dataView.horizontal = true;
			dataView.vertical = true;

			var data_drag = Utilities.GetOrAddComponent<DragListener>(dataView);
			data_drag.OnInitializePotentialDragEvent.AddListener(OnDataDragInit);
			data_drag.OnDragStartEvent.AddListener(OnDataDragBegin);
			data_drag.OnDragEvent.AddListener(OnDataDrag);
			data_drag.OnDragEndEvent.AddListener(OnDataDragEnd);
			data_drag.OnScrollEvent.AddListener(OnDataScroll);

			var data_resize = Utilities.GetOrAddComponent<ResizeListener>(dataView);
			data_resize.OnResizeNextFrame.AddListener(OnResize);
		}

		/// <summary>
		/// Disable data view.
		/// </summary>
		/// <param name="dataView">Data view.</param>
		protected virtual void DisableDataView(ScrollRect dataView)
		{
			if (dataView == null)
			{
				return;
			}

			var data_drag = dataView.GetComponent<DragListener>();
			if (data_drag != null)
			{
				data_drag.OnInitializePotentialDragEvent.RemoveListener(OnDataDragInit);
				data_drag.OnDragStartEvent.RemoveListener(OnDataDragBegin);
				data_drag.OnDragEvent.RemoveListener(OnDataDrag);
				data_drag.OnDragEndEvent.RemoveListener(OnDataDragEnd);
				data_drag.OnScrollEvent.RemoveListener(OnDataScroll);
			}

			var data_resize = dataView.GetComponent<ResizeListener>();
			if (data_resize != null)
			{
				data_resize.OnResizeNextFrame.RemoveListener(OnResize);
			}
		}

		/// <summary>
		/// Enable tracks view.
		/// </summary>
		/// <param name="tracksView">Tracks view.</param>
		protected virtual void EnableTracksView(ScrollRect tracksView)
		{
			if (tracksView == null)
			{
				return;
			}

			tracksView.content.pivot = new Vector2(0.5f, 1f);
			tracksView.inertia = false;
			tracksView.horizontal = true;
			tracksView.vertical = true;

			var tracks_drag = Utilities.GetOrAddComponent<DragListener>(tracksView);
			tracks_drag.OnInitializePotentialDragEvent.AddListener(OnTracksDragInit);
			tracks_drag.OnDragStartEvent.AddListener(OnTracksDragBegin);
			tracks_drag.OnDragEvent.AddListener(OnTracksDrag);
			tracks_drag.OnDragEndEvent.AddListener(OnTracksDragEnd);
			tracks_drag.OnScrollEvent.AddListener(OnTracksScroll);
		}

		/// <summary>
		/// Disable tracks view.
		/// </summary>
		/// <param name="tracksView">Tracks view.</param>
		protected virtual void DisableTracksView(ScrollRect tracksView)
		{
			if (tracksView == null)
			{
				return;
			}

			var tracks_drag = tracksView.GetComponent<DragListener>();
			if (tracks_drag != null)
			{
				tracks_drag.OnInitializePotentialDragEvent.RemoveListener(OnTracksDragInit);
				tracks_drag.OnDragStartEvent.RemoveListener(OnTracksDragBegin);
				tracks_drag.OnDragEvent.RemoveListener(OnTracksDrag);
				tracks_drag.OnDragEndEvent.RemoveListener(OnTracksDragEnd);
				tracks_drag.OnScrollEvent.RemoveListener(OnTracksScroll);
			}
		}

		/// <summary>
		/// Enable points view.
		/// </summary>
		/// <param name="pointsView">Points view.</param>
		protected virtual void EnablePointsView(ScrollBlockBase pointsView)
		{
			if (pointsView == null)
			{
				return;
			}

			pointsView.AlwaysCenter = false;
			pointsView.Increase = Increase;
			pointsView.Decrease = Decrease;
			pointsView.Value = Value2Text;
			pointsView.UpdateView();

			DataHeaderSize = pointsView.DefaultItemSize;

			var points_drag = Utilities.GetOrAddComponent<DragListener>(pointsView);
			points_drag.OnInitializePotentialDragEvent.AddListener(OnPointsDragInit);
			points_drag.OnDragStartEvent.AddListener(OnPointsDragBegin);
			points_drag.OnDragEvent.AddListener(OnPointsDrag);
			points_drag.OnDragEndEvent.AddListener(OnPointsDragEnd);
			points_drag.OnScrollEvent.AddListener(OnPointsScroll);
		}

		/// <summary>
		/// Disable points view.
		/// </summary>
		/// <param name="pointsView">Points view.</param>
		protected virtual void DisablePointsView(ScrollBlockBase pointsView)
		{
			if (pointsView == null)
			{
				return;
			}

			pointsView.Increase = ScrollBlockBase.DoNothing;
			pointsView.Decrease = ScrollBlockBase.DoNothing;
			pointsView.Value = ScrollBlockBase.DefaultValue;

			var points_drag = pointsView.GetComponent<DragListener>();
			if (points_drag != null)
			{
				points_drag.OnInitializePotentialDragEvent.RemoveListener(OnPointsDragInit);
				points_drag.OnDragStartEvent.RemoveListener(OnPointsDragBegin);
				points_drag.OnDragEvent.RemoveListener(OnPointsDrag);
				points_drag.OnDragEndEvent.RemoveListener(OnPointsDragEnd);
				points_drag.OnScrollEvent.RemoveListener(OnPointsScroll);
			}
		}

		#region drag events

		/// <summary>
		/// Process DataView drag init event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnDataDragInit(PointerEventData eventData)
		{
			TracksNamesView.OnInitializePotentialDrag(eventData);
			PointsNamesView.OnInitializePotentialDrag(eventData);
		}

		/// <summary>
		/// Process DataView drag begin event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnDataDragBegin(PointerEventData eventData)
		{
			TracksNamesView.OnBeginDrag(eventData);
			PointsNamesView.OnBeginDrag(eventData);
		}

		/// <summary>
		/// Process DataView drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnDataDrag(PointerEventData eventData)
		{
			TracksNamesView.OnDrag(eventData);
			PointsNamesView.OnDrag(eventData);

			UpdateViewScroll();
		}

		/// <summary>
		/// Process DataView drag end event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnDataDragEnd(PointerEventData eventData)
		{
			TracksNamesView.OnEndDrag(eventData);
			PointsNamesView.OnEndDrag(eventData);
		}

		/// <summary>
		/// Process DataView scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnDataScroll(PointerEventData eventData)
		{
			TracksNamesView.OnScroll(eventData);
			PointsNamesView.OnScroll(eventData);

			UpdateViewScroll();
		}

		/// <summary>
		/// Process TracksHeadersView drag init event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnTracksDragInit(PointerEventData eventData)
		{
			TracksDataView.OnInitializePotentialDrag(eventData);
			PointsNamesView.OnInitializePotentialDrag(eventData);
		}

		/// <summary>
		/// Process TracksHeadersView drag begin event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnTracksDragBegin(PointerEventData eventData)
		{
			TracksDataView.OnBeginDrag(eventData);
			PointsNamesView.OnBeginDrag(eventData);
		}

		/// <summary>
		/// Process TracksHeadersView drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnTracksDrag(PointerEventData eventData)
		{
			TracksDataView.OnDrag(eventData);
			PointsNamesView.OnDrag(eventData);

			UpdateViewScroll();
		}

		/// <summary>
		/// Process TracksHeadersView drag end event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnTracksDragEnd(PointerEventData eventData)
		{
			TracksDataView.OnEndDrag(eventData);
			PointsNamesView.OnEndDrag(eventData);
		}

		/// <summary>
		/// Process TracksHeadersView scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnTracksScroll(PointerEventData eventData)
		{
			TracksDataView.OnScroll(eventData);
			PointsNamesView.OnScroll(eventData);

			UpdateViewScroll();
		}

		/// <summary>
		/// Process PointsNamesView drag init event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnPointsDragInit(PointerEventData eventData)
		{
			TracksDataView.OnInitializePotentialDrag(eventData);
			TracksNamesView.OnInitializePotentialDrag(eventData);
		}

		/// <summary>
		/// Process PointsNamesView drag begin event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnPointsDragBegin(PointerEventData eventData)
		{
			TracksDataView.OnBeginDrag(eventData);
			TracksNamesView.OnBeginDrag(eventData);
		}

		/// <summary>
		/// Process PointsNamesView drag event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnPointsDrag(PointerEventData eventData)
		{
			TracksDataView.OnDrag(eventData);
			TracksNamesView.OnDrag(eventData);

			UpdateViewScroll();
		}

		/// <summary>
		/// Process PointsNamesView drag end event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnPointsDragEnd(PointerEventData eventData)
		{
			TracksDataView.OnEndDrag(eventData);
			TracksNamesView.OnEndDrag(eventData);
		}

		/// <summary>
		/// Process PointsNamesView scroll event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnPointsScroll(PointerEventData eventData)
		{
			TracksDataView.OnScroll(eventData);
			TracksNamesView.OnScroll(eventData);

			UpdateViewScroll();
		}

		#endregion

		/// <summary>
		/// Open dialog to create track.
		/// </summary>
		public abstract void OpenCreateTrackDialog();

		/// <summary>
		/// Open dialog to edit track.
		/// </summary>
		/// <param name="track">Track.</param>
		public abstract void OpenEditTrackDialog(Track<TData, TPoint> track);

		/// <summary>
		/// Open dialog to create data.
		/// </summary>
		/// <param name="track">Track to created data.</param>
		/// <param name="startPoint">Start point for the data.</param>
		public abstract void OpenCreateTrackDataDialog(Track<TData, TPoint> track, TPoint startPoint);

		/// <summary>
		/// Open dialog to edit data.
		/// </summary>
		/// <param name="data">Data.</param>
		public abstract void OpenEditTrackDataDialog(TData data);

		/// <summary>
		/// Copy data from source to target.
		/// </summary>
		/// <param name="source">Source.</param>
		/// <param name="target">Target.</param>
		public abstract void DataCopy(TData source, TData target);

		#region Auto-Scroll

		/// <summary>
		/// Allow auto-scroll.
		/// </summary>
		[SerializeField]
		[FormerlySerializedAs("AllowAutoDrag")]
		public bool AllowAutoScroll = true;

		/// <summary>
		/// Distance from the border to start auto-scroll.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("AllowAutoScroll")]
		[FormerlySerializedAs("AutoDragBorderDistance")]
		protected float AutoScrollBorderDistance = 20f;

		/// <summary>
		/// Auto-scroll speed.
		/// </summary>
		[SerializeField]
		[EditorConditionBool("AllowAutoScroll")]
		[FormerlySerializedAs("AutoDragSpeed")]
		protected float AutoScrollSpeed = 3f;

		/// <summary>
		/// Is auto-scroll currently enabled?
		/// </summary>
		[NonSerialized]
		protected bool AutoScrollEnabled;

		/// <summary>
		/// Auto-scroll direction.
		/// </summary>
		[NonSerialized]
		protected Vector2 AutoScrollDirection;

		/// <summary>
		/// Action to run when auto-scroll executed.
		/// </summary>
		[NonSerialized]
		protected Action OnAutoScrollAction;

		/// <summary>
		/// Start auto-scroll.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="action">Action to call during auto-scroll.</param>
		public void StartAutoScroll(PointerEventData eventData, Action action)
		{
			if (!AllowAutoScroll)
			{
				return;
			}

			Vector2 point;
			var target = TracksDataView.transform as RectTransform;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(target, eventData.position, eventData.pressEventCamera, out point);
			var pivot = target.pivot;
			var size = target.rect.size;

			point.x += size.x * pivot.x;
			point.y -= size.y * (1f - pivot.y);
			point.y = Mathf.Abs(point.y);

			var allow = false;
			AutoScrollDirection = Vector2.zero;
			if (point.x < AutoScrollBorderDistance)
			{
				allow = true;
				AutoScrollDirection.x = +1f;
			}
			else if (point.x > (size.x - AutoScrollBorderDistance))
			{
				allow = true;
				AutoScrollDirection.x = -1f;
			}

			if (point.y < AutoScrollBorderDistance)
			{
				allow = true;
				AutoScrollDirection.y = -1f;
			}
			else if (point.y > (size.y - AutoScrollBorderDistance))
			{
				allow = true;
				AutoScrollDirection.y = +1f;
			}

			if (allow)
			{
				AutoScrollEnabled = true;
				OnAutoScrollAction = action;
			}
			else
			{
				StopAutoScroll();
			}
		}

		/// <summary>
		/// Stop auto-scroll.
		/// </summary>
		public void StopAutoScroll()
		{
			if (AutoScrollEnabled)
			{
				AutoScrollEnabled = false;
			}
		}

		/// <summary>
		/// Auto-scroll.
		/// </summary>
		protected void AutoScroll()
		{
			var scroll_y = AutoScrollDirection.y * AutoScrollSpeed;
			PointsNamesView.Padding += AutoScrollDirection.x * AutoScrollSpeed;

			var scroll_height = (TracksDataView.transform as RectTransform).rect.height;

			var data_pos = TracksDataView.content.anchoredPosition;
			data_pos.y = Mathf.Clamp(data_pos.y + scroll_y, 0f, TracksDataView.content.rect.height - scroll_height);
			TracksDataView.content.anchoredPosition = data_pos;

			var names_pos = TracksNamesView.content.anchoredPosition;
			names_pos.y = Mathf.Clamp(names_pos.y + scroll_y, 0f, TracksNamesView.content.rect.height - scroll_height);
			TracksNamesView.content.anchoredPosition = names_pos;

			UpdateView();

			if (OnAutoScrollAction != null)
			{
				OnAutoScrollAction();
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();
			Updater.Add(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			Updater.Remove(this);
		}

		/// <summary>
		/// Update.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (AllowAutoScroll && AutoScrollEnabled && CompatibilityInput.IsPointerOverScreen)
			{
				AutoScroll();
			}
		}

		#endregion

		#region serialization support

		/// <summary>
		/// Serialized data.
		/// </summary>
		public class SerializedData
		{
			/// <summary>
			/// Tracks names.
			/// </summary>
			public List<string> TracksNames;

			/// <summary>
			/// Tracks data.
			/// </summary>
			public List<TData[]> TracksData;

			/// <summary>
			/// Is serialized data valid?
			/// </summary>
			public bool IsValid
			{
				get
				{
					if ((TracksNames == null) || (TracksData == null))
					{
						return false;
					}

					return TracksNames.Count == TracksData.Count;
				}
			}
		}

		/// <summary>
		/// Convert this instance to serialized data.
		/// </summary>
		/// <returns>Serialized data.</returns>
		public SerializedData AsSerialized()
		{
			var serialized = new SerializedData()
			{
				TracksNames = new List<string>(Tracks.Count),
				TracksData = new List<TData[]>(Tracks.Count),
			};

			serialized.TracksNames.Capacity = Tracks.Count;
			serialized.TracksData.Capacity = Tracks.Count;

			for (int i = 0; i < Tracks.Count; i++)
			{
				serialized.TracksNames.Add(Tracks[i].Name);
				serialized.TracksData.Add(Tracks[i].Data.ToArray());
			}

			return serialized;
		}

		/// <summary>
		/// Restore this instance from serialized data.
		/// </summary>
		/// <param name="serialized">Serialized data.</param>
		public void FromSerialized(SerializedData serialized)
		{
			if (!serialized.IsValid)
			{
				Debug.LogWarning("Invalid serialized data.", this);
				return;
			}

			Tracks.BeginUpdate();
			Tracks.Clear();

			for (int i = 0; i < serialized.TracksNames.Count; i++)
			{
				var track = new Track<TData, TPoint>()
				{
					Name = serialized.TracksNames[i],
				};

				track.Data.AddRange(serialized.TracksData[i]);

				Tracks.Add(track);
			}

			Tracks.EndUpdate();
		}

		#endregion
	}
}