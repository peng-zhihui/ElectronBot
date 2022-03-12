namespace UIWidgets
{
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// NotifyInfoBase.
	/// </summary>
	public class NotifyInfoBase : MonoBehaviour, IStylable
	{
		/// <summary>
		/// Localization support.
		/// </summary>
		[SerializeField]
		public bool LocalizationSupport;

		/// <summary>
		/// The message.
		/// </summary>
		[SerializeField]
		public TextAdapter MessageAdapter;

		/// <summary>
		/// Content.
		/// </summary>
		[SerializeField]
		public RectTransform ContentRoot;

		/// <summary>
		/// Current message.
		/// </summary>
		protected string CurrentMessage = null;

		/// <summary>
		/// Current message arguments.
		/// </summary>
		protected object[] CurrentMessageArgs = Compatibility.EmptyArray<object>();

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
			if ((MessageAdapter != null) && (CurrentMessage != null))
			{
				var message = LocalizationSupport ? Localization.GetTranslation(CurrentMessage) : CurrentMessage;
				MessageAdapter.text = string.Format(message, CurrentMessageArgs);
			}
		}

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="message">Message.</param>
		public virtual void SetInfo(string message)
		{
			SetInfo(message, Compatibility.EmptyArray<object>());
		}

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="args">An object array that contains zero or more objects to message format.</param>
		public virtual void SetInfo(string message, params object[] args)
		{
			SaveDefaultValues();

			if (message != null)
			{
				CurrentMessage = message;
			}

			CurrentMessageArgs = (args != null) ? args : Compatibility.EmptyArray<object>();

			UpdateText();
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

		bool defaultSaved;
		string defaultMessage;

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

			if (MessageAdapter != null)
			{
				defaultMessage = MessageAdapter.text;
			}
		}

		/// <summary>
		/// Restore default values.
		/// </summary>
		public virtual void RestoreDefaultValues()
		{
			if (defaultSaved)
			{
				SetInfo(defaultMessage, null);
			}
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			if (MessageAdapter != null)
			{
				style.Notify.Text.ApplyTo(MessageAdapter.gameObject);
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			if (MessageAdapter != null)
			{
				style.Notify.Text.GetFrom(MessageAdapter.gameObject);
			}

			return true;
		}
		#endregion
	}
}