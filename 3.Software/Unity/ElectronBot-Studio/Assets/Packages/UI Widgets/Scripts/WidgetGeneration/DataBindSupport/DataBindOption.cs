#if UIWIDGETS_DATABIND_SUPPORT && UNITY_EDITOR
namespace UIWidgets.DataBindSupport
{
	using System;
	using System.Collections.Generic;
	using System.Reflection;
	using UIWidgets;
	using UIWidgets.Attributes;
	using UnityEngine.Events;

	/// <summary>
	/// DataBind option info.
	/// </summary>
	public class DataBindOption
	{
		/// <summary>
		/// Namespace.
		/// </summary>
		public string Namespace;

		/// <summary>
		/// Class name.
		/// </summary>
		public string ClassName;

		/// <summary>
		/// Short class name.
		/// </summary>
		public string ShortClassName;

		/// <summary>
		/// Field name.
		/// </summary>
		public string FieldName;

		/// <summary>
		/// Field type.
		/// </summary>
		public string FieldType;

		/// <summary>
		/// Can read field or property?
		/// </summary>
		public bool CanRead = true;

		/// <summary>
		/// Can write field or property?
		/// </summary>
		public bool CanWrite = true;

		/// <summary>
		/// Event field (or property) name -> list of parameter name
		/// </summary>
		public Dictionary<string, List<string>> Events;

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <returns>String.</returns>
		public override string ToString()
		{
			var events = string.Empty;

			foreach (var ev in Events.Keys)
			{
				events += ev + "(" + string.Join(",", Events[ev].ToArray()) + "); ";
			}

			return string.Format("{0}: {4} {1} -> {2} // {3}", ClassName, FieldName, events, ShortClassName, FieldType);
		}

		/// <summary>
		/// Get options for specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Options list.</returns>
		public static List<DataBindOption> GetOptions(Type type)
		{
			var result = new List<DataBindOption>();

			if (type == null)
			{
				return result;
			}

			if (type.GetCustomAttributes(typeof(DataBindSupportAttribute), true).Length == 0)
			{
				return result;
			}

			foreach (var field in type.GetFields())
			{
				var option = GetOption(type, field);
				if (option != null)
				{
					result.Add(option);
				}
			}

			foreach (var property in type.GetProperties())
			{
				var option = GetOption(type, property);
				if (option != null)
				{
					result.Add(option);
				}
			}

			return result;
		}

		static DataBindOption GetOption(Type type, FieldInfo field)
		{
			if (field.GetCustomAttributes(typeof(DataBindFieldAttribute), true).Length == 0)
			{
				return null;
			}

			var option = new DataBindOption()
			{
				Namespace = type.Namespace,
				ClassName = type.FullName,
				ShortClassName = type.Name,
				FieldName = field.Name,
				FieldType = UtilitiesEditor.GetFriendlyTypeName(field.FieldType),
				Events = GetEvents(type, field.Name),
			};

			return option;
		}

		static DataBindOption GetOption(Type type, PropertyInfo property)
		{
			if (property.GetCustomAttributes(typeof(DataBindFieldAttribute), true).Length == 0)
			{
				return null;
			}

			if (!property.CanRead)
			{
				return null;
			}

			var option = new DataBindOption()
			{
				Namespace = type.Namespace,
				ClassName = type.FullName,
				ShortClassName = type.Name,
				FieldName = property.Name,
				FieldType = UtilitiesEditor.GetFriendlyTypeName(property.PropertyType),
				CanRead = property.CanRead,
				CanWrite = property.CanWrite,
				Events = GetEvents(type, property.Name),
			};

			return option;
		}

		static Dictionary<string, List<string>> GetEvents(Type type, string fieldName)
		{
			var result = new Dictionary<string, List<string>>();

			foreach (var field in type.GetFields())
			{
				var parameters = GetEventParameters(field, fieldName);
				if (parameters != null)
				{
					result.Add(field.Name, parameters);
				}
			}

			foreach (var property in type.GetProperties())
			{
				var parameters = GetEventParameters(property, fieldName);
				if (parameters != null)
				{
					result.Add(property.Name, parameters);
				}
			}

			return result;
		}

		static List<string> GetEventParameters(MemberInfo type, string fieldName)
		{
			var field_type = (type is FieldInfo)
				? (type as FieldInfo).FieldType
				: (type as PropertyInfo).PropertyType;
			if (!typeof(UnityEventBase).IsAssignableFrom(field_type))
			{
				return null;
			}

			var attrs = type.GetCustomAttributes(typeof(DataBindEventAttribute), true);
			if (attrs.Length == 0)
			{
				return null;
			}

			foreach (var attr in attrs)
			{
				var data = attr as DataBindEventAttribute;
				if (data == null)
				{
					continue;
				}

				if (Array.IndexOf(data.Fields, fieldName) == -1)
				{
					continue;
				}

				var method = field_type.GetMethod("Invoke");

				var result = new List<string>();
				foreach (var p in method.GetParameters())
				{
					result.Add(UtilitiesEditor.GetFriendlyTypeName(p.ParameterType));
				}

				return result;
			}

			return null;
		}
	}
}
#endif