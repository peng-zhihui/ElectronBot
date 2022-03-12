namespace UIWidgets.Examples
{
	using System;
	using UnityEngine.Events;

	/// <summary>
	/// TimeRangeSlider event.
	/// </summary>
	[Serializable]
	public class TimeRangeSliderEvent : UnityEvent<DateTime, DateTime>
	{
	}
}