#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Conditional property drawer.
	/// </summary>
	public abstract class ConditionalPropertyDrawer : PropertyDrawer
	{
		/// <summary>
		/// The indent.
		/// </summary>
		protected const float Indent = 16;

		/// <summary>
		/// The height.
		/// </summary>
		protected const float EmptySpace = 2;

		/// <summary>
		/// The indent level.
		/// </summary>
		protected int IndentLevel = 0;

		/// <summary>
		/// Fields to display.
		/// </summary>
		protected List<ConditionalFieldInfo> Fields;

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected abstract void Init();

		/// <summary>
		/// Check is field can be displayed.
		/// </summary>
		/// <param name="info">Field info.</param>
		/// <param name="property">Property data.</param>
		/// <returns>true if field can be displayed; otherwise false.</returns>
		protected static bool CanShow(ConditionalFieldInfo info, SerializedProperty property)
		{
			var p = property.FindPropertyRelative(info.Name);
			if (p == null)
			{
				Debug.LogWarning("Field with name '" + info.Name + "' not found");
				return false;
			}

			foreach (var condition in info.Conditions)
			{
				var field = property.FindPropertyRelative(condition.Key);
				if (!condition.Value(field))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Draw GUI.
		/// </summary>
		/// <param name="position">Start position.</param>
		/// <param name="property">Property data.</param>
		/// <param name="label">Label.</param>
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Init();

			EditorGUI.BeginProperty(position, label, property);

			foreach (var field in Fields)
			{
				if (!CanShow(field, property))
				{
					continue;
				}

				IndentLevel += field.Indent;
				position = DrawProperty(position, property.FindPropertyRelative(field.Name));
				IndentLevel -= field.Indent;
			}

			EditorGUI.EndProperty();
		}

		/// <summary>
		/// Get GUI height for the specified property.
		/// </summary>
		/// <param name="property">Property data.</param>
		/// <param name="label">Label.</param>
		/// <returns>GUI height.</returns>
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			Init();

			var result = 0f;

			foreach (var field in Fields)
			{
				if (!CanShow(field, property))
				{
					continue;
				}

				result += EditorGUI.GetPropertyHeight(property.FindPropertyRelative(field.Name)) + EmptySpace;
			}

			return result;
		}

		/// <summary>
		/// Draws the property.
		/// </summary>
		/// <returns>The new position.</returns>
		/// <param name="position">Position.</param>
		/// <param name="field">Field.</param>
		protected Rect DrawProperty(Rect position, SerializedProperty field)
		{
			var height = EditorGUI.GetPropertyHeight(field);
			var indent = Indent * IndentLevel;

			var rect = new Rect(position.x + indent, position.y, position.width - indent, height);
			EditorGUI.PropertyField(rect, field);

			position.y += rect.height + EmptySpace;

			return position;
		}
	}
}
#endif