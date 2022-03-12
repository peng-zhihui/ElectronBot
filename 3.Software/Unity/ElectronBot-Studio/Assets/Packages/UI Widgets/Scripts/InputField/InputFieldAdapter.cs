namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// InputField adapter to work with both Unity text and TMPro text.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Adapters/Input Field Adapter")]
	public class InputFieldAdapter : MonoBehaviour, IInputFieldProxy
	{
		IInputFieldProxy proxy;

		/// <summary>
		/// Proxy.
		/// </summary>
		protected IInputFieldProxy Proxy
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
		protected virtual IInputFieldProxy GetProxy()
		{
			var input_unity = GetComponent<InputField>();
			if (input_unity != null)
			{
				return new InputFieldProxy(input_unity);
			}

#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
			var input_tmpro = GetComponent<TMPro.TMP_InputField>();
			if (input_tmpro != null)
			{
				return new InputFieldTMProProxy(input_tmpro);
			}
#endif

			Debug.LogWarning("Not found any InputField component.", this);

			return new InputFieldNull();
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

#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected virtual void OnValidate()
		{
			GetProxy();
		}
#endif
	}
}