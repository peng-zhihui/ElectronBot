namespace UIWidgets
{
	using UnityEngine.EventSystems;

	/// <summary>
	/// IDropSupport.
	/// Receive drops from DragSupport{TItem}.
	/// </summary>
	/// <typeparam name="TItem">Type of dropped data.</typeparam>
	public interface IDropSupport<TItem>
	{
		/// <summary>
		/// Determines whether this instance can receive drop with the specified data and eventData.
		/// </summary>
		/// <returns><c>true</c> if this instance can receive drop with the specified data and eventData; otherwise, <c>false</c>.</returns>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		bool CanReceiveDrop(TItem data, PointerEventData eventData);

		/// <summary>
		/// Handle dropped data.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		void Drop(TItem data, PointerEventData eventData);

		/// <summary>
		/// Handle canceled drop.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <param name="eventData">Event data.</param>
		void DropCanceled(TItem data, PointerEventData eventData);
	}
}