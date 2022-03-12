namespace UIWidgets
{
	using System;
	using UnityEngine.Events;

	/// <summary>
	/// Time event.
	/// </summary>
	[Serializable]
	public class TimeEvent : UnityEvent<TimeSpan>
	{
	}
}