namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeView with onDoubleClick event process in TreeView script.
	/// </summary>
	public class TreeViewDoubleClick : TreeView
	{
		/// <summary>
		/// Removes the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void RemoveCallback(TreeViewComponent item)
		{
			base.RemoveCallback(item);
			if (item != null)
			{
				item.onDoubleClick.RemoveListener(DoubleClickListener);
			}
		}

		/// <summary>
		/// Adds the callback.
		/// </summary>
		/// <param name="item">Item.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected override void AddCallback(TreeViewComponent item)
		{
			base.AddCallback(item);
			item.onDoubleClick.AddListener(DoubleClickListener);
		}

		void DoubleClickListener(int index)
		{
			var node = DataSource[index].Node;

			// var component = GetItemComponent(index);
			Debug.Log(node.Item.Name);
		}
	}
}