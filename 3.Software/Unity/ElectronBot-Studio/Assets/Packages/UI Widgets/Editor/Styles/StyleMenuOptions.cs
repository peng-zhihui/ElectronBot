namespace UIWidgets.Styles
{
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class StyleMenuOptions
	{
		/// <summary>
		/// Creates the style.
		/// </summary>
		[MenuItem("Assets/Create/New UI Widgets/Style", false)]
		public static void CreateStyle()
		{
			var folder = "Assets";
			if (Selection.activeObject != null)
			{
				folder = AssetDatabase.GetAssetPath(Selection.activeObject);
				if (!System.IO.Directory.Exists(folder))
				{
					folder = System.IO.Path.GetDirectoryName(folder);
				}
			}

			var path = folder + "/New UI Widgets Style.asset";
			var file = AssetDatabase.GenerateUniqueAssetPath(path);
			var style = ScriptableObject.CreateInstance<Style>();
			style.Upgrade();

			AssetDatabase.CreateAsset(style, file);
			EditorUtility.SetDirty(style);
			AssetDatabase.SaveAssets();

			style.SetDefaultValues();
			EditorUtility.SetDirty(style);
			AssetDatabase.SaveAssets();

			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(file);
		}

		/// <summary>
		/// Apply the default style.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Apply Default Style", false, 10)]
		public static void ApplyDefaultStyle()
		{
			var style = PrefabsMenu.Instance.DefaultStyle;
			if (style == null)
			{
				Debug.LogWarning("Default style not found.");
				return;
			}

			var target = Selection.activeGameObject;
			if (target == null)
			{
				return;
			}

			Undo.RegisterFullObjectHierarchyUndo(target, "Apply Style");
			style.ApplyTo(target);
			RecordPrefabInstanceModifications(target);
		}

		static readonly List<Component> Components = new List<Component>();

		/// <summary>
		/// Record prefab instance modifications.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void RecordPrefabInstanceModifications(GameObject target)
		{
#if UNITY_2018_3_OR_NEWER
			if (PrefabUtility.IsPartOfAnyPrefab(target))
#endif
			{
				PrefabUtility.RecordPrefabInstancePropertyModifications(target);

				target.GetComponents(Components);

				foreach (var c in Components)
				{
					PrefabUtility.RecordPrefabInstancePropertyModifications(c);
				}

				Components.Clear();
			}

			var t = target.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				RecordPrefabInstanceModifications(t.GetChild(i).gameObject);
			}
		}

		/// <summary>
		/// Check is selected object is not null.
		/// </summary>
		/// <returns><c>true</c>, if selected object is not null, <c>false</c> otherwise.</returns>
		[MenuItem("GameObject/UI/New UI Widgets/Apply Default Style", true, 10)]
		public static bool CanApplyStyle()
		{
			return Selection.activeGameObject != null;
		}

		/// <summary>
		/// Update the default style.
		/// </summary>
		[MenuItem("GameObject/UI/New UI Widgets/Update Default Style", false, 10)]
		public static void UpdateDefaultStyle()
		{
			var style = PrefabsMenu.Instance.DefaultStyle;
			if (style == null)
			{
				Debug.LogWarning("Default style not found.");
				return;
			}

			var target = Selection.activeGameObject;
			if (target == null)
			{
				return;
			}

			Undo.RecordObject(style, "Update Style");
			style.GetFrom(target);
			EditorUtility.SetDirty(style);
		}

		/// <summary>
		/// Check is selected object is not null.
		/// </summary>
		/// <returns><c>true</c>, if selected object is not null, <c>false</c> otherwise.</returns>
		[MenuItem("GameObject/UI/New UI Widgets/Update Default Style", true, 10)]
		public static bool CanUpdateStyle()
		{
			return Selection.activeGameObject != null;
		}
	}
}