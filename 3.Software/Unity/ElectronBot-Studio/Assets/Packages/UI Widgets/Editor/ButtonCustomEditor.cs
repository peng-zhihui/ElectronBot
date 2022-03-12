#if UNITY_EDITOR
namespace UIWidgets
{
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEditor.UI;
	using UnityEngine.Events;

	/// <summary>
	/// Custom editor for buttons.
	/// </summary>
	public class ButtonCustomEditor : ButtonEditor
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
		/// The properties to exclude.
		/// </summary>
		protected List<string> Exclude = new List<string>()
		{
			"m_Navigation",
			"m_Transition",
			"m_Colors",
			"m_SpriteState",
			"m_AnimationTriggers",
			"m_Interactable",
			"m_TargetGraphic",
			"m_OnClick",
		};

		/// <summary>
		/// The properties names.
		/// </summary>
		protected List<string> Properties = new List<string>();

		/// <summary>
		/// The properties names.
		/// </summary>
		protected List<string> Events = new List<string>();

		/// <summary>
		/// Init.
		/// </summary>
		protected override void OnEnable()
		{
			FillProperties();

			foreach (var p in Properties)
			{
				var property = serializedObject.FindProperty(p);
				if (property != null)
				{
					SerializedProperties.Add(p, property);
				}
			}

			foreach (var ev in Events)
			{
				var property = serializedObject.FindProperty(ev);
				if (property != null)
				{
					SerializedEvents.Add(ev, property);
				}
			}

			base.OnEnable();
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
		/// Validate targets.
		/// </summary>
		protected virtual void ValidateTargets()
		{
			foreach (var t in targets)
			{
				var v = t as IValidateable;
				if (v != null)
				{
					v.Validate();
				}
			}
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			ValidateTargets();

			foreach (var sp in SerializedProperties)
			{
				EditorGUILayout.PropertyField(sp.Value);
			}

			serializedObject.ApplyModifiedProperties();

			base.OnInspectorGUI();

			foreach (var se in SerializedEvents)
			{
				EditorGUILayout.PropertyField(se.Value);
			}

			serializedObject.ApplyModifiedProperties();

			ValidateTargets();
		}
	}
}
#endif