#if UNITY_EDITOR
namespace UIWidgets.Examples.Shops
{
	using UIWidgets;
	using UnityEditor;

	/// <summary>
	/// HarbotListView editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(HarborListView), true)]
	public class HarborListViewEditor : ListViewCustomEditor
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HarborListViewEditor"/> class.
		/// </summary>
		public HarborListViewEditor()
		{
			Properties.Remove("customItems");
		}
	}
}
#endif