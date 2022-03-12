#if UNITY_EDITOR
namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Splitter editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(Splitter), true)]
	public class SplitterEditor : UIWidgetsMonoEditor
	{
		/// <summary>
		/// Serialized properties.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Serialized cursors.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedCursors = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Properties.
		/// </summary>
		protected List<string> Properties = new List<string>();

		/// <summary>
		/// Cursors.
		/// </summary>
		protected List<string> Cursors = new List<string>()
		{
			"CursorTexture",
			"CursorHotSpot",
			"DefaultCursorTexture",
			"DefaultCursorHotSpot",
		};

		GUILayoutOption[] toggleOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Properties.Clear();
			SerializedProperties.Clear();
			SerializedCursors.Clear();

			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				AddProperty(property);
			}

			foreach (var p in Properties)
			{
				SerializedProperties.Add(p, serializedObject.FindProperty(p));
			}

			foreach (var c in Cursors)
			{
				SerializedCursors.Add(c, serializedObject.FindProperty(c));
			}
		}

		void AddProperty(SerializedProperty property)
		{
			if (!Cursors.Contains(property.name))
			{
				Properties.Add(property.name);
			}
		}

		/// <summary>
		/// Toggle cursors block.
		/// </summary>
		protected bool ShowCursors;

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			serializedObject.Update();

			EditorGUILayout.PropertyField(SerializedProperties["interactable"], true);
			EditorGUILayout.PropertyField(SerializedProperties["Type"], true);
			EditorGUILayout.PropertyField(SerializedProperties["UpdateRectTransforms"], true);
			EditorGUILayout.PropertyField(SerializedProperties["UpdateLayoutElements"], true);
			EditorGUILayout.PropertyField(SerializedProperties["Mode"], true);

			if (SerializedProperties["Mode"].enumValueIndex == 1)
			{
				EditorGUILayout.PropertyField(SerializedProperties["PreviousObject"], true);
				EditorGUILayout.PropertyField(SerializedProperties["NextObject"], true);
			}

			EditorGUILayout.BeginVertical();

			ShowCursors = GUILayout.Toggle(ShowCursors, "Cursors", EditorStyles.foldout, toggleOptions);
			if (ShowCursors)
			{
				foreach (var sc in SerializedCursors)
				{
					EditorGUILayout.PropertyField(sc.Value, true);
				}
			}

			EditorGUILayout.EndVertical();

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}
#endif