namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the track background.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	[RequireComponent(typeof(Graphic))]
	public class TrackBackgroundBase<TData, TPoint> : TrackViewBase<TData, TPoint>, IDropSupport<TData>
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Padding from start drag point to left border of the item.
		/// Required for the correct StartPoint calculation.
		/// </summary>
		[NonSerialized]
		public float DragPositionPadding;

		/// <summary>
		/// Process double click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected override void OnDoubleClick(PointerEventData eventData)
		{
			if (!Owner.IsInteractable())
			{
				return;
			}

			if (!AllowDialog)
			{
				return;
			}

			var start = Owner.Position2Point(GetPosition(eventData), eventData.pressEventCamera);
			Owner.OpenCreateTrackDataDialog(Track, start);
		}

		/// <summary>
		/// Get position.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>Position.</returns>
		protected Vector2 GetPosition(PointerEventData eventData)
		{
			var pos = eventData.position;
			pos.x -= DragPositionPadding;

			return pos;
		}

		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual bool CanReceiveDrop(TData data, PointerEventData eventData)
		{
			if (data == null)
			{
				return true;
			}

			var prev_order = data.Order;

			var start = Owner.Position2Point(eventData.position, eventData.pressEventCamera);
			var end = data.EndPointByStartPoint(start);
			var order = GetOrder(eventData);

			var can_move = Owner.AllowIntersection || !Owner.TrackIntersection(Track, start, end, order, data);
			if (can_move)
			{
				Track.Data.BeginUpdate();

				data.ChangePoints(start, end);
				if (!Track.Data.Contains(data))
				{
					Track.Data.Add(data);
					prev_order = -1;
				}

				Track.SetItemOrder(data, order, prev_order);
				Track.Data.EndUpdate();
			}

			return can_move;
		}

		/// <summary>
		/// Get order.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <returns>Order.</returns>
		protected virtual int GetOrder(PointerEventData eventData)
		{
			Vector2 point;
			if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(transform as RectTransform, eventData.position, eventData.pressEventCamera, out point))
			{
				return 0;
			}

			var line = (-point.y) / (Owner.DefaultItemSize.y + Owner.ItemsSpacing);

			return Mathf.FloorToInt(line);
		}

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void Drop(TData data, PointerEventData eventData)
		{
		}

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		public virtual void DropCanceled(TData data, PointerEventData eventData)
		{
		}
	}
}