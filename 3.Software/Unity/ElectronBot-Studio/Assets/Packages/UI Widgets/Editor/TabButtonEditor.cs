#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// TabButton editor.
	/// </summary>
	[CustomEditor(typeof(TabButtonBase), true)]
	[CanEditMultipleObjects]
	public class TabButtonEditor : ButtonCustomEditor
	{
	}
}
#endif