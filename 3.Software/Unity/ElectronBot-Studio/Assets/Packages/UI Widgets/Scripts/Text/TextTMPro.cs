#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TextTMPro.
	/// </summary>
	public class TextTMPro : ITextProxy
	{
		/// <summary>
		/// Is enum value has specified flag?
		/// </summary>
		/// <param name="value">Enum value.</param>
		/// <param name="flag">Flag.</param>
		/// <returns>true if enum has flag; otherwise false.</returns>
		public static bool IsSet(TMPro.FontStyles value, TMPro.FontStyles flag)
		{
			return (value & flag) == flag;
		}

		/// <summary>
		/// Text component.
		/// </summary>
		protected TMPro.TextMeshProUGUI Component;

		/// <summary>
		/// GameObject.
		/// </summary>
		public GameObject GameObject
		{
			get
			{
				return Component.gameObject;
			}
		}

		/// <summary>
		/// Graphic component.
		/// </summary>
		public Graphic Graphic
		{
			get
			{
				return Component;
			}
		}

		/// <summary>
		/// Color.
		/// </summary>
		public Color color
		{
			get
			{
				return Component.color;
			}

			set
			{
				Component.color = value;
			}
		}

		/// <summary>
		/// Font size.
		/// </summary>
		public float fontSize
		{
			get
			{
				return Component.fontSize;
			}

			set
			{
				Component.fontSize = value;
			}
		}

		/// <summary>
		/// Font style.
		/// </summary>
		public FontStyle fontStyle
		{
			get
			{
				if (Bold && Italic)
				{
					return FontStyle.BoldAndItalic;
				}

				if (Bold)
				{
					return FontStyle.Bold;
				}

				if (Italic)
				{
					return FontStyle.Italic;
				}

				return FontStyle.Normal;
			}

			set
			{
				Bold = (value == FontStyle.Bold) || (value == FontStyle.BoldAndItalic);
				Italic = (value == FontStyle.Italic) || (value == FontStyle.BoldAndItalic);
			}
		}

		/// <summary>
		/// Bold.
		/// </summary>
		public bool Bold
		{
			get
			{
				return IsSet(Component.fontStyle, TMPro.FontStyles.Bold);
			}

			set
			{
				if (Bold == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle |= TMPro.FontStyles.Bold;
				}
				else
				{
					Component.fontStyle &= ~TMPro.FontStyles.Bold;
				}
			}
		}

		/// <summary>
		/// Italic.
		/// </summary>
		public bool Italic
		{
			get
			{
				return IsSet(Component.fontStyle, TMPro.FontStyles.Italic);
			}

			set
			{
				if (Italic == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle |= TMPro.FontStyles.Italic;
				}
				else
				{
					Component.fontStyle &= ~TMPro.FontStyles.Italic;
				}
			}
		}

		/// <summary>
		/// Text.
		/// </summary>
		public string text
		{
			get
			{
				return Component.text;
			}

			set
			{
				Component.text = value;
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextTMPro"/> class.
		/// </summary>
		/// <param name="component">Component.</param>
		public TextTMPro(TMPro.TextMeshProUGUI component)
		{
			Component = component;
		}
	}
}
#endif