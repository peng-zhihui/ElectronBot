namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Base class for TracksView.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	/// <typeparam name="TDataView">DataView type.</typeparam>
	/// <typeparam name="TTrackView">TrackView type.</typeparam>
	/// <typeparam name="TTrackBackground">TrackBackground type.</typeparam>
	/// <typeparam name="TTrackDataDialog">TrackDataDialog type.</typeparam>
	/// <typeparam name="TTrackDataForm">TrackDataForm type.</typeparam>
	/// <typeparam name="TTrackDialog">TrackDialog type.</typeparam>
	/// <typeparam name="TTrackForm">TrackForm type.</typeparam>
	public abstract partial class TracksViewCustom<TData, TPoint, TDataView, TTrackView, TTrackBackground, TTrackDataDialog, TTrackDataForm, TTrackDialog, TTrackForm> : TracksViewBase<TData, TPoint>
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
		where TDataView : TrackDataViewBase<TData, TPoint>
		where TTrackView : TrackViewBase<TData, TPoint>
		where TTrackBackground : TrackBackgroundBase<TData, TPoint>
		where TTrackDataDialog : TrackDataDialogBase<TData, TPoint, TTrackDataForm>
		where TTrackDataForm : TrackDataFormBase<TData, TPoint>
		where TTrackDialog : TrackDialogBase<TData, TPoint, TTrackForm>
		where TTrackForm : TrackFormBase<TData, TPoint>
	{
		[SerializeField]
		TDataView defaultItem;

		/// <summary>
		/// Default item to display data.
		/// </summary>
		public TDataView DefaultItem
		{
			get
			{
				return defaultItem;
			}

			set
			{
				if (defaultItem != value)
				{
					defaultItem = value;

					var rt = defaultItem.transform as RectTransform;
					rt.pivot = new Vector2(0f, 1f);
					DefaultItemSize = rt.rect.size;

					DataViews.Template = defaultItem;

					ChangeDefaultItemResizable();

					UpdateView();
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		List<TDataView> dataViewsActive = new List<TDataView>();

		[SerializeField]
		[HideInInspector]
		List<TDataView> dataViewsCache = new List<TDataView>();

		ListComponentPool<TDataView> dataViews;

		/// <summary>
		/// Data views.
		/// </summary>
		protected ListComponentPool<TDataView> DataViews
		{
			get
			{
				if (dataViews == null)
				{
					dataViews = new ListComponentPool<TDataView>(DefaultItem, dataViewsActive, dataViewsCache, TracksDataView.content);
				}

				return dataViews;
			}
		}

		[SerializeField]
		TTrackView defaultTrackHeader;

		/// <summary>
		/// Default item to display tracks headers.
		/// </summary>
		public TTrackView DefaultTrackHeader
		{
			get
			{
				return defaultTrackHeader;
			}

			set
			{
				if (defaultTrackHeader != value)
				{
					defaultTrackHeader = value;

					var rt = defaultTrackHeader as RectTransform;
					rt.pivot = new Vector2(0f, 1f);

					TracksHeaders.Template = defaultTrackHeader;

					UpdateView();
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		List<TTrackView> trackHeadersActive = new List<TTrackView>();

		[SerializeField]
		[HideInInspector]
		List<TTrackView> trackHeadersCache = new List<TTrackView>();

		ListComponentPool<TTrackView> tracksHeaders;

		/// <summary>
		/// Tracks headers.
		/// </summary>
		protected ListComponentPool<TTrackView> TracksHeaders
		{
			get
			{
				if (tracksHeaders == null)
				{
					tracksHeaders = new ListComponentPool<TTrackView>(DefaultTrackHeader, trackHeadersActive, trackHeadersCache, TracksNamesView.content);
				}

				return tracksHeaders;
			}
		}

		[SerializeField]
		TTrackBackground defaultTrackBackground;

		/// <summary>
		/// Default item to display tracks backgrounds.
		/// </summary>
		public TTrackBackground DefaultTrackBackground
		{
			get
			{
				return defaultTrackBackground;
			}

			set
			{
				if (defaultTrackBackground != value)
				{
					defaultTrackBackground = value;

					var rt = defaultTrackBackground as RectTransform;
					rt.pivot = new Vector2(0f, 1f);

					TracksBackgrounds.Template = defaultTrackBackground;

					UpdateView();
				}
			}
		}

		[SerializeField]
		[HideInInspector]
		List<TTrackBackground> tracksBackgroundsActive = new List<TTrackBackground>();

		[SerializeField]
		[HideInInspector]
		List<TTrackBackground> tracksBackgroundsCache = new List<TTrackBackground>();

		ListComponentPool<TTrackBackground> tracksBackgrounds;

		/// <summary>
		/// Tracks backgrounds.
		/// </summary>
		protected ListComponentPool<TTrackBackground> TracksBackgrounds
		{
			get
			{
				if (tracksBackgrounds == null)
				{
					tracksBackgrounds = new ListComponentPool<TTrackBackground>(DefaultTrackBackground, tracksBackgroundsActive, tracksBackgroundsCache, TracksDataView.content);
				}

				return tracksBackgrounds;
			}
		}

		/// <summary>
		/// Dialog to add/edit data.
		/// </summary>
		[SerializeField]
		public TTrackDataDialog TrackDataDialog;

		/// <summary>
		/// Dialog to add/edit track.
		/// </summary>
		[SerializeField]
		public TTrackDialog TrackDialog;

		/// <summary>
		/// Views for the visible data.
		/// </summary>
		[NonSerialized]
		protected TracksDataViews VisibleDataViews;

		/// <summary>
		/// Visible items of the track.
		/// Reusable list.
		/// </summary>
		[NonSerialized]
		protected List<TData> TrackVisibleItems = new List<TData>();

		bool isCustomInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isCustomInited)
			{
				return;
			}

			isCustomInited = true;

			base.Init();

			var rt = DefaultItem.transform as RectTransform;
			rt.pivot = new Vector2(0f, 1f);
			DefaultItemSize = rt.rect.size;

			var t_rt = DefaultTrackHeader.transform as RectTransform;
			t_rt.pivot = new Vector2(0f, 1f);

			var tb_rt = DefaultTrackBackground.transform as RectTransform;
			tb_rt.pivot = new Vector2(0f, 1f);

			ChangeDefaultItemResizable();

			EnableDataView(TracksDataView);
			EnableTracksView(TracksNamesView);
			EnablePointsView(PointsNamesView);

			VisibleDataViews = new TracksDataViews(this, DataViews);

			UpdateView();
		}

		/// <summary>
		/// Change settings of DefaultItem.Resizable component.
		/// </summary>
		protected override void ChangeDefaultItemResizable()
		{
			var width = GetItemMinWidth();
			var size = new Vector2(width, DefaultItemSize.y);

			SetResizableSize(DefaultItem, size);

			for (int i = 0; i < dataViewsActive.Count; i++)
			{
				SetResizableSize(dataViewsActive[i], size);
			}

			for (int i = 0; i < dataViewsCache.Count; i++)
			{
				SetResizableSize(dataViewsCache[i], size);
			}
		}

		/// <summary>
		/// Set Resizable.MinSize for the specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		/// <param name="size">Size.</param>
		protected void SetResizableSize(TDataView component, Vector2 size)
		{
			var resizable = Utilities.GetOrAddComponent<Resizable>(component);
			resizable.MinSize = size;
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			dataViews = null;
			tracksHeaders = null;
			tracksBackgrounds = null;

			base.OnDestroy();
		}

		/// <summary>
		/// Update data view.
		/// </summary>
		protected override void UpdateDataView()
		{
			VisibleDataViews.Clear();

			var height = 0f;
			foreach (var track in Tracks)
			{
				track.VisibleRangeSet(VisibleStart, VisibleEnd);

				track.GetVisibleItems(TrackVisibleItems);
				foreach (var data in TrackVisibleItems)
				{
					VisibleDataViews.Add(track, data, height);
				}

				TrackVisibleItems.Clear();

				height += TrackHeight(track) + TracksSpacing;
			}

			if (Tracks.Count > 0)
			{
				height -= TracksSpacing;
			}

			VisibleDataViews.UpdateView();

			var view_height = Mathf.Max(height, (TracksDataView.transform as RectTransform).rect.height);
			TracksDataView.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, view_height);
			TracksNamesView.content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, view_height);
		}

		/// <summary>
		/// Update data view positions.
		/// </summary>
		protected override void UpdateDataViewPositions()
		{
			VisibleDataViews.UpdateViewPositions();
		}

		/// <summary>
		/// Update tracks view.
		/// </summary>
		protected override void UpdateTracksView()
		{
			TracksHeaders.Require(Tracks.Count);
			TracksBackgrounds.Require(Tracks.Count);

			var position = new Vector2(Point2Position(VisibleStart), 0f);
			var size = new Vector2(Point2Position(VisibleEnd) - position.x, 0f);
			for (int i = 0; i < Tracks.Count; i++)
			{
				var track = Tracks[i];
				size.y = TrackHeight(track);

				var header = TracksHeaders[i];
				header.Owner = this;
				header.SetData(track);
				header.SetHeight(size.y);
				header.SetVerticalPosition(position.y);

				var bg = TracksBackgrounds[i];
				bg.Owner = this;
				bg.SetData(track);
				bg.SetSize(size);
				bg.SetPosition(position);
				bg.transform.SetAsFirstSibling();

				position.y += size.y + TracksSpacing;
			}
		}

		/// <summary>
		/// Open dialog to create track.
		/// </summary>
		public override void OpenCreateTrackDialog()
		{
			TrackDialog.Create(Tracks.Add);
		}

		/// <summary>
		/// Open dialog to edit track.
		/// </summary>
		/// <param name="track">Track.</param>
		public override void OpenEditTrackDialog(Track<TData, TPoint> track)
		{
#if CSHARP_7_3_OR_NEWER
			void Action(Track<TData, TPoint> x)
#else
			Action<Track<TData, TPoint>> Action = (x) =>
#endif
			{
				x.CopyTo(track);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			TrackDialog.Edit(track, Action);
		}

		/// <summary>
		/// Open dialog to create data.
		/// </summary>
		/// <param name="track">Track to created data.</param>
		/// <param name="startPoint">Start point for the data.</param>
		public override void OpenCreateTrackDataDialog(Track<TData, TPoint> track, TPoint startPoint)
		{
			TrackDataDialog.Create(startPoint, track.Data.Add);
		}

		/// <summary>
		/// Open dialog to edit data.
		/// </summary>
		/// <param name="data">Data.</param>
		public override void OpenEditTrackDataDialog(TData data)
		{
#if CSHARP_7_3_OR_NEWER
			void Action(TData x)
#else
			Action<TData> Action = (x) =>
#endif
			{
				DataCopy(x, data);
			}
#if !CSHARP_7_3_OR_NEWER
			;
#endif

			TrackDataDialog.Edit(data, Action);
		}
	}
}