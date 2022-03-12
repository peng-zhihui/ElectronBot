namespace UIWidgets
{
	using System;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// ListViewItem event for the BaseEventData.
	/// </summary>
	[Serializable]
	public class ListViewItemBaseEvent : UnityEvent<int, ListViewItem, BaseEventData>
	{
	}
}