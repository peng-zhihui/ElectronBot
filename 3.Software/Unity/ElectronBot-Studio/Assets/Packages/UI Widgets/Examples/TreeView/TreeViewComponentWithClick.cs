namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// TreeViewComponent with function to process click event.
	/// </summary>
	public class TreeViewComponentWithClick : TreeViewComponent
	{
		/// <summary>
		/// Function to process click event.
		/// Attach this function to DefaultItem.OnClick.
		/// </summary>
		public void ProcessClick()
		{
			// do something with Node or Item
			Debug.Log(string.Format("{0} clicked", Node.Item.Name));
		}
	}
}