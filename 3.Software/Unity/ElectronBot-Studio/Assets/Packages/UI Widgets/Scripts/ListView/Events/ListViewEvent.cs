namespace UIWidgets
{
	using System;
	using UnityEngine.Events;

	/// <summary>
	/// List view event.
	/// </summary>
	[Serializable]
	public class ListViewEvent : UnityEvent<int, string>
	{
	}
}