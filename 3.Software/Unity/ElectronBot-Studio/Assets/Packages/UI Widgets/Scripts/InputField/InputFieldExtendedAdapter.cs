namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// InputField adapter to work with both Unity text and TMPro text.
	/// </summary>
	public class InputFieldExtendedAdapter : MonoBehaviour, IInputFieldExtended
	{
		IInputFieldExtended proxy;

		/// <summary>
		/// Proxy.
		/// </summary>
		protected IInputFieldExtended Proxy
		{
			get
			{
				if (proxy == null)
				{
					proxy = GetProxy();
				}

				return proxy;
			}
		}

		/// <summary>
		/// Proxy gameobject.
		/// </summary>
		public GameObject GameObject
		{
			get
			{
				return Proxy.gameObject;
			}
		}

		/// <summary>
		/// Proxy value.
		/// </summary>
		public string Value
		{
			get
			{
				return Proxy.text;
			}

			set
			{
				Proxy.text = (value == null) ? string.Empty : value;
			}
		}

		/// <summary>
		/// Proxy value.
		/// Alias for Value.
		/// </summary>
		public string text
		{
			get
			{
				return Proxy.text;
			}

			set
			{
				Proxy.text = value;
			}
		}

		/// <summary>
		/// The Unity Event to call when edit.
		/// </summary>
		/// <value>The OnValueChange.</value>
		public UnityEvent<string> onValueChanged
		{
			get
			{
				return Proxy.onValueChanged;
			}
		}

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		public UnityEvent<string> onEndEdit
		{
			get
			{
				return Proxy.onEndEdit;
			}
		}

		/// <summary>
		/// Current InputField caret position (also selection tail).
		/// </summary>
		/// <value>The caret position.</value>
		public int caretPosition
		{
			get
			{
				return Proxy.caretPosition;
			}

			set
			{
				Proxy.caretPosition = value;
			}
		}

		/// <summary>
		/// Is the InputField eligible for interaction (excludes canvas groups).
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		public bool interactable
		{
			get
			{
				return Proxy.interactable;
			}

			set
			{
				Proxy.interactable = value;
			}
		}

		/// <summary>
		/// Text component.
		/// </summary>
		public Graphic textComponent
		{
			get
			{
				return Proxy.textComponent;
			}

			set
			{
				Proxy.textComponent = value;
			}
		}

		/// <summary>
		/// Placeholder.
		/// </summary>
		public Graphic placeholder
		{
			get
			{
				return Proxy.placeholder;
			}

			set
			{
				Proxy.placeholder = value;
			}
		}

		/// <summary>
		/// If the proxy was canceled and will revert back to the original text.
		/// </summary>
		public bool wasCanceled
		{
			get
			{
				return Proxy.wasCanceled;
			}
		}

		/// <summary>
		/// Get text proxy.
		/// </summary>
		/// <returns>Proxy instance.</returns>
		protected virtual IInputFieldExtended GetProxy()
		{
			var input = GetComponent<InputFieldExtended>();
			if (input != null)
			{
				return input;
			}

#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
			var tmpro = GetComponent<InputFieldTMProExtended>();
			if (tmpro != null)
			{
				return tmpro;
			}
#endif

			Debug.LogWarning("Not found any InputFieldExtended component.", this);

			return new InputFieldExtendedNull();
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns><c>true</c> if InputField instance is null; otherwise, <c>false</c>.</returns>
		public bool IsNull()
		{
			return Proxy.IsNull();
		}

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns><c>true</c> if lineType is LineType.MultiLineNewline; otherwise, <c>false</c>.</returns>
		public bool IsMultiLineNewline()
		{
			return Proxy.IsMultiLineNewline();
		}

		/// <summary>
		/// Function to activate the InputField to begin processing Events.
		/// </summary>
		public void ActivateInputField()
		{
			Proxy.ActivateInputField();
		}

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		/// <summary>
		/// Move caret to end.
		/// </summary>
		public void MoveToEnd()
		{
			Proxy.MoveToEnd();
		}
#endif

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Focus()
		{
			Proxy.Focus();
		}

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Select()
		{
			Proxy.Select();
		}

		/// <summary>
		/// Function to validate input.
		/// </summary>
		public InputField.OnValidateInput Validation
		{
			get
			{
				return Proxy.Validation;
			}

			set
			{
				Proxy.Validation = value;
			}
		}

		/// <summary>
		/// Function to process changed value.
		/// </summary>
		public UnityAction<string> ValueChanged
		{
			get
			{
				return Proxy.ValueChanged;
			}

			set
			{
				Proxy.ValueChanged = value;
			}
		}

		/// <summary>
		/// The Unity Event to call when edit.
		/// </summary>
		/// <value>The OnValueChange.</value>
		UnityEvent<string> IInputFieldProxy.onValueChanged
		{
			get
			{
				return Proxy.onValueChanged;
			}
		}

		/// <summary>
		/// Function to process end edit.
		/// </summary>
		public UnityAction<string> ValueEndEdit
		{
			get
			{
				return Proxy.ValueEndEdit;
			}

			set
			{
				Proxy.ValueEndEdit = value;
			}
		}

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		UnityEvent<string> IInputFieldProxy.onEndEdit
		{
			get
			{
				return Proxy.onEndEdit;
			}
		}

		/// <summary>
		/// Start selection position.
		/// </summary>
		public int SelectionStart
		{
			get
			{
				return Proxy.SelectionStart;
			}
		}

		/// <summary>
		/// End selection position.
		/// </summary>
		public int SelectionEnd
		{
			get
			{
				return Proxy.SelectionEnd;
			}
		}

		/// <inheritdoc/>
		public virtual void SetStyle(StyleSpinner styleSpinner, Style style)
		{
			Proxy.SetStyle(styleSpinner, style);
		}

		/// <inheritdoc/>
		public virtual void GetStyle(StyleSpinner styleSpinner, Style style)
		{
			Proxy.GetStyle(styleSpinner, style);
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			GetProxy();
		}
#endif
	}
}