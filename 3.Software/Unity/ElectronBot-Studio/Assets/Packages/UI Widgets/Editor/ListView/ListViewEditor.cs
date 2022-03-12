#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// ListView editor.
	/// </summary>
	[CanEditMultipleObjects]
	#pragma warning disable 0618
	[CustomEditor(typeof(ListView), true)]
	#pragma warning restore 0618
	public class ListViewEditor : UIWidgetsMonoEditor
	{
		readonly Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		readonly string[] properties = new string[]
		{
			"interactable",
			"Source",
			"strings",
			"file",
			"CommentsStartWith",
			"sort",
			"Unique",
			"AllowEmptyItems",

			"multipleSelect",
			"selectedIndex",
			"direction",
			"Container",
			"defaultItem",
			"scrollRect",

			"backgroundColor",
			"textColor",
			"HighlightedBackgroundColor",
			"HighlightedTextColor",
			"selectedBackgroundColor",
			"selectedTextColor",
			"disabledColor",

			// other
			"FadeDuration",
			"LimitScrollValue",
			"loopedList",
			"setContentSizeFitter",
			"Navigation",
			"centerTheItems",

			"OnSelectString",
			"OnDeselectString",
		};

		readonly HashSet<string> exclude = new HashSet<string>()
		{
			"interactable",
			"Source",
			"strings",
			"file",
			"CommentsStartWith",
			"AllowEmptyItems",
			"Unique",

			// obsolete
			"LimitScrollValue",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			foreach (var p in properties)
			{
				var sp = serializedObject.FindProperty(p);
				if (sp == null)
				{
					continue;
				}

				if (serializedProperties.ContainsKey(p))
				{
					continue;
				}

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

			EditorGUILayout.PropertyField(serializedProperties["interactable"]);
			EditorGUILayout.PropertyField(serializedProperties["Source"]);

			EditorGUI.indentLevel++;
			if (serializedProperties["Source"].enumValueIndex == 0)
			{
				var options = new GUILayoutOption[] { };
				EditorGUILayout.PropertyField(serializedProperties["strings"], new GUIContent("Data Source"), true, options);
			}
			else
			{
				EditorGUILayout.PropertyField(serializedProperties["file"]);
				EditorGUILayout.PropertyField(serializedProperties["CommentsStartWith"], true);
				EditorGUILayout.PropertyField(serializedProperties["AllowEmptyItems"]);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.PropertyField(serializedProperties["Unique"], new GUIContent("Only unique items"));

			foreach (var sp in serializedProperties)
			{
				if (!exclude.Contains(sp.Key))
				{
					EditorGUILayout.PropertyField(sp.Value);
				}
			}

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}
#endif