namespace UIWidgets.WidgetGeneration
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TreeView generator helper.
	/// </summary>
	public class TreeViewGeneratorHelper : ListViewGeneratorHelper
	{
		/// <summary>
		/// Viewport.
		/// </summary>
		public LayoutElement Indentation;

		/// <summary>
		/// Toggle.
		/// </summary>
		public TreeNodeToggle Toggle;
	}
}