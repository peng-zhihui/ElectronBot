namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker HSV palette.
	/// </summary>
	public class ColorPickerHSVPalette : MonoBehaviour
	{
		[SerializeField]
		Image palette;

		RectTransform paletteRect;

		DragListener dragListener;

		ClickListener clickListener;

		ResizeListener resizeListener;

		/// <summary>
		/// Gets or sets the palette.
		/// </summary>
		/// <value>The palette.</value>
		public Image Palette
		{
			get
			{
				return palette;
			}

			set
			{
				SetPalette(value);
			}
		}

		[SerializeField]
		Shader paletteShader;

		/// <summary>
		/// Gets or sets the shader to display gradient in palette.
		/// </summary>
		/// <value>The palette shader.</value>
		public Shader PaletteShader
		{
			get
			{
				return paletteShader;
			}

			set
			{
				paletteShader = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Shader circleShader;

		/// <summary>
		/// Gets or sets the shader to display circle gradient in palette.
		/// </summary>
		/// <value>The palette circle shader.</value>
		public Shader CircleShader
		{
			get
			{
				return circleShader;
			}

			set
			{
				circleShader = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		RectTransform paletteCursor;

		/// <summary>
		/// Gets or sets the palette cursor.
		/// </summary>
		/// <value>The palette cursor.</value>
		public RectTransform PaletteCursor
		{
			get
			{
				return paletteCursor;
			}

			set
			{
				paletteCursor = value;
				if (paletteCursor != null)
				{
					UpdateView();
				}
			}
		}

		[SerializeField]
		Slider slider;

		/// <summary>
		/// Gets or sets the slider.
		/// </summary>
		/// <value>The slider.</value>
		public Slider Slider
		{
			get
			{
				return slider;
			}

			set
			{
				SetSlider(value);
			}
		}

		[SerializeField]
		Image sliderBackground;

		/// <summary>
		/// Gets or sets the slider background.
		/// </summary>
		/// <value>The slider background.</value>
		public Image SliderBackground
		{
			get
			{
				return sliderBackground;
			}

			set
			{
				sliderBackground = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Shader sliderShader;

		/// <summary>
		/// Gets or sets the shader to display gradient for slider background.
		/// </summary>
		/// <value>The slider shader.</value>
		public Shader SliderShader
		{
			get
			{
				return sliderShader;
			}

			set
			{
				sliderShader = value;
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
				SetPaletteMode(value);
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

		/// <summary>
		/// Is current pallete mode is HSVCircle?
		/// </summary>
		protected bool IsCircle
		{
			get
			{
				return PaletteMode == ColorPickerPaletteMode.HSVCircle;
			}
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

			Palette = palette;
			Slider = slider;
			SliderBackground = sliderBackground;
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Sets the palette.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetPalette(Image value)
		{
			if (dragListener != null)
			{
				dragListener.OnDragEvent.RemoveListener(OnDrag);
			}

			if (clickListener != null)
			{
				clickListener.ClickEvent.RemoveListener(OnDrag);
			}

			if (resizeListener != null)
			{
				resizeListener.OnResizeNextFrame.RemoveListener(UpdateView);
			}

			palette = value;
			if (palette != null)
			{
				paletteRect = palette.transform as RectTransform;

				dragListener = Utilities.GetOrAddComponent<DragListener>(palette);
				dragListener.OnDragEvent.AddListener(OnDrag);

				clickListener = Utilities.GetOrAddComponent<ClickListener>(palette);
				clickListener.ClickEvent.AddListener(OnDrag);

				resizeListener = Utilities.GetOrAddComponent<ResizeListener>(palette);
				resizeListener.OnResizeNextFrame.AddListener(UpdateView);

				UpdateMaterial();
			}
			else
			{
				paletteRect = null;
			}
		}

		/// <summary>
		/// Sets the slider.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetSlider(Slider value)
		{
			if (slider != null)
			{
				slider.onValueChanged.RemoveListener(SliderValueChanged);
			}

			slider = value;
			if (slider != null)
			{
				slider.onValueChanged.AddListener(SliderValueChanged);
			}
		}

		/// <summary>
		/// Sets the palette mode.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetPaletteMode(ColorPickerPaletteMode value)
		{
			paletteMode = value;

			bool is_active = paletteMode == ColorPickerPaletteMode.Hue
				|| paletteMode == ColorPickerPaletteMode.Saturation
				|| paletteMode == ColorPickerPaletteMode.Value
				|| paletteMode == ColorPickerPaletteMode.HSVCircle;
			gameObject.SetActive(is_active);
			slider.maxValue = (paletteMode == ColorPickerPaletteMode.Hue) ? 359 : 255;

			if (is_active)
			{
				UpdateMaterial();
			}
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
		protected virtual void ValueChanged()
		{
			if (inUpdateMode)
			{
				return;
			}

			currentColorHSV = GetColor();

			OnChangeHSV.Invoke(currentColorHSV);
		}

		/// <summary>
		/// Сurrent color.
		/// </summary>
		protected ColorHSV currentColorHSV;

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(Color32 color)
		{
			currentColorHSV = new ColorHSV(color);

			UpdateView();
		}

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="color">Color.</param>
		public void SetColor(ColorHSV color)
		{
			currentColorHSV = color;

			UpdateView();
		}

		/// <summary>
		/// When dragging is occurring this will be called every time the cursor is moved.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		protected virtual void OnDrag(PointerEventData eventData)
		{
			Vector2 size = paletteRect.rect.size;
			Vector2 cur_pos;
			RectTransformUtility.ScreenPointToLocalPointInRectangle(paletteRect, eventData.position, eventData.pressEventCamera, out cur_pos);

			cur_pos.x = Mathf.Clamp(cur_pos.x, 0, size.x);
			cur_pos.y = Mathf.Clamp(cur_pos.y, -size.y, 0);

			if (IsCircle)
			{
				var x1 = (cur_pos.x / size.x) - 0.5f;
				var y1 = (cur_pos.y / size.y) + 0.5f;
				var angle = Mathf.Atan2(x1, y1);
				var radius = Mathf.Min(1f, Mathf.Sqrt((x1 * x1 * 4f) + (y1 * y1 * 4f)));

				var x2 = Mathf.Sin(angle) * radius / 2f;
				var y2 = Mathf.Cos(angle) * radius / 2f;
				var x = size.x * (x2 + 0.5f);
				var y = size.y * (y2 - 0.5f);

				paletteCursor.localPosition = new Vector2(x, y);
			}
			else
			{
				paletteCursor.localPosition = cur_pos;
			}

			ValueChanged();
		}

		/// <summary>
		/// Gets the color.
		/// </summary>
		/// <returns>The color.</returns>
		protected ColorHSV GetColor()
		{
			var coords = GetCursorCoords();

			var s = Mathf.RoundToInt(slider.value);
			var x = coords.x;
			var y = coords.y;

			switch (paletteMode)
			{
				case ColorPickerPaletteMode.Hue:
					return new ColorHSV(s, Mathf.RoundToInt(x * 255f), Mathf.RoundToInt(y * 255f), currentColorHSV.A);
				case ColorPickerPaletteMode.Saturation:
					return new ColorHSV(Mathf.RoundToInt(x * 359f), s, Mathf.RoundToInt(y * 255f), currentColorHSV.A);
				case ColorPickerPaletteMode.Value:
					return new ColorHSV(Mathf.RoundToInt(x * 359f), Mathf.RoundToInt(y * 255f), s, currentColorHSV.A);
				case ColorPickerPaletteMode.HSVCircle:
					var x1 = x - 0.5f;
					var y1 = y - 0.5f;
					var hue = Mathf.Atan2(x1, y1) / (Mathf.PI * 2);
					if (hue < 0)
					{
						hue += 1;
					}

					var saturation = Mathf.Sqrt((x1 * x1 * 4f) + (y1 * y1 * 4f));

					return new ColorHSV(Mathf.RoundToInt(hue * 359f), Mathf.RoundToInt(saturation * 255f), s, currentColorHSV.A);
				default:
					return currentColorHSV;
			}
		}

		/// <summary>
		/// Gets the cursor coords.
		/// </summary>
		/// <returns>The cursor coords.</returns>
		protected Vector2 GetCursorCoords()
		{
			var coords = paletteCursor.localPosition;
			var size = paletteRect.rect.size;

			var x = coords.x / size.x;
			var y = (coords.y / size.y) + 1;

			return new Vector2(x, y);
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			UpdateViewReal();

			Compatibility.ToggleGameObject(Palette);
		}

		/// <summary>
		/// Updates the view real.
		/// </summary>
		protected virtual void UpdateViewReal()
		{
			inUpdateMode = true;

			// set slider value
			if (slider != null)
			{
				slider.value = GetSliderValue();
			}

			// set slider colors
			if (sliderBackground != null)
			{
				var colors = GetSliderColors();
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Bottom, colors[0]);
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Top, colors[1]);
				sliderBackground.SetMaterialDirty();
			}

			// set palette drag position
			if ((paletteCursor != null) && (palette != null) && (paletteRect != null))
			{
				var coords = GetPaletteCoords();
				var size = paletteRect.rect.size;

				paletteCursor.localPosition = new Vector3(coords.x * size.x, (coords.y - 1) * size.y, 0);
			}

			// set palette colors
			if (palette != null)
			{
				if (IsCircle)
				{
					var quality = 4 * Mathf.Pow(Mathf.Abs(paletteRect.rect.width), -1);
					palette.material.SetFloat(ColorPicker.ShaderIDs.Quality, quality);
					palette.material.SetFloat(ColorPicker.ShaderIDs.Value, currentColorHSV.V / 255f);
					palette.SetMaterialDirty();
				}
				else
				{
					var colors = GetPaletteColors();

					palette.material.SetColor(ColorPicker.ShaderIDs.Left, colors[0]);
					palette.material.SetColor(ColorPicker.ShaderIDs.Right, colors[1]);
					palette.material.SetColor(ColorPicker.ShaderIDs.Bottom, colors[2]);
					palette.material.SetColor(ColorPicker.ShaderIDs.Top, colors[3]);
					palette.SetMaterialDirty();
				}
			}

			inUpdateMode = false;
		}

		/// <summary>
		/// Gets the slider value.
		/// </summary>
		/// <returns>The slider value.</returns>
		protected int GetSliderValue()
		{
			switch (paletteMode)
			{
				case ColorPickerPaletteMode.Hue:
					return currentColorHSV.H;
				case ColorPickerPaletteMode.Saturation:
					return currentColorHSV.S;
				case ColorPickerPaletteMode.Value:
				case ColorPickerPaletteMode.HSVCircle:
					return currentColorHSV.V;
				default:
					return 0;
			}
		}

		/// <summary>
		/// Gets the slider colors.
		/// </summary>
		/// <returns>The slider colors.</returns>
		protected Color[] GetSliderColors()
		{
			switch (paletteMode)
			{
				case ColorPickerPaletteMode.Hue:
					return new Color[]
					{
						new Color(0f, 1f, 1f, 1f),
						new Color(1f, 1f, 1f, 1f),
					};
				case ColorPickerPaletteMode.Saturation:
					return new Color[]
					{
						new Color(currentColorHSV.H / 359f, 0f, Mathf.Max(ColorPicker.ValueLimit, currentColorHSV.V) / 255f, 1f),
						new Color(currentColorHSV.H / 359f, 1f, Mathf.Max(ColorPicker.ValueLimit, currentColorHSV.V) / 255f, 1f),
					};
				case ColorPickerPaletteMode.Value:
				case ColorPickerPaletteMode.HSVCircle:
					return new Color[]
					{
						new Color(currentColorHSV.H / 359f, currentColorHSV.S / 255f, 0f, 1f),
						new Color(currentColorHSV.H / 359f, currentColorHSV.S / 255f, 1f, 1f),
					};
				default:
					return new Color[]
					{
						new Color(0f, 0f, 0f, 1f),
						new Color(1f, 1f, 1f, 1f),
					};
			}
		}

		/// <summary>
		/// Gets the palette coords.
		/// </summary>
		/// <returns>The palette coords.</returns>
		protected Vector2 GetPaletteCoords()
		{
			switch (paletteMode)
			{
				case ColorPickerPaletteMode.Hue:
					return new Vector2(currentColorHSV.S / 255f, currentColorHSV.V / 255f);
				case ColorPickerPaletteMode.Saturation:
					return new Vector2(currentColorHSV.H / 359f, currentColorHSV.V / 255f);
				case ColorPickerPaletteMode.Value:
					return new Vector2(currentColorHSV.H / 359f, currentColorHSV.S / 255f);
				case ColorPickerPaletteMode.HSVCircle:
					var angle = (currentColorHSV.H / 359f) * Mathf.PI * 2;
					var radius = currentColorHSV.S / 255f;

					var x2 = Mathf.Sin(angle) * radius / 2f;
					var y2 = Mathf.Cos(angle) * radius / 2f;

					return new Vector2(x2 + 0.5f, y2 + 0.5f);
				default:
					return new Vector2(0, 0);
			}
		}

		/// <summary>
		/// Gets the palette colors.
		/// </summary>
		/// <returns>The palette colors.</returns>
		protected Color[] GetPaletteColors()
		{
			switch (paletteMode)
			{
				case ColorPickerPaletteMode.Hue:
					return new Color[]
					{
						new Color(currentColorHSV.H / 359f / 2f, 0f, 0f, 1f),
						new Color(currentColorHSV.H / 359f / 2f, 1f, 0f, 1f),
						new Color(currentColorHSV.H / 359f / 2f, 0f, 0f, 1f),
						new Color(currentColorHSV.H / 359f / 2f, 0f, 1f, 1f),
					};
				case ColorPickerPaletteMode.Saturation:
					return new Color[]
					{
						new Color(0f, currentColorHSV.S / 255f / 2f, 0f, 1f),
						new Color(1f, currentColorHSV.S / 255f / 2f, 0f, 1f),
						new Color(0f, currentColorHSV.S / 255f / 2f, 0f, 1f),
						new Color(0f, currentColorHSV.S / 255f / 2f, 1f, 1f),
					};
				case ColorPickerPaletteMode.Value:
					return new Color[]
					{
						new Color(0f, 0f, currentColorHSV.V / 255f / 2f, 1f),
						new Color(1f, 0f, currentColorHSV.V / 255f / 2f, 1f),
						new Color(0f, 0f, currentColorHSV.V / 255f / 2f, 1f),
						new Color(0f, 1f, currentColorHSV.V / 255f / 2f, 1f),
					};
				default:
					return new Color[]
					{
						new Color(0f, 0f, 1f, 1f),
						new Color(1f, 1f, 1f, 1f),
						new Color(0f, 0f, 1f, 1f),
						new Color(1f, 1f, 1f, 1f),
					};
			}
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			if ((paletteShader != null) && (palette != null))
			{
				var shader = IsCircle ? circleShader : paletteShader;
				palette.material = new Material(shader);
			}

			if ((sliderShader != null) && (sliderBackground != null))
			{
				sliderBackground.material = new Material(sliderShader);
			}

			UpdateViewReal();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			dragListener = null;
			slider = null;
		}

		/// <summary>
		/// Set the specified style.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void SetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			styleColorPicker.PaletteBorder.ApplyTo(palette.transform.parent.GetComponent<Image>());

			if (paletteCursor != null)
			{
				styleColorPicker.PaletteCursor.ApplyTo(paletteCursor.GetComponent<Image>());
			}

			if ((slider != null) && (slider.handleRect != null))
			{
				var handle_style = UtilitiesUI.IsHorizontal(slider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.ApplyTo(slider.handleRect.GetComponent<Image>());
			}
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleColorPicker">Style for the ColorPicker.</param>
		/// <param name="style">Style data.</param>
		public virtual void GetStyle(StyleColorPicker styleColorPicker, Style style)
		{
			styleColorPicker.PaletteBorder.GetFrom(palette.transform.parent.GetComponent<Image>());

			if (paletteCursor != null)
			{
				styleColorPicker.PaletteCursor.GetFrom(paletteCursor.GetComponent<Image>());
			}

			if ((slider != null) && (slider.handleRect != null))
			{
				var handle_style = UtilitiesUI.IsHorizontal(slider)
					? styleColorPicker.SliderHorizontalHandle
					: styleColorPicker.SliderVerticalHandle;
				handle_style.GetFrom(slider.handleRect.GetComponent<Image>());
			}
		}
	}
}