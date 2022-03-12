namespace UIWidgets
{
	using System;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// DialogInfoBase.
	/// </summary>
	public class DialogInfoBase : MonoBehaviour
	{
		/// <summary>
		/// Localization support.
		/// </summary>
		[SerializeField]
		public bool LocalizationSupport;

		/// <summary>
		/// The title.
		/// </summary>
		[SerializeField]
		public TextAdapter TitleAdapter;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		public TextAdapter MessageAdapter;

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// Content.
		/// </summary>
		[SerializeField]
		public RectTransform ContentRoot;

		/// <summary>
		/// Title value.
		/// </summary>
		[Obsolete("Replaced with SetTitle().")]
		public virtual string TitleValue
		{
			get
			{
				return (TitleAdapter != null) ? TitleAdapter.text : string.Empty;
			}

			set
			{
				if (TitleAdapter != null)
				{
					TitleAdapter.text = LocalizationSupport ? Localization.GetTranslation(value) : value;
				}
			}
		}

		/// <summary>
		/// Message value.
		/// </summary>
		[Obsolete("Replaced with SetMessage().")]
		public virtual string MessageValue
		{
			get
			{
				return (MessageAdapter != null) ? MessageAdapter.text : string.Empty;
			}

			set
			{
				if (MessageAdapter != null)
				{
					MessageAdapter.text = LocalizationSupport ? Localization.GetTranslation(value) : value;
				}
			}
		}

		/// <summary>
		/// Icon value.
		/// </summary>
		[Obsolete("Replaced with SetIcon().")]
		public virtual Sprite IconValue
		{
			get
			{
				return (Icon != null) ? Icon.sprite : null;
			}

			set
			{
				if (Icon != null)
				{
					Icon.sprite = value;
				}
			}
		}

		/// <summary>
		/// Current title.
		/// </summary>
		protected string CurrentTitle = null;

		/// <summary>
		/// Current title arguments.
		/// </summary>
		protected object[] CurrentTitleArgs = Compatibility.EmptyArray<object>();

		/// <summary>
		/// Current message.
		/// </summary>
		protected string CurrentMessage = null;

		/// <summary>
		/// Current message arguments.
		/// </summary>
		protected object[] CurrentMessageArgs = Compatibility.EmptyArray<object>();

		/// <summary>
		/// Current icon.
		/// </summary>
		protected Sprite CurrentIcon;

		bool defaultSaved;
		string defaultTitle;
		string defaultMessage;
		Sprite defaultIcon;

		/// <summary>
		/// Process enable event.
		/// </summary>
		protected virtual void OnEnable()
		{
			Localization.OnLocaleChanged += UpdateText;
		}

		/// <summary>
		/// Process disable event.
		/// </summary>
		protected virtual void OnDisable()
		{
			Localization.OnLocaleChanged -= UpdateText;
		}

		/// <summary>
		/// Update text.
		/// </summary>
		protected virtual void UpdateText()
		{
			if ((CurrentTitle != null) && (TitleAdapter != null))
			{
				var title = LocalizationSupport ? Localization.GetTranslation(CurrentTitle) : CurrentTitle;
				TitleAdapter.text = string.Format(title, CurrentTitleArgs);
			}

			if ((CurrentMessage != null) && (MessageAdapter != null))
			{
				var message = LocalizationSupport ? Localization.GetTranslation(CurrentMessage) : CurrentMessage;
				MessageAdapter.text = string.Format(message, CurrentMessageArgs);
			}

			if ((CurrentIcon != null) && (Icon != null))
			{
				Icon.sprite = CurrentIcon;
			}
		}

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title, string message, Sprite icon)
		{
			SetInfo(title, null, message, null, icon);
		}

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="titleArgs">Title arguments.</param>
		/// <param name="message">Message.</param>
		/// <param name="messageArgs">Message arguments.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title, object[] titleArgs, string message, object[] messageArgs, Sprite icon)
		{
			SaveDefaultValues();

			if (title != null)
			{
				CurrentTitle = title;
			}

			CurrentTitleArgs = (titleArgs != null) ? titleArgs : Compatibility.EmptyArray<object>();

			if (message != null)
			{
				CurrentMessage = message;
			}

			CurrentMessageArgs = (messageArgs != null) ? messageArgs : Compatibility.EmptyArray<object>();

			if (icon != null)
			{
				CurrentIcon = icon;
			}

			UpdateText();
		}

		/// <summary>
		/// Save default values.
		/// </summary>
		public virtual void SaveDefaultValues()
		{
			if (defaultSaved)
			{
				return;
			}

			defaultSaved = true;

			if (TitleAdapter != null)
			{
				CurrentTitle = TitleAdapter.text;
				defaultTitle = TitleAdapter.text;
			}

			if (MessageAdapter != null)
			{
				CurrentMessage = MessageAdapter.text;
				defaultMessage = MessageAdapter.text;
			}

			if (Icon != null)
			{
				CurrentIcon = Icon.sprite;
				defaultIcon = Icon.sprite;
			}
		}

		/// <summary>
		/// Restore default values.
		/// </summary>
		public virtual void RestoreDefaultValues()
		{
			if (defaultSaved)
			{
				SetInfo(defaultTitle, null, defaultMessage, null, defaultIcon);
			}
		}

		/// <summary>
		/// Set content.
		/// </summary>
		/// <param name="content">Content.</param>
		public virtual void SetContent(RectTransform content)
		{
			if (content == null)
			{
				return;
			}

			if (ContentRoot == null)
			{
				Debug.LogWarning("ContentRoot not specified.", this);
				return;
			}

			content.SetParent(ContentRoot, false);
		}

		/// <summary>
		/// Set widget properties from specified style.
		/// </summary>
		/// <param name="style">Dialog style.</param>
		public virtual void SetStyle(StyleDialog style)
		{
			if (TitleAdapter != null)
			{
				style.Title.ApplyTo(TitleAdapter.gameObject);
			}

			if (MessageAdapter != null)
			{
				style.ContentText.ApplyTo(MessageAdapter.gameObject);
			}

			style.ContentBackground.ApplyTo(ContentRoot);
		}

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="style">Dialog style.</param>
		public virtual void GetStyle(StyleDialog style)
		{
			if (TitleAdapter != null)
			{
				style.Title.GetFrom(TitleAdapter.gameObject);
			}

			if (MessageAdapter != null)
			{
				style.ContentText.GetFrom(MessageAdapter.gameObject);
			}

			style.ContentBackground.GetFrom(ContentRoot);
		}
	}
}