namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// TreeViewNode drag support.
	/// </summary>
	[RequireComponent(typeof(TreeViewComponent))]
	public class TreeViewNodeDragSupport : TreeViewCustomNodeDragSupport<TreeViewComponent, ListViewIconsItemComponent, TreeViewItem>
	{
	}
}