#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// ListViewIcons editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(ListViewIcons), true)]
	public class ListViewIconsEditor : ListViewCustomBaseEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="ListViewIconsEditor"/> class.
		/// </summary>
		public ListViewIconsEditor()
		{
			Properties.Insert(2, "sort");
		}
	}
}
#endif