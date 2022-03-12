#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;

	/// <summary>
	/// SpinnerFloat editor.
	/// </summary>
	[CanEditMultipleObjects]
	[CustomEditor(typeof(SpinnerFloat), true)]
	public class SpinnerFloatEditor : UIWidgetsMonoEditor
	{
		readonly Dictionary<string, SerializedProperty> serializedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Properties.
		/// </summary>
		protected readonly string[] properties = new string[]
		{
			// Spinner
			"ValueMin",
			"ValueMax",
			"ValueStep",
			"SpinnerValue",
			"Validation",
			"format",
			"DecimalSeparators",
			"plusButton",
			"minusButton",
			"AllowHold",
			"HoldStartDelay",
			"HoldChangeDelay",
			"onValueChangeFloat",
			"onPlusClick",
			"onMinusClick",
		};

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			foreach (var p in properties)
			{
				serializedProperties.Add(p, serializedObject.FindProperty(p));
			}
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			serializedObject.Update();

			EditorGUILayout.PropertyField(serializedProperties["ValueMin"], true);
			EditorGUILayout.PropertyField(serializedProperties["ValueMax"], true);
			EditorGUILayout.PropertyField(serializedProperties["ValueStep"], true);
			EditorGUILayout.PropertyField(serializedProperties["SpinnerValue"], true);
			EditorGUILayout.PropertyField(serializedProperties["Validation"], true);
			EditorGUILayout.PropertyField(serializedProperties["format"], true);
			EditorGUILayout.PropertyField(serializedProperties["DecimalSeparators"], true);
			EditorGUILayout.PropertyField(serializedProperties["AllowHold"], true);
			EditorGUILayout.PropertyField(serializedProperties["HoldStartDelay"], true);
			EditorGUILayout.PropertyField(serializedProperties["HoldChangeDelay"], true);
			EditorGUILayout.PropertyField(serializedProperties["plusButton"], true);
			EditorGUILayout.PropertyField(serializedProperties["minusButton"], true);

			EditorGUILayout.PropertyField(serializedProperties["onValueChangeFloat"]);
			EditorGUILayout.PropertyField(serializedProperties["onPlusClick"]);
			EditorGUILayout.PropertyField(serializedProperties["onMinusClick"]);

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}
#endif