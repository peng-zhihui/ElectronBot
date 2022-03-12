namespace UIWidgets
{
	using System;
	using UnityEngine.Events;

	/// <summary>
	/// TreeViewNode event.
	/// </summary>
	[Serializable]
	public class TreeViewNodeEvent : UnityEvent<TreeNode<TreeViewItem>>
	{
	}
}