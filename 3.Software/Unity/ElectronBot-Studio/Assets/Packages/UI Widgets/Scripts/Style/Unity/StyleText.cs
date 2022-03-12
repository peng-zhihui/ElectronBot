namespace UIWidgets.Styles
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Style for the text.
	/// </summary>
	[Serializable]
	public class StyleText : IStyleDefaultValues
	{
		/// <summary>
		/// Is should change font?
		/// </summary>
		[SerializeField]
		[Header("Character")]
		public bool ChangeFont = true;

		/// <summary>
		/// The font.
		/// </summary>
		[SerializeField]
		public Font Font;

		/// <summary>
		/// The TMPro font.
		/// </summary>
		[SerializeField]
#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
		public TMPro.TMP_FontAsset FontTMPro;
#elif UIWIDGETS_TMPRO_SUPPORT
		public TMPro.TextMeshProFont FontTMPro;
#else
		public ScriptableObject FontTMPro;
#endif

		/// <summary>
		/// Is should change font style?
		/// </summary>
		[SerializeField]
		public bool ChangeFontStyle = false;

		/// <summary>
		/// The font style.
		/// </summary>
		[SerializeField]
		public FontStyle FontStyle = FontStyle.Normal;

		/// <summary>
		/// Is should change size?
		/// </summary>
		[SerializeField]
		public bool ChangeSize = false;

		/// <summary>
		/// The size.
		/// </summary>
		[SerializeField]
		public int Size = 18;

		/// <summary>
		/// Is should change line spacing?
		/// </summary>
		[SerializeField]
		public bool ChangeLineSpacing = false;

		/// <summary>
		/// The line spacing.
		/// </summary>
		[SerializeField]
		public int LineSpacing = 1;

		/// <summary>
		/// Is should change rich text?
		/// </summary>
		[SerializeField]
		public bool ChangeRichText = false;

		/// <summary>
		/// The rich text.
		/// </summary>
		[SerializeField]
		public bool RichText = true;

		/// <summary>
		/// Is should change alignment?
		/// </summary>
		[Header("Paragraph")]
		[SerializeField]
		public bool ChangeAlignment = false;

		/// <summary>
		/// The alignment.
		/// </summary>
		[SerializeField]
		public TextAnchor Alignment = TextAnchor.UpperLeft;

		/// <summary>
		/// Is should change horizontal overflow?
		/// </summary>
		[SerializeField]
		public bool ChangeHorizontalOverflow = false;

		/// <summary>
		/// The horizontal overflow.
		/// </summary>
		[SerializeField]
		public HorizontalWrapMode HorizontalOverflow = HorizontalWrapMode.Wrap;

		/// <summary>
		/// Is should change vertical overflow?
		/// </summary>
		[SerializeField]
		public bool ChangeVerticalOverflow = false;

		/// <summary>
		/// The vertical overflow.
		/// </summary>
		[SerializeField]
		public VerticalWrapMode VerticalOverflow = VerticalWrapMode.Truncate;

		/// <summary>
		/// Is should change best fit?
		/// </summary>
		[SerializeField]
		public bool ChangeBestFit = false;

		/// <summary>
		/// The best fit.
		/// </summary>
		[SerializeField]
		public bool BestFit = false;

		/// <summary>
		/// The minimum size.
		/// </summary>
		[SerializeField]
		public int MinSize = 10;

		/// <summary>
		/// The size of the max.
		/// </summary>
		[SerializeField]
		public int MaxSize = 40;

		/// <summary>
		/// Is should change color?
		/// </summary>
		[Header("Other")]
		[SerializeField]
		public bool ChangeColor = true;

		/// <summary>
		/// The color.
		/// </summary>
		[SerializeField]
		public Color Color = new Color32(255, 215, 115, 255);

		/// <summary>
		/// Is should change material?
		/// </summary>
		[SerializeField]
		public bool ChangeMaterial = false;

		/// <summary>
		/// The material.
		/// </summary>
		[SerializeField]
		public Material Material = null;

		/// <summary>
		/// Apply style to the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		/// <param name="isInputField">Is gameobject belongs to the InputField component?</param>
		public virtual void ApplyTo(GameObject go, bool isInputField = false)
		{
			if (go != null)
			{
				ApplyTo(go.GetComponent<Text>(), isInputField);
			}

			Style.TMProSupport(this, go);
		}

		/// <summary>
		/// Apply style to the specified component.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <param name="isInputField">Is transform belongs to the InputField component?</param>
		public virtual void ApplyTo(Transform transform, bool isInputField = false)
		{
			if (transform != null)
			{
				ApplyTo(transform.gameObject, isInputField);
			}
		}

		/// <summary>
		/// Apply style to the specified InputField.
		/// </summary>
		/// <param name="component">InputField.</param>
		public virtual void ApplyTo(InputField component)
		{
			if (component != null)
			{
				ApplyTo(component.textComponent, true);

				if (component.placeholder != null)
				{
					ApplyTo(component.placeholder.gameObject, true);
				}
			}
		}

		/// <summary>
		/// Apply style to the specified Text component.
		/// </summary>
		/// <param name="component">Text.</param>
		/// <param name="isInputField">Is text belongs to the InputField component?</param>
		public virtual void ApplyTo(Text component, bool isInputField = false)
		{
			if (component == null)
			{
				return;
			}

			if (ChangeFont && (Font != null))
			{
				component.font = Font;
			}

			if (ChangeFontStyle)
			{
				component.fontStyle = FontStyle;
			}

			if (ChangeSize)
			{
				component.fontSize = Size;
			}

			if (ChangeLineSpacing)
			{
				component.lineSpacing = LineSpacing;
			}

			if (ChangeRichText && (!isInputField))
			{
				component.supportRichText = RichText;
			}

			if (ChangeAlignment)
			{
				component.alignment = Alignment;
			}

			if (ChangeHorizontalOverflow)
			{
				component.horizontalOverflow = HorizontalOverflow;
			}

			if (ChangeVerticalOverflow)
			{
				component.verticalOverflow = VerticalOverflow;
			}

			if (ChangeBestFit && (!isInputField))
			{
				component.resizeTextForBestFit = BestFit;
				component.resizeTextMinSize = MinSize;
				component.resizeTextMaxSize = MaxSize;
			}

			if (ChangeColor)
			{
				component.color = Color;
			}

			if (ChangeMaterial)
			{
				component.material = Material;
			}

			component.SetAllDirty();
		}

		/// <summary>
		/// Set style options from the specified gameobject.
		/// </summary>
		/// <param name="go">Gameobject.</param>
		/// <param name="isInputField">Is gameobject belongs to the InputField component?</param>
		public virtual void GetFrom(GameObject go, bool isInputField = false)
		{
			if (go != null)
			{
				GetFrom(go.GetComponent<Text>(), isInputField);
			}

			Style.TMProSupportGetFrom(this, go);
		}

		/// <summary>
		/// Set style options from the specified component.
		/// </summary>
		/// <param name="transform">Transform.</param>
		/// <param name="isInputField">Is transform belongs to the InputField component?</param>
		public virtual void GetFrom(Transform transform, bool isInputField = false)
		{
			if (transform != null)
			{
				GetFrom(transform.gameObject, isInputField);
			}
		}

		/// <summary>
		/// Set style options from the specified InputField.
		/// </summary>
		/// <param name="component">InputField.</param>
		public virtual void GetFrom(InputField component)
		{
			if (component != null)
			{
				GetFrom(component.textComponent, true);

				if (component.placeholder != null)
				{
					GetFrom(component.placeholder.gameObject, true);
				}
			}
		}

		/// <summary>
		/// Set style options from the specified Text component.
		/// </summary>
		/// <param name="component">Text.</param>
		/// <param name="isInputField">Is text belongs to the InputField component?</param>
		public virtual void GetFrom(Text component, bool isInputField = false)
		{
			if (component == null)
			{
				return;
			}

			Style.SetValue(component.font, ref Font);
			ChangeFont = Font != null;

			FontStyle = component.fontStyle;
			ChangeFontStyle = true;

			Size = component.fontSize;
			ChangeSize = true;

			LineSpacing = Mathf.RoundToInt(component.lineSpacing);
			ChangeLineSpacing = true;

			if (!isInputField)
			{
				RichText = component.supportRichText;
				ChangeRichText = true;
			}

			Alignment = component.alignment;
			ChangeAlignment = true;

			HorizontalOverflow = component.horizontalOverflow;
			ChangeHorizontalOverflow = true;

			VerticalOverflow = component.verticalOverflow;
			ChangeVerticalOverflow = true;

			if (!isInputField)
			{
				BestFit = component.resizeTextForBestFit;
				MinSize = component.resizeTextMinSize;
				MaxSize = component.resizeTextMaxSize;
				ChangeBestFit = true;
			}

			Color = component.color;
			ChangeColor = true;

			Style.SetValue(component.material, ref Material);
			ChangeMaterial = true;
		}

		/// <summary>
		/// Clone this instance.
		/// </summary>
		/// <returns>Copy of the object.</returns>
		public StyleText Clone()
		{
			return (StyleText)MemberwiseClone();
		}

#if UNITY_EDITOR
		/// <inheritdoc/>
		public void SetDefaultValues()
		{
			if (Font == null)
			{
				Font = Resources.GetBuiltinResource<Font>("Arial.ttf");
			}

			if (FontTMPro == null)
			{
#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
				FontTMPro = Resources.Load<TMPro.TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
#elif UIWIDGETS_TMPRO_SUPPORT
				FontTMPro = Resources.Load<TMPro.TextMeshProFont>("Fonts & Materials/ARIAL SDF");
#else
				FontTMPro = Resources.Load<ScriptableObject>("ARIAL SDF");
#endif
			}
		}
#endif
	}
}