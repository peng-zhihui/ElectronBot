#if UNITY_EDITOR
namespace UIWidgets
{
	/// <summary>
	/// ListViewCustom editor.
	/// </summary>
	public class ListViewCustomEditor : ListViewCustomBaseEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewCustomEditor"/> class.
		/// </summary>
		public ListViewCustomEditor()
		{
			IsListViewCustom = true;
		}
	}
}
#endif