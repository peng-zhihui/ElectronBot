#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// InputField with exposed functions to validate input.
	/// </summary>
	public class InputFieldTMProExtended : TMP_InputField, IInputFieldExtended
	{
		/// <summary>
		/// Function to validate input.
		/// </summary>
		public InputField.OnValidateInput Validation
		{
			get;
			set;
		}

		/// <summary>
		/// Function to process changed value.
		/// </summary>
		public UnityAction<string> ValueChanged
		{
			get;
			set;
		}

		/// <summary>
		/// The Unity Event to call when value changed.
		/// </summary>
		UnityEvent<string> IInputFieldProxy.onValueChanged
		{
			get
			{
#if UNITY_5_3 || UNITY_5_3_OR_NEWER
				return onValueChanged;
#else
				return onValueChange;
#endif
			}
		}

		/// <summary>
		/// Function to process end edit event.
		/// </summary>
		public UnityAction<string> ValueEndEdit
		{
			get;
			set;
		}

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		UnityEvent<string> IInputFieldProxy.onEndEdit
		{
			get
			{
				return onEndEdit;
			}
		}

		/// <summary>
		/// Current value.
		/// </summary>
		public string Value
		{
			get
			{
				return text;
			}

			set
			{
#if UNITY_5_3_4 || UNITY_5_3_OR_NEWER
				if (m_Text != value)
				{
					m_Text = value;
					UpdateLabel();
				}
#else
				text = value;
#endif
			}
		}

		/// <summary>
		/// Start selection position.
		/// </summary>
		public int SelectionStart
		{
			get
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				return Mathf.Min(selectionAnchorPosition, selectionFocusPosition);
#else
				return Mathf.Min(caretSelectPos, caretPosition);
#endif
			}
		}

		/// <summary>
		/// End selection position.
		/// </summary>
		public int SelectionEnd
		{
			get
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				return Mathf.Max(selectionAnchorPosition, selectionFocusPosition);
#else
				return Mathf.Max(caretSelectPos, caretPosition);
#endif
			}
		}

		/// <summary>
		/// Text component.
		/// </summary>
		Graphic IInputFieldProxy.textComponent
		{
			get
			{
				return textComponent;
			}

			set
			{
				textComponent = value as TMP_Text;
			}
		}

		static char NoValidation(string text, int charIndex, char addedChar)
		{
			return addedChar;
		}

		static void DoNothing(string value)
		{
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected override void Start()
		{
			base.Start();

			if (Validation == null)
			{
				Validation = NoValidation;
			}

			if (ValueChanged == null)
			{
				ValueChanged = DoNothing;
			}

			if (ValueEndEdit == null)
			{
				ValueEndEdit = DoNothing;
			}

			onValidateInput = ProcessValidation;

			onValueChanged.AddListener(ProcessValueChanged);
			onEndEdit.AddListener(ProcessValueEndEdit);
		}

		char ProcessValidation(string validateText, int charIndex, char addedChar)
		{
			return Validation(validateText, charIndex, addedChar);
		}

		void ProcessValueChanged(string text)
		{
			ValueChanged(text);
		}

		void ProcessValueEndEdit(string text)
		{
			ValueEndEdit(text);
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns><c>true</c> if InputField instance is null; otherwise, <c>false</c>.</returns>
		public bool IsNull()
		{
			return this != null;
		}

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns><c>true</c> if lineType is LineType.MultiLineNewline; otherwise, <c>false</c>.</returns>
		public bool IsMultiLineNewline()
		{
			return lineType == LineType.MultiLineNewline;
		}

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Focus()
		{
			ActivateInputField();
			Select();
		}

		/// <summary>
		/// Remove callbacks.
		/// </summary>
		protected override void OnDestroy()
		{
			base.OnDestroy();
			onValueChanged.RemoveListener(ProcessValueChanged);
			onEndEdit.RemoveListener(ProcessValueEndEdit);
		}

		/// <inheritdoc/>
		public virtual void SetStyle(StyleSpinner styleSpinner, Style style)
		{
			styleSpinner.InputText.ApplyTo(textComponent.gameObject, true);

			if (placeholder != null)
			{
				styleSpinner.InputPlaceholder.ApplyTo(placeholder.gameObject, true);
			}
		}

		/// <inheritdoc/>
		public virtual void GetStyle(StyleSpinner styleSpinner, Style style)
		{
			styleSpinner.InputText.GetFrom(textComponent.gameObject, true);

			if (placeholder != null)
			{
				styleSpinner.InputPlaceholder.GetFrom(placeholder.gameObject, true);
			}
		}
	}
}
#endif