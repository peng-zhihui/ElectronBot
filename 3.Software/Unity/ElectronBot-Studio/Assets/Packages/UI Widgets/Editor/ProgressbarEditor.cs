#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// Progress bar editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Progressbar), true)]
	public class ProgressbarEditor : UIWidgetsMonoEditor
	{
		readonly Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		readonly string[] properties = new string[]
		{
			"Max",
			"progressValue",
			"type",
			"Direction",
			"IndeterminateBar",
			"DeterminateBar",
			"EmptyBar",
			"EmptyBarTextAdapter",
			"fullBar",
			"FullBarTextAdapter",
			"BarMask",
			"textType",
			"Speed",
			"SpeedType",
			"UnscaledTime",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected void OnEnable()
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

			EditorGUILayout.Space();

			EditorGUILayout.PropertyField(serializedProperties["Max"]);
			EditorGUILayout.PropertyField(serializedProperties["progressValue"]);
			EditorGUILayout.PropertyField(serializedProperties["type"]);

			EditorGUI.indentLevel++;
			if (serializedProperties["type"].enumValueIndex == 0)
			{
				EditorGUILayout.PropertyField(serializedProperties["DeterminateBar"]);
				EditorGUILayout.PropertyField(serializedProperties["BarMask"]);
				EditorGUILayout.PropertyField(serializedProperties["EmptyBar"]);
				EditorGUILayout.PropertyField(serializedProperties["EmptyBarTextAdapter"]);
				EditorGUILayout.PropertyField(serializedProperties["fullBar"]);
				EditorGUILayout.PropertyField(serializedProperties["FullBarTextAdapter"]);
				EditorGUILayout.PropertyField(serializedProperties["textType"]);
			}
			else
			{
				EditorGUILayout.PropertyField(serializedProperties["IndeterminateBar"]);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(serializedProperties["Direction"]);
			EditorGUILayout.PropertyField(serializedProperties["Speed"]);
			EditorGUILayout.PropertyField(serializedProperties["SpeedType"]);
			EditorGUILayout.PropertyField(serializedProperties["UnscaledTime"]);

			serializedObject.ApplyModifiedProperties();

			foreach (var t in targets)
			{
				((Progressbar)t).Refresh();
			}

			ValidateTargets();
		}
	}
}
#endif