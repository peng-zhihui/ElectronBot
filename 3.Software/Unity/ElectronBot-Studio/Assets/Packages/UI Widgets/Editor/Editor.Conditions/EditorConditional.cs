#if UNITY_EDITOR
namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UIWidgets.Attributes;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.Events;

	/// <summary>
	/// Conditional editor.
	/// Fields displayed only if match attribute conditions.
	/// </summary>
	public class EditorConditional : Editor
	{
		/// <summary>
		/// Fields block data.
		/// </summary>
		public class FieldsBlock
		{
			/// <summary>
			/// Is block visible.
			/// </summary>
			public bool Visible = false;

			/// <summary>
			/// Block name.
			/// </summary>
			public string Name;

			/// <summary>
			/// Block fields.
			/// </summary>
			public Dictionary<string, SerializedProperty> Fields = new Dictionary<string, SerializedProperty>();
		}

		/// <summary>
		/// Conditions to display field.
		/// </summary>
		public class DisplayConditions
		{
			/// <summary>
			/// Field name.
			/// </summary>
			public string Name;

			/// <summary>
			/// List of the conditions to display field.
			/// </summary>
			public List<IEditorCondition> Conditions;

			/// <summary>
			/// Initializes a new instance of the <see cref="DisplayConditions"/> class.
			/// </summary>
			/// <param name="name">Field name.</param>
			public DisplayConditions(string name)
			{
				Name = name;
				Conditions = new List<IEditorCondition>();
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="DisplayConditions"/> class.
			/// </summary>
			/// <param name="name">Field name.</param>
			/// <param name="conditions">List of the conditions to display field.</param>
			public DisplayConditions(string name, List<IEditorCondition> conditions)
			{
				Name = name;
				Conditions = conditions;
			}

			/// <summary>
			/// Check if field can be displayed.
			/// </summary>
			/// <param name="properties">Properties.</param>
			/// <returns>true if field should be displayed; otherwise false.</returns>
			public bool IsValid(Dictionary<string, SerializedProperty> properties)
			{
				foreach (var condition in Conditions)
				{
					if (!properties.ContainsKey(condition.Field))
					{
						Debug.LogWarning(string.Format("Field \"{0}\" referenced to non existing field \"{1}\"; conditional check ignored.", Name, condition.Field));
						continue;
					}

					var property = properties[condition.Field];
					if (!condition.IsValid(property))
					{
						return false;
					}
				}

				return true;
			}
		}

		/// <summary>
		/// Serialized object data.
		/// </summary>
		public class TypeData
		{
			/// <summary>
			/// Serialized object.
			/// </summary>
			protected SerializedObject SerializedObject
			{
				get;
				set;
			}

			/// <summary>
			/// Serialized property.
			/// </summary>
			protected SerializedProperty SerializedProperty
			{
				get;
				set;
			}

			UnityEngine.Object target;

			/// <summary>
			/// Target.
			/// </summary>
			public UnityEngine.Object Target
			{
				get
				{
					return target;
				}

				set
				{
					if (target != value)
					{
						target = value;
						SerializedObject = (target != null) ? new SerializedObject(target) : null;

						Init();
					}
				}
			}

			Type targetType;

			/// <summary>
			/// Type of the editable object.
			/// </summary>
			protected Type TargetType
			{
				get
				{
					return targetType;
				}

				set
				{
					if (targetType != value)
					{
						targetType = value;

						DetectFields();

						foreach (var f in Fields)
						{
							AddField(f);
						}

						foreach (var f in Events)
						{
							AddEvent(f);
						}
					}
				}
			}

			/// <summary>
			/// Fields blocks.
			/// </summary>
			protected Dictionary<string, FieldsBlock> Blocks = new Dictionary<string, FieldsBlock>();

			/// <summary>
			/// Nested objects.
			/// </summary>
			protected Dictionary<string, TypeData> NestedObjects = new Dictionary<string, TypeData>();

			/// <summary>
			/// Serialized properties.
			/// </summary>
			protected Dictionary<string, SerializedProperty> SerializedProperties = new Dictionary<string, SerializedProperty>();

			/// <summary>
			/// Serialized events.
			/// </summary>
			protected Dictionary<string, SerializedProperty> SerializedEvents = new Dictionary<string, SerializedProperty>();

			/// <summary>
			/// Display conditions for the properties.
			/// </summary>
			protected Dictionary<string, DisplayConditions> PropertyDisplayConditions = new Dictionary<string, DisplayConditions>();

			/// <summary>
			/// Properties.
			/// </summary>
			protected List<string> Fields = new List<string>();

			/// <summary>
			/// Events.
			/// </summary>
			protected List<string> Events = new List<string>();

			/// <summary>
			/// Toggle events block.
			/// </summary>
			protected bool ShowEvents;

			/// <summary>
			/// Allowed fields.
			/// </summary>
			protected Func<SerializedProperty, bool> AllowedProperties = AllowAll;

			/// <summary>
			/// Name.
			/// </summary>
			protected string Name;

			/// <summary>
			/// Is array?
			/// </summary>
			protected bool IsArray = false;

			static readonly GUILayoutOption[] ToggleOptions = new GUILayoutOption[] { GUILayout.ExpandWidth(true) };

			static readonly Func<SerializedProperty, bool> AllowAll = x => true;

			/// <summary>
			/// Initializes a new instance of the <see cref="TypeData"/> class.
			/// </summary>
			/// <param name="obj">Serialized object.</param>
			/// <param name="allowedProperties">Allowed properties.</param>
			public TypeData(SerializedObject obj, Func<SerializedProperty, bool> allowedProperties = null)
			{
				if (allowedProperties != null)
				{
					AllowedProperties = allowedProperties;
				}

				Name = "null";
				if (SerializedObject != null)
				{
					if (SerializedObject.targetObject != null)
					{
						Name = SerializedObject.targetObject.name;
					}
				}

				SerializedObject = obj;
				target = SerializedObject.targetObject;
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="TypeData"/> class.
			/// </summary>
			/// <param name="property">Serialized property.</param>
			/// <param name="allowedProperties">Allowed properties.</param>
			public TypeData(SerializedProperty property, Func<SerializedProperty, bool> allowedProperties)
			{
				property = property.serializedObject.FindProperty(property.propertyPath);

				if (allowedProperties != null)
				{
					AllowedProperties = allowedProperties;
				}

				if (property.propertyType == SerializedPropertyType.ObjectReference)
				{
					Name = property.propertyPath;
					target = property.objectReferenceValue;
					SerializedObject = (target != null) ? new SerializedObject(target) : null;
				}
			}

			/// <summary>
			/// Initializes a new instance of the <see cref="TypeData"/> class.
			/// </summary>
			/// <param name="property">Serialized property.</param>
			/// <param name="allowedProperties">Allowed properties.</param>
			/// <param name="type">Property type.</param>
			/// <param name="isArray">Is array?</param>
			public TypeData(SerializedProperty property, Func<SerializedProperty, bool> allowedProperties, Type type, bool isArray)
			{
				property = property.serializedObject.FindProperty(property.propertyPath);

				if (allowedProperties != null)
				{
					AllowedProperties = allowedProperties;
				}

				Name = property.propertyPath;
				SerializedProperty = property;
				IsArray = isArray;
				TargetType = type;
			}

			/// <summary>
			/// Update data with according property object reference.
			/// </summary>
			/// <param name="property">Property.</param>
			public virtual void Update(SerializedProperty property)
			{
				if (property.propertyType == SerializedPropertyType.ObjectReference)
				{
					Target = property.objectReferenceValue;
				}
			}

			/// <summary>
			/// Init this instance.
			/// </summary>
			public virtual void Init()
			{
				if (SerializedObject != null)
				{
					TargetType = SerializedObject.targetObject.GetType();
				}
				else if (SerializedProperty != null)
				{
					// already specified
				}
				else
				{
					TargetType = null;
				}
			}

			/// <summary>
			/// Find fields.
			/// </summary>
			protected virtual void DetectFields()
			{
				Blocks.Clear();
				SerializedProperties.Clear();
				SerializedEvents.Clear();
				NestedObjects.Clear();
				PropertyDisplayConditions.Clear();

				Events.Clear();
				Fields.Clear();

				if (SerializedProperty != null)
				{
					var temp = SerializedProperty.serializedObject.FindProperty(SerializedProperty.propertyPath);
					foreach (var x in temp)
					{
						var p = x as SerializedProperty;
						AddField(p);

						var condition = IsArray
							? new DisplayConditions(p.name)
							: GetCondition(p.name);
						var name = IsArray ? p.propertyPath : p.name;
						PropertyDisplayConditions[name] = condition;
					}
				}
				else if (SerializedObject != null)
				{
					var property = SerializedObject.GetIterator();
					property.NextVisible(true);
					while (property.NextVisible(false))
					{
						if (!AllowedProperties(property))
						{
							continue;
						}

						if (IsEvent(property))
						{
							Events.Add(property.propertyPath);
						}
						else
						{
							AddField(property);
						}

						var condition = GetCondition(property.name);
						PropertyDisplayConditions[property.propertyPath] = condition;
					}
				}
			}

			/// <summary>
			/// Add field.
			/// </summary>
			/// <param name="property">Property.</param>
			protected virtual void AddField(SerializedProperty property)
			{
				Fields.Add(IsArray ? property.propertyPath : property.name);

				var nested = GetNested(property);
				if (nested != null)
				{
					nested.Init();

					NestedObjects[property.propertyPath] = nested;
				}
			}

			/// <summary>
			/// Get nested object data for the specified property.
			/// </summary>
			/// <param name="property">Property.</param>
			/// <returns>Object data.</returns>
			protected virtual TypeData GetNested(SerializedProperty property)
			{
				if (IsArray)
				{
					return new TypeData(property, AllowedProperties, TargetType, false);
				}

				var field = GetField(TargetType, property.name);
				if (field == null)
				{
					return null;
				}

				var attrs = field.GetCustomAttributes(typeof(NestedInspectorAttribute), true);
				if (attrs.Length == 0)
				{
					return null;
				}

				if (property.propertyType == SerializedPropertyType.ObjectReference)
				{
					return new TypeData(property, AllowedProperties);
				}

				if (property.isArray)
				{
					// return new TypeData(property, AllowedProperties, field.FieldType.GetGenericArguments()[0], true);
					return null;
				}

				return null;
			}

			/// <summary>
			/// Get field info from the specified type by name.
			/// </summary>
			/// <param name="type">Type.</param>
			/// <param name="name">Field name.</param>
			/// <returns>Field info.</returns>
			protected static FieldInfo GetField(Type type, string name)
			{
				FieldInfo field;

				var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
				while ((field = type.GetField(name, flags)) == null && (type = type.BaseType) != null)
				{
					// no nothing
				}

				return field;
			}

			/// <summary>
			/// Get conditions for the specified field.
			/// </summary>
			/// <param name="name">Field name.</param>
			/// <returns>Conditions.</returns>
			protected DisplayConditions GetCondition(string name)
			{
				var field = GetField(TargetType, name);
				var conditions = new List<IEditorCondition>();

				if (field != null)
				{
					var attrs = field.GetCustomAttributes(typeof(IEditorCondition), true);
					foreach (var a in attrs)
					{
						conditions.Add(a as IEditorCondition);
					}
				}
				else
				{
					Debug.LogWarning("Field " + name + " is not found in the class " + UtilitiesEditor.GetFriendlyTypeName(TargetType));
				}

				return new DisplayConditions(name, conditions);
			}

			/// <summary>
			/// Get block name for the field.
			/// </summary>
			/// <param name="name">Field name.</param>
			/// <returns>Block name.</returns>
			protected string GetBlockName(string name)
			{
				var field = GetField(TargetType, name);
				if (field != null)
				{
					var attrs = field.GetCustomAttributes(typeof(EditorConditionBlockAttribute), true);
					foreach (var a in attrs)
					{
						var attr = a as EditorConditionBlockAttribute;
						if (attr != null)
						{
							return attr.Block;
						}
					}
				}
				else
				{
					Debug.LogWarning("Field " + name + " is not found in the class " + UtilitiesEditor.GetFriendlyTypeName(TargetType));
				}

				return string.Empty;
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
			/// Add property.
			/// </summary>
			/// <param name="name">Property name.</param>
			protected void AddField(string name)
			{
				SerializedProperty property = null;

				if (SerializedObject != null)
				{
					property = SerializedObject.FindProperty(name);
				}
				else if (SerializedProperty != null)
				{
					property = IsArray
						? SerializedProperty.serializedObject.FindProperty(name)
						: SerializedProperty.FindPropertyRelative(name);
				}

				if (property != null)
				{
					var block_name = IsArray ? null : GetBlockName(name);

					if (string.IsNullOrEmpty(block_name))
					{
						SerializedProperties[name] = property;
					}
					else
					{
						GetBlock(block_name).Fields[name] = property;
					}
				}
			}

			/// <summary>
			/// Add event.
			/// </summary>
			/// <param name="name">Event name.</param>
			protected void AddEvent(string name)
			{
				if (SerializedObject == null)
				{
					return;
				}

				var property = SerializedObject.FindProperty(name);
				if (property != null)
				{
					GetBlock("Events").Fields[name] = property;
				}
			}

			/// <summary>
			/// Get block by name.
			/// </summary>
			/// <param name="name">Name.</param>
			/// <returns>Block.</returns>
			protected FieldsBlock GetBlock(string name)
			{
				if (!Blocks.ContainsKey(name))
				{
					Blocks[name] = new FieldsBlock() { Name = name, };
				}

				return Blocks[name];
			}

			/// <summary>
			/// Show property.
			/// </summary>
			/// <param name="name">Property name.</param>
			/// <param name="property">Property.</param>
			protected void ShowProperty(string name, SerializedProperty property)
			{
				TypeData nested;

				var condition = PropertyDisplayConditions[name];
				var is_visible = condition.IsValid(SerializedProperties);
				if (is_visible)
				{
					var indent = condition.Conditions.Count;
					var has_nested = NestedObjects.TryGetValue(name, out nested);

					EditorGUI.indentLevel += indent;
					EditorGUILayout.PropertyField(property, !has_nested);

					if (has_nested)
					{
						EditorGUI.indentLevel += 1;
						nested.Update(property);
						nested.GUI();
						EditorGUI.indentLevel -= 1;
					}

					EditorGUI.indentLevel -= indent;
				}
			}

			/// <summary>
			/// Draw inspector GUI.
			/// </summary>
			public void GUI()
			{
				if (SerializedObject != null)
				{
					SerializedObject.Update();

					CommonGUI();

					SerializedObject.ApplyModifiedProperties();
				}
				else
				{
					CommonGUI();
				}
			}

			/// <summary>
			/// Common GUI.
			/// </summary>
			protected void CommonGUI()
			{
				foreach (var property in SerializedProperties)
				{
					ShowProperty(property.Key, property.Value);
				}

				foreach (var block in Blocks.Values)
				{
					block.Visible = GUILayout.Toggle(block.Visible, block.Name, EditorStyles.foldout, ToggleOptions);
					if (block.Visible)
					{
						EditorGUI.indentLevel++;
						foreach (var property in block.Fields)
						{
							ShowProperty(property.Key, property.Value);
						}

						EditorGUI.indentLevel--;
					}
				}
			}
		}

		/// <summary>
		/// Target.
		/// </summary>
		protected TypeData Target;

		/// <summary>
		/// Init.
		/// </summary>
		protected virtual void OnEnable()
		{
			Target = new TypeData(serializedObject);
			Target.Init();
		}

		/// <summary>
		/// Draw inspector GUI.
		/// </summary>
		public override void OnInspectorGUI()
		{
			Target.GUI();
		}
	}
}
#endif