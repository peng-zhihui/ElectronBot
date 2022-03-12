#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Conditional editor for the classes derived from UIBehaviourConditional.
	/// </summary>
	[CustomEditor(typeof(UIBehaviourConditional), true)]
	public class EditorConditionalUI : EditorConditional
	{
	}
}
#endif