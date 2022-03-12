namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for TrackView.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	public abstract class TrackViewBase<TData, TPoint> : MonoBehaviour, IMovableToCache
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
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
		/// Track.
		/// </summary>
		public Track<TData, TPoint> Track
		{
			get;
			protected set;
		}

		/// <summary>
		/// Show dialog to edit track or add data on double click.
		/// </summary>
		[SerializeField]
		[Tooltip("Show dialog to edit track or add data on double click.")]
		public bool AllowDialog = true;

		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public TextAdapter Name;

		/// <summary>
		/// Owner.
		/// </summary>
		public TracksViewBase<TData, TPoint> Owner;

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

			var click = Utilities.GetOrAddComponent<ClickListener>(this);
			click.DoubleClickEvent.AddListener(OnDoubleClick);
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

			Owner.OpenEditTrackDialog(Track);
		}

		/// <summary>
		/// Process destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			var click = GetComponent<ClickListener>();
			if (click != null)
			{
				click.DoubleClickEvent.RemoveListener(OnDoubleClick);
			}
		}

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="track">Track.</param>
		public virtual void SetData(Track<TData, TPoint> track)
		{
			Track = track;

			if (Name != null)
			{
				Name.Value = Track.Name;
			}
		}

		/// <summary>
		/// Set width.
		/// </summary>
		/// <param name="width">Width.</param>
		public virtual void SetWidth(float width)
		{
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
		}

		/// <summary>
		/// Set height.
		/// </summary>
		/// <param name="height">Height.</param>
		public virtual void SetHeight(float height)
		{
			RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);
		}

		/// <summary>
		/// Set size.
		/// </summary>
		/// <param name="size">Size.</param>
		public virtual void SetSize(Vector2 size)
		{
			SetWidth(size.x);
			SetHeight(size.y);
		}

		/// <summary>
		/// Set vertical position.
		/// </summary>
		/// <param name="position">Vertical position.</param>
		public virtual void SetVerticalPosition(float position)
		{
			var pos = RectTransform.localPosition;
			pos.y = -position;
			RectTransform.localPosition = pos;
		}

		/// <summary>
		/// Set horizontal position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void SetHorizontalPosition(float position)
		{
			var pos = RectTransform.localPosition;
			pos.x = position;
			RectTransform.localPosition = pos;
		}

		/// <summary>
		/// Set position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void SetPosition(Vector2 position)
		{
			position.y *= -1;
			RectTransform.localPosition = position;
		}

		/// <summary>
		/// Process move to cache.
		/// </summary>
		public virtual void MovedToCache()
		{
		}
	}
}