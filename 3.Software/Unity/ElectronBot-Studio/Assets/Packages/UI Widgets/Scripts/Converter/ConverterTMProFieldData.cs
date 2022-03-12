#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Reflection;
	using UnityEditor;

	/// <summary>
	/// Converter functions to replace component with another component.
	/// </summary>
	public partial class ConverterTMPro
	{
		/// <summary>
		/// Field data.
		/// </summary>
		public class FieldData
		{
			/// <summary>
			/// Field info.
			/// </summary>
			public FieldInfo Info
			{
				get;
				protected set;
			}

			/// <summary>
			/// Type.
			/// </summary>
			public Type Type
			{
				get;
				protected set;
			}

			/// <summary>
			/// Parent object.
			/// </summary>
			public object Object
			{
				get;
				protected set;
			}

			object fieldValue;

			/// <summary>
			/// Value of the field.
			/// </summary>
			public object Value
			{
				get
				{
					return fieldValue;
				}

				set
				{
					fieldValue = value;
					Info.SetValue(Object, value);
				}
			}

			/// <summary>
			/// Is field marked obsolete?
			/// </summary>
			public bool IsObsolete
			{
				get
				{
					if (Info == null)
					{
						return false;
					}

					return Info.GetCustomAttributes(typeof(ObsoleteAttribute), true).Length > 0;
				}
			}

			/// <summary>
			/// Is specified type is a base type of the field type?
			/// </summary>
			/// <typeparam name="T">Type.</typeparam>
			/// <returns>true if specified type is a base type of the field type; otherwise false.</returns>
			public bool TypeReplaceable<T>()
			{
				if (Type == null)
				{
					return false;
				}

				var type = typeof(T);

				if (Type.IsSubclassOf(type) || (Type == type))
				{
					return false;
				}

				return true;
			}

			static readonly char[] NameSeparator = new char[] { '.' };
			static readonly char[] ArrayTrimChars = new char[] { ']' };

			/// <summary>
			/// Initializes a new instance of the <see cref="UIWidgets.ConverterTMPro.FieldData"/> class.
			/// </summary>
			/// <param name="obj">Object.</param>
			/// <param name="type">Type of the object.</param>
			/// <param name="path">Path to field.</param>
			public FieldData(object obj, Type type, string path)
			{
				Object = obj;

				Type = type;
				var fields = path.Split(NameSeparator);
				for (int i = 0; i < fields.Length; i++)
				{
					var field = fields[i];
					if (field == "Array")
					{
						var index = int.Parse(fields[i + 1].Substring(5).TrimEnd(ArrayTrimChars));
						if (Type.IsGenericType && (Type.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>)))
						{
							var list = Object as IList;
							Object = list[index];
							Type = Type.GenericTypeArguments[0];
							i += 1;
							continue;
						}

						if (Type.IsArray)
						{
							var arr = Object as object[];
							Object = arr[index];
							Type = Type.GetElementType();
							i += 1;
							continue;
						}
					}

					Info = GetFieldInfo(Type, field);
					if (Info == null)
					{
						Type = null;
						return;
					}

					Type = Info.FieldType;

					if (i < (fields.Length - 1))
					{
						Object = Info.GetValue(Object);
					}
					else
					{
						fieldValue = Info.GetValue(Object);
					}
				}
			}

			static FieldInfo GetFieldInfo(Type type, string name)
			{
				var flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

				FieldInfo fi = null;
				while (type != null)
				{
					fi = type.GetField(name, flags);

					if (fi != null)
					{
						break;
					}

					type = type.BaseType;
				}

				return fi;
			}

			/// <summary>
			/// Get event data of the specified object.
			/// </summary>
			/// <typeparam name="T">Type of the object.</typeparam>
			/// <param name="source">Source object.</param>
			/// <param name="eventName">Event name.</param>
			/// <returns>Event listeners.</returns>
			public static object GetEventData<T>(T source, string eventName)
				where T : UnityEngine.Object
			{
				var serialized = new SerializedObject(source);
				var property = serialized.FindProperty(eventName + ".m_PersistentCalls");

				return GetValue(property);
			}

			/// <summary>
			/// Set event data to the specified object.
			/// </summary>
			/// <typeparam name="T">Type of the object.</typeparam>
			/// <param name="target">Target object.</param>
			/// <param name="eventName">Event name.</param>
			/// <param name="data">Event listeners.</param>
			public static void SetEventData<T>(T target, string eventName, object data)
				where T : UnityEngine.Object
			{
				var serialized = new SerializedObject(target);
				var property = serialized.FindProperty(eventName + ".m_PersistentCalls");

				SetValue(property, data);
			}

			static object GetValue(SerializedProperty property)
			{
				var target = property.serializedObject.targetObject;
				var type = target.GetType();
				var field = new FieldData(target, type, property.propertyPath);

				return field.Value;
			}

			static void SetValue(SerializedProperty property, object value)
			{
				var target = property.serializedObject.targetObject;
				var type = target.GetType();
				var field = new FieldData(target, type, property.propertyPath);

				field.Value = value;
			}
		}
	}
}
#endif