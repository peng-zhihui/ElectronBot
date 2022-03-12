namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Color picker color view block.
	/// </summary>
	public class ColorPickerColorBlock : MonoBehaviour
	{
		[SerializeField]
		Image colorView;

		/// <summary>
		/// Gets or sets the color view.
		/// </summary>
		/// <value>The color view.</value>
		public Image ColorView
		{
			get
			{
				return colorView;
			}

			set
			{
				colorView = value;
				if (colorView != null)
				{
					UpdateView();
				}
			}
		}

		[SerializeField]
		RectTransform alphaView;

		/// <summary>
		/// Gets or sets the alpha view.
		/// </summary>
		/// <value>The alpha view.</value>
		public RectTransform AlphaView
		{
			get
			{
				return alphaView;
			}

			set
			{
				alphaView = value;
				if (alphaView != null)
				{
					UpdateView();
				}
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

			ColorView = colorView;
			AlphaView = alphaView;
		}

		/// <summary>
		/// This function is called when the object becomes enabled and active.
		/// </summary>
		protected virtual void OnEnable()
		{
			UpdateView();
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
			var main_color = (Color)currentColor;
			if (alphaView != null)
			{
				alphaView.anchorMax = new Vector2(main_color.a, alphaView.anchorMax.y);
				alphaView.sizeDelta = Vector2.zero;
			}

			if (colorView != null)
			{
				main_color.a = 1f;
				colorView.color = main_color;
			}
		}
	}
}