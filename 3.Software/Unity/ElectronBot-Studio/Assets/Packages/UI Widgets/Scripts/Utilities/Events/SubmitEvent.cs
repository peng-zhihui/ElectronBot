namespace UIWidgets
{
	using System;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Submit event.
	/// </summary>
	[Serializable]
	public class SubmitEvent : UnityEvent<BaseEventData, bool>
	{
	}
}