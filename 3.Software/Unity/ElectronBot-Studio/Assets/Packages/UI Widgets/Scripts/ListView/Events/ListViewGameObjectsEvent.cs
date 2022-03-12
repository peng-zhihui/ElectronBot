namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// ListViewGameObjects event.
	/// </summary>
	[Serializable]
	public class ListViewGameObjectsEvent : UnityEvent<int, GameObject>
	{
	}
}