#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// LayoutSwitcher editor.
	/// </summary>
	[CustomEditor(typeof(LayoutSwitcher), true)]
	public class LayoutSwitcherEditor : UIWidgetsMonoEditor
	{
		readonly Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		readonly string[] properties = new string[]
		{
			"Objects",
			"Layouts",
			"DefaultDisplaySize",
			"LayoutChanged",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			foreach (var p in properties)
			{
				var sp = serializedObject.FindProperty(p);
				serializedProperties.Add(p, sp);
			}
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedProperties["Objects"], true);

			DisplayLayouts();

			EditorGUILayout.PropertyField(serializedProperties["DefaultDisplaySize"], true);
			EditorGUILayout.PropertyField(serializedProperties["LayoutChanged"], true);

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}

		void DisplayLayouts()
		{
			var switcher = target as LayoutSwitcher;
			var layouts = serializedProperties["Layouts"];

			layouts.isExpanded = EditorGUILayout.Foldout(layouts.isExpanded, new GUIContent("Layouts"));
			if (layouts.isExpanded)
			{
				EditorGUI.indentLevel++;
				layouts.arraySize = EditorGUILayout.IntField("Size", layouts.arraySize);
				for (int i = 0; i < layouts.arraySize; i++)
				{
					var property = layouts.GetArrayElementAtIndex(i);
					EditorGUILayout.PropertyField(property, true);
					if (GUILayout.Button("Save"))
					{
						switcher.SaveLayout(switcher.Layouts[i]);
					}
				}

				EditorGUI.indentLevel--;
			}
		}
	}
}
#endif