namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.Serialization;
	using UnityEngine.UI;

	/// <summary>
	/// Popup.
	/// </summary>
	public class Popup : MonoBehaviour, ITemplatable, IStylable, IUpgradeable
	{
		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Info component.")]
		Text titleText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		[Obsolete("Replaced with Info component.")]
		public Text TitleText
		{
			get
			{
				return titleText;
			}

			set
			{
				titleText = value;
			}
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Info component.")]
		Text contentText;

		/// <summary>
		/// Gets or sets the text component.
		/// </summary>
		/// <value>The text.</value>
		[Obsolete("Replaced with Info component.")]
		public Text ContentText
		{
			get
			{
				return contentText;
			}

			set
			{
				contentText = value;
			}
		}

		[SerializeField]
		[HideInInspector]
		[Obsolete("Replaced with Info component.")]
		Image popupIcon;

		/// <summary>
		/// Gets or sets the icon component.
		/// </summary>
		/// <value>The icon.</value>
		[Obsolete("Replaced with Info component.")]
		public Image Icon
		{
			get
			{
				return popupIcon;
			}

			set
			{
				popupIcon = value;
			}
		}

		[SerializeField]
		DialogInfoBase info;

		/// <summary>
		/// Gets the info component.
		/// </summary>
		/// <value>The info component.</value>
		public DialogInfoBase Info
		{
			get
			{
				if (info == null)
				{
					info = GetComponent<DialogInfoBase>();
				}

				return info;
			}
		}

		bool isTemplate = true;

		/// <summary>
		/// Gets a value indicating whether this instance is template.
		/// </summary>
		/// <value><c>true</c> if this instance is template; otherwise, <c>false</c>.</value>
		public bool IsTemplate
		{
			get
			{
				return isTemplate;
			}

			set
			{
				isTemplate = value;
			}
		}

		/// <summary>
		/// Gets the name of the template.
		/// </summary>
		/// <value>The name of the template.</value>
		public string TemplateName
		{
			get;
			set;
		}

		static Templates<Popup> templates;

		/// <summary>
		/// Popup templates.
		/// </summary>
		public static Templates<Popup> Templates
		{
			get
			{
				if (templates == null)
				{
					templates = new Templates<Popup>();
				}

				return templates;
			}

			set
			{
				templates = value;
			}
		}

		[SerializeField]
		Button closeButton;

		/// <summary>
		/// Close button.
		/// </summary>
		public Button CloseButton
		{
			get
			{
				return closeButton;
			}

			set
			{
				if (isInited && (closeButton != null))
				{
					closeButton.onClick.RemoveListener(Close);
				}

				closeButton = value;

				if (isInited && (closeButton != null))
				{
					closeButton.onClick.AddListener(Close);
				}
			}
		}

		/// <summary>
		/// Callback on popup close.
		/// </summary>
		public Action OnClose;

		/// <summary>
		/// Awake is called when the script instance is being loaded.
		/// </summary>
		protected virtual void Awake()
		{
			if (IsTemplate)
			{
				gameObject.SetActive(false);
			}
		}

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		bool isInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			if (closeButton != null)
			{
				closeButton.onClick.AddListener(Close);
			}

			isInited = true;
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			if (!IsTemplate)
			{
				templates = null;
				return;
			}

			// if FindTemplates never called than TemplateName == null
			if (TemplateName != null)
			{
				Templates.Delete(TemplateName);
			}
		}

		/// <summary>
		/// Return popup instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New popup instance.</returns>
		[Obsolete("Use Clone(templateName) instead.")]
		public static Popup Template(string templateName)
		{
			return Clone(templateName);
		}

		/// <summary>
		/// Return popup instance using current instance as template.
		/// </summary>
		/// <returns>New popup instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public Popup Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return popup instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New popup instance.</returns>
		public static Popup Clone(string templateName)
		{
			return Templates.Instance(templateName);
		}

		/// <summary>
		/// Return popup instance using current instance as template.
		/// </summary>
		/// <returns>New popup instance.</returns>
		public Popup Clone()
		{
			if ((TemplateName != null) && Templates.Exists(TemplateName))
			{
				// do nothing
			}
			else if (!Templates.Exists(gameObject.name))
			{
				Templates.Add(gameObject.name, this);
			}
			else if (Templates.Get(gameObject.name) != this)
			{
				Templates.Add(gameObject.name, this);
			}

			var id = gameObject.GetInstanceID().ToString();
			if (!Templates.Exists(id))
			{
				Templates.Add(id, this);
			}
			else if (Templates.Get(id) != this)
			{
				Templates.Add(id, this);
			}

			return Templates.Instance(id);
		}

		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Show popup.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="position">Position.</param>
		/// <param name="icon">Icon.</param>
		/// <param name="modal">If set to <c>true</c> modal.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="canvas">Canvas.</param>
		/// <param name="content">Content. Also can be changed with SetContent().</param>
		/// <param name="onClose">On close callback. Also can be changed with OnClose field.</param>
		public virtual void Show(
			string title = null,
			string message = null,
			Vector3? position = null,
			Sprite icon = null,
			bool modal = false,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null,
			RectTransform content = null,
			Action onClose = null)
		{
			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: PopupTemplate.Clone().Show(...), not PopupTemplate.Show(...)");
			}

			OnClose = onClose;

			SetInfo(title, null, message, null, icon);
			SetContent(content);

			var canvas_rt = SetCanvas(canvas);

			SetModal(modal, modalSprite, modalColor, canvas_rt);

			if (position.HasValue)
			{
				SetPosition(position.Value);
			}

			gameObject.SetActive(true);
		}

		/// <summary>
		/// Set canvas.
		/// </summary>
		/// <param name="canvas">Canvas.</param>
		/// <returns>Canvas RectTransform.</returns>
		public virtual RectTransform SetCanvas(Canvas canvas)
		{
			var parent = (canvas != null) ? canvas.transform as RectTransform : UtilitiesUI.FindTopmostCanvas(gameObject.transform);
			if (parent != null)
			{
				transform.SetParent(parent, false);
			}

			transform.SetAsLastSibling();

			return parent;
		}

		/// <summary>
		/// Set modal mode.
		/// Warning: modal block is created at the current root canvas.
		/// </summary>
		/// <param name="modal">If set to <c>true</c> modal.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="parent">Parent.</param>
		public virtual void SetModal(bool modal = false, Sprite modalSprite = null, Color? modalColor = null, RectTransform parent = null)
		{
			if (ModalKey != null)
			{
				ModalHelper.Close(ModalKey.Value);
				ModalKey = null;
			}

			if (modal)
			{
				ModalKey = ModalHelper.Open(this, modalSprite, modalColor, Close, parent);
			}
			else
			{
				ModalKey = null;
			}

			transform.SetAsLastSibling();
		}

		/// <summary>
		/// Set position.
		/// </summary>
		/// <param name="position">Position.</param>
		public virtual void SetPosition(Vector3 position)
		{
			transform.localPosition = position;
		}

		/// <summary>
		/// Sets the info.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="message">Message.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title = null, string message = null, Sprite icon = null)
		{
			Info.SetInfo(title, null, message, null, icon);
		}

		/// <summary>
		/// Sets the info. Pass null to leave default value.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <param name="titleArgs">Title arguments.</param>
		/// <param name="message">Message.</param>
		/// <param name="messageArgs">Message arguments.</param>
		/// <param name="icon">Icon.</param>
		public virtual void SetInfo(string title = null, object[] titleArgs = null, string message = null, object[] messageArgs = null, Sprite icon = null)
		{
			Info.SetInfo(title, titleArgs, message, messageArgs, icon);
		}

		/// <summary>
		/// Sets the content.
		/// </summary>
		/// <param name="content">Content.</param>
		public virtual void SetContent(RectTransform content)
		{
			if (content == null)
			{
				return;
			}

			Info.SetContent(content);
		}

		/// <summary>
		/// Close popup.
		/// </summary>
		public virtual void Close()
		{
			if (OnClose != null)
			{
				OnClose();
			}

			if (ModalKey.HasValue)
			{
				ModalHelper.Close(ModalKey.Value);
			}

			Return();
		}

		/// <summary>
		/// Return this instance to cache.
		/// </summary>
		protected virtual void Return()
		{
			Templates.ToCache(this);

			ResetParameters();
		}

		/// <summary>
		/// Resets the parameters.
		/// </summary>
		protected virtual void ResetParameters()
		{
			OnClose = null;

			info.RestoreDefaultValues();
		}

		/// <summary>
		/// Upgrade fields data to the latest version.
		/// </summary>
		public virtual void Upgrade()
		{
#pragma warning disable 0618
			if (info == null)
			{
				info = gameObject.AddComponent<DialogInfoBase>();
				Utilities.GetOrAddComponent(titleText, ref info.TitleAdapter);
				Utilities.GetOrAddComponent(contentText, ref info.MessageAdapter);
				info.Icon = Icon;
			}

			if (info.ContentRoot == null)
			{
				info.ContentRoot = transform.Find("Content") as RectTransform;
			}
#pragma warning restore 0618
		}

#if UNITY_EDITOR
		/// <summary>
		/// Update layout when parameters changed.
		/// </summary>
		protected virtual void OnValidate()
		{
			Compatibility.Upgrade(this);
		}
#endif

		#region IStylable implementation

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			style.Dialog.Background.ApplyTo(GetComponent<Image>());
			style.Dialog.ContentBackground.ApplyTo(transform.Find("Content"));
			style.Dialog.Delimiter.ApplyTo(transform.Find("Delimiter/Delimiter"));
			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Close"));

			if (closeButton != null)
			{
				style.ButtonClose.ApplyTo(closeButton);
			}
			else
			{
				style.ButtonClose.Background.ApplyTo(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.ApplyTo(transform.Find("Header/CloseButton/Text"));
			}

			if (Info != null)
			{
				Info.SetStyle(style.Dialog);
			}

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.Dialog.Background.GetFrom(GetComponent<Image>());
			style.Dialog.ContentBackground.GetFrom(transform.Find("Content"));
			style.Dialog.Delimiter.GetFrom(transform.Find("Delimiter/Delimiter"));
			style.Dialog.Button.GetFrom(transform.Find("Buttons/Close"));

			if (closeButton != null)
			{
				style.ButtonClose.GetFrom(closeButton);
			}
			else
			{
				style.ButtonClose.Background.GetFrom(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.GetFrom(transform.Find("Header/CloseButton/Text"));
			}

			if (Info != null)
			{
				Info.GetStyle(style.Dialog);
			}

			return true;
		}
		#endregion
	}
}