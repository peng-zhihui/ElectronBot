namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Color RGB changed event.
	/// </summary>
	[Serializable]
	public class ColorRGBChangedEvent : UnityEvent<Color32>
	{
	}
}