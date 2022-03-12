namespace UIWidgets.Menu
{
	using System.ComponentModel;
	using System.Runtime.CompilerServices;
	using UIWidgets.l10n;
	using UnityEngine;
#if ENABLE_INPUT_SYSTEM
	using UnityEngine.InputSystem;
#endif

	/// <summary>
	/// Menu item.
	/// </summary>
	public class MenuItem : IObservable, INotifyPropertyChanged
	{
		string name;

		/// <summary>
		/// Name.
		/// </summary>
		public string Name
		{
			get
			{
				return name;
			}

			set
			{
				if (name != value)
				{
					name = value;
					NotifyPropertyChanged("Name");
				}
			}
		}

		string template;

		/// <summary>
		/// Template.
		/// </summary>
		public string Template
		{
			get
			{
				return template;
			}

			set
			{
				if (template != value)
				{
					template = value;
					NotifyPropertyChanged("Template");
				}
			}
		}

		bool isChecked;

		/// <summary>
		/// Checked.
		/// </summary>
		public bool Checked
		{
			get
			{
				return isChecked;
			}

			set
			{
				if (isChecked != value)
				{
					isChecked = value;
					NotifyPropertyChanged("Checked");
				}
			}
		}

		Sprite icon;

		/// <summary>
		/// Icon.
		/// </summary>
		public Sprite Icon
		{
			get
			{
				return icon;
			}

			set
			{
				if (icon != value)
				{
					icon = value;
					NotifyPropertyChanged("Icon");
				}
			}
		}

		HotKey hotKey;

		/// <summary>
		/// Hot Key.
		/// </summary>
		public HotKey HotKey
		{
			get
			{
				return hotKey;
			}

			set
			{
				if (hotKey != value)
				{
					DestroyInputAction();

					hotKey = value;

					CreateHotKeyAction();

					NotifyPropertyChanged("HotKey");
				}
			}
		}

		bool visible = true;

		/// <summary>
		/// Is item visible?
		/// </summary>
		public bool Visible
		{
			get
			{
				return visible;
			}

			set
			{
				if (visible != value)
				{
					visible = value;
					NotifyPropertyChanged("Visible");
				}
			}
		}

		bool interactable = true;

		/// <summary>
		/// Is item interactable?
		/// </summary>
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
					NotifyPropertyChanged("Interactable");
				}
			}
		}

		object tag;

		/// <summary>
		/// Icon.
		/// </summary>
		public object Tag
		{
			get
			{
				return tag;
			}

			set
			{
				if (tag != value)
				{
					tag = value;
					NotifyPropertyChanged("Tag");
				}
			}
		}

		ObservableList<MenuItem> items;

		/// <summary>
		/// Nested items.
		/// </summary>
		public ObservableList<MenuItem> Items
		{
			get
			{
				return items;
			}

			set
			{
				if (items == value)
				{
					return;
				}

				if (items != null)
				{
					items.OnCollectionChange -= ItemsChanged;
				}

				items = value;

				if (items != null)
				{
					items.OnCollectionChange += ItemsChanged;
				}

				NotifyPropertyChanged("Items");
			}
		}

		MenuItemAction action = new MenuItemAction();

		/// <summary>
		/// Action.
		/// </summary>
		public MenuItemAction Action
		{
			get
			{
				return action;
			}

			set
			{
				if (action != value)
				{
					action = value;
					NotifyPropertyChanged("Action");
				}
			}
		}

		/// <summary>
		/// Has visible items?
		/// </summary>
		public bool HasVisibleItems
		{
			get
			{
				if (Items == null)
				{
					return false;
				}

				for (int i = 0; i < Items.Count; i++)
				{
					if (Items[i].Visible)
					{
						return true;
					}
				}

				return false;
			}
		}

		/// <summary>
		/// Property changed event.
		/// </summary>
		public event OnChange OnChange;

		/// <summary>
		/// Property changed event.
		/// </summary>
		public event PropertyChangedEventHandler PropertyChanged;

		/// <summary>
		/// Initializes a new instance of the <see cref="MenuItem"/> class.
		/// </summary>
		public MenuItem()
		{
			Items = new ObservableList<MenuItem>();
		}

		/// <summary>
		/// Items changed.
		/// </summary>
		protected void ItemsChanged()
		{
			NotifyPropertyChanged("Items");
		}

		/// <summary>
		/// Property changed.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		protected void NotifyPropertyChanged(string propertyName)
		{
			var c_handlers = OnChange;
			if (c_handlers != null)
			{
				c_handlers();
			}

			var handlers = PropertyChanged;
			if (handlers != null)
			{
				handlers(this, new PropertyChangedEventArgs(propertyName));
			}
		}

		#if ENABLE_INPUT_SYSTEM
		/// <summary>
		/// HotKey action.
		/// </summary>
		protected InputAction HotKeyAction;
		#endif

		/// <summary>
		/// Is hot key enabled.
		/// Supported for the InputSystem only.
		/// </summary>
		public bool IsEnabledHotKey
		{
			get
			{
				#if ENABLE_INPUT_SYSTEM
				if (HotKeyAction == null)
				{
					return false;
				}

				return HotKeyAction.enabled;
				#else
				return false;
				#endif
			}
		}

		/// <summary>
		/// Enable hot key.
		/// Supported for the InputSystem only.
		/// </summary>
		public void EnableHotKey()
		{
			#if ENABLE_INPUT_SYSTEM
			if (HotKeyAction == null)
			{
				CreateHotKeyAction();
			}

			if (HotKeyAction != null)
			{
				HotKeyAction.Enable();
			}
			#endif
		}

		/// <summary>
		/// Disable hot key.
		/// Supported for the InputSystem only.
		/// </summary>
		public void DisableHotKey()
		{
			#if ENABLE_INPUT_SYSTEM
			if (HotKeyAction == null)
			{
				return;
			}

			HotKeyAction.Disable();
			#endif
		}

		/// <summary>
		/// Destroy current HotKey action.
		/// </summary>
		protected virtual void DestroyInputAction()
		{
			#if ENABLE_INPUT_SYSTEM
			if (HotKeyAction != null)
			{
				HotKeyAction.Disable();
				HotKeyAction.Dispose();
				HotKeyAction = null;
			}
			#endif
		}

		/// <summary>
		/// Create HotKey action.
		/// </summary>
		protected virtual void CreateHotKeyAction()
		{
			#if ENABLE_INPUT_SYSTEM
			if (!hotKey.Valid)
			{
				return;
			}

			HotKeyAction = new InputAction(Name);
			var group = CompatibilityInput.KeysGroup(hotKey.Key);

			if (hotKey.Modifiers > 0)
			{
				InputActionSetupExtensions.CompositeSyntax composite;

				if (hotKey.Modifiers == 3)
				{
					composite = HotKeyAction.AddCompositeBinding("CustomButtonWithThreeModifiers");

					composite = composite.With("Modifier1", "<Keyboard>/Ctrl");
					composite = composite.With("Modifier2", "<Keyboard>/Alt");
					composite = composite.With("Modifier3", "<Keyboard>/Shift");
				}
				else if (hotKey.Modifiers == 2)
				{
					composite = HotKeyAction.AddCompositeBinding("ButtonWithTwoModifiers");

					var index = 1;
					if (hotKey.Ctrl)
					{
						composite = composite.With(string.Format("Modifier{0}", index.ToString()), "<Keyboard>/Ctrl");
						index += 1;
					}

					if (hotKey.Alt)
					{
						composite = composite.With(string.Format("Modifier{0}", index.ToString()), "<Keyboard>/Alt");
						index += 1;
					}

					if (hotKey.Shift)
					{
						composite = composite.With(string.Format("Modifier{0}", index.ToString()), "<Keyboard>/Shift");
						index += 1;
					}
				}
				else
				{
					composite = HotKeyAction.AddCompositeBinding("ButtonWithOneModifier");

					if (hotKey.Ctrl)
					{
						composite = composite.With("Modifier", "<Keyboard>/Ctrl");
					}
					else if (hotKey.Alt)
					{
						composite = composite.With("Modifier", "<Keyboard>/Alt");
					}
					else if (hotKey.Shift)
					{
						composite = composite.With("Modifier", "<Keyboard>/Shift");
					}
				}

				for (int i = 0; i < group.Keys.Length; i++)
				{
					composite = composite.With("Button", string.Format("<Keyboard>/{0}", Key2String(group.Keys[i])));
				}
			}
			else
			{
				for (int i = 0; i < group.Keys.Length; i++)
				{
					HotKeyAction.AddBinding(string.Format("<Keyboard>/{0}", Key2String(group.Keys[i])));
				}
			}

			HotKeyAction.performed += InvokeAction;
			#endif
		}

#if ENABLE_INPUT_SYSTEM

		/// <summary>
		/// InputSystem key to string.
		/// </summary>
		/// <param name="key">Key.</param>
		/// <returns>String.</returns>
		protected virtual string Key2String(Key key)
		{
			switch (key)
			{
				case Key.Digit0:
					return "0";
				case Key.Digit1:
					return "1";
				case Key.Digit2:
					return "2";
				case Key.Digit3:
					return "3";
				case Key.Digit4:
					return "4";
				case Key.Digit5:
					return "5";
				case Key.Digit6:
					return "6";
				case Key.Digit7:
					return "7";
				case Key.Digit8:
					return "8";
				case Key.Digit9:
					return "9";
			}

			return EnumHelper<Key>.ToString(key);
		}

		/// <summary>
		/// Invoke action.
		/// </summary>
		/// <param name="context">Context.</param>
		protected virtual void InvokeAction(InputAction.CallbackContext context)
		{
			if (Visible && Interactable)
			{
				Action.Invoke(this);
			}
		}
#endif
	}
}