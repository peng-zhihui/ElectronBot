namespace UIWidgets
{
	using System;
	using UnityEngine.Events;

	/// <summary>
	/// ListViewBase event.
	/// </summary>
	[Serializable]
	public class ListViewBaseEvent : UnityEvent<int, ListViewItem>
	{
	}
}