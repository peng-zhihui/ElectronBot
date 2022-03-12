namespace UIWidgets
{
	using System.Collections.Generic;
	using UIWidgets.Attributes;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ColorPickerRangeHSV.
	/// Allow to select colors in range between specified colors.
	/// </summary>
	[DataBindSupport]
	public class ColorPickerRangeHSV : MonoBehaviour, IStylable
	{
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
		/// Gets or sets the Blue slider background.
		/// </summary>
		/// <value>The Blue slider background.</value>
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
		Shader defaultShaderHorizontal;

		/// <summary>
		/// Gets or sets the default shader to display gradients for horizontal slider background.
		/// </summary>
		/// <value>The default shader.</value>
		public Shader DefaultShaderHorizontal
		{
			get
			{
				return defaultShaderHorizontal;
			}

			set
			{
				defaultShaderHorizontal = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		Shader defaultShaderVertical;

		/// <summary>
		/// Gets or sets the default shader to display gradients for vertical slider background.
		/// </summary>
		/// <value>The default shader.</value>
		public Shader DefaultShaderVertical
		{
			get
			{
				return defaultShaderVertical;
			}

			set
			{
				defaultShaderVertical = value;
				UpdateMaterial();
			}
		}

		[SerializeField]
		ColorHSV colorLeft = new ColorHSV(UnityEngine.Color.black);

		/// <summary>
		/// Gets or sets the color on left side (or bottom side).
		/// </summary>
		/// <value>The color.</value>
		public ColorHSV ColorLeft
		{
			get
			{
				return colorLeft;
			}

			set
			{
				colorLeft = value;
				UpdateView();
				ValueChanged();
			}
		}

		/// <summary>
		/// Gets or sets the color on left side (or bottom side).
		/// </summary>
		/// <value>The color.</value>
		public Color ColorLeftRGB
		{
			get
			{
				return colorLeft;
			}

			set
			{
				colorLeft = new ColorHSV(value);
				UpdateView();
				ValueChanged();
			}
		}

		[SerializeField]
		ColorHSV colorRight = new ColorHSV(UnityEngine.Color.white);

		/// <summary>
		/// Gets or sets the color on right side (or top side).
		/// </summary>
		/// <value>The color right.</value>
		public ColorHSV ColorRight
		{
			get
			{
				return colorRight;
			}

			set
			{
				colorRight = value;
				UpdateView();
				ValueChanged();
			}
		}

		/// <summary>
		/// Gets or sets the color on right side (or top side).
		/// </summary>
		/// <value>The color right.</value>
		public Color ColorRightRGB
		{
			get
			{
				return colorRight;
			}

			set
			{
				colorRight = new ColorHSV(value);
				UpdateView();
				ValueChanged();
			}
		}

		[SerializeField]
		ColorHSV color = new ColorHSV(UnityEngine.Color.white);

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		[DataBindField]
		public ColorHSV Color
		{
			get
			{
				return color;
			}

			set
			{
				inUpdateMode = true;

				SetColor(value);
				UpdateView();
				OnChange.Invoke(color);

				inUpdateMode = false;
			}
		}

		/// <summary>
		/// Gets or sets the color.
		/// </summary>
		/// <value>The color.</value>
		[DataBindField]
		public Color ColorRGB
		{
			get
			{
				return color;
			}

			set
			{
				inUpdateMode = true;

				SetColor(new ColorHSV(value));
				UpdateView();
				OnChange.Invoke(color);

				inUpdateMode = false;
			}
		}

		/// <summary>
		/// OnChangeRGB event.
		/// </summary>
		[SerializeField]
		[DataBindEvent("Color", "ColorRGB")]
		public ColorHSVChangedEvent OnChange = new ColorHSVChangedEvent();

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

			Slider = slider;
			Slider.minValue = 0;
			Slider.maxValue = 359;
			Slider.wholeNumbers = true;
			UpdateMaterial();
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateMaterial();
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

		void SliderValueChanged(float value)
		{
			ValueChanged();
		}

		/// <summary>
		/// If in update mode?
		/// </summary>
		protected bool inUpdateMode;

		/// <summary>
		/// Difference between color components.
		/// </summary>
		protected List<float> Diffs = new List<float>();

		/// <summary>
		/// Sets the color.
		/// </summary>
		/// <param name="value">Value.</param>
		protected virtual void SetColor(ColorHSV value)
		{
			Diffs.Clear();

			if ((ColorRight.H - ColorLeft.H) != 0f)
			{
				Diffs.Add(Mathf.Abs((value.H - ColorLeft.H) / (ColorRight.H - ColorLeft.H)));
			}

			if ((ColorRight.S - ColorLeft.S) != 0f)
			{
				Diffs.Add(Mathf.Abs((value.S - ColorLeft.S) / (ColorRight.S - ColorLeft.S)));
			}

			if ((ColorRight.V - ColorLeft.V) != 0f)
			{
				Diffs.Add(Mathf.Abs((value.V - ColorLeft.V) / (ColorRight.V - ColorLeft.V)));
			}

			if (ColorRight.A != ColorLeft.A)
			{
				Diffs.Add(Mathf.Abs((value.A - ColorLeft.A) / (ColorRight.A - ColorLeft.A)));
			}

			var t = Diffs.Count == 0 ? 1 : UtilitiesCollections.Sum(Diffs) / (float)Diffs.Count;
			color = ColorHSV.Lerp(ColorLeft, ColorRight, t);
			Slider.value = t * (Slider.maxValue - Slider.minValue);
		}

		/// <summary>
		/// Values the changed.
		/// </summary>
		protected virtual void ValueChanged()
		{
			if (inUpdateMode)
			{
				return;
			}

			color = ColorHSV.Lerp(ColorLeft, ColorRight, Slider.value / (Slider.maxValue - Slider.minValue));
			OnChange.Invoke(color);
		}

		/// <summary>
		/// Updates the material.
		/// </summary>
		protected virtual void UpdateMaterial()
		{
			var shader = UtilitiesUI.IsHorizontal(slider) ? defaultShaderHorizontal : DefaultShaderVertical;
			if (shader == null)
			{
				return;
			}

			sliderBackground.material = new Material(shader);

			UpdateViewReal();
		}

		/// <summary>
		/// Should be called if slider direction was changed.
		/// </summary>
		public virtual void DirectionChanged()
		{
			UpdateMaterial();
		}

		/// <summary>
		/// Updates the view.
		/// </summary>
		protected virtual void UpdateView()
		{
			UpdateViewReal();

			Compatibility.ToggleGameObject(slider);
		}

		/// <summary>
		/// Updates the view real.
		/// </summary>
		protected virtual void UpdateViewReal()
		{
			inUpdateMode = true;

			SetColor(color);

			if (slider.direction == Slider.Direction.LeftToRight || slider.direction == Slider.Direction.RightToLeft)
			{
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Left, ColorLeft.ShaderColor);
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Right, ColorRight.ShaderColor);
				sliderBackground.SetMaterialDirty();
			}
			else
			{
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Bottom, ColorLeft.ShaderColor);
				sliderBackground.material.SetColor(ColorPicker.ShaderIDs.Top, ColorRight.ShaderColor);
				sliderBackground.SetMaterialDirty();
			}

			inUpdateMode = false;
		}

		#if UNITY_EDITOR
		/// <summary>
		/// Process the validate event.
		/// </summary>
		protected virtual void OnValidate()
		{
			color.Validate();
			colorLeft.Validate();
			colorRight.Validate();
		}
		#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (slider != null)
			{
				var colorpicker_style = UtilitiesUI.IsHorizontal(slider)
					? style.ColorPickerRangeHorizontal
					: style.ColorPickerRangeVertical;
				colorpicker_style.Background.ApplyTo(GetComponent<Image>());
				if (slider.handleRect != null)
				{
					colorpicker_style.Handle.ApplyTo(slider.handleRect.GetComponent<Image>());
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (slider != null)
			{
				var colorpicker_style = UtilitiesUI.IsHorizontal(slider)
					? style.ColorPickerRangeHorizontal
					: style.ColorPickerRangeVertical;
				colorpicker_style.Background.GetFrom(GetComponent<Image>());
				if (slider.handleRect != null)
				{
					colorpicker_style.Handle.GetFrom(slider.handleRect.GetComponent<Image>());
				}
			}

			return true;
		}
		#endregion
	}
}