namespace UIWidgets
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Text adapter to work with both Unity text and TMPro text.
	/// </summary>
	[AddComponentMenu("UI/New UI Widgets/Adapters/Text Adapter")]
	public class TextAdapter : MonoBehaviour, ITextProxy
	{
		ITextProxy proxy;

		/// <summary>
		/// Proxy.
		/// </summary>
		protected ITextProxy Proxy
		{
			get
			{
				if (proxy == null)
				{
					proxy = GetProxy();
				}

				return proxy;
			}
		}

		/// <summary>
		/// Proxy gameobject.
		/// </summary>
		public GameObject GameObject
		{
			get
			{
				return Proxy.GameObject;
			}
		}

		/// <summary>
		/// Proxy graphic component.
		/// </summary>
		public Graphic Graphic
		{
			get
			{
				return Proxy.Graphic;
			}
		}

		/// <summary>
		/// Proxy color.
		/// </summary>
		public Color color
		{
			get
			{
				return Proxy.color;
			}

			set
			{
				Proxy.color = value;
			}
		}

		/// <summary>
		/// Font size.
		/// </summary>
		public float fontSize
		{
			get
			{
				return Proxy.fontSize;
			}

			set
			{
				Proxy.fontSize = value;
			}
		}

		/// <summary>
		/// Font style.
		/// </summary>
		public FontStyle fontStyle
		{
			get
			{
				return Proxy.fontStyle;
			}

			set
			{
				Proxy.fontStyle = value;
			}
		}

		/// <summary>
		/// Bold.
		/// </summary>
		public bool Bold
		{
			get
			{
				return Proxy.Bold;
			}

			set
			{
				Proxy.Bold = value;
			}
		}

		/// <summary>
		/// Italic.
		/// </summary>
		public bool Italic
		{
			get
			{
				return Proxy.Italic;
			}

			set
			{
				Proxy.Italic = value;
			}
		}

		/// <summary>
		/// Proxy value.
		/// </summary>
		public string Value
		{
			get
			{
				return Proxy.text;
			}

			set
			{
				Proxy.text = value;
			}
		}

		/// <summary>
		/// Proxy value.
		/// Alias for Value property.
		/// </summary>
		public string text
		{
			get
			{
				return Proxy.text;
			}

			set
			{
				Proxy.text = value;
			}
		}

		/// <summary>
		/// Proxy value.
		/// Alias for Value property.
		/// </summary>
		[Obsolete("Renamed to text.")]
		public string Text
		{
			get
			{
				return Proxy.text;
			}

			set
			{
				Proxy.text = value;
			}
		}

		/// <summary>
		/// Get text proxy.
		/// </summary>
		/// <returns>Proxy instance.</returns>
		protected virtual ITextProxy GetProxy()
		{
			var text_unity = GetComponent<Text>();
			if (text_unity != null)
			{
				return new TextUnity(text_unity);
			}

#if UIWIDGETS_TMPRO_SUPPORT
			var text_tmpro = GetComponent<TMPro.TextMeshProUGUI>();
			if (text_tmpro != null)
			{
				return new TextTMPro(text_tmpro);
			}
#endif

			Debug.LogWarning("Not found any Text component. Probably TextMeshPro support is disabled.", this);

			return new TextNull();
		}

#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected virtual void OnValidate()
		{
			GetProxy();
		}
#endif
	}
}