#if UNITY_EDITOR && UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Text component converter.
	/// </summary>
	public class ConverterText
	{
		readonly int fontSize;

		readonly FontStyle fontStyle;
		readonly TextAnchor alignment;

		readonly Color color;
		readonly string value;

		readonly bool resizeTextForBestFit;
		readonly int resizeTextMinSize;
		readonly int resizeTextMaxSize;
		readonly float lineSpacing;

		readonly HorizontalWrapMode horizontalWrapMode;
		readonly VerticalWrapMode verticalWrapMode;
		readonly bool supportRichText;

		readonly Vector2 sizeDelta;

		/// <summary>
		/// Initializes a new instance of the <see cref="UIWidgets.ConverterText"/> class.
		/// </summary>
		/// <param name="text">Original component.</param>
		public ConverterText(Text text)
		{
			fontSize = text.fontSize;
			fontStyle = text.fontStyle;
			alignment = text.alignment;

			color = text.color;
			value = text.text;

			resizeTextForBestFit = text.resizeTextForBestFit;
			resizeTextMinSize = text.resizeTextMinSize;
			resizeTextMaxSize = text.resizeTextMaxSize;
			lineSpacing = text.lineSpacing;

			horizontalWrapMode = text.horizontalOverflow;
			verticalWrapMode = text.verticalOverflow;
			supportRichText = text.supportRichText;

			sizeDelta = (text.transform as RectTransform).sizeDelta;
		}

		/// <summary>
		/// Set saved values to the new TMP_InputField component.
		/// </summary>
		/// <param name="text">New component.</param>
		public void Set(TMPro.TextMeshProUGUI text)
		{
			text.font = ConverterTMPro.GetTMProFont();

			text.fontSize = fontSize;
			text.fontStyle = Convert(fontStyle);
			text.alignment = Convert(alignment);

			text.color = color;
			text.text = value;

			text.enableAutoSizing = resizeTextForBestFit;
			text.fontSizeMin = resizeTextMinSize;
			text.fontSizeMax = resizeTextMaxSize;
			text.lineSpacing = (lineSpacing - 1) * fontSize * (98f / 36f);

			text.enableWordWrapping = horizontalWrapMode == HorizontalWrapMode.Wrap;
			text.overflowMode = verticalWrapMode == VerticalWrapMode.Overflow ? TMPro.TextOverflowModes.Overflow : TMPro.TextOverflowModes.Truncate;
			text.richText = supportRichText;

			(text.transform as RectTransform).sizeDelta = sizeDelta;
		}

		static TMPro.FontStyles Convert(FontStyle style)
		{
			switch (style)
			{
				case FontStyle.Normal:
					return TMPro.FontStyles.Normal;
				case FontStyle.Bold:
					return TMPro.FontStyles.Bold;
				case FontStyle.Italic:
					return TMPro.FontStyles.Italic;
				case FontStyle.BoldAndItalic:
					return TMPro.FontStyles.Bold | TMPro.FontStyles.Italic;
			}

			return TMPro.FontStyles.Normal;
		}

		/// <summary>
		/// Convert text alignment.
		/// </summary>
		/// <param name="align">Original alignment.</param>
		/// <returns>TmPro alignment.</returns>
		static TMPro.TextAlignmentOptions Convert(TextAnchor align)
		{
			switch (align)
			{
				case TextAnchor.UpperLeft:
					return TMPro.TextAlignmentOptions.TopLeft;
				case TextAnchor.UpperCenter:
					return TMPro.TextAlignmentOptions.Top;
				case TextAnchor.UpperRight:
					return TMPro.TextAlignmentOptions.TopRight;
				case TextAnchor.MiddleLeft:
					return TMPro.TextAlignmentOptions.MidlineLeft;
				case TextAnchor.MiddleCenter:
					return TMPro.TextAlignmentOptions.Center;
				case TextAnchor.MiddleRight:
					return TMPro.TextAlignmentOptions.MidlineRight;
				case TextAnchor.LowerLeft:
					return TMPro.TextAlignmentOptions.BottomLeft;
				case TextAnchor.LowerCenter:
					return TMPro.TextAlignmentOptions.Bottom;
				case TextAnchor.LowerRight:
					return TMPro.TextAlignmentOptions.BottomRight;
			}

			return TMPro.TextAlignmentOptions.TopLeft;
		}
	}
}
#endif