#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using TMPro;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Style support for TextMeshPro.
	/// </summary>
	public static class StyleTMPro
	{
		/// <summary>
		/// Apply style for the specified gameobject.
		/// </summary>
		/// <param name="style">Style.</param>
		/// <param name="go">Gameobject.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public static bool ApplyTo(StyleText style, GameObject go)
		{
			var applied = false;

			if (go != null)
			{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				applied |= ApplyTo(style, go.GetComponent<TMP_InputField>());
				applied |= ApplyTo(style, go.GetComponent<TMP_Text>());
#else
				applied |= ApplyTo(style, go.GetComponent<TextMeshProUGUI>());
#endif

			}

			return applied;
		}

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		/// <summary>
		/// Apply style for the specified InputField.
		/// </summary>
		/// <param name="style">Style.</param>
		/// <param name="component">InputField.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public static bool ApplyTo(StyleText style, TMP_InputField component)
		{
			if (component == null)
			{
				return false;
			}

			ApplyTo(style, component.textComponent, true);

			if (component.placeholder != null)
			{
				ApplyTo(style, component.placeholder.GetComponent<TextMeshProUGUI>(), true);
			}

			return true;
		}
#endif

		/// <summary>
		/// Apply style for the specified Text.
		/// </summary>
		/// <param name="style">Style for text.</param>
		/// <param name="component">Text.</param>
		/// <param name="isInputField">Is transform belongs to the InputField component?</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		public static bool ApplyTo(StyleText style, TMP_Text component, bool isInputField = false)
#else
		public static bool ApplyTo(StyleText style, TextMeshProUGUI component, bool isInputField = false)
#endif
		{
			if (component == null)
			{
				return false;
			}

			if (style.ChangeFont && (style.FontTMPro != null))
			{
				component.font = style.FontTMPro;
			}

			if (style.ChangeFontStyle)
			{
				component.fontStyle = ConvertStyle(style.FontStyle);
			}

			if (style.ChangeLineSpacing)
			{
				component.lineSpacing = style.LineSpacing;
			}

			if (style.ChangeRichText && (!isInputField))
			{
				component.richText = style.RichText;
			}

			if (style.ChangeAlignment && (!isInputField))
			{
				component.alignment = ConvertAlignment(style.Alignment);
			}

			if (style.ChangeSize)
			{
				component.fontSize = style.Size;
			}

			if (style.ChangeHorizontalOverflow)
			{
				component.enableWordWrapping = style.HorizontalOverflow == HorizontalWrapMode.Wrap;
			}

			if (style.ChangeVerticalOverflow)
			{
				if (style.VerticalOverflow == VerticalWrapMode.Overflow)
				{
					component.overflowMode = TextOverflowModes.Overflow;
				}

				if (style.VerticalOverflow == VerticalWrapMode.Truncate)
				{
					component.overflowMode = TextOverflowModes.Truncate;
				}
			}

			if (style.ChangeBestFit && (!isInputField))
			{
				component.enableAutoSizing = style.BestFit;
				component.fontSizeMin = style.MinSize;
				component.fontSizeMax = style.MaxSize;
			}

			if (style.ChangeColor)
			{
				component.color = style.Color;
			}

			if (style.ChangeMaterial)
			{
				component.material = style.Material;
			}

			component.SetAllDirty();

			return true;
		}

		/// <summary>
		/// Set style options from the specified gameobject.
		/// </summary>
		/// <param name="style">Style.</param>
		/// <param name="go">Gameobject.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public static bool GetFrom(StyleText style, GameObject go)
		{
			var processed = false;

			if (go != null)
			{
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
				processed |= GetFrom(style, go.GetComponent<TMP_InputField>());
				processed |= GetFrom(style, go.GetComponent<TMP_Text>());
#else
				processed |= GetFrom(style, go.GetComponent<TextMeshProUGUI>());
#endif

			}

			return processed;
		}

#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		/// <summary>
		/// Set style options from the specified InputField.
		/// </summary>
		/// <param name="style">Style.</param>
		/// <param name="component">InputField.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		public static bool GetFrom(StyleText style, TMP_InputField component)
		{
			if (component == null)
			{
				return false;
			}

			GetFrom(style, component.textComponent, true);

			if (component.placeholder != null)
			{
				GetFrom(style, component.placeholder.GetComponent<TextMeshProUGUI>(), true);
			}

			return true;
		}
#endif

		/// <summary>
		/// Set style options from the specified Text.
		/// </summary>
		/// <param name="style">Style for text.</param>
		/// <param name="component">Text.</param>
		/// <param name="isInputField">Is transform belongs to the InputField component?</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
#if UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER
		public static bool GetFrom(StyleText style, TMP_Text component, bool isInputField = false)
#else
		public static bool GetFrom(StyleText style, TextMeshProUGUI component, bool isInputField = false)
#endif
		{
			if (component == null)
			{
				return false;
			}

			Style.SetValue(component.font, ref style.FontTMPro);
			style.ChangeFont = true;

			style.FontStyle = ConvertStyle(component.fontStyle);
			style.ChangeFontStyle = true;

			style.LineSpacing = Mathf.CeilToInt(component.lineSpacing);
			style.ChangeLineSpacing = true;

			if (!isInputField)
			{
				style.RichText = component.richText;
				style.ChangeRichText = true;
			}

			if (!isInputField)
			{
				style.Alignment = ConvertAlignment(component.alignment);
				style.ChangeAlignment = true;
			}

			style.Size = Mathf.RoundToInt(component.fontSize);
			style.ChangeSize = true;

			style.HorizontalOverflow = component.enableWordWrapping ? HorizontalWrapMode.Wrap : HorizontalWrapMode.Overflow;
			style.ChangeHorizontalOverflow = true;

			if (component.overflowMode == TextOverflowModes.Overflow)
			{
				style.VerticalOverflow = VerticalWrapMode.Overflow;
				style.ChangeVerticalOverflow = true;
			}

			if (component.overflowMode == TextOverflowModes.Truncate)
			{
				style.VerticalOverflow = VerticalWrapMode.Truncate;
				style.ChangeVerticalOverflow = true;
			}

			if (!isInputField)
			{
				style.BestFit = component.enableAutoSizing;
				style.MinSize = Mathf.RoundToInt(component.fontSizeMin);
				style.MaxSize = Mathf.RoundToInt(component.fontSizeMax);
				style.ChangeBestFit = true;
			}

			style.Color = component.color;
			style.ChangeColor = true;

			Style.SetValue(component.material, ref style.Material);
			style.ChangeMaterial = true;

			return true;
		}

		/// <summary>
		/// Convert alignment.
		/// </summary>
		/// <param name="alignment">Unity alignment.</param>
		/// <returns>TMPro alignment.</returns>
		public static TextAlignmentOptions ConvertAlignment(TextAnchor alignment)
		{
			// upper
			if (alignment == TextAnchor.UpperLeft)
			{
				return TextAlignmentOptions.TopLeft;
			}

			if (alignment == TextAnchor.UpperCenter)
			{
				return TextAlignmentOptions.Top;
			}

			if (alignment == TextAnchor.UpperRight)
			{
				return TextAlignmentOptions.TopRight;
			}

			// middle
			if (alignment == TextAnchor.MiddleLeft)
			{
				return TextAlignmentOptions.Left;
			}

			if (alignment == TextAnchor.MiddleCenter)
			{
				return TextAlignmentOptions.Center;
			}

			if (alignment == TextAnchor.MiddleRight)
			{
				return TextAlignmentOptions.Right;
			}

			// lower
			if (alignment == TextAnchor.LowerLeft)
			{
				return TextAlignmentOptions.BottomLeft;
			}

			if (alignment == TextAnchor.LowerCenter)
			{
				return TextAlignmentOptions.Bottom;
			}

			if (alignment == TextAnchor.LowerRight)
			{
				return TextAlignmentOptions.BottomRight;
			}

			return TextAlignmentOptions.TopLeft;
		}

		/// <summary>
		/// Convert alignment.
		/// </summary>
		/// <param name="alignment">TMPro alignment.</param>
		/// <returns>Unity alignment.</returns>
		public static TextAnchor ConvertAlignment(TextAlignmentOptions alignment)
		{
			// upper
			if (alignment == TextAlignmentOptions.TopLeft)
			{
				return TextAnchor.UpperLeft;
			}

			if (alignment == TextAlignmentOptions.Top)
			{
				return TextAnchor.UpperCenter;
			}

			if (alignment == TextAlignmentOptions.TopRight)
			{
				return TextAnchor.UpperRight;
			}

			// middle
			if (alignment == TextAlignmentOptions.Left)
			{
				return TextAnchor.MiddleLeft;
			}

			if (alignment == TextAlignmentOptions.Center)
			{
				return TextAnchor.MiddleCenter;
			}

			if (alignment == TextAlignmentOptions.Right)
			{
				return TextAnchor.MiddleRight;
			}

			// lower
			if (alignment == TextAlignmentOptions.BottomLeft)
			{
				return TextAnchor.LowerLeft;
			}

			if (alignment == TextAlignmentOptions.Bottom)
			{
				return TextAnchor.LowerCenter;
			}

			if (alignment == TextAlignmentOptions.BottomRight)
			{
				return TextAnchor.LowerRight;
			}

			return TextAnchor.UpperLeft;
		}

		/// <summary>
		/// Convert font style.
		/// </summary>
		/// <param name="fontStyle">Unity font style.</param>
		/// <returns>TMPro font style.</returns>
		public static FontStyles ConvertStyle(FontStyle fontStyle)
		{
			if (fontStyle == FontStyle.Normal)
			{
				return FontStyles.Normal;
			}

			if (fontStyle == FontStyle.Bold)
			{
				return FontStyles.Bold;
			}

			if (fontStyle == FontStyle.Italic)
			{
				return FontStyles.Italic;
			}

			if (fontStyle == FontStyle.BoldAndItalic)
			{
				return FontStyles.Bold | FontStyles.Italic;
			}

			return FontStyles.Normal;
		}

		/// <summary>
		/// Convert font style.
		/// </summary>
		/// <param name="fontStyle">TMPro font style.</param>
		/// <returns>Unity font style.</returns>
		public static FontStyle ConvertStyle(FontStyles fontStyle)
		{
			if (fontStyle == FontStyles.Normal)
			{
				return FontStyle.Normal;
			}

			if (fontStyle == FontStyles.Bold)
			{
				return FontStyle.Bold;
			}

			if (fontStyle == FontStyles.Italic)
			{
				return FontStyle.Italic;
			}

			if (fontStyle == (FontStyles.Bold | FontStyles.Italic))
			{
				return FontStyle.BoldAndItalic;
			}

			return FontStyle.Normal;
		}
	}
}
#endif