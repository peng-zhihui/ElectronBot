namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// ListViewItem event for resize.
	/// </summary>
	[Serializable]
	public class ListViewItemResizeEvent : UnityEvent<int, ListViewItem, Vector2>
	{
	}
}