namespace UIWidgets
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Autocomplete.
	/// Allow quickly find and select from a list of values as user type.
	/// DisplayListView - used to display list of values.
	/// TargetListView - if specified selected value will be added to this list.
	/// DataSource - list of values.
	/// </summary>
	[Obsolete("Replaced with AutocompleteString.")]
	public class Autocomplete : MonoBehaviour, IStylable, IUpgradeable, IUpdatable, ILateUpdatable
	{
		/// <summary>
		/// InputField for autocomplete.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with InputFieldAdapter")]
		protected InputField InputField;

		/// <summary>
		/// Adapter for InputField.
		/// Allow to work with default InputField and TMPro InputField.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter inputFieldAdapter;

		/// <summary>
		/// Adapter for InputField.
		/// Allow to work with default InputField and TMPro InputField.
		/// </summary>
		public virtual InputFieldAdapter InputFieldAdapter
		{
			get
			{
				return inputFieldAdapter;
			}
		}

		/// <summary>
		/// Proxy for InputField.
		/// </summary>
		[Obsolete("Replaced with InputFieldAdapter")]
		protected IInputFieldProxy inputFieldProxy;

		/// <summary>
		/// Proxy for InputField.
		/// Allow to work with default InputField and TMPro InputField.
		/// </summary>
		[Obsolete("Replaced with InputFieldAdapter")]
		public virtual IInputFieldProxy InputFieldProxy
		{
			get
			{
				return inputFieldAdapter;
			}
		}

		/// <summary>
		/// ListView to display available values.
		/// </summary>
		[SerializeField]
		public ListView DisplayListView;

		/// <summary>
		/// Selected value will be added to this ListView.
		/// </summary>
		[SerializeField]
		public ListView TargetListView;

		/// <summary>
		/// The allow duplicate of TargetListView items.
		/// </summary>
		[SerializeField]
		public bool AllowDuplicate = false;

		/// <summary>
		/// List of values.
		/// </summary>
		[SerializeField]
		public List<string> DataSource;

		/// <summary>
		/// The filter.
		/// </summary>
		[SerializeField]
		protected AutocompleteFilter filter;

		/// <summary>
		/// Gets or sets the filter.
		/// </summary>
		/// <value>The filter.</value>
		public AutocompleteFilter Filter
		{
			get
			{
				return filter;
			}

			set
			{
				filter = value;
				CustomFilter = null;
			}
		}

		/// <summary>
		/// Is filter case sensitive?
		/// </summary>
		[SerializeField]
		public bool CaseSensitive;

		/// <summary>
		/// The delimiter chars to find word for autocomplete if InputType == Word.
		/// </summary>
		[SerializeField]
		public char[] DelimiterChars = new char[] { ' ', '\n' };

		/// <summary>
		/// Custom filter.
		/// </summary>
		public Func<string, ObservableList<string>> CustomFilter;

		/// <summary>
		/// Use entire input or current word in input.
		/// </summary>
		[SerializeField]
		protected AutocompleteInput InputType = AutocompleteInput.Word;

		/// <summary>
		/// Append value to input or replace input.
		/// </summary>
		[SerializeField]
		protected AutocompleteResult Result = AutocompleteResult.Append;

		/// <summary>
		/// OnOptionSelected event.
		/// </summary>
		public AutocompleteEvent OnOptionSelected = new AutocompleteEvent();

		/// <summary>
		/// Current word in input or whole input for autocomplete.
		/// </summary>
		[HideInInspector]
		protected string Query = string.Empty;

		/// <summary>
		/// Current word in input or whole input for autocomplete.
		/// </summary>
		[HideInInspector]
		[Obsolete("Use Query instead.")]
		protected string Input
		{
			get
			{
				return Query;
			}

			set
			{
				Query = value;
			}
		}

		/// <summary>
		/// The previous input string.
		/// </summary>
		protected string PrevQuery;

		/// <summary>
		/// InputField.caretPosition. Used to keep caretPosition with Up and Down actions.
		/// </summary>
		protected int CaretPosition;

		/// <summary>
		/// The minimum number of characters a user must type before a search is performed.
		/// </summary>
		[SerializeField]
		public int MinLength = 0;

		/// <summary>
		/// The delay in seconds between when a keystroke occurs and when a search is performed.
		/// </summary>
		[SerializeField]
		public float SearchDelay = 0f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		/// <summary>
		/// Coroutine to performs search.
		/// </summary>
		protected IEnumerator SearchCoroutine;

		/// <summary>
		/// Determines whether the beginning of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Input; otherwise, false.</returns>
		public virtual bool Startswith(string value)
		{
			return UtilitiesCompare.StartsWith(value, Query, CaseSensitive);
		}

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public virtual bool Contains(string value)
		{
			return UtilitiesCompare.Contains(value, Query, CaseSensitive);
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected virtual string GetStringValue(string value)
		{
			return value;
		}

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		protected virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			Upgrade();

			InputFieldAdapter.onValueChanged.AddListener(ApplyFilter);

			var inputListener = Utilities.GetOrAddComponent<InputFieldListener>(InputFieldAdapter.gameObject);
			inputListener.OnMoveEvent.AddListener(SelectResult);
			inputListener.OnSubmitEvent.AddListener(SubmitResult);
			inputListener.onDeselect.AddListener(InputDeselected);

			DisplayListView.gameObject.SetActive(false);

			DisplayListView.MultipleSelect = false;
			DisplayListView.OnSelect.AddListener(ItemSelected);
		}

		/// <summary>
		/// Gets the input field text.
		/// </summary>
		/// <returns>The input field text.</returns>
		public virtual string GetInputFieldText()
		{
			return InputFieldAdapter.text;
		}

		/// <summary>
		/// Allow to handle item selection event.
		/// </summary>
		protected bool AllowItemSelectionEvent;

		/// <summary>
		/// Handle input deselected event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void InputDeselected(BaseEventData eventData)
		{
			var ev = eventData as PointerEventData;
			if ((ev != null) && (ev.pointerCurrentRaycast.gameObject != null) && ev.pointerCurrentRaycast.gameObject.transform.IsChildOf(DisplayListView.transform))
			{
				AllowItemSelectionEvent = true;
			}
			else
			{
				AllowItemSelectionEvent = false;
				HideOptions();
			}
		}

		/// <summary>
		/// Handle item selected event.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="component">Component.</param>
		protected virtual void ItemSelected(int index, ListViewItem component)
		{
			if (AllowItemSelectionEvent)
			{
				AllowItemSelectionEvent = false;
				SubmitResult(null);
			}
		}

		/// <summary>
		/// Show all options.
		/// </summary>
		public void ShowAllOptions()
		{
			InputFieldAdapter.Focus();
			ApplyFilter(string.Empty, false);
		}

		/// <summary>
		/// Canvas will be used as parent for DisplayListView.
		/// </summary>
		protected Transform CanvasTransform;

		/// <summary>
		/// Closes the options.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void HideOptions(string input)
		{
			HideOptions();
		}

		/// <summary>
		/// Closes the options.
		/// </summary>
		protected virtual void HideOptions()
		{
			if (CanvasTransform != null)
			{
				Utilities.GetOrAddComponent<HierarchyToggle>(DisplayListView).Restore();
			}

			DisplayListView.gameObject.SetActive(false);
		}

		/// <summary>
		/// Shows the options.
		/// </summary>
		protected virtual void ShowOptions()
		{
			CanvasTransform = UtilitiesUI.FindTopmostCanvas(DisplayListView.transform);
			if (CanvasTransform != null)
			{
				Utilities.GetOrAddComponent<HierarchyToggle>(DisplayListView).SetParent(CanvasTransform);
			}

			DisplayListView.gameObject.SetActive(true);
		}

		/// <summary>
		/// Gets the results.
		/// </summary>
		/// <returns>Values matches filter.</returns>
		protected virtual ObservableList<string> GetResults()
		{
			if (CustomFilter != null)
			{
				return CustomFilter(Query);
			}
			else
			{
				if (Filter == AutocompleteFilter.Startswith)
				{
					return UtilitiesCollections.FindAll(DataSource, Startswith);
				}
				else
				{
					return UtilitiesCollections.FindAll(DataSource, Contains);
				}
			}
		}

		/// <summary>
		/// Sets the input.
		/// </summary>
		/// <param name="input">Input string.</param>
		/// <returns>Query string.</returns>
		protected virtual string Input2Query(string input)
		{
			if (InputType == AutocompleteInput.AllInput)
			{
				return input;
			}

			int end_position = InputFieldAdapter.caretPosition;

			if (input.Length >= end_position)
			{
				var text = input.Substring(0, end_position);
				var start_position = text.LastIndexOfAny(DelimiterChars) + 1;
				return text.Substring(start_position).Trim();
			}

			return input;
		}

		/// <summary>
		/// Applies the filter.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void ApplyFilter(string input)
		{
			ApplyFilter(input, true);
		}

		/// <summary>
		/// Applies the filter.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <param name="skipIfSame">Does not show options, if input not changed.</param>
		protected virtual void ApplyFilter(string input, bool skipIfSame)
		{
			if (SearchCoroutine != null)
			{
				StopCoroutine(SearchCoroutine);
			}

			if (EventSystem.current.currentSelectedGameObject != InputFieldAdapter.gameObject)
			{
				return;
			}

			Query = Input2Query(input);

			if (skipIfSame && (Query == PrevQuery))
			{
				return;
			}

			PrevQuery = Query;

			if (Query.Length < MinLength)
			{
				DisplayListView.DataSource.Clear();
				HideOptions();
				return;
			}

			DisplayListView.Init();
			DisplayListView.MultipleSelect = false;

			SearchCoroutine = Search();
			StartCoroutine(SearchCoroutine);
		}

		/// <summary>
		/// Performs search with delay.
		/// </summary>
		/// <returns>Yield instruction.</returns>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0401:Possible allocation of reference type enumerator", Justification = "Enumerator is reusable.")]
		protected virtual IEnumerator Search()
		{
			if (SearchDelay > 0)
			{
				yield return UtilitiesTime.Wait(SearchDelay, UnscaledTime);
			}

			DisplayListView.DataSource = GetResults();

			if (DisplayListView.DataSource.Count > 0)
			{
				ShowOptions();
				DisplayListView.SelectedIndex = -1;
			}
			else
			{
				HideOptions();
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Updater.Add(this);
			Updater.AddLateUpdate(this);
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Updater.Remove(this);
			Updater.RemoveLateUpdate(this);
		}

		/// <summary>
		/// Update this instance.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (!AllowItemSelectionEvent)
			{
				CaretPosition = InputFieldAdapter.caretPosition;
			}
		}

		/// <summary>
		/// Caret position after Enter pressed.
		/// </summary>
		protected int FixCaretPosition = -1;

		/// <summary>
		/// LateUpdate.
		/// </summary>
		public virtual void RunLateUpdate()
		{
			if (FixCaretPosition != -1)
			{
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				InputFieldAdapter.caretPosition = FixCaretPosition;
#else
				InputFieldAdapter.MoveToEnd();
#endif
				FixCaretPosition = -1;
			}
		}

		/// <summary>
		/// Selects the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SelectResult(AxisEventData eventData)
		{
			if (!DisplayListView.gameObject.activeInHierarchy)
			{
				return;
			}

			if (DisplayListView.DataSource.Count == 0)
			{
				return;
			}

			switch (eventData.moveDir)
			{
				case MoveDirection.Up:
					if (DisplayListView.SelectedIndex > 0)
					{
						DisplayListView.SelectedIndex -= 1;
					}
					else
					{
						DisplayListView.SelectedIndex = DisplayListView.DataSource.Count - 1;
					}

					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldAdapter.caretPosition = CaretPosition;
					break;
				case MoveDirection.Down:
					if (DisplayListView.SelectedIndex == (DisplayListView.DataSource.Count - 1))
					{
						DisplayListView.SelectedIndex = 0;
					}
					else
					{
						DisplayListView.SelectedIndex += 1;
					}

					DisplayListView.ScrollTo(DisplayListView.SelectedIndex);
					InputFieldAdapter.caretPosition = CaretPosition;
					break;
				default:
					if (Input2Query(InputFieldAdapter.text) != Query)
					{
						ApplyFilter(InputFieldAdapter.text);
					}

					break;
			}
		}

		/// <summary>
		/// Submits the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void SubmitResult(BaseEventData eventData)
		{
			SubmitResult(eventData, false);
		}

		/// <summary>
		/// Submits the result.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		/// <param name="isEnter">Is Enter pressed.</param>
		protected virtual void SubmitResult(BaseEventData eventData, bool isEnter)
		{
			if (DisplayListView.SelectedIndex == -1)
			{
				return;
			}

			if (InputFieldAdapter.IsMultiLineNewline())
			{
				if (!DisplayListView.gameObject.activeInHierarchy)
				{
					return;
				}
				else
				{
					isEnter = false;
				}
			}

			var item = DisplayListView.DataSource[DisplayListView.SelectedIndex];

			if (TargetListView != null)
			{
				TargetListView.Init();
				TargetListView.Set(item, AllowDuplicate);
			}

			var value = GetStringValue(item);
			if (Result == AutocompleteResult.Append)
			{
				int end_position = (DisplayListView.gameObject.activeInHierarchy && eventData != null && !isEnter) ? InputFieldAdapter.caretPosition : CaretPosition;
				var text = InputFieldAdapter.text.Substring(0, end_position);
				var start_position = text.LastIndexOfAny(DelimiterChars) + 1;

				InputFieldAdapter.text = string.Format("{0}{1}{2}", InputFieldAdapter.text.Substring(0, start_position), value, InputFieldAdapter.text.Substring(end_position));

				InputFieldAdapter.caretPosition = start_position + value.Length;
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				// InputField.selectionFocusPosition = start_position + value.Length;
#else
				InputFieldAdapter.MoveToEnd();
#endif
				if (isEnter)
				{
					FixCaretPosition = start_position + value.Length;
					InputFieldAdapter.ActivateInputField();
				}
			}
			else
			{
				InputFieldAdapter.text = value;
#if UNITY_5_1 || UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				InputFieldAdapter.caretPosition = value.Length;
#else
				InputFieldAdapter.ActivateInputField();
#endif
				FixCaretPosition = value.Length;
			}

			OnOptionSelected.Invoke(item);

			HideOptions();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (DisplayListView != null)
			{
				DisplayListView.OnSelect.RemoveListener(ItemSelected);
			}

			if (InputFieldAdapter != null)
			{
				InputFieldAdapter.onValueChanged.RemoveListener(ApplyFilter);

				var inputListener = InputFieldAdapter.gameObject.GetComponent<InputFieldListener>();
				if (inputListener != null)
				{
					inputListener.OnMoveEvent.RemoveListener(SelectResult);
					inputListener.OnSubmitEvent.RemoveListener(SubmitResult);

					inputListener.onDeselect.RemoveListener(InputDeselected);
				}
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			Utilities.GetOrAddComponent(InputField, ref inputFieldAdapter);
#pragma warning restore 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif

		#region IStylable implementation

		/// <summary>
		/// Set InputField style.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void SetStyleInput(Style style)
		{
			if (InputFieldAdapter == null)
			{
				return;
			}

			if (InputFieldAdapter.textComponent != null)
			{
				style.Autocomplete.InputField.ApplyTo(InputFieldAdapter.textComponent.gameObject, true);
			}

			if (InputFieldAdapter.placeholder != null)
			{
				style.Autocomplete.Placeholder.ApplyTo(InputFieldAdapter.placeholder.gameObject);
			}
		}

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Autocomplete.Background.ApplyTo(GetComponent<Image>());

			SetStyleInput(style);

			if (DisplayListView != null)
			{
				DisplayListView.SetStyle(style);
			}

			if (TargetListView != null)
			{
				TargetListView.SetStyle(style);
			}

			return true;
		}

		/// <summary>
		/// Set style options from InputField.
		/// </summary>
		/// <param name="style">Style data.</param>
		protected virtual void GetStyleInput(Style style)
		{
			if (InputFieldAdapter == null)
			{
				return;
			}

			if (InputFieldAdapter.textComponent != null)
			{
				style.Autocomplete.InputField.GetFrom(InputFieldAdapter.textComponent.gameObject, true);
			}

			if (InputFieldAdapter.placeholder != null)
			{
				style.Autocomplete.Placeholder.GetFrom(InputFieldAdapter.placeholder.gameObject);
			}
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Autocomplete.Background.GetFrom(GetComponent<Image>());

			GetStyleInput(style);

			if (DisplayListView != null)
			{
				DisplayListView.GetStyle(style);
			}

			if (TargetListView != null)
			{
				TargetListView.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}