namespace UIWidgets
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UIWidgets.Extensions;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// Class for the buttons instances.
	/// </summary>
	/// <typeparam name="TOwner">Type of the owner.</typeparam>
	/// <typeparam name="TOwnerBase">Base type of the owner.</typeparam>
	/// <typeparam name="TButtonInstance">Type of the button instance.</typeparam>
	/// <typeparam name="TButtonConfig">Type of the button configuration.</typeparam>
	public abstract class DialogButtonsPoolCustom<TOwner, TOwnerBase, TButtonInstance, TButtonConfig>
		where TOwner : class, IHideable, TOwnerBase
		where TOwnerBase : class, IHideable
		where TButtonInstance : DialogButtonCustom<TOwner, TOwnerBase, TButtonConfig>
		where TButtonConfig : ButtonConfiguration<TOwnerBase>
	{
		/// <summary>
		/// Owner.
		/// </summary>
		protected TOwner Owner;

		/// <summary>
		/// Buttons templates.
		/// </summary>
		protected ReadOnlyCollection<Button> Templates;

		/// <summary>
		/// Active buttons.
		/// </summary>
		protected List<TButtonInstance> Active;

		/// <summary>
		/// Cached buttons.
		/// </summary>
		protected List<List<TButtonInstance>> Cache;

		/// <summary>
		/// Count.
		/// </summary>
		public int Count
		{
			get
			{
				return Templates.Count;
			}
		}

		/// <summary>
		/// Buttons container.
		/// </summary>
		protected RectTransform Container;

		/// <summary>
		/// Initializes a new instance of the <see cref="DialogButtonsPoolCustom{TOwner, TOwnerBase, TButtonInstance, TButtonConfig}"/> class.
		/// </summary>
		/// <param name="owner">Dialog.</param>
		/// <param name="templates">Templates.</param>
		/// <param name="container">Container.</param>
		/// <param name="active">List for the active buttons.</param>
		/// <param name="cache">List for the cached buttons.</param>
		public DialogButtonsPoolCustom(TOwner owner, ReadOnlyCollection<Button> templates, RectTransform container, List<TButtonInstance> active, List<List<TButtonInstance>> cache)
		{
			Owner = owner;
			Active = active;
			Cache = cache;
			Container = container;

			SetTemplates(templates);
		}

		/// <summary>
		/// Set focus to the specified button.
		/// </summary>
		/// <param name="focusButton">Button label.</param>
		/// <returns>true if button found with specified label; otherwise false.</returns>
		public bool Focus(string focusButton)
		{
			for (int i = 0; i < Active.Count; i++)
			{
				if (Active[i].Info.Label == focusButton)
				{
					Active[i].Button.Select();
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Set templates.
		/// </summary>
		/// <param name="templates">Templates.</param>
		public void SetTemplates(ReadOnlyCollection<Button> templates)
		{
			Templates = templates;

			for (int i = 0; i < Templates.Count; i++)
			{
				Templates[i].gameObject.SetActive(false);
			}

			EnsureListSize(Cache, Templates.Count);
		}

		/// <summary>
		/// Ensure list size.
		/// </summary>
		/// <typeparam name="T">Type of the list data.</typeparam>
		/// <param name="list">List.</param>
		/// <param name="size">Required size.</param>
		protected static void EnsureListSize<T>(List<List<T>> list, int size)
		{
			for (int i = list.Count; i < size; i++)
			{
				list.Add(new List<T>());
			}

			for (int i = size; i > list.Count; i--)
			{
				list.RemoveAt(i - 1);
			}
		}

		/// <summary>
		/// Get the button.
		/// </summary>
		/// <param name="buttonIndex">Index of the button.</param>
		/// <param name="info">Button info.</param>
		/// <returns>Button.</returns>
		public TButtonInstance Get(int buttonIndex, TButtonConfig info)
		{
			TButtonInstance instance;
			if (Cache[info.TemplateIndex].Count > 0)
			{
				instance = Cache[info.TemplateIndex].Pop();
				instance.Change(buttonIndex, info);
			}
			else
			{
				instance = CreateButtonInstance(buttonIndex, info);
			}

			Active.Add(instance);

			instance.SetActive(true);

			return instance;
		}

		/// <summary>
		/// Create the button.
		/// </summary>
		/// <param name="buttonIndex">Index of the button.</param>
		/// <param name="info">Button info.</param>
		/// <returns>Button.</returns>
		protected abstract TButtonInstance CreateButtonInstance(int buttonIndex, TButtonConfig info);

		/// <summary>
		/// Replace buttons templates.
		/// </summary>
		/// <param name="templates">Templates.</param>
		public void Replace(ReadOnlyCollection<Button> templates)
		{
			ClearCache();

			SetTemplates(templates);

			for (int button_index = 0; button_index < Active.Count; button_index++)
			{
				var button = Active[button_index];
				button.Replace(Templates[button.TemplateIndex]);
			}
		}

		/// <summary>
		/// Disable.
		/// </summary>
		public void Disable()
		{
			for (int button_index = 0; button_index < Active.Count; button_index++)
			{
				var button = Active[button_index];
				button.SetActive(false);

				Cache[button.TemplateIndex].Add(button);
			}

			Active.Clear();
		}

		/// <summary>
		/// Clear cache.
		/// </summary>
		protected void ClearCache()
		{
			for (int template_index = 0; template_index < Cache.Count; template_index++)
			{
				for (int i = 0; i < Cache[template_index].Count; i++)
				{
					Cache[template_index][i].Destroy();
				}

				Cache[template_index].Clear();
			}
		}

		/// <summary>
		/// Execute action for each button.
		/// </summary>
		/// <param name="action">Action.</param>
		public void ForEach(Action<Button> action)
		{
			for (int template_index = 0; template_index < Templates.Count; template_index++)
			{
				action(Templates[template_index]);

				for (int button_index = 0; button_index < Cache[template_index].Count; button_index++)
				{
					action(Cache[template_index][button_index].Button);
				}
			}

			for (int button_index = 0; button_index < Active.Count; button_index++)
			{
				action(Active[button_index].Button);
			}
		}

		/// <summary>
		/// Set style.
		/// </summary>
		/// <param name="styleButton">Button style.</param>
		public void SetStyle(StyleButton styleButton)
		{
			for (int template_index = 0; template_index < Templates.Count; template_index++)
			{
				styleButton.ApplyTo(Templates[template_index].gameObject);

				for (int button_index = 0; button_index < Cache[template_index].Count; button_index++)
				{
					styleButton.ApplyTo(Cache[template_index][button_index].Button.gameObject);
				}
			}

			for (int button_index = 0; button_index < Active.Count; button_index++)
			{
				styleButton.ApplyTo(Active[button_index].Button.gameObject);
			}
		}

		/// <summary>
		/// Update buttons name.
		/// </summary>
		public void UpdateButtonsName()
		{
			for (int i = 0; i < Active.Count; i++)
			{
				Active[i].UpdateButtonName();
			}
		}
	}
}