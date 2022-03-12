namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Resizable delta event.
	/// </summary>
	[Serializable]
	public class ResizableDeltaEvent : UnityEvent<Resizable, Resizable.Regions, Vector2>
	{
	}
}