namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;

	/// <summary>
	/// Base class for the AutoCombobox widget.
	/// </summary>
	/// <typeparam name="TItem">Item type.</typeparam>
	/// <typeparam name="TListView">ListView type.</typeparam>
	/// <typeparam name="TListViewComponent">ListView.DefaultItem type.</typeparam>
	/// <typeparam name="TAutocomplete">Autocomplete type.</typeparam>
	/// <typeparam name="TCombobox">Combobox type.</typeparam>
	public abstract class AutoCombobox<TItem, TListView, TListViewComponent, TAutocomplete, TCombobox> : MonoBehaviour, IStylable
		where TListView : ListViewCustom<TListViewComponent, TItem>
		where TListViewComponent : ListViewItem
		where TAutocomplete : AutocompleteCustom<TItem, TListViewComponent, TListView>
		where TCombobox : ComboboxCustom<TListView, TListViewComponent, TItem>
	{
		/// <summary>
		/// Autocomplete.
		/// </summary>
		[SerializeField]
		public TAutocomplete Autocomplete;

		/// <summary>
		/// Combobox.
		/// </summary>
		[SerializeField]
		public TCombobox Combobox;

		/// <summary>
		/// Add items if not found.
		/// </summary>
		[SerializeField]
		[Tooltip("Requires overrided Input2Item method.")]
		public bool AddItems = false;

		/// <summary>
		/// Keep selected items for Autocomplete.DisplayListView.
		/// </summary>
		[SerializeField]
		public bool KeepSelection = false;

		/// <summary>
		/// Autocomplete input listener.
		/// </summary>
		protected DeselectListener AutocompleteInputListener;

		/// <summary>
		/// Temporary list
		/// </summary>
		protected List<TItem> SelectedItems = new List<TItem>();

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
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			Autocomplete.Init();
			Autocomplete.OnOptionSelectedItem.AddListener(OnOptionSelected);
			Autocomplete.OnItemNotFound.AddListener(ProcessItemNotFound);
			Autocomplete.OnCancelInput.AddListener(ProcessCancel);
			Autocomplete.OnSearchCompleted.AddListener(UpdateAutocompleteListViewSelected);
			Autocomplete.ResetListViewSelection = !KeepSelection;

			Combobox.Init();
			Combobox.ListView.OnSelectObject.AddListener(OnSelect);
			Combobox.OnCurrentClick.AddListener(AutocompleteShow);
			Combobox.ListView.OnDataSourceChanged.AddListener(ListViewDataSourceChanged);

			AutocompleteInputListener = Utilities.GetOrAddComponent<DeselectListener>(Autocomplete.InputFieldAdapter);
			AutocompleteInputListener.Deselect.AddListener(AutocompleteHide);

			Autocomplete.DataSource = Combobox.ListView.DataSource.ListReference();

			AutocompleteHide();
		}

		/// <summary>
		/// Set Autocomplete.DisplayListView selected items.
		/// </summary>
		protected virtual void UpdateAutocompleteListViewSelected()
		{
			if (!KeepSelection)
			{
				return;
			}

			Combobox.ListView.GetSelectedItems(SelectedItems);
			Autocomplete.DisplayListView.SetSelectedItems(SelectedItems, true);
			SelectedItems.Clear();
		}

		/// <summary>
		/// Process DataSource changed event.
		/// </summary>
		/// <param name="listView">ListView.</param>
		protected virtual void ListViewDataSourceChanged(ListViewCustom<TListViewComponent, TItem> listView)
		{
			Autocomplete.DataSource = listView.DataSource.ListReference();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (Autocomplete != null)
			{
				Autocomplete.OnOptionSelectedItem.RemoveListener(OnOptionSelected);
				Autocomplete.OnItemNotFound.RemoveListener(ProcessItemNotFound);
				Autocomplete.OnCancelInput.RemoveListener(ProcessCancel);
				Autocomplete.OnSearchCompleted.RemoveListener(UpdateAutocompleteListViewSelected);
			}

			if (Combobox != null)
			{
				Combobox.ListView.OnSelectObject.RemoveListener(OnSelect);
				Combobox.OnCurrentClick.RemoveListener(AutocompleteShow);
				Combobox.ListView.OnDataSourceChanged.RemoveListener(ListViewDataSourceChanged);
			}

			if (AutocompleteInputListener != null)
			{
				AutocompleteInputListener.Deselect.RemoveListener(AutocompleteHide);
			}
		}

		/// <summary>
		/// Process selected option.
		/// </summary>
		/// <param name="item">Item.</param>
		protected virtual void OnOptionSelected(TItem item)
		{
			var index = Combobox.ListView.DataSource.IndexOf(item);
			Combobox.ListView.Select(index);

			AutocompleteHide();
		}

		/// <summary>
		/// Process item not found event.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void ProcessItemNotFound(string input)
		{
			var index = Input2Index(input);

			if (Combobox.ListView.IsValid(index))
			{
				Combobox.ListView.Select(index);
			}

			AutocompleteHide();
		}

		/// <summary>
		/// Process cancel event.
		/// </summary>
		protected virtual void ProcessCancel()
		{
			AutocompleteHide();
		}

		/// <summary>
		/// Get item index by input.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <returns>Index.</returns>
		protected virtual int Input2Index(string input)
		{
			for (int i = 0; i < Combobox.ListView.DataSource.Count; i++)
			{
				var item = Combobox.ListView.DataSource[i];
				if (UtilitiesCompare.Compare(GetStringValue(item), input) == 0)
				{
					return i;
				}
			}

			if (AddItems)
			{
				var new_item = Input2Item(input);
				if (new_item != null)
				{
					return Combobox.ListView.Add(new_item);
				}
			}

			return -1;
		}

		/// <summary>
		/// Create a new item by specified input.
		/// </summary>
		/// <param name="input">Input.</param>
		/// <returns>New item.</returns>
		protected virtual TItem Input2Item(string input)
		{
			return default(TItem);
		}

		/// <summary>
		/// Process the select event.
		/// </summary>
		/// <param name="index">Index.</param>
		protected virtual void OnSelect(int index)
		{
			Autocomplete.InputFieldAdapter.text = GetStringValue(Combobox.ListView.DataSource[index]);
		}

		/// <summary>
		/// Convert item to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="item">Item.</param>
		protected abstract string GetStringValue(TItem item);

		/// <summary>
		/// Has selected item?
		/// </summary>
		/// <returns>true if ListView has selected item; otherwise false.</returns>
		protected bool HasSelected()
		{
			return Combobox.ListView.SelectedIndex >= 0;
		}

		/// <summary>
		/// Show autocomplete.
		/// </summary>
		/// <param name="index">Index.</param>
		/// <param name="item">Item.</param>
		protected virtual void AutocompleteShow(int index, TItem item)
		{
			AutocompleteShow();
		}

		/// <summary>
		/// Show autocomplete.
		/// </summary>
		protected virtual void AutocompleteShow()
		{
			if (HasSelected())
			{
				Autocomplete.InputFieldAdapter.text = GetStringValue(Combobox.ListView.SelectedItem);
			}

			Autocomplete.gameObject.SetActive(true);
			Combobox.HideCurrent();

			if (!EventSystem.current.alreadySelecting)
			{
				EventSystem.current.SetSelectedGameObject(Autocomplete.InputFieldAdapter.gameObject);
			}
		}

		/// <summary>
		/// Hide autocomplete.
		/// </summary>
		protected virtual void AutocompleteHide()
		{
			if (HasSelected())
			{
				Autocomplete.gameObject.SetActive(false);
				Combobox.ShowCurrent();
			}
			else
			{
				AutocompleteShow();
			}
		}

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			Autocomplete.SetStyle(style);
			Combobox.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			Autocomplete.GetStyle(style);
			Combobox.GetStyle(style);

			return true;
		}
	}
}