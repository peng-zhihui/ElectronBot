#if UNITY_EDITOR
namespace EasyLayoutNS
{
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Conditional editor.
	/// </summary>
	public abstract class ConditionalEditor : Editor
	{
		/// <summary>
		/// Not displayable fields.
		/// </summary>
		protected List<string> IgnoreFields;

		/// <summary>
		/// Fields to display.
		/// </summary>
		protected List<ConditionalFieldInfo> Fields;

		/// <summary>
		/// Serialized properties.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerizalizedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Serialized events.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedEvents = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Init();

			SerizalizedProperties.Clear();
			foreach (var field in Fields)
			{
				SerizalizedProperties[field.Name] = null;
			}

			GetSerizalizedProperties();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected abstract void Init();

		/// <summary>
		/// Get serialized properties.
		/// </summary>
		protected void GetSerizalizedProperties()
		{
			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				if (IsEvent(property))
				{
					SerializedEvents[property.name] = serializedObject.FindProperty(property.name);
				}
				else
				{
					if (SerizalizedProperties.ContainsKey(property.name))
					{
						SerizalizedProperties[property.name] = serializedObject.FindProperty(property.name);
					}
					else if (!IgnoreFields.Contains(property.name))
					{
						Debug.LogWarning("Field info not found: " + property.name);
					}
				}
			}
		}

		/// <summary>
		/// Is property event?
		/// </summary>
		/// <param name="property">Property</param>
		/// <returns>true if property is event; otherwise false.</returns>
		protected virtual bool IsEvent(SerializedProperty property)
		{
			var object_type = property.serializedObject.targetObject.GetType();
			var property_type = object_type.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			if (property_type == null)
			{
				return false;
			}

			return typeof(UnityEventBase).IsAssignableFrom(property_type.FieldType);
		}

		/// <summary>
		/// Check is all displayable fields exists.
		/// </summary>
		/// <returns>true if all displayable fields exists; otherwise false.</returns>
		protected bool AllFieldsExists()
		{
			var result = true;
			foreach (var kv in SerizalizedProperties)
			{
				if (kv.Value == null)
				{
					Debug.LogWarning("Field with name '" + kv.Key + "' not found");
					result = false;
				}
			}

			return result;
		}

		/// <summary>
		/// Check is field can be displayed.
		/// </summary>
		/// <param name="info">Field info.</param>
		/// <returns>true if field can be displayed; otherwise false.</returns>
		protected bool CanShow(ConditionalFieldInfo info)
		{
			foreach (var condition in info.Conditions)
			{
				var field = SerizalizedProperties[condition.Key];
				if (!condition.Value(field))
				{
					return false;
				}
			}

			return true;
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			if (!AllFieldsExists())
			{
				return;
			}

			serializedObject.Update();

			foreach (var field in Fields)
			{
				if (!CanShow(field))
				{
					continue;
				}

				EditorGUI.indentLevel += field.Indent;
				EditorGUILayout.PropertyField(SerizalizedProperties[field.Name], true);
				EditorGUI.indentLevel -= field.Indent;
			}

			foreach (var ev in SerializedEvents)
			{
				EditorGUILayout.PropertyField(ev.Value, true);
			}

			serializedObject.ApplyModifiedProperties();

			AdditionalGUI();
		}

		/// <summary>
		/// Display additional GUI.
		/// </summary>
		protected virtual void AdditionalGUI()
		{
		}
	}
}
#endif