#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// ScrollRectPaginator editor.
	/// </summary>
	[CustomEditor(typeof(ScrollRectPaginator), true)]
	[CanEditMultipleObjects]
	public class ScrollRectPaginatorEditor : UIWidgetsMonoEditor
	{
		readonly Dictionary<string, SerializedProperty> Properties = new Dictionary<string, SerializedProperty>();

		readonly string[] properties = new string[]
		{
			"ScrollRect",
			"DefaultPage",
			"ActivePage",
			"PrevPage",
			"NextPage",

			"Direction",
			"pageSizeType",
			"pageSize",
			"pageSpacing",
			"FastDragDistance",
			"FastDragTime",
			"currentPage",
			"ForcedPosition",
			"lastPageFullSize",

			"Animation",
			"Movement",
			"UnscaledTime",
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

			EditorGUILayout.PropertyField(Properties["ScrollRect"], true);

			EditorGUILayout.PropertyField(Properties["DefaultPage"], true);
			EditorGUILayout.PropertyField(Properties["ActivePage"], true);
			EditorGUILayout.PropertyField(Properties["PrevPage"], true);
			EditorGUILayout.PropertyField(Properties["NextPage"], true);

			EditorGUILayout.PropertyField(Properties["Direction"], true);
			EditorGUILayout.PropertyField(Properties["pageSizeType"], true);
			EditorGUI.indentLevel++;

			// fixed
			if (Properties["pageSizeType"].enumValueIndex == 1)
			{
				EditorGUILayout.PropertyField(Properties["pageSize"], true);
			}

			EditorGUI.indentLevel--;
			EditorGUILayout.PropertyField(Properties["pageSpacing"], true);
			EditorGUILayout.PropertyField(Properties["FastDragDistance"], true);
			EditorGUILayout.PropertyField(Properties["FastDragTime"], true);
			EditorGUILayout.PropertyField(Properties["currentPage"], true);
			EditorGUILayout.PropertyField(Properties["ForcedPosition"], true);
			EditorGUILayout.PropertyField(Properties["lastPageFullSize"], true);

			EditorGUILayout.PropertyField(Properties["Animation"], true);

			EditorGUI.indentLevel++;

			if (Properties["Animation"].boolValue)
			{
				EditorGUILayout.PropertyField(Properties["Movement"], true);
				EditorGUILayout.PropertyField(Properties["UnscaledTime"], true);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(Properties["OnPageSelect"], true);

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}
#endif