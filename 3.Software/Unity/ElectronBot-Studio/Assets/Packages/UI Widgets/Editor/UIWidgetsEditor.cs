#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;

	/// <summary>
	/// Base editor for the UIWidgets components.
	/// </summary>
	[CustomEditor(typeof(UIWidgetsBehaviour), true)]
	public class UIWidgetsEditor : UIWidgetsMonoEditor
	{
	}
}
#endif