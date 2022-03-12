namespace UIWidgets
{
	using System;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for Pickers.
	/// </summary>
	/// <typeparam name="TValue">Type of value.</typeparam>
	/// <typeparam name="TPicker">Type of picker.</typeparam>
	public abstract class Picker<TValue, TPicker> : MonoBehaviour, ITemplatable, IStylable
		where TPicker : Picker<TValue, TPicker>
	{
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

		static Templates<TPicker> templates;

		/// <summary>
		/// Picker templates.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1000:DoNotDeclareStaticMembersOnGenericTypes", Justification = "Reviewed")]
		public static Templates<TPicker> Templates
		{
			get
			{
				if (templates == null)
				{
					templates = new Templates<TPicker>();
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
					closeButton.onClick.RemoveListener(Cancel);
				}

				closeButton = value;

				if (isInited && (closeButton != null))
				{
					closeButton.onClick.AddListener(Cancel);
				}
			}
		}

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
				closeButton.onClick.AddListener(Cancel);
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
		/// Return picker instance using current instance as template.
		/// </summary>
		/// <returns>New picker instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public TPicker Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return picker instance using current instance as template.
		/// </summary>
		/// <returns>New picker instance.</returns>
		public virtual TPicker Clone()
		{
			var picker = this as TPicker;
			if ((TemplateName != null) && Templates.Exists(TemplateName))
			{
				// do nothing
			}
			else if (!Templates.Exists(gameObject.name))
			{
				Templates.Add(gameObject.name, picker);
			}
			else if (Templates.Get(gameObject.name) != this)
			{
				Templates.Add(gameObject.name, picker);
			}

			var id = gameObject.GetInstanceID().ToString();
			if (!Templates.Exists(id))
			{
				Templates.Add(id, picker);
			}
			else if (Templates.Get(id) != this)
			{
				Templates.Add(id, picker);
			}

			return Templates.Instance(id);
		}

		/// <summary>
		/// The modal key.
		/// </summary>
		protected int? ModalKey;

		/// <summary>
		/// Callback with selected value.
		/// </summary>
		protected Action<TValue> OnSelect;

		/// <summary>
		/// Callback when picker closed without any value selected.
		/// </summary>
		protected Action OnCancel;

		/// <summary>
		/// Show picker.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		/// <param name="onSelect">Callback with selected value.</param>
		/// <param name="onCancel">Callback when picker closed without any value selected.</param>
		/// <param name="modalSprite">Modal sprite.</param>
		/// <param name="modalColor">Modal color.</param>
		/// <param name="canvas">Canvas.</param>
		public virtual void Show(
			TValue defaultValue,
			Action<TValue> onSelect,
			Action onCancel = null,
			Sprite modalSprite = null,
			Color? modalColor = null,
			Canvas canvas = null)
		{
			if (IsTemplate)
			{
				Debug.LogWarning("Use the template clone, not the template itself: PickerTemplate.Clone().Show(...), not PickerTemplate.Show(...)");
			}

			OnSelect = onSelect;
			OnCancel = onCancel;

			var parent = (canvas != null) ? canvas.transform as RectTransform : UtilitiesUI.FindTopmostCanvas(gameObject.transform);
			if (parent != null)
			{
				transform.SetParent(parent, false);
			}

			if (modalColor == null)
			{
				modalColor = new Color(0, 0, 0, 0.8f);
			}

			ModalKey = ModalHelper.Open(this, modalSprite, modalColor, Close, parent);

			transform.SetAsLastSibling();

			gameObject.SetActive(true);

			BeforeOpen(defaultValue);
		}

		/// <summary>
		/// Close picker with specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		public virtual void Selected(TValue value)
		{
			OnSelect(value);
			Close();
		}

		/// <summary>
		/// Close picker without specified value.
		/// </summary>
		public virtual void Cancel()
		{
			if (OnCancel != null)
			{
				OnCancel();
			}

			Close();
		}

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public abstract void BeforeOpen(TValue defaultValue);

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public abstract void BeforeClose();

		/// <summary>
		/// Close picker.
		/// </summary>
		protected virtual void Close()
		{
			BeforeClose();

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
			Templates.ToCache(this as TPicker);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public virtual bool SetStyle(Style style)
		{
			style.Dialog.Background.ApplyTo(GetComponent<Image>());
			style.Dialog.Title.ApplyTo(transform.Find("Header/Title"));

			style.Dialog.ContentBackground.ApplyTo(transform.Find("Content"));

			style.Dialog.Delimiter.ApplyTo(transform.Find("Delimiter/Delimiter"));

			if (closeButton != null)
			{
				style.ButtonClose.ApplyTo(closeButton);
			}
			else
			{
				style.ButtonClose.Background.ApplyTo(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.ApplyTo(transform.Find("Header/CloseButton/Text"));
			}

			return true;
		}

		/// <inheritdoc/>
		public virtual bool GetStyle(Style style)
		{
			style.Dialog.Background.GetFrom(GetComponent<Image>());
			style.Dialog.Title.GetFrom(transform.Find("Header/Title"));

			style.Dialog.ContentBackground.GetFrom(transform.Find("Content"));

			style.Dialog.Delimiter.GetFrom(transform.Find("Delimiter/Delimiter"));

			if (closeButton != null)
			{
				style.ButtonClose.GetFrom(closeButton);
			}
			else
			{
				style.ButtonClose.Background.GetFrom(transform.Find("Header/CloseButton"));
				style.ButtonClose.Text.GetFrom(transform.Find("Header/CloseButton/Text"));
			}

			return true;
		}

		#endregion
	}
}