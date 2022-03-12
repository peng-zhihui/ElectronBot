namespace UIWidgets
{
	using System;

	/// <summary>
	/// Track data.
	/// </summary>
	/// <typeparam name="TPoint">Point type.</typeparam>
	public interface ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Start point.
		/// </summary>
		TPoint StartPoint
		{
			get;
		}

		/// <summary>
		/// End point.
		/// </summary>
		TPoint EndPoint
		{
			get;
		}

		/// <summary>
		/// Order.
		/// </summary>
		int Order
		{
			get;
			set;
		}

		/// <summary>
		/// Is order fixed?
		/// </summary>
		bool FixedOrder
		{
			get;
			set;
		}

		/// <summary>
		/// Is item dragged?
		/// </summary>
		bool IsDragged
		{
			get;
			set;
		}

		/// <summary>
		/// Get new EndPoint by specified StartPoint to maintain same length.
		/// </summary>
		/// <param name="newStart">New start point.</param>
		/// <returns>New EndPoint.</returns>
		TPoint EndPointByStartPoint(TPoint newStart);

		/// <summary>
		/// Set StartPoint and EndPoint to maintain same length.
		/// </summary>
		/// <param name="newStart">New start point.</param>
		/// <param name="newEnd">New end point.</param>
		void SetPoints(TPoint newStart, TPoint newEnd);

		/// <summary>
		/// Change StartPoint and EndPoint.
		/// </summary>
		/// <param name="newStart">New StartPoint.</param>
		/// <param name="newEnd">New EndPoint.</param>
		void ChangePoints(TPoint newStart, TPoint newEnd);
	}
}