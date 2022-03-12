namespace UIWidgets
{
	using System;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Pointer unity event.
	/// </summary>
	[Serializable]
	public class PointerUnityEvent : UnityEvent<PointerEventData>
	{
	}
}