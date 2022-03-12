namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for the data view.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	[RequireComponent(typeof(RectTransform))]
	public abstract class TrackDataViewBase<TData, TPoint> : MonoBehaviour, IMovableToCache
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Resizable component.
		/// </summary>
		[NonSerialized]
		protected Resizable Resizable;

		/// <summary>
		/// Show dialog to edit data on double click.
		/// </summary>
		[SerializeField]
		[Tooltip("Show dialog to edit data on double click.")]
		public bool AllowDialog = true;

		/// <summary>
		/// Owner.
		/// </summary>
		public TracksViewBase<TData, TPoint> Owner;

		/// <summary>
		/// Data.
		/// </summary>
		public TData Data
		{
			get;
			protected set;
		}

		RectTransform rt;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rt == null)
				{
					rt = transform as RectTransform;
				}

				return rt;
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
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

			AddListeners();
		}

		/// <summary>
		/// Add listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			var click = Utilities.GetOrAddComponent<ClickListener>(this);
			click.DoubleClickEvent.AddListener(OnDoubleClick);

			Resizable = GetComponent<Resizable>();
			if (Resizable != null)
			{
				var directions = Resizable.ResizeDirections;
				directions.Top = false;
				directions.TopLeft = false;
				directions.TopRight = false;
				directions.Bottom = false;
				directions.BottomLeft = false;
				directions.BottomRight = false;

				Resizable.ResizeDirections = directions;
				Resizable.OnEndResize.AddListener(OnEndResize);
			}
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			var click = GetComponent<ClickListener>();
			if (click != null)
			{
				click.DoubleClickEvent.RemoveListener(OnDoubleClick);
			}

			Resizable = GetComponent<Resizable>();
			if (Resizable != null)
			{
				Resizable.OnEndResize.RemoveListener(OnEndResize);
			}
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}

		/// <summary>
		/// Process end resize event.
		/// </summary>
		/// <param name="resizable">Resizable component.</param>
		protected virtual void OnEndResize(Resizable resizable)
		{
			var start = RectTransform.localPosition.x;
			var end = RectTransform.localPosition.x + RectTransform.rect.width;
			Data.ChangePoints(Owner.Position2Point(start), Owner.Position2Point(end));
		}

		/// <summary>
		/// Enable resizable component.
		/// </summary>
		public void EnableResizable()
		{
			if (Resizable != null)
			{
				Resizable.Interactable = true;
			}
		}

		/// <summary>
		/// Disable resizable component.
		/// </summary>
		public void DisableResizable()
		{
			if (Resizable != null)
			{
				Resizable.Interactable = false;
			}
		}

		/// <summary>
		/// Process double click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnDoubleClick(PointerEventData eventData)
		{
			if (!Owner.IsInteractable())
			{
				return;
			}

			if (!AllowDialog)
			{
				return;
			}

			Owner.OpenEditTrackDataDialog(Data);
		}

		/// <summary>
		/// Process move to cache event.
		/// </summary>
		public virtual void MovedToCache()
		{
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="track">Track.</param>
		/// <param name="data">Data.</param>
		/// <param name="baseVerticalPostion">Base vertical position.</param>
		public abstract void SetData(Track<TData, TPoint> track, TData data, float baseVerticalPostion);

		/// <summary>
		/// Set position.
		/// </summary>
		public virtual void SetPosition()
		{
			if (Owner == null)
			{
				return;
			}

			if (Data == null)
			{
				return;
			}

			var pos = RectTransform.localPosition;
			pos.x = Owner.Point2Position(Data.StartPoint);
			RectTransform.localPosition = pos;
		}

		/// <summary>
		/// Set size and position.
		/// </summary>
		/// <param name="baseVerticalPostion">Base vertical position.</param>
		public virtual void SetSizeAndPosition(float baseVerticalPostion)
		{
			if (Owner == null)
			{
				return;
			}

			if (Data == null)
			{
				return;
			}

			var start = Owner.Point2Position(Data.StartPoint);
			var width = Owner.Point2Position(Data.EndPoint) - start;
			var height = (Data.Order * (Owner.DefaultItemSize.y + Owner.ItemsSpacing)) + baseVerticalPostion;

			RectTransform.localPosition = new Vector2(start, -height);
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}
	}
}