#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// ListViewPaginator editor.
	/// </summary>
	[CustomEditor(typeof(ListViewPaginator), true)]
	[CanEditMultipleObjects]
	public class ListViewPaginatorEditor : UIWidgetsMonoEditor
	{
		readonly Dictionary<string, SerializedProperty> Properties = new Dictionary<string, SerializedProperty>();

		readonly string[] properties = new string[]
		{
			"ListView",
			"perPage",
			"DefaultPage",
			"ActivePage",
			"PrevPage",
			"NextPage",
			"FastDragDistance",
			"FastDragTime",
			"currentPage",
			"ForcedPosition",
			"lastPageFullSize",
			"Animation",
			"OnPageSelect",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Properties.Clear();

			foreach (var p in properties)
			{
				Properties.Add(p, serializedObject.FindProperty(p));
			}
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			serializedObject.Update();

			EditorGUILayout.PropertyField(Properties["ListView"], true);
			EditorGUILayout.PropertyField(Properties["perPage"], true);

			EditorGUILayout.PropertyField(Properties["DefaultPage"], true);
			EditorGUILayout.PropertyField(Properties["ActivePage"], true);
			EditorGUILayout.PropertyField(Properties["PrevPage"], true);
			EditorGUILayout.PropertyField(Properties["NextPage"], true);

			EditorGUILayout.PropertyField(Properties["FastDragDistance"], true);
			EditorGUILayout.PropertyField(Properties["FastDragTime"], true);

			EditorGUILayout.PropertyField(Properties["currentPage"], true);
			EditorGUILayout.PropertyField(Properties["ForcedPosition"], true);
			EditorGUILayout.PropertyField(Properties["lastPageFullSize"], true);

			EditorGUILayout.PropertyField(Properties["Animation"], true);

			EditorGUILayout.PropertyField(Properties["OnPageSelect"], true);

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}
#endif