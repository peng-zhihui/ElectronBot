#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Conditional editor for the classes derived from MonoBehaviourConditional.
	/// </summary>
	[CustomEditor(typeof(MonoBehaviourConditional), true)]
	public class EditorConditionalMono : EditorConditional
	{
	}
}
#endif