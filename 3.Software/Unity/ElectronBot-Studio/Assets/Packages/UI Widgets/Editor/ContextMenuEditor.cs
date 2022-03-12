#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// ContextMenu editor.
	/// </summary>
	[CustomEditor(typeof(UIWidgets.Menu.ContextMenu), true)]
	public class ContextMenuEditor : UIWidgetsMonoEditor
	{
		/// <summary>
		/// Open editor window.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			base.OnInspectorGUI();

			if (GUILayout.Button("Edit MenuItems"))
			{
				ContextMenuWindow.Init(serializedObject);
			}

			ValidateTargets();
		}
	}
}
#endif