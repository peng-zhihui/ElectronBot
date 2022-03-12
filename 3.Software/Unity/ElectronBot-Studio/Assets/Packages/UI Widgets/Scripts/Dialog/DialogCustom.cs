namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UIWidgets.l10n;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Base class for the Dialogs.
	/// </summary>
	/// <typeparam name="TDialog">Dialog type.</typeparam>
	public abstract class DialogCustom<TDialog> : DialogBase
		where TDialog : DialogCustom<TDialog>, IHideable
	{
		/// <summary>
		/// Button instance.
		/// </summary>
		public class ButtonInstance : DialogButtonCustom<TDialog, DialogBase, DialogButton>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ButtonInstance"/> class.
			/// </summary>
			/// <param name="owner">Owner.</param>
			/// <param name="index">Index.</param>
			/// <param name="info">Info.</param>
			/// <param name="template">Template.</param>
			/// <param name="container">Button container.</param>
			public ButtonInstance(TDialog owner, int index, DialogButton info, Button template, RectTransform container)
				: base(owner, index, info, template, container)
			{
			}
		}

		/// <summary>
		/// Class for the buttons instances.
		/// </summary>
		protected class ButtonsPool : DialogButtonsPoolCustom<TDialog, DialogBase, ButtonInstance, DialogButton>
		{
			/// <summary>
			/// Initializes a new instance of the <see cref="ButtonsPool"/> class.
			/// </summary>
			/// <param name="owner">Dialog.</param>
			/// <param name="templates">Templates.</param>
			/// <param name="container">Container.</param>
			/// <param name="active">List for the active buttons.</param>
			/// <param name="cache">List for the cached buttons.</param>
			public ButtonsPool(TDialog owner, ReadOnlyCollection<Button> templates, RectTransform container, List<ButtonInstance> active, List<List<ButtonInstance>> cache)
				: base(owner, templates, container, active, cache)
			{
			}

			/// <summary>
			/// Create the button.
			/// </summary>
			/// <param name="buttonIndex">Index of the button.</param>
			/// <param name="info">Button info.</param>
			/// <returns>Button.</returns>
			protected override ButtonInstance CreateButtonInstance(int buttonIndex, DialogButton info)
			{
				return new ButtonInstance(Owner, buttonIndex, info, Templates[info.TemplateIndex], Container);
			}
		}

		ButtonsPool buttonsPool;

		/// <summary>
		/// Buttons pool.
		/// </summary>
		protected ButtonsPool Buttons
		{
			get
			{
				if (buttonsPool == null)
				{
					buttonsPool = new ButtonsPool(this as TDialog, ButtonsTemplates, ButtonsContainer, ButtonsActive, ButtonsCached);
				}

				return buttonsPool;
			}
		}

		/// <summary>
		/// The buttons in use.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ButtonInstance> ButtonsActive = new List<ButtonInstance>();

		/// <summary>
		/// Current buttons.
		/// </summary>
		public ReadOnlyCollection<ButtonInstance> CurrentButtons
		{
			get
			{
				return ButtonsActive.AsReadOnly();
			}
		}

		/// <summary>
		/// The cached buttons.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<List<ButtonInstance>> ButtonsCached = new List<List<ButtonInstance>>();

		/// <inheritdoc/>
		public override ReadOnlyCollection<Button> ButtonsTemplates
		{
			get
			{
				Upgrade();

				return buttonsTemplates.AsReadOnly();
			}

			set
			{
				if (IsTemplate && (buttonsTemplates.Count > value.Count))
				{
					throw new ArgumentOutOfRangeException("value", string.Format("Buttons count cannot be decreased. Current is {0}; New is {1}", buttonsTemplates.Count.ToString(), value.Count.ToString()));
				}

				Buttons.Replace(value);

				buttonsTemplates.Clear();
				buttonsTemplates.AddRange(value);

				if (buttonsTemplates.Count > 0)
				{
					Upgrade();
				}
			}
		}

		static Templates<TDialog> templates;

		/// <summary>
		/// Dialog templates.
		/// </summary>
		public static Templates<TDialog> Templates
		{
			get
			{
				if (templates == null)
				{
					templates = new Templates<TDialog>();
				}

				return templates;
			}

			set
			{
				templates = value;
			}
		}

		/// <summary>
		/// Opened dialogs.
		/// </summary>
		protected static HashSet<TDialog> openedDialogs = new HashSet<TDialog>();

		/// <summary>
		/// List of the opened dialogs.
		/// </summary>
		protected static List<TDialog> OpenedDialogsList = new List<TDialog>();

		/// <summary>
		/// Opened dialogs with the same template.
		/// </summary>
		public static ReadOnlyCollection<TDialog> OpenedDialogs
		{
			get
			{
				OpenedDialogsList.Clear();
				OpenedDialogsList.AddRange(openedDialogs);

				return OpenedDialogsList.AsReadOnly();
			}
		}

		/// <summary>
		/// Inactive dialogs with the same template.
		/// </summary>
		public List<TDialog> InactiveDialogs
		{
			get
			{
				return Templates.CachedInstances(TemplateName);
			}
		}

		/// <summary>
		/// All dialogs with the same template.
		/// </summary>
		public List<TDialog> AllDialogs
		{
			get
			{
				var dialogs = InactiveDialogs;
				dialogs.AddRange(OpenedDialogs);

				return dialogs;
			}
		}

		/// <summary>
		/// Count of the opened dialogs.
		/// </summary>
		public static int Opened
		{
			get
			{
				return openedDialogs.Count;
			}
		}

		/// <summary>
		/// Find templates.
		/// </summary>
		protected override void FindTemplates()
		{
			Templates.FindTemplates();
		}

		/// <inheritdoc/>
		protected override void LocaleChanged()
		{
			Buttons.UpdateButtonsName();
		}

		/// <inheritdoc/>
		protected override void OnEnable()
		{
			openedDialogs.Add(this as TDialog);
		}

		/// <inheritdoc/>
		protected override void OnDisable()
		{
			openedDialogs.Remove(this as TDialog);
		}

		/// <inheritdoc/>
		protected override void OnDestroy()
		{
			Buttons.Disable();

			if (!IsTemplate)
			{
				templates = null;
			}
			else if (TemplateName != null)
			{
				Templates.Delete(TemplateName);
			}

			base.OnDestroy();
		}

		/// <summary>
		/// Return dialog instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New Dialog instance.</returns>
		[Obsolete("Use Clone(templateName) instead.")]
		public static TDialog Template(string templateName)
		{
			return Clone(templateName);
		}

		/// <summary>
		/// Return dialog instance using current instance as template.
		/// </summary>
		/// <returns>New Dialog instance.</returns>
		[Obsolete("Use Clone() instead.")]
		public TDialog Template()
		{
			return Clone();
		}

		/// <summary>
		/// Return dialog instance by the specified template name.
		/// </summary>
		/// <param name="templateName">Template name.</param>
		/// <returns>New Dialog instance.</returns>
		public static TDialog Clone(string templateName)
		{
			return Templates.Instance(templateName);
		}

		/// <summary>
		/// Return dialog instance using current instance as template.
		/// </summary>
		/// <returns>New Dialog instance.</returns>
		public TDialog Clone()
		{
			var dialog = this as TDialog;
			if ((TemplateName != null) && Templates.Exists(TemplateName))
			{
				// do nothing
			}
			else if (!Templates.Exists(gameObject.name))
			{
				Templates.Add(gameObject.name, dialog);
			}
			else if (Templates.Get(gameObject.name) != dialog)
			{
				Templates.Add(gameObject.name, dialog);
			}

			var id = gameObject.GetInstanceID().ToString();
			if (!Templates.Exists(id))
			{
				Templates.Add(id, dialog);
			}
			else if (Templates.Get(id) != dialog)
			{
				Templates.Add(id, dialog);
			}

			return Templates.Instance(id);
		}

		/// <inheritdoc/>
		public override void SetButtons(IList<DialogButton> buttons, string focusButton = null)
		{
			Buttons.Disable();

			if (buttons == null)
			{
				return;
			}

			for (int index = 0; index < buttons.Count; index++)
			{
				var info = buttons[index];
				info.TemplateIndex = GetTemplateIndex(info);
				Buttons.Get(index, info);
			}

			FocusButton(focusButton);
		}

		/// <inheritdoc/>
		public override bool FocusButton(string focusButton)
		{
			return Buttons.Focus(focusButton);
		}

		/// <inheritdoc/>
		protected override int GetTemplateIndex(DialogButton button)
		{
			var template = button.TemplateIndex;
			if (template < 0)
			{
				Debug.LogWarning(string.Format("Negative button index not supported. Button: {0}. Index: {1}.", button.Label, template.ToString()));
				template = 0;
			}

			if (template >= Buttons.Count)
			{
				Debug.LogWarning(string.Format(
					"Not found button template with index {0} for the button: {1}. Available indices: 0..{2}",
					template.ToString(),
					button.Label,
					(Buttons.Count - 1).ToString()));
				template = 0;
			}

			return template;
		}

		/// <inheritdoc/>
		protected override void Return()
		{
			Templates.ToCache(this as TDialog);

			Buttons.Disable();

			base.Return();
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			if (buttonsPool != null)
			{
				buttonsPool.SetStyle(style.Dialog.Button);
			}
			else
			{
				for (int template_index = 0; template_index < ButtonsTemplates.Count; template_index++)
				{
					style.Dialog.Button.ApplyTo(ButtonsTemplates[template_index].gameObject);
				}
			}

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			for (int template_index = 0; template_index < ButtonsTemplates.Count; template_index++)
			{
				style.Dialog.Button.GetFrom(ButtonsTemplates[template_index].gameObject);
			}

			return true;
		}
		#endregion
	}
}