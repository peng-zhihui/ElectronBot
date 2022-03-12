namespace UIWidgets
{
	using System;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Move event.
	/// </summary>
	[Serializable]
	public class MoveEvent : UnityEvent<AxisEventData>
	{
	}
}