namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;

	/// <summary>
	/// Base class for TracksView.
	/// </summary>
	[SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1619:GenericTypeParametersMustBeDocumentedPartialClass", Justification = "Reviewed.")]
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
		/// <summary>
		/// Class needed to have consistent connection between data and view.
		/// </summary>
		protected class TracksDataViews
		{
			/// <summary>
			/// Visible item info.
			/// </summary>
			protected class VisibleItem
			{
				/// <summary>
				/// Track.
				/// </summary>
				public Track<TData, TPoint> Track;

				/// <summary>
				/// Data.
				/// </summary>
				public TData Data;

				/// <summary>
				/// Height.
				/// </summary>
				public float Height;

				/// <summary>
				/// View.
				/// </summary>
				public TDataView View;

				/// <summary>
				/// Set view data.
				/// </summary>
				/// <param name="owner">Owner.</param>
				public void SetData(TracksViewBase<TData, TPoint> owner)
				{
					View.Owner = owner;
					View.SetData(Track, Data, Height);
					View.transform.SetAsLastSibling();
				}
			}

			/// <summary>
			/// Cache.
			/// </summary>
			protected Stack<VisibleItem> Cache = new Stack<VisibleItem>();

			/// <summary>
			/// Data.
			/// </summary>
			protected List<VisibleItem> Data = new List<VisibleItem>();

			/// <summary>
			/// Indices of the views to disable.
			/// </summary>
			protected List<int> DisableIndices = new List<int>();

			/// <summary>
			/// Views component pool.
			/// </summary>
			protected ListComponentPool<TDataView> Views;

			/// <summary>
			/// Owner.
			/// </summary>
			protected TracksViewBase<TData, TPoint> Owner;

			/// <summary>
			/// Initializes a new instance of the <see cref="TracksDataViews"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="views">Views.</param>
			public TracksDataViews(TracksViewBase<TData, TPoint> owner, ListComponentPool<TDataView> views)
			{
				Owner = owner;
				Views = views;
			}

			/// <summary>
			/// Clear.
			/// </summary>
			public void Clear()
			{
				foreach (var item in Data)
				{
					item.View = null;
					Cache.Push(item);
				}

				Data.Clear();

				DisableIndices.Clear();
			}

			/// <summary>
			/// Add data.
			/// </summary>
			/// <param name="track">Track.</param>
			/// <param name="data">Item.</param>
			/// <param name="height">Height.</param>
			public void Add(Track<TData, TPoint> track, TData data, float height)
			{
				var v_item = Cache.Count > 0 ? Cache.Pop() : new VisibleItem();
				v_item.Track = track;
				v_item.Data = data;
				v_item.Height = height;
				v_item.View = null;

				Data.Add(v_item);
			}

			/// <summary>
			/// Find item by data.
			/// </summary>
			/// <param name="data">Data.</param>
			/// <returns>Item or null.</returns>
			protected VisibleItem Find(TData data)
			{
				foreach (var visible in Data)
				{
					if ((visible.View == null) && (visible.Data == data))
					{
						return visible;
					}
				}

				return null;
			}

			/// <summary>
			/// Update view.
			/// </summary>
			public void UpdateView()
			{
				for (int i = 0; i < Views.Count; i++)
				{
					var view = Views[i];
					var visible = Find(view.Data);
					if (visible != null)
					{
						visible.View = view;
					}
					else
					{
						DisableIndices.Add(i);
					}
				}

				Views.Disable(DisableIndices);

				foreach (var v_item in Data)
				{
					if (v_item.View == null)
					{
						v_item.View = Views.GetInstance();
					}

					v_item.SetData(Owner);
				}
			}

			/// <summary>
			/// Update view positions.
			/// </summary>
			public void UpdateViewPositions()
			{
				foreach (var v_item in Data)
				{
					v_item.View.SetPosition();
				}
			}
		}
	}
}