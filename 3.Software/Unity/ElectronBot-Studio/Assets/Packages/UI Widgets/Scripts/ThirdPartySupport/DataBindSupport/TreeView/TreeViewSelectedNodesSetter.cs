#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SelectedNodes of a TreeView depending on the System.Collections.Generic.List{UIWidgets.TreeNode{UIWidgets.TreeViewItem}} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] TreeView SelectedNodes Setter")]
	public class TreeViewSelectedNodesSetter : ComponentSingleSetter<UIWidgets.TreeView, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.TreeViewItem>>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.TreeView target, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.TreeViewItem>> value)
		{
			target.SelectedNodes = value;
		}
	}
}
#endif