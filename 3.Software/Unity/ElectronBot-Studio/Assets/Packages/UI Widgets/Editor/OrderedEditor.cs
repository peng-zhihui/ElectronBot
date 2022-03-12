#if UNITY_EDITOR
namespace UIWidgets
{
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Ordered editor - events will be displayed last.
	/// </summary>
	public class OrderedEditor : UIWidgetsMonoEditor
	{
		/// <summary>
		/// The serialized properties.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// The serialized events.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedEvents = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// Serialized cursors.
		/// </summary>
		protected Dictionary<string, SerializedProperty> SerializedCursors = new Dictionary<string, SerializedProperty>();

		/// <summary>
		/// The properties to exclude.
		/// </summary>
		protected List<string> Exclude = new List<string>() { };

		/// <summary>
		/// The properties names.
		/// </summary>
		protected List<string> Properties = new List<string>();

		/// <summary>
		/// The properties names.
		/// </summary>
		protected List<string> Events = new List<string>();

		/// <summary>
		/// Cursors.
		/// </summary>
		protected List<string> Cursors = new List<string>();

		GUILayoutOption[] toggleOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			FillProperties();

			foreach (var p in Properties)
			{
				var property = serializedObject.FindProperty(p);
				if (property != null)
				{
					SerializedProperties[p] = property;
				}
			}

			foreach (var ev in Events)
			{
				var property = serializedObject.FindProperty(ev);
				if (property != null)
				{
					SerializedEvents[ev] = property;
				}
			}

			foreach (var c in Cursors)
			{
				var property = serializedObject.FindProperty(c);
				if (property != null)
				{
					SerializedCursors[c] = property;
				}
			}
		}

		/// <summary>
		/// Fills the properties list.
		/// </summary>
		protected virtual void FillProperties()
		{
			var property = serializedObject.GetIterator();
			property.NextVisible(true);
			while (property.NextVisible(false))
			{
				AddProperty(property);
			}
		}

		/// <summary>
		/// Add property.
		/// </summary>
		/// <param name="property">Property.</param>
		protected void AddProperty(SerializedProperty property)
		{
			if (Exclude.Contains(property.name))
			{
				return;
			}

			if (Properties.Contains(property.name))
			{
				return;
			}

			if (Events.Contains(property.name))
			{
				return;
			}

			if (Cursors.Contains(property.name))
			{
				return;
			}

			if (IsEvent(property))
			{
				Events.Add(property.name);
			}
			else
			{
				Properties.Add(property.name);
			}
		}

		/// <summary>
		/// Toggle cursors block.
		/// </summary>
		protected bool ShowCursors;

		/// <summary>
		/// Allow to show cursors block.
		/// </summary>
		protected bool AllowedCursors;

		/// <summary>
		/// Is it event?
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
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			serializedObject.Update();

			foreach (var sp in SerializedProperties)
			{
				EditorGUILayout.PropertyField(sp.Value, true);
			}

			if (AllowedCursors)
			{
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
			}

			foreach (var se in SerializedEvents)
			{
				EditorGUILayout.PropertyField(se.Value, true);
			}

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}
#endif