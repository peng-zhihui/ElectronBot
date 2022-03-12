#if UNITY_EDITOR
namespace UIWidgets.WidgetGeneration
{
	using System.IO;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Widgets generation window.
	/// </summary>
	public class WidgetsGenerationWindow : EditorWindow
	{
		/// <summary>
		/// Show window.
		/// </summary>
		[MenuItem("Window/New UI Widgets/Widgets Generation")]
		public static void Open()
		{
			Open(null, null);
		}

		/// <summary>
		/// Show window.
		/// </summary>
		/// <param name="script">Script.</param>
		/// <param name="info">Class info.</param>
		public static void Open(MonoScript script, ClassInfo info)
		{
			var window = GetWindow<WidgetsGenerationWindow>("Widgets Generation");
			window.minSize = new Vector2(520, 200);
			window.currentScript = script;
			window.info = info;
		}

		readonly GUIStyle styleLabel = new GUIStyle();

		MonoScript previousScript;

		MonoScript currentScript;

		string previousType;

		string currentType;

		Vector2 scrollPosition;

		ClassInfo info;

		GUILayoutOption[] scrollOptions = new GUILayoutOption[] { GUILayout.Height(100) };

		GUILayoutOption[] errorOptions = new GUILayoutOption[] { GUILayout.ExpandHeight(true), GUILayout.MaxHeight(100) };

		/// <summary>
		/// Set styles.
		/// </summary>
		protected virtual void SetStyles()
		{
			styleLabel.margin = new RectOffset(4, 4, 2, 2);
			styleLabel.richText = true;
		}

		/// <summary>
		/// Draw GUI.
		/// </summary>
		protected virtual void OnGUI()
		{
			SetStyles();

			GUILayout.Label("Widgets Generation", EditorStyles.boldLabel);
			currentScript = EditorGUILayout.ObjectField("Data Script", currentScript, typeof(MonoScript), false, new GUILayoutOption[] { }) as MonoScript;

			if ((previousScript != currentScript) || (info == null))
			{
				info = new ClassInfo(currentScript);
				previousScript = currentScript;

				previousType = info.FullTypeName;
				currentType = info.FullTypeName;
			}

			currentType = EditorGUILayout.TextField("Data Type", currentType);

			if (previousType != currentType)
			{
				info = new ClassInfo(currentType);
				previousType = currentType;

				currentScript = null;
				previousScript = null;
			}

			if (!info.IsValid)
			{
				GUILayout.Label("<b>Errors:</b>", styleLabel);
				scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, scrollOptions);
				foreach (var error in info.Errors)
				{
					GUILayout.Label(error, styleLabel, errorOptions);
				}

				EditorGUILayout.EndScrollView();
				return;
			}

			var button_label = "Generate Widgets";

			if (info.IsUnityObject)
			{
				GUILayout.Label("<b>Warning:</b>", styleLabel);
				GUILayout.Label("Class is derived from Unity.Object.\nUsing it as a data class can be a bad practice and lead to future problems.", styleLabel);
				button_label = "Continue Generation";
			}

			if (GUILayout.Button(button_label))
			{
				var path = Path.GetDirectoryName(AssetDatabase.GetAssetPath(currentScript));
				var gen = new ScriptsGenerator(info, path);
				gen.Generate();
				Close();
			}
		}
	}
}
#endif