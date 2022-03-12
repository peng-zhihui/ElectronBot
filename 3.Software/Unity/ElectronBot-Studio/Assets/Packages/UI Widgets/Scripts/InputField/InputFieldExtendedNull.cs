namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.UI;

	/// <summary>
	/// InputFieldProxy.
	/// </summary>
	public class InputFieldExtendedNull : IInputFieldExtended
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.InputFieldExtendedNull"/> class.
		/// </summary>
		public InputFieldExtendedNull()
		{
		}

		#region IInputFieldExtended implementation

		/// <summary>
		/// The current value of the input field.
		/// </summary>
		/// <value>The text.</value>
		public string text
		{
			get
			{
				return string.Empty;
			}

			set
			{
			}
		}

		readonly InputField.OnChangeEvent changeEvent = new InputField.OnChangeEvent();

		/// <summary>
		/// The Unity Event to call when value changed.
		/// </summary>
		public UnityEvent<string> onValueChanged
		{
			get
			{
				return changeEvent;
			}
		}

		readonly InputField.SubmitEvent submitEvent = new InputField.SubmitEvent();

		/// <summary>
		/// The Unity Event to call when editing has ended.
		/// </summary>
		/// <value>The OnEndEdit.</value>
		public UnityEvent<string> onEndEdit
		{
			get
			{
				return submitEvent;
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
				return 0;
			}

			set
			{
			}
		}

		/// <summary>
		/// Is the InputField eligible for interaction (excludes canvas groups).
		/// </summary>
		/// <value>true</value>
		/// <c>false</c>
		public bool interactable
		{
			get
			{
				return true;
			}

			set
			{
			}
		}

		/// <summary>
		/// Determines whether InputField instance is null.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsNull()
		{
			return true;
		}

		/// <summary>
		/// Gets the gameobject.
		/// </summary>
		/// <value>The gameobject.</value>
		public GameObject gameObject
		{
			get
			{
				return null;
			}
		}

		/// <summary>
		/// Text component.
		/// </summary>
		public Graphic textComponent
		{
			get
			{
				return null;
			}

			set
			{
			}
		}

		/// <summary>
		/// Placeholder.
		/// </summary>
		public Graphic placeholder
		{
			get
			{
				return null;
			}

			set
			{
			}
		}

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
			get
			{
				return null;
			}

			set
			{
			}
		}

		/// <summary>
		/// Function to process end edit event.
		/// </summary>
		public UnityAction<string> ValueEndEdit
		{
			get
			{
				return null;
			}

			set
			{
			}
		}

		/// <summary>
		/// Current value.
		/// </summary>
		public string Value
		{
			get
			{
				return string.Empty;
			}

			set
			{
			}
		}

		/// <summary>
		/// Start selection position.
		/// </summary>
		public int SelectionStart
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// End selection position.
		/// </summary>
		public int SelectionEnd
		{
			get
			{
				return 0;
			}
		}

		/// <summary>
		/// If the proxy was canceled and will revert back to the original text.
		/// </summary>
		public bool wasCanceled
		{
			get
			{
				return false;
			}
		}

		/// <summary>
		/// Determines whether this lineType is LineType.MultiLineNewline.
		/// </summary>
		/// <returns>true</returns>
		/// <c>false</c>
		public bool IsMultiLineNewline()
		{
			return false;
		}

		/// <summary>
		/// Function to activate the InputField to begin processing Events.
		/// </summary>
		public void ActivateInputField()
		{
		}

#if UNITY_4_6 || UNITY_4_7 || UNITY_5_0
		/// <summary>
		/// Move caret to end.
		/// </summary>
		public void MoveToEnd()
		{
		}
#endif

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Focus()
		{
		}

		/// <summary>
		/// Set focus to InputField.
		/// </summary>
		public void Select()
		{
		}

		/// <inheritdoc/>
		public void SetStyle(StyleSpinner styleSpinner, Style style)
		{
		}

		/// <inheritdoc/>
		public void GetStyle(StyleSpinner styleSpinner, Style style)
		{
		}
		#endregion
	}
}