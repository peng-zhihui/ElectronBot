namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// ListView item events.
	/// </summary>
	[Serializable]
	public class ListViewItemEvents
	{
		/// <summary>
		/// Pointer click event.
		/// </summary>
		[SerializeField]
		public ListViewItemPointerEvent PointerClick = new ListViewItemPointerEvent();

		/// <summary>
		/// Pointer single click event.
		/// </summary>
		[SerializeField]
		public ListViewItemPointerEvent FirstClick = new ListViewItemPointerEvent();

		/// <summary>
		/// Pointer double click event.
		/// </summary>
		[SerializeField]
		public ListViewItemPointerEvent DoubleClick = new ListViewItemPointerEvent();

		/// <summary>
		/// Node toggle event.
		/// </summary>
		[SerializeField]
		public ListViewBaseEvent NodeToggleClick = new ListViewBaseEvent();

		/// <summary>
		/// Pointer up event.
		/// </summary>
		[SerializeField]
		public ListViewItemPointerEvent PointerUp = new ListViewItemPointerEvent();

		/// <summary>
		/// Pointer down event.
		/// </summary>
		[SerializeField]
		public ListViewItemPointerEvent PointerDown = new ListViewItemPointerEvent();

		/// <summary>
		/// Pointer enter event.
		/// </summary>
		[SerializeField]
		public ListViewItemPointerEvent PointerEnter = new ListViewItemPointerEvent();

		/// <summary>
		/// Pointer exit event.
		/// </summary>
		[SerializeField]
		public ListViewItemPointerEvent PointerExit = new ListViewItemPointerEvent();

		/// <summary>
		/// Move event.
		/// </summary>
		[SerializeField]
		public ListViewItemAxisEvent Move = new ListViewItemAxisEvent();

		/// <summary>
		/// Submit event.
		/// </summary>
		[SerializeField]
		public ListViewItemBaseEvent Submit = new ListViewItemBaseEvent();

		/// <summary>
		/// Cancel event.
		/// </summary>
		[SerializeField]
		public ListViewItemBaseEvent Cancel = new ListViewItemBaseEvent();

		/// <summary>
		/// Select event.
		/// </summary>
		[SerializeField]
		public ListViewItemBaseEvent Select = new ListViewItemBaseEvent();

		/// <summary>
		/// Deselect event.
		/// </summary>
		[SerializeField]
		public ListViewItemBaseEvent Deselect = new ListViewItemBaseEvent();

		/// <summary>
		/// Resize event.
		/// </summary>
		[SerializeField]
		public ListViewItemResizeEvent Resize = new ListViewItemResizeEvent();
	}
}