namespace UIWidgets
{
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// TextUnity.
	/// </summary>
	public class TextUnity : ITextProxy
	{
		/// <summary>
		/// Text component.
		/// </summary>
		protected Text Component;

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
				Component.fontSize = Mathf.RoundToInt(value);
			}
		}

		/// <summary>
		/// Font style.
		/// </summary>
		public FontStyle fontStyle
		{
			get
			{
				return Component.fontStyle;
			}

			set
			{
				Component.fontStyle = value;
			}
		}

		/// <summary>
		/// Bold.
		/// </summary>
		public bool Bold
		{
			get
			{
				return (Component.fontStyle == FontStyle.Bold) || (Component.fontStyle == FontStyle.BoldAndItalic);
			}

			set
			{
				if (Bold == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Normal)
						? FontStyle.Bold
						: FontStyle.BoldAndItalic;
				}
				else
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Bold)
						? FontStyle.Normal
						: FontStyle.Italic;
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
				return (Component.fontStyle == FontStyle.Italic) || (Component.fontStyle == FontStyle.BoldAndItalic);
			}

			set
			{
				if (Italic == value)
				{
					return;
				}

				if (value)
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Normal)
						? FontStyle.Italic
						: FontStyle.BoldAndItalic;
				}
				else
				{
					Component.fontStyle = (Component.fontStyle == FontStyle.Italic)
						? FontStyle.Normal
						: FontStyle.Bold;
				}
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextUnity"/> class.
		/// </summary>
		/// <param name="component">Component.</param>
		public TextUnity(Text component)
		{
			Component = component;
		}
	}
}