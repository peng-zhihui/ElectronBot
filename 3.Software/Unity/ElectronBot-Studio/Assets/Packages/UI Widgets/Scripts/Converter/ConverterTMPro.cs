namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Converter functions to replace component with another component.
	/// </summary>
	public partial class ConverterTMPro
	{
		/// <summary>
		/// Replace Unity Text and InputField with TextMeshPro components for the specified widget.
		/// It should be used only for just instantiated widgets because external references to the target are ignored.
		/// </summary>
		/// <param name="target">Target.</param>
		public static void Widget2TMPro(GameObject target)
		{
#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT
			var converter = new ConverterTMPro(target, new List<GameObject> { target, })
			{
				UseUndo = false,
			};

			foreach (var w in converter.Warnings)
			{
				Debug.LogWarning(w.Info, w.Target);
			}

			foreach (var e in converter.Errors)
			{
				Debug.LogError(e.Info, e.Target);
			}

			if (converter.IsValid)
			{
				converter.Convert();
			}
#endif
		}

#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT
		/// <summary>
		/// Generate widget.
		/// </summary>
		[UnityEditor.MenuItem("GameObject/UI/New UI Widgets/Replace Unity Text with TextMeshPro", false, 20)]
		public static void Convert2TMPro()
		{
			var converter = new ConverterTMPro(UnityEditor.Selection.activeObject as GameObject);
			if (!converter.IsValid)
			{
				ConverterTMProWindow.Open(converter);
				return;
			}

			converter.Convert();
		}

		/// <summary>
		/// Can widget be created?
		/// </summary>
		/// <returns>True if widget can be created; otherwise false.</returns>
		[UnityEditor.MenuItem("GameObject/UI/New UI Widgets/Replace Unity Text with TextMeshPro", true, 20)]
		public static bool CanConvert2TMPro()
		{
			if (UnityEditor.Selection.activeObject == null)
			{
				return false;
			}

			var go = UnityEditor.Selection.activeObject as GameObject;
			if (go == null)
			{
				return false;
			}

			var rt = go.transform as RectTransform;
			if (rt == null)
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Get default TextMeshPro font in the editor mode.
		/// </summary>
		/// <returns>TextMeshPro font.</returns>
		static T EditorTMProFont<T>()
			where T : UnityEngine.Object
		{
			var paths = new string[]
			{
				"Assets/TextMesh Pro/Resources/Fonts & Materials/ARIAL SDF.asset",
				"Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset",
			};

			foreach (var path in paths)
			{
				var font = Compatibility.LoadAssetAtPath<T>(path);
				if (font != null)
				{
					return font;
				}
			}

			// try to find any font
			var guids = UnityEditor.AssetDatabase.FindAssets("t:" + typeof(T).Name);
			for (int i = 0; i < guids.Length; i++)
			{
				var font = UtilitiesEditor.LoadAssetWithGUID<T>(guids[i]);
				if (font != null)
				{
					return font;
				}
			}

			return null;
		}

		/// <summary>
		/// Get TextMeshPro font.
		/// </summary>
#if UNITY_4_6 || UNITY_4_7
		public static Func<TMPro.TextMeshProFont> GetTMProFont = EditorTMProFont<TMPro.TextMeshProFont>;
#else
		public static Func<TMPro.TMP_FontAsset> GetTMProFont = EditorTMProFont<TMPro.TMP_FontAsset>;
#endif

		readonly ReplacementInfo<InputFieldExtended> inputFieldsExtended = new ReplacementInfo<InputFieldExtended>();
		readonly ReplacementInfo<InputField> inputFields = new ReplacementInfo<InputField>();
		readonly ReplacementInfo<Text> texts = new ReplacementInfo<Text>();

		readonly List<Message> errors = new List<Message>();
		readonly List<Message> warnings = new List<Message>();

		readonly string undoName = "Replace Unity Text with TMPro";

		/// <summary>
		/// Target.
		/// </summary>
		public GameObject Target
		{
			get;
			protected set;
		}

		/// <summary>
		/// Is convertation can be done automatically without any notification?
		/// </summary>
		public bool IsValid
		{
			get
			{
				if ((errors.Count + warnings.Count) > 0)
				{
					return false;
				}

				if (TotalCount == 0)
				{
					return false;
				}

				return true;
			}
		}

		/// <summary>
		/// Use Editor Undo.
		/// Should not be used with the widgets created from menu.
		/// </summary>
		public bool UseUndo = true;

		/// <summary>
		/// Count of the InputField components.
		/// </summary>
		public int InputFieldsCount
		{
			get
			{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				return inputFields.Count + inputFieldsExtended.Count;
#else
				return 0;
#endif
			}
		}

		/// <summary>
		/// Count of the Text components.
		/// </summary>
		public int TextsCount
		{
			get
			{
				return texts.Count;
			}
		}

		/// <summary>
		/// Count of the Text and InputField components.
		/// </summary>
		public int TotalCount
		{
			get
			{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				return TextsCount + InputFieldsCount;
#else
				return TextsCount;
#endif
			}
		}

		/// <summary>
		/// Errors.
		/// </summary>
		public List<Message> Errors
		{
			get
			{
				return errors;
			}
		}

		/// <summary>
		/// Warnings.
		/// </summary>
		public List<Message> Warnings
		{
			get
			{
				return warnings;
			}
		}

		/// <summary>
		/// Destroy component.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="component">Component.</param>
		protected void DestroyComponent<T>(T component)
			where T : UnityEngine.Object
		{
			if (UseUndo)
			{
				UnityEditor.Undo.DestroyObjectImmediate(component);
			}
			else
			{
				UnityEngine.Object.DestroyImmediate(component);
			}
		}

		/// <summary>
		/// Add component to the specified target.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="target">Game object.</param>
		/// <returns>Added component.</returns>
		protected T AddComponent<T>(GameObject target)
			where T : Component
		{
			if (UseUndo)
			{
				return UnityEditor.Undo.AddComponent<T>(target);
			}
			else
			{
				return target.AddComponent<T>();
			}
		}

		/// <summary>
		/// Create game object.
		/// </summary>
		/// <param name="name">Name.</param>
		/// <returns>Game object.</returns>
		public GameObject CreateGameObject(string name)
		{
			var go = new GameObject(name, typeof(RectTransform));

			if (UseUndo)
			{
				UnityEditor.Undo.RegisterCreatedObjectUndo(go, undoName);
			}

			return go;
		}

		/// <summary>
		/// Set parent to the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="parent">Parent.</param>
		public void SetParent(Transform target, Transform parent)
		{
			if (UseUndo)
			{
				UnityEditor.Undo.SetTransformParent(target, parent, undoName);
			}
			else
			{
				target.SetParent(parent);
			}
		}

		static void GetGameObjectHierarchy(Transform current, List<GameObject> result)
		{
			result.Add(current.gameObject);

			for (int i = 0; i < current.childCount; i++)
			{
				GetGameObjectHierarchy(current.GetChild(i), result);
			}
		}

		/// <summary>
		/// Is fonts installed?
		/// </summary>
		/// <returns>true if fonts installed; otherwise false.</returns>
		protected bool FontsInstalled()
		{
			var paths = UnityEditor.AssetDatabase.FindAssets("t:TMPro.TMP_FontAsset");
			if (paths.Length > 0)
			{
				return true;
			}

			UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF - Fallback.asset");
			UnityEditor.AssetDatabase.LoadAssetAtPath<TMPro.TMP_FontAsset>("Assets/TextMesh Pro/Resources/Fonts & Materials/LiberationSans SDF.asset");

			var fonts = Resources.FindObjectsOfTypeAll<TMPro.TMP_FontAsset>();
			if (fonts.Length > 0)
			{
				return true;
			}

			return false;
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.ConverterTMPro"/> class.
		/// </summary>
		/// <param name="go">Target.</param>
		/// <param name="rootGameObjects">Root gameobjects to find references.</param>
		public ConverterTMPro(GameObject go, List<GameObject> rootGameObjects = null)
		{
			Target = go;

			if (!FontsInstalled())
			{
				errors.Add(new Message("TextMeshPro fonts not found.\nPlease install with \"Window / TextMeshPro / Import TMPro Essential Resources\"."));
				return;
			}

			if (Target == null)
			{
				errors.Add(new Message("GameObject not specified."));
				return;
			}

			var root_gos = (rootGameObjects == null)
				? Compatibility.GetRootGameObjects()
				: rootGameObjects;

			var references_gos = new List<GameObject>();
			foreach (var root_go in root_gos)
			{
				GetGameObjectHierarchy(root_go.transform, references_gos);
			}

			Upgrade();

			inputFieldsExtended.GameObjectsWithReferences = references_gos;
			inputFields.GameObjectsWithReferences = references_gos;
			texts.GameObjectsWithReferences = references_gos;

			FindInputFields(inputFieldsExtended);
			FillTextsExclude(inputFieldsExtended);
			inputFields.Exclude.UnionWith(inputFieldsExtended.Components);

			FindInputFields(inputFields);
			FillTextsExclude(inputFields);

			FindTexts(texts);

			inputFieldsExtended.FindReferences<Text>();
			inputFields.FindReferences<Text>();
			texts.FindReferences<Text>();

			errors.AddRange(inputFieldsExtended.Errors);
			errors.AddRange(inputFields.Errors);
			errors.AddRange(texts.Errors);

			warnings.AddRange(inputFieldsExtended.Warnings);
			warnings.AddRange(inputFields.Warnings);
			warnings.AddRange(texts.Warnings);
		}

		/// <summary>
		/// Replace Unity components with TextMeshPro components.
		/// </summary>
		public void Convert()
		{
			if (errors.Count > 0)
			{
				Debug.LogError("Errors found. Replacement canceled.");
				return;
			}

			if (UseUndo)
			{
				UnityEditor.Undo.IncrementCurrentGroup();
				UnityEditor.Undo.RegisterFullObjectHierarchyUndo(Target, undoName);
			}

			ShowProgress();

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
			progressInfo = "Converting InputFields.";
			inputFieldsExtended.Convert(Replace, ShowProgress);
			inputFields.Convert(Replace, ShowProgress);
#endif

			progressInfo = "Converting Texts.";
			texts.Convert(Replace, ShowProgress);

			if (UseUndo)
			{
				UnityEditor.EditorUtility.ClearProgressBar();
			}
		}

		int progress = -1;
		string progressInfo = string.Empty;

		/// <summary>
		/// Show progress.
		/// </summary>
		protected void ShowProgress()
		{
			if (!UseUndo)
			{
				return;
			}

			progress += 1;
			if (progress == TotalCount)
			{
				UnityEditor.EditorUtility.ClearProgressBar();
			}
			else
			{
				UnityEditor.EditorUtility.DisplayProgressBar("Replacing Unity Text with TextMeshPro", progressInfo, progress / (float)TotalCount);
			}
		}

		/// <summary>
		/// Upgrade recursive IUpgradeable components to the latest version.
		/// </summary>
		protected void Upgrade()
		{
			var components = new List<IUpgradeable>();
			Compatibility.GetComponentsInChildren<IUpgradeable>(Target.transform, true, components);
			foreach (var c in components)
			{
				c.Upgrade();
			}
		}

		/// <summary>
		/// Find recursive UnityEngine.UI.Text components.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <typeparam name="T">Text type.</typeparam>
		protected void FindTexts<T>(ReplacementInfo<T> data)
			where T : Text
		{
			Compatibility.GetComponentsInChildren<T>(Target.transform, true, data.Components);

			for (int i = data.Components.Count - 1; i >= 0; i--)
			{
				var text = data.Components[i];
				if (!CanReplaceText(text, data))
				{
					data.Components.RemoveAt(i);
				}
			}

			for (int i = 0; i < data.Components.Count; i++)
			{
				var component = data.Components[i];
				data.Deleted[component] = component;
			}

			data.FillReplaces();
		}

		/// <summary>
		/// Replace recursive UnityEngine.UI.InputField components with TMPro.TMP_InputField components.
		/// </summary>
		/// <param name="data">Data.</param>
		/// <typeparam name="T">InputField type.</typeparam>
		protected void FindInputFields<T>(ReplacementInfo<T> data)
			where T : InputField
		{
			Compatibility.GetComponentsInChildren(Target.transform, true, data.Components);

			foreach (var exclude in data.Exclude)
			{
				while (data.Components.Remove(exclude))
				{
					// do nothing
				}
			}

			for (int i = data.Components.Count - 1; i >= 0; i--)
			{
				var input = data.Components[i];
				if (!CanReplaceInputField(input, data))
				{
					data.Components.RemoveAt(i);
				}
			}

			foreach (var input in data.Components)
			{
				data.Deleted[input] = input;

				if (input.placeholder != null)
				{
					data.Deleted[input.placeholder] = input;
				}

				if (input.textComponent != null)
				{
					data.Deleted[input.textComponent] = input;
				}
			}

			data.FillReplaces();
		}

		/// <summary>
		/// Fill texts exclude list.
		/// </summary>
		/// <typeparam name="T">Component type.</typeparam>
		/// <param name="info">Info.</param>
		protected void FillTextsExclude<T>(ReplacementInfo<T> info)
			where T : InputField
		{
			foreach (var input in info.Components)
			{
				var placeholder_text = input.placeholder as Text;
				if (placeholder_text != null)
				{
					texts.Exclude.Add(placeholder_text);
				}

				if (input.textComponent != null)
				{
					texts.Exclude.Add(input.textComponent);
				}
			}
		}

		/// <summary>
		/// Check is possible to replace UnityEngine.UI.InputField component with TMPro.TMP_InputField.
		/// </summary>
		/// <param name="input">InputField component.</param>
		/// <param name="data">Replacement info.</param>
		/// <returns>true if possible to replace; otherwise false.</returns>
		/// <typeparam name="T">InputField type.</typeparam>
		protected static bool CanReplaceInputField<T>(T input, ReplacementInfo<T> data)
			where T : InputField
		{
			var type = typeof(T);

			if (data.Exclude.Contains(input))
			{
				return false;
			}

			// do not replace derived classes
			if (input.GetType().IsSubclassOf(type))
			{
				data.CannotReplace.Add(input);

				var warning = string.Format("Classes derived from {0} cannot be automatically replaced.", type.Name);
				data.Warnings.Add(new Message(warning, input));
				return false;
			}

			if (input.textComponent == null)
			{
				data.CannotReplace.Add(input);

				var warning = string.Format("{0} without textComponent cannot be automatically replaced.\nPlease specify textComponent.", type.Name);
				data.Warnings.Add(new Message(warning, input));
				return false;
			}

			return true;
		}

		/// <summary>
		/// Check is possible to replace UnityEngine.UI.Text component with TMPro.TextMeshProUGUI.
		/// </summary>
		/// <param name="text">Text component.</param>
		/// <param name="data">Replacement info.</param>
		/// <returns>true if possible to replace; otherwise false.</returns>
		/// <typeparam name="T">Text type.</typeparam>
		protected static bool CanReplaceText<T>(T text, ReplacementInfo<T> data)
			where T : Text
		{
			if (data.Exclude.Contains(text))
			{
				return false;
			}

			if (text.GetType().IsSubclassOf(typeof(Text)))
			{
				data.CannotReplace.Add(text);
				data.Warnings.Add(new Message("Classes derived from Text cannot be automatically replaced.", text));
				return false;
			}

			return true;
		}

		/// <summary>
		/// Replace Text with TextMeshProUGUI.
		/// </summary>
		/// <param name="text">Original component.</param>
		/// <returns>New component.</returns>
		public TMPro.TextMeshProUGUI Replace(Text text)
		{
			var go = text.gameObject;

			var settings = new ConverterText(text);

			DestroyComponent(text);

			var tmpro = AddComponent<TMPro.TextMeshProUGUI>(go);

			settings.Set(tmpro);

			return tmpro;
		}

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		/// <summary>
		/// Replace Input field with TMP_InputField.
		/// </summary>
		/// <param name="input">Original component.</param>
		/// <returns>New component.</returns>
		protected TMPro.TMP_InputField Replace(InputField input)
		{
			var text_replaces = inputFields.References[input.textComponent];
			var placeholder_replaces = (input.placeholder != null) ? inputFields.References[input.placeholder] : new List<ReplacementInfo<InputField>.PropertyReference>();

			var settings = new ConverterInputField(input);
			var go = input.gameObject;

			DestroyComponent(input);

			var tmpro = AddComponent<TMPro.TMP_InputField>(go);
			settings.Set(tmpro, this);

			for (int i = 0; i < text_replaces.Count; i++)
			{
				text_replaces[i].Set(tmpro.textComponent);
			}

			for (int i = 0; i < placeholder_replaces.Count; i++)
			{
				placeholder_replaces[i].Set(tmpro.placeholder);
			}

			return tmpro;
		}

		/// <summary>
		/// Replace Input field with TMP_InputField.
		/// </summary>
		/// <param name="input">Original component.</param>
		/// <returns>New component.</returns>
		protected InputFieldTMProExtended Replace(InputFieldExtended input)
		{
			var text_replaces = inputFieldsExtended.References[input.textComponent];
			var placeholder_replaces = (input.placeholder != null) ? inputFieldsExtended.References[input.placeholder] : new List<ReplacementInfo<InputFieldExtended>.PropertyReference>();

			var settings = new ConverterInputField(input);
			var go = input.gameObject;

			DestroyComponent(input);

			var tmpro = AddComponent<InputFieldTMProExtended>(go);
			settings.Set(tmpro, this);

			for (int i = 0; i < text_replaces.Count; i++)
			{
				text_replaces[i].Set(tmpro.textComponent);
			}

			for (int i = 0; i < placeholder_replaces.Count; i++)
			{
				placeholder_replaces[i].Set(tmpro.placeholder);
			}

			return tmpro;
		}
#endif
#endif
	}
}