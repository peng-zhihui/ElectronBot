namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Tabs.
	/// http://ilih.ru/images/unity-assets/UIWidgets/Tabs.png
	/// </summary>
	public class Tabs : MonoBehaviour, IStylable<StyleTabs>, IUpgradeable
	{
		/// <summary>
		/// Information of the Tab button.
		/// </summary>
		[Serializable]
		protected class TabButtonInfo
		{
			[SerializeField]
			Tabs Owner;

			[SerializeField]
			Button DefaultButton;

			[SerializeField]
			Button ActiveButton;

			TabButtonComponentBase DefaultComponent;

			TabButtonComponentBase ActiveComponent;

			[SerializeField]
			Tab Tab;

			/// <summary>
			/// Initializes a new instance of the <see cref="TabButtonInfo"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="defaultButton">Default button.</param>
			/// <param name="activeButton">Active button.</param>
			public TabButtonInfo(Tabs owner, Button defaultButton, Button activeButton)
			{
				Owner = owner;

				DefaultButton = defaultButton;
				DefaultButton.transform.SetParent(Owner.Container, false);
				DefaultComponent = DefaultButton.GetComponent<TabButtonComponentBase>();

				ActiveButton = activeButton;
				ActiveButton.transform.SetParent(Owner.Container, false);
				ActiveComponent = ActiveButton.GetComponent<TabButtonComponentBase>();

				DefaultButton.onClick.AddListener(Click);
			}

			/// <summary>
			/// Process the click event.
			/// </summary>
			protected void Click()
			{
				Owner.ProcessButtonClick(Tab);
			}

			/// <summary>
			/// Set the tab.
			/// </summary>
			/// <param name="tab">Tab.</param>
			public void SetTab(Tab tab)
			{
				Tab = tab;

				SetData();
			}

			/// <summary>
			/// Set buttons data.
			/// </summary>
			public void SetData()
			{
				DefaultComponent.SetButtonData(Tab);
				ActiveComponent.SetButtonData(Tab);
			}

			/// <summary>
			/// Remove buttons callback.
			/// </summary>
			protected void RemoveCallback()
			{
				if (DefaultButton != null)
				{
					DefaultButton.onClick.RemoveListener(Click);
				}
			}

			/// <summary>
			/// Enable buttons interactions.
			/// </summary>
			public void EnableInteractable()
			{
				DefaultButton.interactable = true;
				ActiveButton.interactable = true;
			}

			/// <summary>
			/// Disable buttons interactions
			/// </summary>
			public void DisableInteractable()
			{
				DefaultButton.interactable = false;
				ActiveButton.interactable = false;
			}

			/// <summary>
			/// Toggle to the default state.
			/// </summary>
			public void Default()
			{
				DefaultButton.gameObject.SetActive(true);
				ActiveButton.gameObject.SetActive(false);
			}

			/// <summary>
			/// Toggle to the active state.
			/// </summary>
			public void Active()
			{
				DefaultButton.gameObject.SetActive(false);
				ActiveButton.gameObject.SetActive(true);

				EventSystem.current.SetSelectedGameObject(ActiveButton.gameObject);
			}

			/// <summary>
			/// Set the style.
			/// </summary>
			/// <param name="style">Style.</param>
			public void SetStyle(StyleTabs style)
			{
				style.DefaultButton.ApplyTo(DefaultButton.gameObject);
				style.ActiveButton.ApplyTo(ActiveButton.gameObject);
			}

			/// <summary>
			/// Destroy buttons.
			/// </summary>
			public void Destroy()
			{
				RemoveCallback();

				if (DefaultButton != null)
				{
					UnityEngine.Object.Destroy(DefaultButton.gameObject);
				}

				if (ActiveButton != null)
				{
					UnityEngine.Object.Destroy(ActiveButton.gameObject);
				}

				Owner = null;
			}
		}

		/// <summary>
		/// Container for the tabs buttons.
		/// </summary>
		[SerializeField]
		public Transform Container;

		/// <summary>
		/// The default tab button.
		/// </summary>
		[SerializeField]
		public Button DefaultTabButton;

		/// <summary>
		/// The active tab button.
		/// </summary>
		[SerializeField]
		public Button ActiveTabButton;

		[SerializeField]
		Tab[] tabObjects = Compatibility.EmptyArray<Tab>();

		/// <summary>
		/// Gets or sets the tab objects.
		/// </summary>
		/// <value>The tab objects.</value>
		public Tab[] TabObjects
		{
			get
			{
				return tabObjects;
			}

			set
			{
				tabObjects = value;
				UpdateButtons();
			}
		}

		/// <summary>
		/// The name of the default tab.
		/// </summary>
		[SerializeField]
		[Tooltip("Tab name which will be active by default, if not specified will be opened first Tab.")]
		public string DefaultTabName = string.Empty;

		/// <summary>
		/// If true does not deactivate hidden tabs.
		/// </summary>
		[SerializeField]
		[Tooltip("If true does not deactivate hidden tabs.")]
		public bool KeepTabsActive = false;

		/// <summary>
		/// OnTabSelect event.
		/// </summary>
		[SerializeField]
		public TabSelectEvent OnTabSelect = new TabSelectEvent();

		/// <summary>
		/// Gets or sets the selected tab.
		/// </summary>
		/// <value>The selected tab.</value>
		public Tab SelectedTab
		{
			get;
			protected set;
		}

		/// <summary>
		/// Index of the selected tab.
		/// </summary>
		public int SelectedTabIndex
		{
			get
			{
				return Array.IndexOf(TabObjects, SelectedTab);
			}
		}

		/// <summary>
		/// Buttons.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<TabButtonInfo> Buttons = new List<TabButtonInfo>();

		/// <summary>
		/// Check is tab can be selected.
		/// </summary>
		public Func<Tab, bool> CanSelectTab = AllowSelect;

		/// <summary>
		/// Default function for the CanSelectTab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		/// <returns>true</returns>
		public static bool AllowSelect(Tab tab)
		{
			return true;
		}

		bool isInited;

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
			if (isInited)
			{
				return;
			}

			isInited = true;

			if (Container == null)
			{
				throw new InvalidOperationException("Container is null. Set object of type GameObject to Container.");
			}

			if (DefaultTabButton == null)
			{
				throw new InvalidOperationException("DefaultTabButton is null. Set object of type GameObject to DefaultTabButton.");
			}

			if (ActiveTabButton == null)
			{
				throw new InvalidOperationException("ActiveTabButton is null. Set object of type GameObject to ActiveTabButton.");
			}

			DefaultTabButton.gameObject.SetActive(false);
			ActiveTabButton.gameObject.SetActive(false);

			UpdateButtons();

			Localization.OnLocaleChanged += UpdateButtonsData;
		}

		/// <summary>
		/// Update buttons data.
		/// </summary>
		protected virtual void UpdateButtonsData()
		{
			for (int i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].SetData();
			}
		}

		/// <summary>
		/// Process tab button click.
		/// </summary>
		/// <param name="tab">Tab.</param>
		protected virtual void ProcessButtonClick(Tab tab)
		{
			if (CanSelectTab(tab))
			{
				SelectTab(tab.Name);
			}
		}

		/// <summary>
		/// Updates the buttons.
		/// </summary>
		protected virtual void UpdateButtons()
		{
			CreateButtons();

			if (tabObjects.Length == 0)
			{
				return;
			}

			if (!string.IsNullOrEmpty(DefaultTabName))
			{
				if (IsExistsTabName(DefaultTabName))
				{
					SelectTab(DefaultTabName);
				}
				else
				{
					Debug.LogWarning(string.Format("Tab with specified DefaultTabName \"{0}\" not found. Opened first Tab.", DefaultTabName), this);
					SelectTab(tabObjects[0].Name);
				}
			}
			else
			{
				SelectTab(tabObjects[0].Name);
			}
		}

		/// <summary>
		/// Is exists tab with specified name.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		/// <returns>true if exists tab with specified name; otherwise, false.</returns>
		protected virtual bool IsExistsTabName(string tabName)
		{
			for (int i = 0; i < tabObjects.Length; i++)
			{
				if (tabObjects[i].Name == tabName)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Remove listeners.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Localization.OnLocaleChanged -= UpdateButtonsData;

			for (int i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].Destroy();
			}

			Buttons.Clear();
		}

		/// <summary>
		/// Get index for the specified Tab name.
		/// </summary>
		/// <param name="name">Tab name.</param>
		/// <returns>Index.</returns>
		protected int Name2Index(string name)
		{
			for (int i = 0; i < tabObjects.Length; i++)
			{
				if (tabObjects[i].Name == name)
				{
					return i;
				}
			}

			return -1;
		}

		/// <summary>
		/// Selects the tab.
		/// </summary>
		/// <param name="tabName">Tab name.</param>
		public void SelectTab(string tabName)
		{
			var index = Name2Index(tabName);
			if (index == -1)
			{
				throw new ArgumentException(string.Format("Tab with name \"{0}\" not found.", tabName));
			}

			if (KeepTabsActive)
			{
				tabObjects[index].TabObject.transform.SetAsLastSibling();
			}
			else
			{
				foreach (var tab in tabObjects)
				{
					DeactivateTab(tab);
				}

				tabObjects[index].TabObject.SetActive(true);
			}

			foreach (var button in Buttons)
			{
				DefaultState(button);
			}

			Buttons[index].Active();

			SelectedTab = tabObjects[index];
			OnTabSelect.Invoke(index);
		}

		/// <summary>
		/// Deactivate tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		protected virtual void DeactivateTab(Tab tab)
		{
			tab.TabObject.SetActive(false);
		}

		/// <summary>
		/// Activate button.
		/// </summary>
		/// <param name="button">Button.</param>
		protected virtual void DefaultState(TabButtonInfo button)
		{
			button.Default();
		}

		/// <summary>
		/// Enable button interactions.
		/// </summary>
		/// <param name="button">Button.</param>
		protected virtual void EnableInteractable(TabButtonInfo button)
		{
			button.EnableInteractable();
		}

		/// <summary>
		/// Creates the buttons.
		/// </summary>
		protected virtual void CreateButtons()
		{
			foreach (var button in Buttons)
			{
				EnableInteractable(button);
			}

			if (tabObjects.Length > Buttons.Count)
			{
				for (int i = Buttons.Count; i < tabObjects.Length; i++)
				{
					var defaultButton = Compatibility.Instantiate(DefaultTabButton);

					var activeButton = Compatibility.Instantiate(ActiveTabButton);

					Buttons.Add(new TabButtonInfo(this, defaultButton, activeButton));
				}
			}

			// delete existing buttons if necessary
			if (tabObjects.Length < Buttons.Count)
			{
				for (int i = Buttons.Count - 1; i > tabObjects.Length - 1; i--)
				{
					Buttons[i].Destroy();

					Buttons.RemoveAt(i);
				}
			}

			for (int i = 0; i < Buttons.Count; i++)
			{
				SetButtonName(Buttons[i], i);
			}
		}

		/// <summary>
		/// Sets the name of the button.
		/// </summary>
		/// <param name="button">Button.</param>
		/// <param name="index">Index.</param>
		protected virtual void SetButtonName(TabButtonInfo button, int index)
		{
			button.SetTab(TabObjects[index]);
		}

		/// <summary>
		/// Disable the tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void DisableTab(Tab tab)
		{
			var i = Array.IndexOf(TabObjects, tab);
			if (i != -1)
			{
				Buttons[i].DisableInteractable();
			}
		}

		/// <summary>
		/// Enable the tab.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public virtual void EnableTab(Tab tab)
		{
			var i = Array.IndexOf(TabObjects, tab);
			if (i != -1)
			{
				Buttons[i].EnableInteractable();
			}
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
			if (DefaultTabButton != null)
			{
				var default_info = Utilities.GetOrAddComponent<TabButtonComponentBase>(DefaultTabButton);
				default_info.Upgrade();
				if (default_info.NameAdapter == null)
				{
					Utilities.GetOrAddComponent(Compatibility.GetComponentInChildren<Text>(default_info, true), ref default_info.NameAdapter);
				}
			}

			if (ActiveTabButton != null)
			{
				var active_info = Utilities.GetOrAddComponent<TabButtonComponentBase>(ActiveTabButton);
				active_info.Upgrade();
				if (active_info.NameAdapter == null)
				{
					Utilities.GetOrAddComponent(Compatibility.GetComponentInChildren<Text>(active_info, true), ref active_info.NameAdapter);
				}
			}
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

		/// <inheritdoc/>
		public virtual bool SetStyle(StyleTabs styleTyped, Style style)
		{
			if (DefaultTabButton != null)
			{
				styleTyped.DefaultButton.ApplyTo(DefaultTabButton.gameObject);
			}

			if (ActiveTabButton != null)
			{
				styleTyped.ActiveButton.ApplyTo(ActiveTabButton.gameObject);
			}

			for (int i = 0; i < Buttons.Count; i++)
			{
				Buttons[i].SetStyle(styleTyped);
			}

			for (var i = 0; i < tabObjects.Length; i++)
			{
				var tab = tabObjects[i];
				if (tab.TabObject != null)
				{
					styleTyped.ContentBackground.ApplyTo(tab.TabObject.GetComponent<Image>());
					style.ApplyForChildren(tab.TabObject);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(StyleTabs styleTyped, Style style)
		{
			if (DefaultTabButton != null)
			{
				styleTyped.DefaultButton.GetFrom(DefaultTabButton.gameObject);
			}

			if (ActiveTabButton != null)
			{
				styleTyped.ActiveButton.GetFrom(ActiveTabButton.gameObject);
			}

			for (var i = 0; i < tabObjects.Length; i++)
			{
				var tab = tabObjects[i];
				if (tab.TabObject != null)
				{
					styleTyped.ContentBackground.GetFrom(tab.TabObject.GetComponent<Image>());
					style.GetFromChildren(tab.TabObject);
				}
			}

			return true;
		}

		#endregion
	}
}