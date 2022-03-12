#if UNITY_EDITOR
namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// TreeView.DataSource editor.
	/// </summary>
	[CustomEditor(typeof(TreeViewDataSource), true)]
	public class TreeViewDataSourceEditor : UIWidgetsMonoEditor
	{
		/// <summary>
		/// Open editor window.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			if (GUILayout.Button("Edit"))
			{
				TreeViewDataSourceWindow.Init();
			}

			ValidateTargets();
		}
	}
}
#endif