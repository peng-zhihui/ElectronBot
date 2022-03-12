namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.IO;
	using System.Runtime.Serialization.Formatters.Binary;
	using System.Text;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;
#if UNITY_EDITOR
	using UnityEditor;
	using UnityEditor.Events;
#endif

	/// <summary>
	/// Editor functions.
	/// </summary>
	public static class UtilitiesEditor
	{
		/// <summary>
		/// Get friendly name of the specified type.
		/// </summary>
		/// <param name="type">Type.</param>
		/// <returns>Friendly name.</returns>
		public static string GetFriendlyTypeName(Type type)
		{
			var friendly_name = type.Name;

			if (!type.IsGenericType)
			{
				return string.IsNullOrEmpty(type.Namespace) ? friendly_name : string.Format("{0}.{1}", type.Namespace, friendly_name);
			}

			var backtick_index = friendly_name.IndexOf('`');
			if (backtick_index > 0)
			{
				friendly_name = friendly_name.Remove(backtick_index);
			}

			var sb = new StringBuilder();
			if (!string.IsNullOrEmpty(type.Namespace))
			{
				sb.Append(type.Namespace);
				sb.Append(".");
			}

			sb.Append(friendly_name);
			sb.Append('<');

			var type_parameters = type.GetGenericArguments();
			for (int i = 0; i < type_parameters.Length; ++i)
			{
				var type_param_name = GetFriendlyTypeName(type_parameters[i]);
				if (i > 0)
				{
					sb.Append(',');
				}

				sb.Append(type_param_name);
			}

			sb.Append('>');

			return sb.ToString();
		}

		/// <summary>
		/// Get type by full name.
		/// </summary>
		/// <param name="typename">Type name.</param>
		/// <returns>Type.</returns>
		public static Type GetType(string typename)
		{
			foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				Type[] types;
				try
				{
					types = assembly.GetTypes();
				}
				catch (System.Reflection.ReflectionTypeLoadException e)
				{
					types = e.Types;
				}

				foreach (var assembly_type in types)
				{
					if ((assembly_type != null) && (assembly_type.FullName == typename))
					{
						return assembly_type;
					}
				}
			}

			return null;
		}

		/// <summary>
		/// Serialize object with BinaryFormatter to base64 string.
		/// </summary>
		/// <param name="obj">Object to serialize.</param>
		/// <returns>Serialized string.</returns>
		public static string Serialize(object obj)
		{
			var serializer = new BinaryFormatter();

			using (var ms = new MemoryStream())
			{
				serializer.Serialize(ms, obj);
				return Convert.ToBase64String(ms.ToArray());
			}
		}

		/// <summary>
		/// De-serialize object with BinaryFormatter from base64 string.
		/// </summary>
		/// <typeparam name="T">Object type.</typeparam>
		/// <param name="encoded">Serialized string.</param>
		/// <returns>De-serialized object.</returns>
		public static T Deserialize<T>(string encoded)
		{
			var serializer = new BinaryFormatter();
			var ms = new MemoryStream();

			var bytes = Convert.FromBase64String(encoded);
			ms.Write(bytes, 0, bytes.Length);
			ms.Seek(0, SeekOrigin.Begin);

			return (T)serializer.Deserialize(ms);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Creates the object by path to asset prefab.
		/// </summary>
		/// <returns>The created object.</returns>
		/// <param name="path">Path.</param>
		static GameObject CreateObject(string path)
		{
			var prefab = Compatibility.LoadAssetAtPath<GameObject>(path);
			if (prefab == null)
			{
				throw new ArgumentException(string.Format("Prefab not found at path {0}.", path));
			}

			return CreateGameObject(prefab);
		}

		/// <summary>
		/// Create gameobject.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		/// <param name="undo">Support editor undo.</param>
		/// <returns>Created gameobject.</returns>
		public static GameObject CreateGameObject(GameObject prefab, bool undo = true)
		{
			var go = Compatibility.Instantiate(prefab);

			if (undo)
			{
				Undo.RegisterCreatedObjectUndo(go, "Create " + prefab.name);
			}

			var go_parent = Selection.activeTransform;
			if ((go_parent == null) || (!(go_parent is RectTransform)))
			{
				go_parent = GetCanvasTransform();
			}

			if (go_parent != null)
			{
				if (undo)
				{
					Undo.SetTransformParent(go.transform, go_parent, "Create " + prefab.name);
				}
				else
				{
					go.transform.SetParent(go_parent, false);
				}
			}

			go.name = prefab.name;

			var rectTransform = go.transform as RectTransform;
			if (rectTransform != null)
			{
				rectTransform.anchoredPosition = new Vector2(0, 0);

				Utilities.FixInstantiated(prefab, go);
			}

			return go;
		}

		/// <summary>
		/// Returns the asset object of type T with the specified GUID.
		/// </summary>
		/// <param name="guid">GUID.</param>
		/// <returns>Asset with the specified GUID.</returns>
		/// <typeparam name="T">Asset type.</typeparam>
		public static T LoadAssetWithGUID<T>(string guid)
			where T : UnityEngine.Object
		{
			var path = AssetDatabase.GUIDToAssetPath(guid);
			if (string.IsNullOrEmpty(path))
			{
				Debug.LogWarning("Path not found for the GUID: " + guid);
				return null;
			}

			return Compatibility.LoadAssetAtPath<T>(path);
		}

		/// <summary>
		/// Find assets by specified search.
		/// </summary>
		/// <typeparam name="T">Assets type.</typeparam>
		/// <param name="search">Search string.</param>
		/// <returns>Assets list.</returns>
		public static List<T> GetAssets<T>(string search)
			where T : UnityEngine.Object
		{
			var guids = AssetDatabase.FindAssets(search);

			var result = new List<T>(guids.Length);
			foreach (var guid in guids)
			{
				var asset = LoadAssetWithGUID<T>(guid);
				if (asset != null)
				{
					result.Add(asset);
				}
			}

			return result;
		}

		/// <summary>
		/// Creates the object from asset.
		/// </summary>
		/// <returns>The object from asset.</returns>
		/// <param name="key">Search string.</param>
		static GameObject CreateObjectFromAsset(string key)
		{
			var path = GetAssetPath(key);
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			var go = CreateObject(path);

			Selection.activeObject = go;

			return go;
		}

		/// <summary>
		/// Get asset by label.
		/// </summary>
		/// <typeparam name="T">Asset type.</typeparam>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset.</returns>
		public static T GetAsset<T>(string label)
			where T : UnityEngine.Object
		{
			var path = GetAssetPath(label + "Asset");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return Compatibility.LoadAssetAtPath<T>(path);
		}

		/// <summary>
		/// Get asset path by label.
		/// </summary>
		/// <param name="label">Asset label.</param>
		/// <returns>Asset path.</returns>
		public static string GetAssetPath(string label)
		{
			var key = "l:Uiwidgets" + label;
			var guids = AssetDatabase.FindAssets(key);
			if (guids.Length == 0)
			{
				Debug.LogWarning("Label not found: " + label);
				return null;
			}

			return AssetDatabase.GUIDToAssetPath(guids[0]);
		}

		/// <summary>
		/// Get prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		public static GameObject GetPrefab(string label)
		{
			var path = GetAssetPath(label + "Prefab");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return Compatibility.LoadAssetAtPath<GameObject>(path);
		}

		/// <summary>
		/// Get generated prefab by label.
		/// </summary>
		/// <param name="label">Prefab label.</param>
		/// <returns>Prefab.</returns>
		public static GameObject GetGeneratedPrefab(string label)
		{
			return GetPrefab("Generated" + label);
		}

		/// <summary>
		/// Set prefabs label.
		/// </summary>
		/// <param name="prefab">Prefab.</param>
		public static void SetPrefabLabel(UnityEngine.Object prefab)
		{
			AssetDatabase.SetLabels(prefab, new[] { "UiwidgetsGenerated" + prefab.name + "Prefab", });
		}

		/// <summary>
		/// Create widget template from asset specified by label.
		/// </summary>
		/// <param name="templateLabel">Template label.</param>
		/// <returns>Widget template.</returns>
		public static GameObject CreateWidgetTemplateFromAsset(string templateLabel)
		{
			var path = GetAssetPath(templateLabel + "Prefab");
			if (string.IsNullOrEmpty(path))
			{
				return null;
			}

			return CreateObject(path);
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="widget">Widget name.</param>
		/// <param name="applyStyle">Apply style to created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <returns>Created GameObject.</returns>
		public static GameObject CreateWidgetFromAsset(string widget, bool applyStyle = true, Action<GameObject> converter = null)
		{
			var go = CreateObjectFromAsset(widget + "Prefab");

			if (go != null)
			{
				if (converter != null)
				{
					converter(go);
				}

				if (applyStyle)
				{
					var style = PrefabsMenu.Instance.DefaultStyle;
					if (style != null)
					{
						style.ApplyTo(go);
					}
				}
			}

			Upgrade(go);

			FixDialogCloseButton(go);

			return go;
		}

		/// <summary>
		/// Creates the widget from prefab by name.
		/// </summary>
		/// <param name="prefab">Widget name.</param>
		/// <param name="applyStyle">Apply style to created widget.</param>
		/// <param name="converter">Converter for the created widget (mostly used to replace Unity Text with TMPro Text).</param>
		/// <returns>Created GameObject.</returns>
		public static GameObject CreateWidgetFromPrefab(GameObject prefab, bool applyStyle = true, Action<GameObject> converter = null)
		{
			var go = CreateGameObject(prefab);

			Selection.activeObject = go;

			if (go != null)
			{
				if (converter != null)
				{
					converter(go);
				}

				if (applyStyle)
				{
					var style = PrefabsMenu.Instance.DefaultStyle;
					if (style != null)
					{
						style.ApplyTo(go);
					}
				}
			}

			Upgrade(go);

			FixDialogCloseButton(go);

			return go;
		}

		static void Upgrade(GameObject go)
		{
			var upgrades = new List<IUpgradeable>();
			Compatibility.GetComponentsInChildren(go.transform, true, upgrades);
			for (int i = 0; i < upgrades.Count; i++)
			{
				upgrades[i].Upgrade();
			}
		}

		/// <summary>
		/// Replace Close button callback on Cancel instead of the Hide for the Dialog components in the specified GameObject.
		/// </summary>
		/// <param name="go">GameObject.</param>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		public static void FixDialogCloseButton(GameObject go)
		{
			var dialogs = go.GetComponentsInChildren<Dialog>(true);

			foreach (var dialog in dialogs)
			{
				var button_go = dialog.transform.Find("Header/CloseButton");
				if (button_go == null)
				{
					continue;
				}

				var button = button_go.GetComponent<Button>();
				if (button == null)
				{
					continue;
				}

				if (IsEventCallMethod(button.onClick, dialog, "Hide"))
				{
					UnityEventTools.RemovePersistentListener(button.onClick, dialog.Hide);
					UnityEventTools.AddPersistentListener(button.onClick, dialog.Cancel);
				}
			}
		}

		static bool IsEventCallMethod(UnityEvent ev, MonoBehaviour target, string method)
		{
			var n = ev.GetPersistentEventCount();
			for (int i = 0; i < n; i++)
			{
				if ((ev.GetPersistentMethodName(i) == method) && (ev.GetPersistentTarget(i) == target))
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the canvas transform.
		/// </summary>
		/// <returns>The canvas transform.</returns>
		public static Transform GetCanvasTransform()
		{
			var canvas = (Selection.activeGameObject != null) ? Selection.activeGameObject.GetComponentInParent<Canvas>() : null;
			if (canvas == null)
			{
				canvas = UnityEngine.Object.FindObjectOfType<Canvas>();
			}

			if (canvas != null)
			{
				return canvas.transform;
			}

			var canvasGO = new GameObject("Canvas")
			{
				layer = LayerMask.NameToLayer("UI"),
			};
			canvas = canvasGO.AddComponent<Canvas>();
			canvas.renderMode = RenderMode.ScreenSpaceOverlay;
			canvasGO.AddComponent<CanvasScaler>();
			canvasGO.AddComponent<GraphicRaycaster>();
			Undo.RegisterCreatedObjectUndo(canvasGO, "Create " + canvasGO.name);

			if (UnityEngine.Object.FindObjectOfType<EventSystem>() == null)
			{
				Compatibility.CreateEventSystem();
			}

			return canvasGO.transform;
		}
#endif
	}
}