namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;

	/// <summary>
	/// Base class for the generic tooltip.
	/// </summary>
	public abstract class TooltipBase : MonoBehaviour
	{
		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		protected RectTransform RectTransform
		{
			get
			{
				if (rectTransform == null)
				{
					rectTransform = transform as RectTransform;
				}

				return rectTransform;
			}
		}

		/// <summary>
		/// Current target.
		/// </summary>
		public GameObject CurrentTarget
		{
			get;
			protected set;
		}

		/// <summary>
		/// Current parent.
		/// </summary>
		protected RectTransform CurrentParent;

		bool isInited;

		/// <summary>
		/// Start this instance.
		/// </summary>
		protected virtual void Start()
		{
			Init();
		}

		/// <summary>
		/// This function is called when the MonoBehaviour will be destroyed.
		/// </summary>
		protected virtual void OnDestroy()
		{
			Localization.OnLocaleChanged -= LocaleChanged;
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public virtual void Init()
		{
			if (isInited)
			{
				return;
			}

			isInited = true;

			if (CurrentTarget == null)
			{
				gameObject.SetActive(false);
			}

			Localization.OnLocaleChanged += LocaleChanged;
		}

		/// <summary>
		/// Process locale changes.
		/// </summary>
		public virtual void LocaleChanged()
		{
			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		protected abstract void UpdateView();

		/// <summary>
		/// Get settings for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <returns>Settings.</returns>
		public abstract TooltipSettings GetSettings(GameObject target);

		/// <summary>
		/// Update settings for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="settings">Settings.</param>
		/// <returns>true if target registered and settings was updated; otherwise false.</returns>
		public abstract bool UpdateSettings(GameObject target, TooltipSettings settings);

		/// <summary>
		/// Show tooltip for the specified target.
		/// </summary>
		/// <param name="target">Target.</param>
		public abstract void Show(GameObject target);

		/// <summary>
		/// Hide tooltip.
		/// </summary>
		public abstract void Hide();
	}
}