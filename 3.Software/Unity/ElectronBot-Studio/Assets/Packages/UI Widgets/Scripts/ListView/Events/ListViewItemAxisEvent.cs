namespace UIWidgets
{
	using System;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewItem event for the AxisEventData.
	/// </summary>
	[Serializable]
	public class ListViewItemAxisEvent : UnityEvent<int, ListViewItem, AxisEventData>
	{
	}
}