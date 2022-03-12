#if UIWIDGETS_DATABIND_SUPPORT && UNITY_EDITOR
namespace UIWidgets.DataBindSupport
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using UIWidgets;
	using UIWidgets.Extensions;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// DataBind scripts generator.
	/// </summary>
	public class DataBindGenerator : IFormattable
	{
		/// <summary>
		/// Data.
		/// </summary>
		protected DataBindOption Option;

		/// <summary>
		/// Path to save generated files.
		/// </summary>
		protected string SavePath;

		/// <summary>
		/// Events.
		/// </summary>
		protected List<DataBindEvents> Events = new List<DataBindEvents>();

		/// <summary>
		/// Initializes a new instance of the <see cref="DataBindGenerator"/> class.
		/// </summary>
		/// <param name="option">Data.</param>
		/// <param name="path">Path to save files.</param>
		public DataBindGenerator(DataBindOption option, string path)
		{
			Option = option;
			SavePath = path;

			foreach (var ev in Option.Events.Keys)
			{
				Events.Add(new DataBindEvents()
				{
					ClassName = Option.ShortClassName,
					EventName = ev,
					Arguments = Option.Events[ev],
				});
			}
		}

		/// <summary>
		/// Create support scripts for the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <param name="path">Path to save scripts.</param>
		public static void Run(Type type, string path)
		{
			var options = DataBindOption.GetOptions(type);

			foreach (var option in options)
			{
				var gen = new DataBindGenerator(option, path);
				gen.Generate();
			}
		}

		/// <summary>
		/// Check is Data Bind support can be added to the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>true if support can be added; otherwise false.</returns>
		public static bool IsValidType(Type type)
		{
			var options = DataBindOption.GetOptions(type);

			return options.Count > 0;
		}

		/// <summary>
		/// Generate scripts files.
		/// </summary>
		public virtual void Generate()
		{
			var templates = DataBindTemplates.Instance;

			if (Option.CanWrite)
			{
				CreateScript("Setter", templates.Setter);
			}

			if (Option.Events.Count > 0)
			{
				CreateScript("Provider", templates.Provider);
				CreateScript("Observer", templates.Observer);
			}

			if (Option.CanWrite && (Option.Events.Count > 0))
			{
				CreateScript("Synchronizer", templates.Synchronizer);
			}

			AssetDatabase.Refresh();
		}

		/// <summary>
		/// Get class name.
		/// </summary>
		/// <param name="type">Class type.</param>
		/// <returns>Class name.</returns>
		protected virtual string GetClassName(string type)
		{
			return Option.ShortClassName + Option.FieldName + type;
		}

		/// <summary>
		/// Create script by the specified template.
		/// </summary>
		/// <param name="type">Template type.</param>
		/// <param name="template">Template.</param>
		protected virtual void CreateScript(string type, TextAsset template)
		{
			var filepath = SavePath + Path.DirectorySeparatorChar + GetClassName(type) + ".cs";
			if (!CanCreateFile(filepath))
			{
				return;
			}

			var code = string.Format(template.text, this);

			File.WriteAllText(filepath, code);
		}

		/// <summary>
		/// Check if file can be created at the specified path.
		/// </summary>
		/// <param name="filepath">Path to file.</param>
		/// <returns>True if file can be created; otherwise false.</returns>
		protected virtual bool CanCreateFile(string filepath)
		{
			if (!File.Exists(filepath))
			{
				return true;
			}

			return EditorUtility.DisplayDialog(
				"Overwrite existing file?",
				"Overwrite existing file " + filepath + "?",
				"Overwrite",
				"Skip");
		}

		/// <summary>
		/// Convert this instance to string.
		/// </summary>
		/// <param name="format">Format.</param>
		/// <param name="formatProvider">Format provider.</param>
		/// <returns>String.</returns>
		public string ToString(string format, IFormatProvider formatProvider)
		{
			switch (format)
			{
				case "Namespace":
					return string.IsNullOrEmpty(Option.Namespace) ? "DataBind" : Option.Namespace + ".DataBind";
				case "TargetFullName":
					return Option.ClassName;
				case "TargetShortName":
					return Option.ShortClassName;
				case "FieldName":
					return Option.FieldName;
				case "FieldType":
					return Option.FieldType;
				default:
					var pos = format.IndexOf("@");
					if (pos != -1)
					{
						var template = format.Substring(pos + 1).Replace("[", "{").Replace("]", "}");
						return Events.ToString(template, formatProvider);
					}
					else
					{
						throw new ArgumentOutOfRangeException("Unsupported format: " + format);
					}
			}
		}
	}
}
#endif