#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets
{
	using System.Collections.Generic;
	using System.Reflection;
	using UnityEditor;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// InputField component converter.
	/// </summary>
	public class ConverterInputField
	{
		readonly Text text;
		readonly Graphic placeholder;

		readonly bool interactable;

		readonly Selectable.Transition transition;
		readonly ColorBlock colors;
		readonly SpriteState spriteState;
		readonly AnimationTriggers animationTriggers;
		readonly GameObject imageGO;
		readonly GameObject targetGraphicGO;

		readonly Navigation navigation;
		readonly string value;
		readonly int characterLimit;

		readonly InputField.ContentType contentType;
		readonly InputField.LineType lineType;
		readonly InputField.InputType inputType;
		readonly TouchScreenKeyboardType keyboardType;
		readonly InputField.CharacterValidation characterValidation;

		readonly float caretBlinkRate;
		readonly int caretWidth;
		readonly bool customCaretColor;
		readonly Color caretColor;
		readonly Color selectionColor;

		readonly bool hideMobileInput;
		readonly bool readOnly;

		readonly object onValueChangedData;
		readonly object onEndEditData;

		readonly List<string> inputFieldRefs = new List<string>();
		readonly List<string> textRefs = new List<string>();
		readonly List<string> placeholderRefs = new List<string>();

		readonly List<RectTransform> children = new List<RectTransform>();

		readonly RectTransform textRectTransform;

		readonly string valueChangedEvent =
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
			"m_OnValueChanged";
#else
			"m_OnValueChange";
#endif

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.ConverterInputField"/> class.
		/// </summary>
		/// <param name="input">Original component.</param>
		public ConverterInputField(InputField input)
		{
			text = input.textComponent;
			placeholder = input.placeholder;

			interactable = input.interactable;

			transition = input.transition;
			colors = input.colors;
			spriteState = input.spriteState;
			animationTriggers = input.animationTriggers;
			imageGO = Component2GameObject(input.image);
			targetGraphicGO = Component2GameObject(input.targetGraphic);

			navigation = input.navigation;
			value = input.text;
			characterLimit = input.characterLimit;

			contentType = input.contentType;
			lineType = input.lineType;
			inputType = input.inputType;
			keyboardType = input.keyboardType;
			characterValidation = input.characterValidation;

			caretBlinkRate = input.caretBlinkRate;
			caretWidth = input.caretWidth;
			customCaretColor = input.customCaretColor;
			caretColor = input.caretColor;
			selectionColor = input.selectionColor;

			hideMobileInput = GetValue<bool>(input, "m_HideMobileInput");

			readOnly = input.readOnly;

			onValueChangedData = ConverterTMPro.FieldData.GetEventData(input, valueChangedEvent);
			onEndEditData = ConverterTMPro.FieldData.GetEventData(input, "m_OnEndEdit");

			FindReferencesInComponent(input, input, inputFieldRefs);
			FindReferencesInComponent(input, input.textComponent, textRefs);
			if (input.placeholder != null)
			{
				FindReferencesInComponent(input, input.placeholder, placeholderRefs);
			}

			var t = input.transform;
			for (int i = 0; i < t.childCount; i++)
			{
				children.Add(t.GetChild(i) as RectTransform);
			}

			textRectTransform = input.textComponent.transform as RectTransform;
		}

		/// <summary>
		/// Get value of the protected or private field of the specified component.
		/// </summary>
		/// <typeparam name="T">Value type.</typeparam>
		/// <param name="component">Component.</param>
		/// <param name="fieldName">Field name.</param>
		/// <returns>Value.</returns>
		protected static T GetValue<T>(Component component, string fieldName)
		{
			var type = component.GetType();

			FieldInfo field;
			do
			{
				field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
				type = type.BaseType;
				if (type == null)
				{
					return default(T);
				}
			}
			while (field == null);

			return (T)field.GetValue(component);
		}

		/// <summary>
		/// Find references to the component in the specified source and fill replaces list.
		/// </summary>
		/// <param name="source">Component with references.</param>
		/// <param name="reference">Reference component.</param>
		/// <param name="replaces">List of the properties with references to the deleted component.</param>
		protected static void FindReferencesInComponent(Component source, Component reference, List<string> replaces)
		{
			var serialized = new SerializedObject(source);
			var property = serialized.GetIterator();

			while (property.NextVisible(true))
			{
				if (property.propertyType != SerializedPropertyType.ObjectReference)
				{
					continue;
				}

				if (property.objectReferenceValue == null)
				{
					continue;
				}

				if (reference == (property.objectReferenceValue as Component))
				{
					replaces.Add(property.propertyPath);
				}
			}
		}

		/// <summary>
		/// Set saved values to the new TMP_InputField component.
		/// </summary>
		/// <param name="input">New component.</param>
		/// <param name="converter">Converter.</param>
		public void Set(TMPro.TMP_InputField input, ConverterTMPro converter)
		{
			var textarea = converter.CreateGameObject("Text Area");
			converter.SetParent(textarea.transform, input.transform);

			var viewport = textarea.transform as RectTransform;

			viewport.localRotation = textRectTransform.localRotation;
			viewport.localPosition = textRectTransform.localPosition;
			viewport.localScale = textRectTransform.localScale;
			viewport.anchorMin = textRectTransform.anchorMin;
			viewport.anchorMax = textRectTransform.anchorMax;
			viewport.anchoredPosition = textRectTransform.anchoredPosition;
			viewport.sizeDelta = textRectTransform.sizeDelta;
			viewport.pivot = textRectTransform.pivot;

			foreach (var child in children)
			{
				converter.SetParent(child, textarea.transform);
			}

			input.textViewport = viewport;
			foreach (var child in children)
			{
				child.localRotation = Quaternion.identity;
				child.localPosition = Vector3.zero;
				child.localScale = Vector3.one;
				child.anchorMin = Vector2.zero;
				child.anchorMax = Vector2.one;
				child.anchoredPosition = Vector2.zero;
				child.sizeDelta = Vector2.zero;
				child.pivot = new Vector2(0.5f, 0.5f);
			}

			input.textComponent = converter.Replace(text);
			var placeholder_text = placeholder as Text;
			input.placeholder = (placeholder_text != null)
				? converter.Replace(placeholder_text)
				: placeholder;

			input.interactable = interactable;

			input.transition = transition;
			input.colors = colors;
			input.spriteState = spriteState;
			input.animationTriggers = animationTriggers;
			input.image = GameObject2Component<Image>(imageGO);
			input.targetGraphic = GameObject2Component<Graphic>(targetGraphicGO);

			input.navigation = navigation;
			input.text = value;
			input.characterLimit = characterLimit;

			input.contentType = Convert(contentType);
			input.lineType = Convert(lineType);
			input.inputType = Convert(inputType);
			input.keyboardType = keyboardType;
			input.characterValidation = Convert(characterValidation);

			input.caretBlinkRate = caretBlinkRate;
			input.caretWidth = caretWidth;
			input.customCaretColor = customCaretColor;
			input.caretColor = caretColor;
			input.selectionColor = selectionColor;

			input.shouldHideMobileInput = hideMobileInput;
			input.readOnly = readOnly;

			input.fontAsset = ConverterTMPro.GetTMProFont();

			ConverterTMPro.FieldData.SetEventData(input, valueChangedEvent, onValueChangedData);
			ConverterTMPro.FieldData.SetEventData(input, "m_OnEndEdit", onEndEditData);

			var s_input = new SerializedObject(input);
			foreach (var input_path in inputFieldRefs)
			{
				s_input.FindProperty(input_path).objectReferenceValue = input;
			}

			foreach (var text_path in textRefs)
			{
				s_input.FindProperty(text_path).objectReferenceValue = input.textComponent;
			}

			foreach (var placeholder_path in placeholderRefs)
			{
				s_input.FindProperty(placeholder_path).objectReferenceValue = input.placeholder;
			}

			s_input.ApplyModifiedProperties();
		}

		static GameObject Component2GameObject<T>(T component)
			where T : Component
		{
			return (component == null) ? null : component.gameObject;
		}

		static T GameObject2Component<T>(GameObject go)
			where T : Component
		{
			return (go == null) ? null : go.GetComponent<T>();
		}

		static TMPro.TMP_InputField.CharacterValidation Convert(InputField.CharacterValidation validation)
		{
			switch (validation)
			{
				case InputField.CharacterValidation.None:
					return TMPro.TMP_InputField.CharacterValidation.None;
				case InputField.CharacterValidation.Integer:
					return TMPro.TMP_InputField.CharacterValidation.Integer;
				case InputField.CharacterValidation.Decimal:
					return TMPro.TMP_InputField.CharacterValidation.Decimal;
				case InputField.CharacterValidation.Alphanumeric:
					return TMPro.TMP_InputField.CharacterValidation.Alphanumeric;
				case InputField.CharacterValidation.Name:
					return TMPro.TMP_InputField.CharacterValidation.Name;
				case InputField.CharacterValidation.EmailAddress:
					return TMPro.TMP_InputField.CharacterValidation.EmailAddress;
			}

			return TMPro.TMP_InputField.CharacterValidation.None;
		}

		static TMPro.TMP_InputField.InputType Convert(InputField.InputType inputType)
		{
			switch (inputType)
			{
				case InputField.InputType.Standard:
					return TMPro.TMP_InputField.InputType.Standard;
				case InputField.InputType.AutoCorrect:
					return TMPro.TMP_InputField.InputType.AutoCorrect;
				case InputField.InputType.Password:
					return TMPro.TMP_InputField.InputType.Password;
			}

			return TMPro.TMP_InputField.InputType.Standard;
		}

		static TMPro.TMP_InputField.LineType Convert(InputField.LineType lineType)
		{
			switch (lineType)
			{
				case InputField.LineType.SingleLine:
					return TMPro.TMP_InputField.LineType.SingleLine;
				case InputField.LineType.MultiLineNewline:
					return TMPro.TMP_InputField.LineType.MultiLineNewline;
				case InputField.LineType.MultiLineSubmit:
					return TMPro.TMP_InputField.LineType.MultiLineSubmit;
			}

			return TMPro.TMP_InputField.LineType.SingleLine;
		}

		static TMPro.TMP_InputField.ContentType Convert(InputField.ContentType contentType)
		{
			switch (contentType)
			{
				case InputField.ContentType.Standard:
					return TMPro.TMP_InputField.ContentType.Standard;
				case InputField.ContentType.Autocorrected:
					return TMPro.TMP_InputField.ContentType.Autocorrected;
				case InputField.ContentType.IntegerNumber:
					return TMPro.TMP_InputField.ContentType.IntegerNumber;
				case InputField.ContentType.DecimalNumber:
					return TMPro.TMP_InputField.ContentType.DecimalNumber;
				case InputField.ContentType.Alphanumeric:
					return TMPro.TMP_InputField.ContentType.Alphanumeric;
				case InputField.ContentType.Name:
					return TMPro.TMP_InputField.ContentType.Name;
				case InputField.ContentType.EmailAddress:
					return TMPro.TMP_InputField.ContentType.EmailAddress;
				case InputField.ContentType.Password:
					return TMPro.TMP_InputField.ContentType.Password;
				case InputField.ContentType.Pin:
					return TMPro.TMP_InputField.ContentType.Pin;
				case InputField.ContentType.Custom:
					return TMPro.TMP_InputField.ContentType.Custom;
			}

			return TMPro.TMP_InputField.ContentType.Standard;
		}
	}
}
#endif