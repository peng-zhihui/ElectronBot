#if UIWIDGETS_DATABIND_SUPPORT && UNITY_EDITOR
namespace UIWidgets.DataBindSupport
{
	using System;
	using System.IO;
	using UnityEditor;

	/// <summary>
	/// Menu options.
	/// </summary>
	class MenuOptions
	{
		static string GetPath()
		{
			return Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
		}

		/// <summary>
		/// Add DataBind support to the selected script.
		/// </summary>
		[MenuItem("Assets/Create/New UI Widgets/Add Data Bind support", false)]
		public static void AddDataBindUISupport()
		{
			DataBindGenerator.Run(GetSelectedType(), GetPath());
		}

		/// <summary>
		/// Is can add DataBind support to the selected script?
		/// </summary>
		/// <returns>true if DataBind support possible; otherwise false.</returns>
		[MenuItem("Assets/Create/New UI Widgets/Add Data Bind support", true)]
		public static bool AddDataBindUISupportValidation()
		{
			return DataBindGenerator.IsValidType(GetSelectedType());
		}

		static Type GetSelectedType()
		{
			if (Selection.activeObject == null)
			{
				return null;
			}

			var type = Selection.activeObject as MonoScript;
			if (type == null)
			{
				return null;
			}

			return type.GetClass();
		}
	}
}
#endif