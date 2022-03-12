#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Nodes of a TreeView depending on the UIWidgets.ObservableList{UIWidgets.TreeNode{UIWidgets.TreeViewItem}} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TreeView Nodes Setter")]
	public class TreeViewNodesSetter : ComponentSingleSetter<UIWidgets.TreeView, UIWidgets.ObservableList<UIWidgets.TreeNode<UIWidgets.TreeViewItem>>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TreeView target, UIWidgets.ObservableList<UIWidgets.TreeNode<UIWidgets.TreeViewItem>> value)
		{
			target.Nodes = value;
		}
	}
}
#endif