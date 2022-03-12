namespace UIWidgets
{
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Menu options.
	/// </summary>
	public static class CursorsMenuOptions
	{
		/// <summary>
		/// Creates the style.
		/// </summary>
		[MenuItem("Assets/Create/New UI Widgets/Cursors", false)]
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

			var path = folder + "/New UI Widgets Cursors.asset";
			var file = AssetDatabase.GenerateUniqueAssetPath(path);
			var style = ScriptableObject.CreateInstance<Cursors>();

			AssetDatabase.CreateAsset(style, file);
			EditorUtility.SetDirty(style);
			AssetDatabase.SaveAssets();

			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(file);
		}
	}
}