#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// ButtonAdvanced editor.
	/// </summary>
	[CustomEditor(typeof(ButtonAdvanced), true)]
	[CanEditMultipleObjects]
	public class ButtonAdvancedEditor : ButtonCustomEditor
	{
	}
}
#endif