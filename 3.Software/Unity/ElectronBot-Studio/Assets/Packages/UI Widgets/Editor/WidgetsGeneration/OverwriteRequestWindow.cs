#if UNITY_EDITOR
namespace UIWidgets.WidgetGeneration
{
	using System;
	using System.Collections.Generic;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Window to overwrite requests.
	/// </summary>
	public class OverwriteRequestWindow : EditorWindow
	{
		readonly Dictionary<string, bool> requestScripts = new Dictionary<string, bool>();

		readonly Dictionary<string, bool> requestPrefabs = new Dictionary<string, bool>();

		readonly Dictionary<string, bool> requestScenes = new Dictionary<string, bool>();

		readonly List<string> Iterator = new List<string>();

		ScriptsGenerator scriptGenerator;

		Action actionContinue;

		List<string> GetIterator(IEnumerable<string> source)
		{
			Iterator.Clear();
			Iterator.AddRange(source);

			return Iterator;
		}

		/// <summary>
		/// Open window.
		/// </summary>
		/// <param name="gen">Script generator.</param>
		/// <param name="action">Action to continue.</param>
		public static void Open(ScriptsGenerator gen, Action action)
		{
			var window = GetWindow<OverwriteRequestWindow>();
			window.Init(gen, action);
		}

		void Init(ScriptsGenerator gen, Action action)
		{
			scriptGenerator = gen;
			actionContinue = action;

			foreach (var script in scriptGenerator.Info.Scripts)
			{
				if (!script.Value)
				{
					requestScripts[script.Key] = true;
				}
			}

			foreach (var prefab in scriptGenerator.Info.Prefabs)
			{
				if (!prefab.Value)
				{
					requestPrefabs[prefab.Key] = true;
				}
			}

			foreach (var scene in scriptGenerator.Info.Scenes)
			{
				if (!scene.Value)
				{
					requestScenes[scene.Key] = true;
				}
			}

			if (RequestsCount == 0)
			{
				Close();
				actionContinue();
			}
			else
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				titleContent = new GUIContent("Widget Generation");
#else
				title = "Widget Generation";
#endif

				var size = new Vector2(600, GetHeight());

				position = new Rect(Screen.width / 2, Screen.height / 2, size[0], size[1]);
				minSize = size;
				Show();
			}
		}

		int RequestsCount
		{
			get
			{
				return requestScripts.Count + requestPrefabs.Count + requestScenes.Count;
			}
		}

		float GetHeight()
		{
			var height = 0f;
			var step = 17;
			if (requestScripts.Count > 0)
			{
				height += (requestScripts.Count + 2) * step;
			}

			if (requestPrefabs.Count > 0)
			{
				height += (requestPrefabs.Count + 2) * step;
			}

			if (requestScenes.Count > 0)
			{
				height += (requestScenes.Count + 2) * step;
			}

			height += step * 2;

			return height;
		}

		void SetOverwrite()
		{
			foreach (var script in requestScripts)
			{
				scriptGenerator.Info.Scripts[script.Key] = script.Value;
			}

			foreach (var prefab in requestPrefabs)
			{
				scriptGenerator.Info.Prefabs[prefab.Key] = prefab.Value;
			}

			foreach (var scene in requestScenes)
			{
				scriptGenerator.Info.Scenes[scene.Key] = scene.Value;
			}
		}

		void SetRequest(bool value)
		{
			foreach (var script in GetIterator(requestScripts.Keys))
			{
				requestScripts[script] = value;
			}

			foreach (var prefab in GetIterator(requestPrefabs.Keys))
			{
				requestPrefabs[prefab] = value;
			}

			foreach (var scene in GetIterator(requestScenes.Keys))
			{
				requestScenes[scene] = value;
			}
		}

		void ContinueClick()
		{
			Close();

			SetOverwrite();

			actionContinue();
		}

		Vector2 scrollPosition;

		/// <summary>
		/// Window GUI function.
		/// </summary>
		protected virtual void OnGUI()
		{
			if ((actionContinue == null) || (scriptGenerator == null))
			{
				Close();
				return;
			}

			var header_style = new GUIStyle
			{
				alignment = TextAnchor.MiddleLeft,
				fontStyle = FontStyle.Bold,
			};

			var toggle_options = new GUILayoutOption[]
			{
				GUILayout.Width(10),
				GUILayout.ExpandWidth(false),
			};

			var options = new GUILayoutOption[] { GUILayout.Height(GetHeight() - 17), GUILayout.ExpandHeight(true) };
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, options);
			if (requestScripts.Count > 0)
			{
				EditorGUILayout.LabelField("Overwrite scripts:", header_style);

				foreach (var script in GetIterator(requestScripts.Keys))
				{
					EditorGUILayout.BeginHorizontal();
					requestScripts[script] = EditorGUILayout.Toggle(requestScripts[script], toggle_options);
					EditorGUILayout.LabelField(scriptGenerator.Templates[script].Path.Replace("\\", "/"));
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.Space();
			}

			if (requestPrefabs.Count > 0)
			{
				EditorGUILayout.LabelField("Overwrite prefabs:", header_style);

				foreach (var prefab in GetIterator(requestPrefabs.Keys))
				{
					EditorGUILayout.BeginHorizontal();
					requestPrefabs[prefab] = EditorGUILayout.Toggle(requestPrefabs[prefab], toggle_options);
					EditorGUILayout.LabelField(scriptGenerator.Prefab2Filename(prefab).Replace("\\", "/"));
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.Space();
			}

			if (requestScenes.Count > 0)
			{
				EditorGUILayout.LabelField("Overwrite scenes:", header_style);

				foreach (var scene in GetIterator(requestScenes.Keys))
				{
					EditorGUILayout.BeginHorizontal();
					requestScenes[scene] = EditorGUILayout.Toggle(requestScenes[scene], toggle_options);
					EditorGUILayout.LabelField(scriptGenerator.Scene2Filename(scene).Replace("\\", "/"));
					EditorGUILayout.EndHorizontal();
				}

				EditorGUILayout.Space();
			}

			EditorGUILayout.EndScrollView();

			EditorGUILayout.BeginHorizontal();

			if (GUILayout.Button("Deselect All"))
			{
				SetRequest(false);
			}

			if (GUILayout.Button("Select All"))
			{
				SetRequest(true);
			}

			EditorGUILayout.Space();

			var is_close = GUILayout.Button("Cancel");

			var is_continue = GUILayout.Button("Continue");

			EditorGUILayout.EndHorizontal();

			if (is_close)
			{
				Close();
			}

			if (is_continue)
			{
				ContinueClick();
			}
		}
	}
}
#endif