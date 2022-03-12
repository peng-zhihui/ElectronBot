namespace UIWidgets.Menu
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
#if ENABLE_INPUT_SYSTEM
	using UnityEngine.InputSystem;
#endif
	using UnityEngine.Serialization;

	/// <summary>
	/// Context menu.
	/// Contains menu items and reference to the menu template.
	/// </summary>
	[RequireComponent(typeof(RectTransform))]
	[DisallowMultipleComponent]
	[AddComponentMenu("UI/New UI Widgets/ContextMenu")]
	[DataBindSupport]
	public partial class ContextMenu : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IStylable
		#if !ENABLE_INPUT_SYSTEM
		, IUpdatable
		#endif
	{
		/// <summary>
		/// Serialized menu item.
		/// </summary>
		[Serializable]
		protected class MenuItemSerialized
		{
			/// <summary>
			/// Depth.
			/// </summary>
			[SerializeField]
			public int Depth;

			/// <summary>
			/// Icon.
			/// </summary>
			[SerializeField]
			public Sprite Icon;

			/// <summary>
			/// Checked.
			/// </summary>
			[SerializeField]
			public bool Checked;

			/// <summary>
			/// Name.
			/// </summary>
			[SerializeField]
			public string Name;

			/// <summary>
			/// Template.
			/// </summary>
			[SerializeField]
			public string Template;

			/// <summary>
			/// Name.
			/// </summary>
			[SerializeField]
			public HotKey HotKey;

			/// <summary>
			/// Is item visible?
			/// </summary>
			[SerializeField]
			public bool Visible;

			/// <summary>
			/// Is item interactable.
			/// </summary>
			[SerializeField]
			public bool Interactable;

			/// <summary>
			/// For the editor use only.
			/// </summary>
			[SerializeField]
			[HideInInspector]
#pragma warning disable 0414
			bool showAction = false;
#pragma warning restore 0414

			/// <summary>
			/// Action.
			/// </summary>
			[SerializeField]
			public MenuItemAction Action = new MenuItemAction();

			/// <summary>
			/// Convert specified instance to the menu item.
			/// </summary>
			/// <param name="serialized">Serialized instance.</param>
			/// <returns>Menu item.</returns>
			public static MenuItem ToMenuItem(MenuItemSerialized serialized)
			{
				return serialized;
			}

			/// <summary>
			/// Convert specified instance to the menu item.
			/// </summary>
			/// <param name="serialized">Serialized instance.</param>
			public static implicit operator MenuItem(MenuItemSerialized serialized)
			{
				return new MenuItem()
				{
					Icon = serialized.Icon,
					Checked = serialized.Checked,
					Name = serialized.Name,
					Template = serialized.Template,
					HotKey = serialized.HotKey,
					Visible = serialized.Visible,
					Interactable = serialized.Interactable,
					Action = serialized.Action,
				};
			}
		}

		#region Interactable
		[SerializeField]
		bool interactable = true;

		/// <summary>
		/// Is widget interactable.
		/// </summary>
		/// <value><c>true</c> if interactable; otherwise, <c>false</c>.</value>
		[DataBindField]
		public bool Interactable
		{
			get
			{
				return interactable;
			}

			set
			{
				if (interactable != value)
				{
					interactable = value;
					InteractableChanged();
				}
			}
		}

		/// <summary>
		/// If the canvas groups allow interaction.
		/// </summary>
		protected bool GroupsAllowInteraction = true;

		/// <summary>
		/// The CanvasGroup cache.
		/// </summary>
		protected List<CanvasGroup> CanvasGroupCache = new List<CanvasGroup>();

		/// <summary>
		/// Process the CanvasGroupChanged event.
		/// </summary>
		protected override void OnCanvasGroupChanged()
		{
			var groupAllowInteraction = true;
			var t = transform;
			while (t != null)
			{
				t.GetComponents(CanvasGroupCache);
				var shouldBreak = false;
				foreach (var canvas_group in CanvasGroupCache)
				{
					if (!canvas_group.interactable)
					{
						groupAllowInteraction = false;
						shouldBreak = true;
					}

					shouldBreak |= canvas_group.ignoreParentGroups;
				}

				if (shouldBreak)
				{
					break;
				}

				t = t.parent;
			}

			if (groupAllowInteraction != GroupsAllowInteraction)
			{
				GroupsAllowInteraction = groupAllowInteraction;
				InteractableChanged();
			}
		}

		/// <summary>
		/// Returns true if the GameObject and the Component are active.
		/// </summary>
		/// <returns>true if the GameObject and the Component are active; otherwise false.</returns>
		public override bool IsActive()
		{
			return base.IsActive() && GroupsAllowInteraction && Interactable && isInited;
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		protected virtual void InteractableChanged()
		{
			if (!base.IsActive() || !isInited)
			{
				return;
			}

			OnInteractableChange(GroupsAllowInteraction && Interactable);
		}

		/// <summary>
		/// Process interactable change.
		/// </summary>
		/// <param name="interactableState">Current interactable state.</param>
		protected virtual void OnInteractableChange(bool interactableState)
		{
		}
		#endregion

		[SerializeField]
		ContextMenuView template;

		/// <summary>
		/// Menu template.
		/// </summary>
		public ContextMenuView Template
		{
			get
			{
				return template;
			}

			set
			{
				template = value;

				if (Instance != null)
				{
					Instance.Template = template;
				}
			}
		}

		/// <summary>
		/// Serialized menu items.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<MenuItemSerialized> MenuItemsSerialized = new List<MenuItemSerialized>();

		ObservableList<MenuItem> menuItems;

		/// <summary>
		/// Menu items.
		/// </summary>
		public ObservableList<MenuItem> MenuItems
		{
			get
			{
				if (menuItems == null)
				{
					menuItems = Deserialize(MenuItemsSerialized);
					menuItems.OnCollectionChange += UpdateMenuItems;
				}

				return menuItems;
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				if (menuItems != null)
				{
					menuItems.OnCollectionChange -= UpdateMenuItems;
					DisableHotKeys(menuItems);
					Instance.ResetItems();
				}

				menuItems = value;
				menuItems.OnCollectionChange += UpdateMenuItems;

				UpdateMenuItems();
			}
		}

		/// <summary>
		/// Is default menu?
		/// </summary>
		[SerializeField]
		public bool IsDefault = false;

		/// <summary>
		/// Allow navigation.
		/// </summary>
		[SerializeField]
		public bool Navigation = true;

		/// <summary>
		/// Open menu on pointer right button click.
		/// </summary>
		[SerializeField]
		public bool OpenOnRightButtonClick = true;

		/// <summary>
		/// Open menu on context menu key.
		/// </summary>
		[SerializeField]
		public bool OpenOnContextMenuKey = true;

		/// <summary>
		/// Time to wait before open or close sub-menu.
		/// </summary>
		[SerializeField]
		public float SubmenuDelay = 0.3f;

		/// <summary>
		/// Use unscaled time.
		/// </summary>
		[SerializeField]
		public bool UnscaledTime = true;

		[SerializeField]
		[FormerlySerializedAs("ParentCanvas")]
		RectTransform parentCanvas;

		/// <summary>
		/// Parent canvas.
		/// </summary>
		public RectTransform ParentCanvas
		{
			get
			{
				return parentCanvas;
			}

			set
			{
				parentCanvas = value;

				if (instance != null)
				{
					instance.ParentCanvas = value;
				}
			}
		}

		/// <summary>
		/// Event on menu open.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuEvent OnOpen = new MenuEvent();

		/// <summary>
		/// Event on menu close.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuEvent OnClose = new MenuEvent();

		/// <summary>
		/// Event on pointer over menu item.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuItemEvent OnItemSelect = new MenuItemEvent();

		/// <summary>
		/// Event on pointer out menu item.
		/// </summary>
		[SerializeField]
		[DataBindEvent]
		public MenuItemEvent OnItemDeselect = new MenuItemEvent();

		MenuInstance instance;

		/// <summary>
		/// Menu instance.
		/// </summary>
		protected MenuInstance Instance
		{
			get
			{
				if (instance == null)
				{
					if (ParentCanvas == null)
					{
						ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
					}

					instance = new MenuInstance(this, Template, MenuItems.AsReadOnly(), ParentCanvas);
				}

				return instance;
			}
		}

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Active menu.
		/// </summary>
		protected static readonly List<ContextMenu> ActiveMenu = new List<ContextMenu>();

		bool isInited;

		/// <summary>
		/// Process the start event.
		/// </summary>
		protected override void Start()
		{
			base.Start();

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

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}

			Template.Init();

			isInited = true;

			CreateToggleAction();

			UpdateMenuItems();

			Localization.OnLocaleChanged += LocaleChanged;
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			if (instance != null)
			{
				instance.LocaleChanged();
			}
		}

		/// <summary>
		/// Process the enable event.
		/// </summary>
		protected override void OnEnable()
		{
			base.OnEnable();

			if (IsDefault)
			{
				for (int i = 0; i < ActiveMenu.Count; i++)
				{
					ActiveMenu[i].IsDefault = false;
				}
			}

			ActiveMenu.Add(this);

			#if !ENABLE_INPUT_SYSTEM
			Updater.Add(this);
			#endif
		}

		/// <summary>
		/// Process the disable event.
		/// </summary>
		protected override void OnDisable()
		{
			base.OnDisable();

			ActiveMenu.Remove(this);

			#if !ENABLE_INPUT_SYSTEM
			Updater.Remove(this);
			#endif
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			base.OnDestroy();

			if (menuItems != null)
			{
				DisableHotKeys(menuItems);
				Instance.ResetItems();

				menuItems.OnCollectionChange -= UpdateMenuItems;
				menuItems = null;
			}

			Localization.OnLocaleChanged -= LocaleChanged;
		}

		/// <summary>
		/// Update menu items.
		/// </summary>
		protected void UpdateMenuItems()
		{
			EnableHotKeys(MenuItems);

			if (Instance.IsOpened)
			{
				Instance.UpdateItems(MenuItems.AsReadOnly());
			}
		}

		/// <summary>
		/// Enable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		/// <param name="items">Items.</param>
		protected virtual void EnableHotKeys(ObservableList<MenuItem> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].EnableHotKey();

				if (items[i].Items != null)
				{
					EnableHotKeys(items[i].Items);
				}
			}
		}

		/// <summary>
		/// Disable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		/// <param name="items">Items.</param>
		protected virtual void DisableHotKeys(ObservableList<MenuItem> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].DisableHotKey();

				if (items[i].Items != null)
				{
					DisableHotKeys(items[i].Items);
				}
			}
		}

		/// <summary>
		/// Enable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		public virtual void EnableHotKeys()
		{
			EnableHotKeys(MenuItems);
		}

		/// <summary>
		/// Disable hot keys.
		/// Supported for the InputSystem only.
		/// </summary>
		public virtual void DisableHotKeys()
		{
			EnableHotKeys(MenuItems);
		}

		/// <summary>
		/// Open.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void Open(PointerEventData eventData)
		{
			var canvas = UtilitiesUI.FindTopmostCanvas(RectTransform);

			Vector2 position;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas, eventData.position, eventData.pressEventCamera, out position);
			var size = canvas.rect.size;
			var pivot = canvas.pivot;

			position.x += size.x * pivot.x;
			position.y -= size.y * pivot.y;

			Open(position);
		}

		/// <summary>
		/// Open menu in the specified position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void Open(Vector2 position)
		{
			Instance.Open(position);

			CurrentMenu = this;
		}

		/// <summary>
		/// Close menu.
		/// </summary>
		public virtual void Close()
		{
			Instance.Close();

			CurrentMenu = null;
		}

		/// <summary>
		/// Get default position.
		/// </summary>
		/// <returns>Position.</returns>
		protected virtual Vector2 GetDefaultPosition()
		{
			var canvas = UtilitiesUI.FindTopmostCanvas(RectTransform);

			var canvas_position = UtilitiesRectTransform.GetTopLeftCornerGlobalPosition(canvas);
			var rt_position = UtilitiesRectTransform.GetTopLeftCornerGlobalPosition(RectTransform);

			return rt_position - canvas_position;
		}

		/// <summary>
		/// Toggle menu.
		/// </summary>
		protected virtual void Toggle()
		{
			if (Instance.IsOpened)
			{
				Instance.Close();
			}
			else
			{
				Instance.Open(GetDefaultPosition());
				Instance.SelectFirst();
			}
		}

		/// <summary>
		/// Process the pointer down event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerDown(PointerEventData eventData)
		{
			// required by OnPointerClick
		}

		/// <summary>
		/// Process the pointer up event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerUp(PointerEventData eventData)
		{
			// required by OnPointerClick
		}

		/// <summary>
		/// Process the pointer click event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public virtual void OnPointerClick(PointerEventData eventData)
		{
			if (!IsActive() || !OpenOnRightButtonClick)
			{
				return;
			}

			if (eventData.button == PointerEventData.InputButton.Right)
			{
				Open(eventData);
			}
		}

		/// <summary>
		/// Deserialize menu items.
		/// </summary>
		/// <param name="serialized">Serializes items.</param>
		/// <returns>Items.</returns>
		protected static ObservableList<MenuItem> Deserialize(List<MenuItemSerialized> serialized)
		{
			var result = new ObservableList<MenuItem>();

			Stack<MenuItem> current_branch = new Stack<MenuItem>();
			var prev_depth = 0;
			for (int i = 0; i < serialized.Count; i++)
			{
				var current = serialized[i];

				if (current.Depth == 0)
				{
					current_branch.Clear();
					current_branch.Push(current);
					result.Add(current_branch.Peek());
				}
				else if (current.Depth == (prev_depth + 1))
				{
					MenuItem item = current;
					current_branch.Peek().Items.Add(item);
					current_branch.Push(item);
				}
				else if (current.Depth <= prev_depth)
				{
					var n = prev_depth + 1 - current.Depth;

					for (int j = 0; j < n; j++)
					{
						current_branch.Pop();
					}

					MenuItem item = current;
					current_branch.Peek().Items.Add(item);
					current_branch.Push(item);
				}
				else
				{
					// Debug.LogWarning("Unknown case");
				}

				prev_depth = current.Depth;
			}

			return result;
		}

		/// <summary>
		/// Is current menu active?
		/// </summary>
		/// <returns>true if current menu active; otherwise false.</returns>
		protected virtual bool IsActiveMenu()
		{
			if (EventSystem.current.currentSelectedGameObject == null)
			{
				return false;
			}

			var t = EventSystem.current.currentSelectedGameObject.transform;

			return (t == transform) || t.IsChildOf(transform);
		}

		/// <summary>
		/// Last frame when toggle was called.
		/// </summary>
		protected static int LastToggledByKey = -2;

		/// <summary>
		/// Current menu.
		/// </summary>
		protected static ContextMenu CurrentMenu;

		/// <summary>
		/// Nested menu.
		/// </summary>
		protected static List<ContextMenu> NestedMenu = new List<ContextMenu>();

		/// <summary>
		/// Toggle active menu.
		/// </summary>
		protected static void ToggleActiveMenuByKey()
		{
			if (LastToggledByKey == UtilitiesTime.GetFrameCount())
			{
				return;
			}

			if ((CurrentMenu != null) && CurrentMenu.OpenOnContextMenuKey)
			{
				CurrentMenu.Toggle();
			}
			else
			{
				var menu = FindActiveMenu();
				if (menu != null)
				{
					menu.Toggle();
				}
			}

			LastToggledByKey = UtilitiesTime.GetFrameCount();
		}

		/// <summary>
		/// Find active menu.
		/// </summary>
		/// <returns>Active menu.</returns>
		protected static ContextMenu FindActiveMenu()
		{
			for (int i = 0; i < ActiveMenu.Count; i++)
			{
				if (ActiveMenu[i].IsActiveMenu() && ActiveMenu[i].OpenOnContextMenuKey)
				{
					NestedMenu.Add(ActiveMenu[i]);
				}
			}

			if (NestedMenu.Count > 0)
			{
				NestedMenu.Sort(MenuComparison);
				var result = NestedMenu[0];
				NestedMenu.Clear();

				return result;
			}

			for (int i = 0; i < ActiveMenu.Count; i++)
			{
				if (ActiveMenu[i].IsDefault && ActiveMenu[i].OpenOnContextMenuKey)
				{
					return ActiveMenu[i];
				}
			}

			return null;
		}

		/// <summary>
		/// Menu comparison.
		/// </summary>
		/// <param name="x">X.</param>
		/// <param name="y">Y.</param>
		/// <returns>Comparison result.</returns>
		protected static int MenuComparison(ContextMenu x, ContextMenu y)
		{
			var x_depth = Utilities.GetDepth(x.transform);
			var y_depth = Utilities.GetDepth(y.transform);

			return -x_depth.CompareTo(y_depth);
		}

		#if UNITY_2019_3_OR_NEWER
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
		static void StaticInit()
		{
			LastToggledByKey = -2;
			NestedMenu.Clear();
			#if ENABLE_INPUT_SYSTEM
			ToggleAction = null;
			#endif
		}
		#endif

		#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// Input action.
		/// </summary>
		protected static InputAction ToggleAction;

		/// <summary>
		/// Create toggle action.
		/// </summary>
		protected virtual void CreateToggleAction()
		{
			if (ToggleAction == null)
			{
				ToggleAction = new InputAction(name);
				ToggleAction.AddBinding(string.Format("<Keyboard>/{0}", EnumHelper<Key>.ToString(Key.ContextMenu)));
				ToggleAction.performed += Toggle;
				ToggleAction.Enable();
			}
		}

		/// <summary>
		/// Toggle active menu.
		/// </summary>
		/// <param name="context">Context.</param>
		protected static void Toggle(InputAction.CallbackContext context)
		{
			ToggleActiveMenuByKey();
		}
		#else
		/// <summary>
		/// Create toggle action.
		/// </summary>
		protected virtual void CreateToggleAction()
		{
		}

		/// <summary>
		/// Process the update event.
		/// </summary>
		public virtual void RunUpdate()
		{
			if (CompatibilityInput.ContextMenuUp)
			{
				ToggleActiveMenuByKey();
			}

			CheckHotKeys(MenuItems);
		}

		/// <summary>
		/// Check hot keys status.
		/// </summary>
		/// <param name="items">Items.</param>
		protected virtual void CheckHotKeys(ObservableList<MenuItem> items)
		{
			for (int i = 0; i < items.Count; i++)
			{
				var item = items[i];

				if (item.Visible && item.Interactable && item.HotKey.IsUp)
				{
					item.Action.Invoke(item);
					Close();
				}

				if (item.Items != null)
				{
					CheckHotKeys(item.Items);
				}
			}
		}
		#endif

		#if UNITY_EDITOR
		/// <summary>
		/// Validate this instance.
		/// </summary>
		protected override void OnValidate()
		{
			base.OnValidate();

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}

		/// <summary>
		/// Reset this instance.
		/// </summary>
		protected override void Reset()
		{
			base.Reset();

			if (ParentCanvas == null)
			{
				ParentCanvas = UtilitiesUI.FindTopmostCanvas(transform);
			}
		}
		#endif

		#region IStyleable

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			Template.SetStyle(style);

			if (instance != null)
			{
				instance.SetStyle(style);
			}

			return false;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			Template.GetStyle(style);

			return false;
		}
		#endregion
	}
}