#if UNITY_EDITOR
namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// ListViewCustomHeight editor.
	/// </summary>
	public class ListViewCustomHeightEditor : ListViewCustomEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewCustomHeightEditor"/> class.
		/// Constructor.
		/// </summary>
		public ListViewCustomHeightEditor()
		{
			Properties.Add("ForceAutoHeightCalculation");
		}
	}
}
#endif