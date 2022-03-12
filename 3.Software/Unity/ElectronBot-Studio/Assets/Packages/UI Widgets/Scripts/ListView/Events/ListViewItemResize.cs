namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// List view item resize event.
	/// </summary>
	[Serializable]
	public class ListViewItemResize : UnityEvent<int, Vector2>
	{
	}
}