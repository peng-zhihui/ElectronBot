namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker Alpha slider block.
	/// </summary>
	public class ColorPickerABlock : MonoBehaviour
	{
		[SerializeField]
		Slider aSlider;

		/// <summary>
		/// Gets or sets Alpha slider.
		/// </summary>
		/// <value>Alpha slider.</value>
		public Slider ASlider
		{
			get
			{
				return aSlider;
			}

			set
			{
				SetASlider(value);
			}
		}

		[SerializeField]
		Spinner aInput;

		/// <summary>
		/// Gets or sets Alpha spinner.
		/// </summary>
		/// <value>Alpha spinner.</value>
		public Spinner AInput
		{
			get
			{
				return aInput;
			}

			set
			{
				SetAInput(value);
			}
		}

		[SerializeField]
		Image aSliderBackground;

		/// <summary>
		/// Gets or sets Alpha slider background.
		/// </summary>
		/// <value>Alpha slider background.</value>
		public Image ASliderBackground
		{
			get
			{
				return aSliderBackground;
			}

			set
			{
				aSliderBackground = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Shader defaultShader;

		/// <summary>
		/// Gets or sets the default shader to display alpha gradient in ASliderBackground.
		/// </summary>
		/// <value>The default shader.</value>
		public Shader DefaultShader
		{
			get
			{
				return defaultShader;
			}

			set
			{
				defaultShader = value;
				UpdateMaterial();
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

		/// <summary>
		/// OnChangeHSV event.
		/// </summary>
		public ColorHSVChangedEvent OnChangeHSV = new ColorHSVChangedEvent();

		/// <summary>
		/// OnChangeAlpha event.
		/// </summary>
		public ColorAlphaChangedEvent OnChangeAlpha = new ColorAlphaChangedEvent();

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

			ASlider = aSlider;
			AInput = aInput;
			ASliderBackground = aSliderBackground;
		}

		/// <summary>
		/// Sets Alpha slider.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetASlider(Slider value)
		{
			if (aSlider != null)
			{
				aSlider.onValueChanged.RemoveListener(SliderValueChanged);
			}

			aSlider = value;

			if (aSlider != null)
			{
				aSlider.onValueChanged.AddListener(SliderValueChanged);
				UpdateView();
			}
		}

		/// <summary>
		/// Sets Alpha input.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetAInput(Spinner value)
		{
			if (aInput != null)
			{
				aInput.onValueChangeInt.RemoveListener(SpinnerValueChanged);
			}

			aInput = value;

			if (aInput != null)
			{
				aInput.onValueChangeInt.AddListener(SpinnerValueChanged);
				UpdateView();
			}
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateMaterial();
		}

		void SpinnerValueChanged(int value)
		{
			ValueChanged(isSlider: false);
		}

		void SliderValueChanged(float value)
		{
			ValueChanged();
		}

		/// <summary>
		/// If in update mode?
		/// </summary>
		protected bool inUpdateMode;

		/// <summary>
		/// Values the changed.
		/// </summary>
		/// <param name="isSlider">Is slider value changed?</param>
		protected virtual void ValueChanged(bool isSlider = true)
		{
			if (inUpdateMode)
			{
				return;
			}

			OnChangeAlpha.Invoke(GetAlpha(isSlider));
		}

		/// <summary>
		/// Gets the alpha.
		/// </summary>
		/// <param name="isSlider">Is slider value changed?</param>
		/// <returns>The alpha.</returns>
		protected byte GetAlpha(bool isSlider = true)
		{
			if ((aSlider != null) && isSlider)
			{
				return (byte)aSlider.value;
			}

			if (aInput != null)
			{
				return (byte)aInput.Value;
			}

			return currentColor.a;
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
			UpdateView();
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(ColorHSV color)
		{
			currentColor = color;
			UpdateView();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			inUpdateMode = true;

			if (aSlider != null)
			{
				aSlider.value = currentColor.a;
			}

			if (aInput != null)
			{
				aInput.Value = currentColor.a;
			}

			inUpdateMode = false;
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			if (defaultShader == null)
			{
				return;
			}

			if (aSliderBackground == null)
			{
				return;
			}

			var material = new Material(defaultShader);
			material.SetColor(ColorPicker.ShaderIDs.Left, Color.black);
			material.SetColor(ColorPicker.ShaderIDs.Right, Color.white);
			aSliderBackground.material = material;
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			aSlider = null;
			aInput = null;
		}

		/// <summary>
		/// Set specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Full style data.</param>
		public virtual void SetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if ((aSlider != null) && (aSlider.handleRect != null))
			{
				var handle_style = UtilitiesUI.IsHorizontal(aSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.ApplyTo(aSlider.handleRect.GetComponent<Image>());
			}

			if (aInput != null)
			{
				aInput.SetStyle(styleColorPicker.InputSpinner, style);
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Full style data.</param>
		public virtual void GetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			if ((aSlider != null) && (aSlider.handleRect != null))
			{
				var handle_style = UtilitiesUI.IsHorizontal(aSlider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.GetFrom(aSlider.handleRect.GetComponent<Image>());
			}

			if (aInput != null)
			{
				aInput.GetStyle(styleColorPicker.InputSpinner, style);
			}
		}
	}
}