namespace UIWidgets
{
	using System;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewItem event for the PointerEventData.
	/// </summary>
	[Serializable]
	public class ListViewItemPointerEvent : UnityEvent<int, ListViewItem, PointerEventData>
	{
	}
}