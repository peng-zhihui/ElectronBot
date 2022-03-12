namespace UIWidgets
{
	using System;
	using UnityEngine;

	/// <summary>
	/// TreeViewDataSourceItem.
	/// </summary>
	[Serializable]
	public class TreeViewDataSourceItem
	{
		/// <summary>
		/// The depth.
		/// </summary>
		[SerializeField]
		public int Depth;

		/// <summary>
		/// The is visible.
		/// </summary>
		[SerializeField]
		public bool IsVisible = true;

		/// <summary>
		/// The is expanded.
		/// </summary>
		[SerializeField]
		public bool IsExpanded;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Sprite Icon;

		/// <summary>
		/// The name.
		/// </summary>
		[SerializeField]
		public string Name;

		/// <summary>
		/// The value.
		/// </summary>
		[SerializeField]
		public int Value;

		/// <summary>
		/// The tag.
		/// </summary>
		[SerializeField]
		public object Tag;
	}
}