namespace UIWidgets
{
	using System;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// List view item pointer move event.
	/// </summary>
	[Serializable]
	public class ListViewItemMove : UnityEvent<AxisEventData, ListViewItem>
	{
	}
}