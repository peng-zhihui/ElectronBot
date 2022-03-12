namespace UIWidgets
{
	using UnityEngine.EventSystems;

	/// <summary>
	/// TableHeader cell drag support.
	/// </summary>
	public class TableHeaderDragCell : DragSupport<TableHeaderDragCell>
	{
		/// <summary>
		/// The position.
		/// </summary>
		public int Position = -1;

		/// <summary>
		/// TableHeader.
		/// </summary>
		public TableHeader TableHeader;

		/// <summary>
		/// Alias for TableHeader.
		/// </summary>
		[System.Obsolete("Renamed to TableHeader.")]
		public TableHeader ResizableHeader
		{
			get
			{
				return TableHeader;
			}
		}

		/// <summary>
		/// Determines whether this instance can be dragged.
		/// </summary>
		/// <returns><c>true</c> if this instance can be dragged; otherwise, <c>false</c>.</returns>
		/// <param name="eventData">Current event data.</param>
		public override bool CanDrag(PointerEventData eventData)
		{
			return TableHeader.AllowReorder && !TableHeader.CheckInActiveRegion(eventData);
		}

		/// <summary>
		/// Set Data, which will be passed to Drop component.
		/// </summary>
		/// <param name="eventData">Current event data.</param>
		protected override void InitDrag(PointerEventData eventData)
		{
			TableHeader.ProcessCellReorder = true;
			Data = this;
		}

		/// <summary>
		/// Called when drop completed.
		/// </summary>
		/// <param name="success"><c>true</c> if Drop component received data; otherwise, <c>false</c>.</param>
		public override void Dropped(bool success)
		{
			TableHeader.ProcessCellReorder = false;

			base.Dropped(success);
		}
	}
}