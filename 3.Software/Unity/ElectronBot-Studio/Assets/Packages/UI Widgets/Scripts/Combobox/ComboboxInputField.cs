namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Events;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Combobox.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Combobox.png
	/// </summary>
	public class ComboboxInputField : MonoBehaviour, ISubmitHandler, IStylable
	{
		[SerializeField]
		ListViewString listView;

		/// <summary>
		/// Gets or sets the ListView.
		/// </summary>
		/// <value>ListView component.</value>
		public ListViewString ListView
		{
			get
			{
				return listView;
			}

			set
			{
				SetListView(value);
			}
		}

		[SerializeField]
		Button toggleButton;

		/// <summary>
		/// Gets or sets the toggle button.
		/// </summary>
		/// <value>The toggle button.</value>
		public Button ToggleButton
		{
			get
			{
				return toggleButton;
			}

			set
			{
				SetToggleButton(value);
			}
		}

		/// <summary>
		/// Is Combobox.InputField editable?
		/// </summary>
		[SerializeField]
		protected bool editable = true;

		/// <summary>
		/// Gets or sets a value indicating whether this is editable and user can add new items.
		/// </summary>
		/// <value><c>true</c> if editable; otherwise, <c>false</c>.</value>
		public bool Editable
		{
			get
			{
				return editable;
			}

			set
			{
				editable = value;
				if (Input != null)
				{
					Input.interactable = editable;
				}
			}
		}

		/// <summary>
		/// Adapter for the InputField.
		/// Required to improve compatibility between different InputFields (like Unity.UI and TextMeshPro versions).
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter Input;

		/// <summary>
		/// Input field value.
		/// </summary>
		/// <value>The input value.</value>
		public string InputValue
		{
			get
			{
				return Input.text;
			}

			set
			{
				if (InputValue != null)
				{
					Input.text = value;
				}
			}
		}

		/// <summary>
		/// Hide ListView on item select or deselect.
		/// </summary>
		[SerializeField]
		[Tooltip("Hide ListView on item select or deselect.")]
		public bool HideAfterItemToggle = true;

		/// <summary>
		/// Allow to add new items to list.
		/// </summary>
		[SerializeField]
		public bool AllowNewItems = true;

		/// <summary>
		/// Reset input if item not found and new items not allowed.
		/// </summary>
		[SerializeField]
		public bool ResetInput = true;

		/// <summary>
		/// OnSelect event.
		/// </summary>
		public ListViewEvent OnSelect = new ListViewEvent();

		/// <summary>
		/// Parent canvas.
		/// </summary>
		[SerializeField]
		protected RectTransform parentCanvas;

		/// <summary>
		/// Canvas where ListView placed.
		/// </summary>
		protected RectTransform ParentCanvas
		{
			get
			{
				if (parentCanvas == null)
				{
					parentCanvas = UtilitiesUI.FindTopmostCanvas(listView.transform.parent);
				}

				return parentCanvas;
			}
		}

		/// <summary>
		/// ListView parent.
		/// </summary>
		protected RectTransform ListViewParent;

		/// <summary>
		/// Raised when ListView opened.
		/// </summary>
		[SerializeField]
		public UnityEvent OnShowListView = new UnityEvent();

		/// <summary>
		/// Raised when ListView closed.
		/// </summary>
		[SerializeField]
		public UnityEvent OnHideListView = new UnityEvent();

		[NonSerialized]
		bool isComboboxInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		public virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isComboboxInited)
			{
				return;
			}

			isComboboxInited = true;

			InitInputFieldProxy();

			SetToggleButton(toggleButton);

			SetListView(listView);

			if (listView != null)
			{
				listView.gameObject.SetActive(true);
				listView.Init();
				if (listView.SelectedIndex != -1)
				{
					InputValue = listView.DataSource[listView.SelectedIndex];
				}

				listView.gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Init InputFieldProxy.
		/// </summary>
		protected virtual void InitInputFieldProxy()
		{
			Input = GetComponent<InputFieldAdapter>();
			Input.interactable = editable;
			Input.onEndEdit.AddListener(InputItem);
		}

		/// <summary>
		/// Sets the list view.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetListView(ListViewString value)
		{
			if (listView != null)
			{
				ListViewParent = null;

				listView.OnSelectObject.RemoveListener(SelectItem);
				listView.OnFocusOut.RemoveListener(OnFocusHideList);

				listView.onCancel.RemoveListener(OnListViewCancel);
				listView.ItemsEvents.Cancel.RemoveListener(OnListViewCancel);

				RemoveDeselectCallbacks();
			}

			listView = value;

			if (listView != null)
			{
				ListViewParent = listView.transform.parent as RectTransform;

				listView.OnSelectObject.AddListener(SelectItem);
				listView.OnFocusOut.AddListener(OnFocusHideList);

				listView.onCancel.AddListener(OnListViewCancel);
				listView.ItemsEvents.Cancel.AddListener(OnListViewCancel);

				AddDeselectCallbacks();
			}
		}

		/// <summary>
		/// Sets the toggle button.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetToggleButton(Button value)
		{
			if (toggleButton != null)
			{
				toggleButton.onClick.RemoveListener(ToggleList);
			}

			toggleButton = value;

			if (toggleButton != null)
			{
				toggleButton.onClick.AddListener(ToggleList);
			}
		}

		/// <summary>
		/// Clear ListView and selected item.
		/// </summary>
		public virtual void Clear()
		{
			listView.DataSource.Clear();
			InputValue = string.Empty;
		}

		/// <summary>
		/// Toggles the list visibility.
		/// </summary>
		public virtual void ToggleList()
		{
			if (listView == null)
			{
				return;
			}

			if (listView.gameObject.activeSelf)
			{
				HideList();
			}
			else
			{
				ShowList();
			}
		}

		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Shows the list.
		/// </summary>
		public virtual void ShowList()
		{
			if (listView == null)
			{
				return;
			}

			listView.gameObject.SetActive(true);

			ModalKey = ModalHelper.Open(this, null, new Color(0, 0, 0, 0f), HideList, ParentCanvas);

			if (ParentCanvas != null)
			{
				ListViewParent = listView.transform.parent as RectTransform;
				listView.transform.SetParent(ParentCanvas);
			}

			if (listView.Layout != null)
			{
				listView.Layout.UpdateLayout();
			}

			if (listView.SelectComponent())
			{
				SetChildDeselectListener(EventSystem.current.currentSelectedGameObject);
			}
			else
			{
				EventSystem.current.SetSelectedGameObject(listView.gameObject);
			}

			OnShowListView.Invoke();
		}

		/// <summary>
		/// Hides the list.
		/// </summary>
		public virtual void HideList()
		{
			if (ModalKey.HasValue)
			{
				ModalHelper.Close(ModalKey.Value);
				ModalKey = null;
			}

			if (listView == null)
			{
				return;
			}

			if (ParentCanvas != null)
			{
				listView.transform.SetParent(ListViewParent);
			}

			listView.gameObject.SetActive(false);
			OnHideListView.Invoke();
		}

		/// <summary>
		/// The children deselect.
		/// </summary>
		protected List<SelectListener> childrenDeselect = new List<SelectListener>();

		/// <summary>
		/// Hide list when focus lost.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected void OnFocusHideList(BaseEventData eventData)
		{
			if (eventData.selectedObject == gameObject)
			{
				return;
			}

			var ev_item = eventData as ListViewItemEventData;
			if (ev_item != null)
			{
				if (ev_item.NewSelectedObject != null)
				{
					SetChildDeselectListener(ev_item.NewSelectedObject);
				}

				return;
			}

			var ev = eventData as PointerEventData;
			if (ev == null)
			{
				HideList();
				return;
			}

			var go = ev.pointerPressRaycast.gameObject;
			if (go == null)
			{
				return;
			}

			if (go.Equals(toggleButton.gameObject))
			{
				return;
			}

			if (go.transform.IsChildOf(listView.transform))
			{
				SetChildDeselectListener(go);
				return;
			}

			HideList();
		}

		/// <summary>
		/// Sets the child deselect listener.
		/// </summary>
		/// <param name="child">Child.</param>
		protected void SetChildDeselectListener(GameObject child)
		{
			var deselectListener = GetDeselectListener(child);
			if (!childrenDeselect.Contains(deselectListener))
			{
				deselectListener.onDeselect.AddListener(OnFocusHideList);
				childrenDeselect.Add(deselectListener);
			}
		}

		/// <summary>
		/// Gets the deselect listener.
		/// </summary>
		/// <returns>The deselect listener.</returns>
		/// <param name="go">Go.</param>
		protected static SelectListener GetDeselectListener(GameObject go)
		{
			return Utilities.GetOrAddComponent<SelectListener>(go);
		}

		/// <summary>
		/// Adds the deselect callbacks.
		/// </summary>
		protected void AddDeselectCallbacks()
		{
			if (listView.ScrollRect == null)
			{
				return;
			}

			if (listView.ScrollRect.verticalScrollbar == null)
			{
				return;
			}

			var scrollbar = listView.ScrollRect.verticalScrollbar.gameObject;
			var deselectListener = GetDeselectListener(scrollbar);

			deselectListener.onDeselect.AddListener(OnFocusHideList);
			childrenDeselect.Add(deselectListener);
		}

		/// <summary>
		/// Removes the deselect callbacks.
		/// </summary>
		protected void RemoveDeselectCallbacks()
		{
			foreach (var c in childrenDeselect)
			{
				RemoveDeselectCallback(c);
			}

			childrenDeselect.Clear();
		}

		/// <summary>
		/// Removes the deselect callback.
		/// </summary>
		/// <param name="listener">Listener.</param>
		protected void RemoveDeselectCallback(SelectListener listener)
		{
			if (listener != null)
			{
				listener.onDeselect.RemoveListener(OnFocusHideList);
			}
		}

		/// <summary>
		/// Set the specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="allowDuplicate">If set to <c>true</c> allow duplicate.</param>
		/// <returns>Index of item.</returns>
		public virtual int Set(string item, bool allowDuplicate = true)
		{
			return listView.Set(item, allowDuplicate);
		}

		/// <summary>
		/// Selects the item.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void SelectItem(int index)
		{
			InputValue = ListView.DataSource[index];

			if (HideAfterItemToggle)
			{
				HideList();

				if ((EventSystem.current != null) && (!EventSystem.current.alreadySelecting))
				{
					EventSystem.current.SetSelectedGameObject(toggleButton.gameObject);
				}
			}

			OnSelect.Invoke(index, InputValue);
		}

		/// <summary>
		/// Work with input.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void InputItem(string item)
		{
			if (!editable)
			{
				return;
			}

			if (string.IsNullOrEmpty(item))
			{
				return;
			}

			if (!listView.DataSource.Contains(item))
			{
				if (AllowNewItems)
				{
					var index = listView.Add(item);
					listView.Select(index);
				}
				else if (ResetInput)
				{
					Input.text = string.Empty;
				}
			}
			else
			{
				var index = listView.DataSource.IndexOf(item);
				listView.Select(index);
			}
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			ListView = null;
			ToggleButton = null;
			if (Input != null)
			{
				Input.onEndEdit.RemoveListener(InputItem);
			}
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		protected void OnListViewCancel()
		{
			HideList();
		}

		/// <summary>
		/// Hide list view.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		/// <param name="eventData">Event data.</param>
		protected void OnListViewCancel(int index, ListViewItem item, BaseEventData eventData)
		{
			HideList();
		}

		/// <summary>
		/// Called when OnSubmit event occurs.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnSubmit(BaseEventData eventData)
		{
			ShowList();
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected virtual void OnValidate()
		{
			if (parentCanvas == null)
			{
				parentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected virtual void Reset()
		{
			if (parentCanvas == null)
			{
				parentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Combobox.SingleInputBackground.ApplyTo(GetComponent<Image>());

			if (toggleButton != null)
			{
				style.Combobox.ToggleButton.ApplyTo(toggleButton.GetComponent<Image>());
			}

			var input = GetComponent<InputField>();
			if (input != null)
			{
				style.Combobox.Input.ApplyTo(input.textComponent, true);
				if (input.placeholder != null)
				{
					style.Combobox.Placeholder.ApplyTo(input.placeholder.gameObject, true);
				}
			}

			if (listView != null)
			{
				listView.SetStyle(style);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Combobox.SingleInputBackground.GetFrom(GetComponent<Image>());

			if (toggleButton != null)
			{
				style.Combobox.ToggleButton.GetFrom(toggleButton.GetComponent<Image>());
			}

			var input = GetComponent<InputField>();
			if (input != null)
			{
				style.Combobox.Input.GetFrom(input.textComponent, true);
				if (input.placeholder != null)
				{
					style.Combobox.Placeholder.GetFrom(input.placeholder.gameObject, true);
				}
			}

			if (listView != null)
			{
				listView.GetStyle(style);
			}

			return true;
		}
		#endregion
	}
}