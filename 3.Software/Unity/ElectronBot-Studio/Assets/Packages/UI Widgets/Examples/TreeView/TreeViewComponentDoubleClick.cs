namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewComponent with function to process double click event.
	/// </summary>
	public class TreeViewComponentDoubleClick : TreeViewComponent
	{
		/// <summary>
		/// Function to process click event.
		/// Attach this function to DefaultItem.OnClick.
		/// </summary>
		public void ProcessDoubleClick()
		{
			// do something with Node or Item
			Debug.Log(Node.Item.Name);

			// var tree = Owner as TreeView;
			// tree.DoSomething();
		}
	}
}