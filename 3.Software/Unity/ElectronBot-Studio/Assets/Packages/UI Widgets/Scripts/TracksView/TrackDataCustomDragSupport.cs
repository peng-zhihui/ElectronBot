namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for the data drag support.
	/// </summary>
	/// <typeparam name="TData">Data type.</typeparam>
	/// <typeparam name="TPoint">Point type.</typeparam>
	/// <typeparam name="TDataView">Data view type.</typeparam>
	public abstract class TrackDataCustomDragSupport<TData, TPoint, TDataView> : DragSupport<TData>
		where TDataView : TrackDataViewBase<TData, TPoint>
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Track.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		public Track<TData, TPoint> Track;

		/// <summary>
		/// Owner.
		/// </summary>
		public TracksViewBase<TData, TPoint> Owner;

		/// <summary>
		/// The drag info.
		/// </summary>
		[SerializeField]
		public TDataView DragInfo;

		/// <summary>
		/// DragInfo offset.
		/// </summary>
		[SerializeField]
		public Vector3 DragInfoOffset = new Vector3(-5, 5, 0);

		/// <summary>
		/// Current target track to drop data.
		/// </summary>
		protected Track<TData, TPoint> CurrentTargetTrack;

		/// <summary>
		/// Distance from left border of the item to pointer press position.
		/// Needed to correctly determine StartPoint.
		/// </summary>
		protected float DragPositionPadding;

		/// <inheritdoc/>
		protected override void Start()
		{
			base.Start();

			if (DragInfo != null)
			{
				if (DragInfo.gameObject.GetInstanceID() == gameObject.GetInstanceID())
				{
					DragInfo = null;
					Debug.LogWarning("DragInfo cannot be same gameobject as DragSupport.", this);
				}
				else
				{
					DragInfo.gameObject.SetActive(false);
				}
			}
		}

		/// <inheritdoc/>
		protected override void OnInitializePotentialDrag(PointerEventData eventData)
		{
			base.OnInitializePotentialDrag(eventData);

			CurrentTargetTrack = Track;
		}

		/// <inheritdoc/>
		protected override void FindCurrentTarget(PointerEventData eventData, List<RaycastResult> raycasts)
		{
			base.FindCurrentTarget(eventData, RaycastResults);

			var target = CurrentTarget as TrackBackgroundBase<TData, TPoint>;

			if ((target != null) && (target.Track != CurrentTargetTrack))
			{
				CurrentTargetTrack.Data.Remove(Data);
				CurrentTargetTrack = target.Track;
			}
		}

		/// <inheritdoc/>
		protected override void InitDrag(PointerEventData eventData)
		{
			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out point))
			{
				DragPositionPadding = 0f;
			}
			else
			{
				DragPositionPadding = point.x;
			}

			var component = GetComponent<TDataView>();
			Data = component.Data;
			Data.IsDragged = true;

			ShowDragInfo();
		}

		/// <inheritdoc/>
		protected override bool CheckTarget(IDropSupport<TData> target, PointerEventData eventData)
		{
			var track_bg = CurrentTarget as TrackBackgroundBase<TData, TPoint>;
			if (track_bg != null)
			{
				track_bg.DragPositionPadding = DragPositionPadding;
			}

			return base.CheckTarget(target, eventData);
		}

		/// <inheritdoc/>
		protected override void OnDrag(PointerEventData eventData)
		{
			if ((Owner != null) && (CurrentTarget != null))
			{
#if CSHARP_7_3_OR_NEWER
				void Action()
#else
				Action Action = () =>
#endif
				{
					if ((Owner != null) && Owner.AllowDragOutside && (CurrentTarget == null))
					{
						var start = Owner.Position2Point(eventData.position, eventData.pressEventCamera);
						var end = Data.EndPointByStartPoint(start);
						var can_move = Owner.AllowIntersection || !Owner.TrackIntersection(Track, start, end, Data.Order, Data);
						if (can_move)
						{
							Data.ChangePoints(start, end);
						}
					}

					base.OnDrag(eventData);
				}
#if !CSHARP_7_3_OR_NEWER
				;
#endif

				Owner.StartAutoScroll(eventData, Action);
			}

			base.OnDrag(eventData);
		}

		/// <inheritdoc/>
		protected override void OnEndDrag(PointerEventData eventData)
		{
			if (Owner != null)
			{
				Owner.StopAutoScroll();
			}

			base.OnEndDrag(eventData);
		}

		/// <summary>
		/// Shows the drag info.
		/// </summary>
		protected virtual void ShowDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			var rt = DragInfo.transform as RectTransform;
			rt.SetParent(DragPoint, false);
			rt.localPosition = DragInfoOffset;

			DragInfo.SetData(null, Data, 0f);

			DragInfo.gameObject.SetActive(true);
		}

		/// <summary>
		/// Hides the drag info.
		/// </summary>
		protected virtual void HideDragInfo()
		{
			if (DragInfo == null)
			{
				return;
			}

			DragInfo.gameObject.SetActive(false);
		}

		/// <inheritdoc/>
		public override void Dropped(bool success)
		{
			Data.IsDragged = false;

			HideDragInfo();

			base.Dropped(success);
		}

		/// <inheritdoc/>
		protected override void OnDisable()
		{
			if (IsDragged)
			{
				if (Owner != null)
				{
					Owner.StopAutoScroll();
				}
			}

			base.OnDisable();
		}
	}
}