#if UIWIDGETS_DATABIND_SUPPORT
namespace UIWidgets.DataBind
{
	using Slash.Unity.DataBind.Foundation.Setters;
	using UnityEngine;

	/// <summary>
	/// Set the SelectedNodes of a DirectoryTreeView depending on the System.Collections.Generic.List{UIWidgets.TreeNode{UIWidgets.FileSystemEntry}} data value.
	/// </summary>
	[AddComponentMenu("Data Bind/New UI Widgets/Setters/[DB] DirectoryTreeView SelectedNodes Setter")]
	public class DirectoryTreeViewSelectedNodesSetter : ComponentSingleSetter<UIWidgets.DirectoryTreeView, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>>>
	{
		/// <inheritdoc />
		protected override void UpdateTargetValue(UIWidgets.DirectoryTreeView target, System.Collections.Generic.List<UIWidgets.TreeNode<UIWidgets.FileSystemEntry>> value)
		{
			target.SelectedNodes = value;
		}
	}
}
#endif