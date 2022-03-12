#if UNITY_EDITOR
namespace UIWidgets.WidgetGeneration
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Reflection;
	using UnityEngine;

	/// <summary>
	/// Class field or property info.
	/// </summary>
	[Serializable]
	public class ClassField : IFormattable
	{
		/// <summary>
		/// Field name.
		/// </summary>
		public string FieldName;

		/// <summary>
		/// Field type name.
		/// </summary>
		public string FieldTypeName;

		/// <summary>
		/// Field type.
		/// </summary>
		protected Type fieldType;

		/// <summary>
		/// Field type.
		/// </summary>
		public Type FieldType
		{
			get
			{
				return fieldType;
			}

			protected set
			{
				fieldType = value;
				SetWidgetByType(value);
			}
		}

		/// <summary>
		/// Is field readable?
		/// </summary>
		public bool CanRead = true;

		/// <summary>
		/// Is field writable?
		/// </summary>
		public bool CanWrite = true;

		/// <summary>
		/// Widget class.
		/// </summary>
		public Type WidgetClass;

		/// <summary>
		/// Widget name to display the current field.
		/// </summary>
		public string WidgetFieldName;

		/// <summary>
		/// Widget field to set value.
		/// </summary>
		public string WidgetValueField;

		/// <summary>
		/// Field format.
		/// </summary>
		public string FieldFormat = string.Empty;

		/// <summary>
		/// Is text field?
		/// </summary>
		public bool IsText
		{
			get;
			protected set;
		}

		/// <summary>
		/// Is Image field?
		/// </summary>
		public bool IsImage
		{
			get;
			protected set;
		}

		/// <summary>
		/// Is field can be null?
		/// </summary>
		public bool IsNullable
		{
			get;
			protected set;
		}

		static readonly Type[] PrintableTypes =
		{
			typeof(Enum),
			typeof(string),
			typeof(decimal),
			typeof(DateTime),
			typeof(DateTimeOffset),
			typeof(TimeSpan),
		};

		static readonly Type[] ImageTypes =
		{
			typeof(Sprite),
			typeof(Texture2D),
			typeof(Color),
			typeof(Color32),
		};

		/// <summary>
		/// Initializes a new instance of the <see cref="ClassField"/> class.
		/// </summary>
		protected ClassField()
		{
		}

		/// <summary>
		/// Set widget field data by type.
		/// </summary>
		/// <param name="type">Type.</param>
		protected void SetWidgetByType(Type type)
		{
			if (Array.IndexOf(ImageTypes, type) != -1)
			{
				if (type == typeof(Sprite))
				{
					WidgetClass = typeof(UnityEngine.UI.Image);
					WidgetValueField = "sprite";

					IsText = false;
					IsImage = true;
					IsNullable = true;
				}
				else if (type == typeof(Texture2D))
				{
					WidgetClass = typeof(UnityEngine.UI.RawImage);
					WidgetValueField = "texture";

					IsText = false;
					IsImage = true;
					IsNullable = true;
				}
				else if ((type == typeof(Color)) || (type == typeof(Color32)))
				{
					WidgetClass = typeof(UnityEngine.UI.Image);
					WidgetValueField = "color";

					IsText = false;
					IsImage = true;
					IsNullable = false;
				}
			}
			else
			{
				WidgetClass = typeof(TextAdapter);
				if (FieldName == "TextAdapter")
				{
					WidgetFieldName = FieldName + "Field";
				}
				else if (FieldName == "Text")
				{
					WidgetFieldName = "TextAdapter";
				}

				WidgetValueField = "text";

				if (type != typeof(string))
				{
					FieldFormat = ".ToString()";
				}

				IsText = true;
				IsImage = false;
				IsNullable = true;
			}

			// rename widget field name if such field already exists
			if (HasDefinition(WidgetFieldName))
			{
				WidgetFieldName += "Field";
			}
		}

		/// <summary>
		/// Prohibited field names.
		/// </summary>
		protected static HashSet<string> ProhibitedNames;

		/// <summary>
		/// Check is the specified field, property or method already exists.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <returns>true if specified field, property or method exists; otherwise false.</returns>
		protected static bool HasDefinition(string name)
		{
			if (ProhibitedNames == null)
			{
				ProhibitedNames = new System.Collections.Generic.HashSet<string>();

				var type = typeof(TreeViewComponentBase<>);
				var flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
				foreach (var field in type.GetFields(flags))
				{
					ProhibitedNames.Add(field.Name);
				}

				foreach (var property in type.GetProperties(flags))
				{
					ProhibitedNames.Add(property.Name);
				}

				foreach (var method in type.GetMethods(flags))
				{
					ProhibitedNames.Add(method.Name);
				}

				// special cases for the TreeViewComponent
				ProhibitedNames.Remove("Icon");
				ProhibitedNames.Remove("TextAdapter");
			}

			return ProhibitedNames.Contains(name);
		}

		/// <summary>
		/// Is type can used to create ClassField?
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Tur if type is allowed; otherwise false.</returns>
		protected static bool IsAllowedType(Type type)
		{
			if (type.IsPrimitive || Array.IndexOf(PrintableTypes, type) != -1 || Array.IndexOf(ImageTypes, type) != -1)
			{
				return true;
			}

			if (type.GetMethod("ToString", Type.EmptyTypes).DeclaringType == typeof(UnityEngine.Object))
			{
				return false;
			}

			if (typeof(IFormattable).IsAssignableFrom(type))
			{
				return true;
			}

			if (type.GetMethod("ToString").DeclaringType != typeof(object))
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Create ClassField from specified field.
		/// </summary>
		/// <param name="field">Field.</param>
		/// <returns>ClassField instance if field type is allowed; otherwise null.</returns>
		public static ClassField Create(FieldInfo field)
		{
			if (!IsAllowedType(field.FieldType))
			{
				return null;
			}

			var result = new ClassField()
			{
				FieldName = field.Name,
				WidgetFieldName = field.Name,
				FieldTypeName = UtilitiesEditor.GetFriendlyTypeName(field.FieldType),
				FieldType = field.FieldType,
			};

			return result;
		}

		/// <summary>
		/// Create ClassField from specified property.
		/// </summary>
		/// <param name="property">Property.</param>
		/// <returns>ClassField instance if property type is allowed; otherwise null.</returns>
		public static ClassField Create(PropertyInfo property)
		{
			if (!property.CanRead)
			{
				return null;
			}

			if (!IsAllowedType(property.PropertyType))
			{
				return null;
			}

			var result = new ClassField()
			{
				FieldName = property.Name,
				WidgetFieldName = property.Name,
				FieldTypeName = UtilitiesEditor.GetFriendlyTypeName(property.PropertyType),
				FieldType = property.PropertyType,
				CanRead = property.CanRead,
				CanWrite = property.CanWrite,
			};

			return result;
		}

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <returns>The value of the current instance in the specified format.</returns>
		public string ToString(string format)
		{
			return ToString(format, CultureInfo.CurrentCulture);
		}

		/// <summary>
		/// Formats the value of the current instance using the specified format.
		/// </summary>
		/// <param name="format">The format to use.</param>
		/// <param name="formatProvider">The provider to use to format the value.</param>
		/// <returns>The value of the current instance in the specified format.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			switch (format)
			{
				case "FieldName":
					return FieldName;
				case "WidgetFieldName":
					return WidgetFieldName;
				case "WidgetClass":
					return UtilitiesEditor.GetFriendlyTypeName(WidgetClass);
				case "WidgetValueField":
					return WidgetValueField;
				case "FieldFormat":
					return FieldFormat;
				default:
					throw new ArgumentOutOfRangeException("Unsupported format: " + format);
			}
		}
	}
}
#endif