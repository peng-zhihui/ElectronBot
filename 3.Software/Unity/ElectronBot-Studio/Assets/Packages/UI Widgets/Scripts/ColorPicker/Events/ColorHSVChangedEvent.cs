namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Color HSV changed event.
	/// </summary>
	[Serializable]
	public class ColorHSVChangedEvent : UnityEvent<ColorHSV>
	{
	}
}