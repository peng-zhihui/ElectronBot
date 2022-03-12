namespace UIWidgets.Examples.ToDoList
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// ToDoList item.
	/// </summary>
	[Serializable]
	public class ToDoListViewItem
	{
		/// <summary>
		/// Is task done?
		/// </summary>
		[SerializeField]
		public bool Done;

		/// <summary>
		/// Task.
		/// </summary>
		[SerializeField]
		public string Task;
	}
}