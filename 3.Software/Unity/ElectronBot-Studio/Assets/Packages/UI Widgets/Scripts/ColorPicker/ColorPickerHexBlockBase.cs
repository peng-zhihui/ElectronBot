namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Color picker Hex block base class.
	/// </summary>
	public class ColorPickerHexBlockBase : MonoBehaviour, IUpgradeable
	{
		/// <summary>
		/// The input field for hex.
		/// </summary>
		[SerializeField]
		protected InputFieldAdapter InputHexAdapter;

		/// <summary>
		/// InputFieldProxy Hex.
		/// </summary>
		[System.Obsolete("Replaced with InputHexAdapter.")]
		protected IInputFieldProxy InputProxyHex
		{
			get
			{
				return InputHexAdapter;
			}
		}

		/// <summary>
		/// Is process color with alpha value?
		/// </summary>
		[SerializeField]
		protected bool withAlpha = true;

		/// <summary>
		/// Is process color with alpha value?
		/// </summary>
		public bool WithAlpha
		{
			get
			{
				return withAlpha;
			}

			set
			{
				withAlpha = value;
				UpdateInputs();
			}
		}

		ColorPickerInputMode inputMode;

		/// <summary>
		/// Gets or sets the input mode.
		/// </summary>
		/// <value>The input mode.</value>
		public ColorPickerInputMode InputMode
		{
			get
			{
				return inputMode;
			}

			set
			{
				inputMode = value;
			}
		}

		ColorPickerPaletteMode paletteMode;

		/// <summary>
		/// Gets or sets the palette mode.
		/// </summary>
		/// <value>The palette mode.</value>
		public ColorPickerPaletteMode PaletteMode
		{
			get
			{
				return paletteMode;
			}

			set
			{
				paletteMode = value;
			}
		}

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		public ColorRGBChangedEvent OnChangeRGB = new ColorRGBChangedEvent();

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

			InitInput();

			AddListeners();

			UpdateInputs();
		}

		/// <summary>
		/// Inits the input.
		/// </summary>
		protected virtual void InitInput()
		{
		}

		/// <summary>
		/// Adds the listeners.
		/// </summary>
		protected virtual void AddListeners()
		{
			InputHexAdapter.onEndEdit.AddListener(InputChanged);
		}

		/// <summary>
		/// Removes the listeners.
		/// </summary>
		protected virtual void RemoveListeners()
		{
			InputHexAdapter.onEndEdit.RemoveListener(InputChanged);
		}

		/// <summary>
		/// Updates the inputs.
		/// </summary>
		protected virtual void UpdateInputs()
		{
			if (InputHexAdapter != null)
			{
				InputHexAdapter.text = WithAlpha ? UtilitiesColor.RGBA2Hex(currentColor) : UtilitiesColor.RGB2Hex(currentColor);
			}
		}

		/// <summary>
		/// Process input changed event.
		/// </summary>
		/// <param name="input">Input.</param>
		protected virtual void InputChanged(string input)
		{
			Color color;
			if (ColorUtility.TryParseHtmlString(input, out color))
			{
				Color32 color32 = color;
				if (!WithAlpha)
				{
					color32.a = currentColor.a;
				}

				currentColor = color32;
				OnChangeRGB.Invoke(currentColor);
			}
		}

		/// <summary>
		/// Current color.
		/// </summary>
		protected Color32 currentColor;

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(Color32 color)
		{
			currentColor = color;
			UpdateInputs();
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(ColorHSV color)
		{
			currentColor = color;
			UpdateInputs();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			RemoveListeners();
		}

		/// <summary>
		/// Upgrade this instance.
		/// </summary>
		public virtual void Upgrade()
		{
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
		/// Set the specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void SetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if (InputHexAdapter != null)
			{
				styleColorPicker.HexInputBackground.ApplyTo(InputHexAdapter);

				if (InputHexAdapter.textComponent != null)
				{
					styleColorPicker.HexInputText.ApplyTo(InputHexAdapter.textComponent.gameObject);
				}

				if (InputHexAdapter.placeholder != null)
				{
					styleColorPicker.HexInputPlaceholder.ApplyTo(InputHexAdapter.placeholder.gameObject);
				}
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void GetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if (InputHexAdapter != null)
			{
				styleColorPicker.HexInputBackground.GetFrom(InputHexAdapter);

				if (InputHexAdapter.textComponent != null)
				{
					styleColorPicker.HexInputText.GetFrom(InputHexAdapter.textComponent.gameObject);
				}

				if (InputHexAdapter.placeholder != null)
				{
					styleColorPicker.HexInputPlaceholder.GetFrom(InputHexAdapter.placeholder.gameObject);
				}
			}
		}
		#endregion
	}
}