#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the Nodes of a TreeGraph depending on the UIWidgets.ObservableList{UIWidgets.TreeNode{UIWidgets.TreeViewItem}} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TreeGraph Nodes Setter")]
	public class TreeGraphNodesSetter : ComponentSingleSetter<UIWidgets.TreeGraph, UIWidgets.ObservableList<UIWidgets.TreeNode<UIWidgets.TreeViewItem>>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TreeGraph target, UIWidgets.ObservableList<UIWidgets.TreeNode<UIWidgets.TreeViewItem>> value)
		{
			target.Nodes = value;
		}
	}
}
#endif