namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewCheckboxes item.
	/// </summary>
	[Serializable]
	public class TreeViewCheckboxesItem : TreeViewItem
	{
		/// <summary>
		/// Is selected.
		/// </summary>
		[SerializeField]
		public bool Selected;

		/// <summary>
		/// Initializes a new instance of the <see cref="TreeViewCheckboxesItem"/> class.
		/// </summary>
		/// <param name="itemName">Name.</param>
		/// <param name="itemIcon">Color.</param>
		/// <param name="itemSelected">Is selected.</param>
		public TreeViewCheckboxesItem(string itemName, Sprite itemIcon = null, bool itemSelected = false)
			: base(itemName, itemIcon)
		{
			Selected = itemSelected;
		}
	}
}