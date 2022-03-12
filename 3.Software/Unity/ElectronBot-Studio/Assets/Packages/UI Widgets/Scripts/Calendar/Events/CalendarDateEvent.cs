namespace UIWidgets
{
	using System;
	using UnityEngine.Events;

	/// <summary>
	/// Calendar event.
	/// </summary>
	[Serializable]
	public class CalendarDateEvent : UnityEvent<DateTime>
	{
	}
}