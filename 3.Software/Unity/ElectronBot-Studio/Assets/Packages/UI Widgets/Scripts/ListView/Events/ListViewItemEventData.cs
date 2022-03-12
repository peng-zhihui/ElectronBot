namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// List view item event data.
	/// </summary>
	public class ListViewItemEventData : BaseEventData
	{
		/// <summary>
		/// The new selected object.
		/// </summary>
		public GameObject NewSelectedObject;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.ListViewItemEventData"/> class.
		/// </summary>
		/// <param name="eventSystem">Event system.</param>
		public ListViewItemEventData(EventSystem eventSystem)
			: base(eventSystem)
		{
		}
	}
}