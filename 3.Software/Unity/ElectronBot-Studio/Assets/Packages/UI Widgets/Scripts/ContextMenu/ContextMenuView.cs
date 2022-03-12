namespace UIWidgets.Menu
{
	using System;
	using System.Collections.Generic;
	using System.Collections.ObjectModel;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.EventSystems;
	using UnityEngine.UI;

	/// <summary>
	/// ContextMenu view.
	/// </summary>
	[RequireComponent(typeof(LayoutGroup))]
	[RequireComponent(typeof(ContentSizeFitter))]
	[RequireComponent(typeof(RectTransform))]
	public partial class ContextMenuView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IStylable
	{
		[SerializeField]
		MenuItemView defaultItem;

		/// <summary>
		/// Default item.
		/// </summary>
		public MenuItemView DefaultItem
		{
			get
			{
				return defaultItem;
			}

			set
			{
				if (defaultItem != value)
				{
					defaultItem = value;
					DefaultItemTemplate = new MenuItemTemplate(null, DefaultItem);

					UpdateEmptyTemplateReferences();
				}
			}
		}

		MenuItemTemplate defaultItemTemplate;

		/// <summary>
		/// Default item template.
		/// </summary>
		protected MenuItemTemplate DefaultItemTemplate
		{
			get
			{
				if (defaultItemTemplate == null)
				{
					defaultItemTemplate = new MenuItemTemplate(null, DefaultItem);

					EmptyTemplate.defaultItemTemplate = defaultItemTemplate;
				}

				return defaultItemTemplate;
			}

			set
			{
				if (defaultItemTemplate != value)
				{
					SubscribersReset();

					defaultItemTemplate = value;

					UpdateEmptyTemplateReferences();

					SubscribersUpdate();
				}
			}
		}

		[SerializeField]
		List<MenuItemTemplate> specialItems = new List<MenuItemTemplate>();

		/// <summary>
		/// Special items.
		/// </summary>
		public ReadOnlyCollection<MenuItemTemplate> SpecialItems
		{
			get
			{
				return specialItems.AsReadOnly();
			}

			set
			{
				if (value == null)
				{
					throw new ArgumentNullException("value");
				}

				SubscribersReset();

				specialItems.Clear();
				specialItems.AddRange(value);

				UpdateEmptyTemplateReferences();

				SubscribersUpdate();
			}
		}

		/// <summary>
		/// Cache.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		protected List<ContextMenuView> Cache = new List<ContextMenuView>();

		/// <summary>
		/// Empty template without nested items templates.
		/// </summary>
		protected ContextMenuView EmptyTemplate;

		RectTransform rectTransform;

		/// <summary>
		/// RectTransform.
		/// </summary>
		public RectTransform RectTransform
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
		/// Is pointer over?
		/// </summary>
		public bool IsPointerOver
		{
			get;
			protected set;
		}

		/// <summary>
		/// Subscribers.
		/// </summary>
		protected readonly List<IMenuSubscriber> Subscribers = new List<IMenuSubscriber>();

		/// <summary>
		/// Process the start event.
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

			gameObject.SetActive(false);

			defaultItem.gameObject.SetActive(false);

			for (int i = 0; i < specialItems.Count; i++)
			{
				specialItems[i].Template.gameObject.SetActive(false);
			}

			isInited = true;
		}

		/// <summary>
		/// Subscribe menu instance.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public void Subscribe(IMenuSubscriber menu)
		{
			Subscribers.Add(menu);
		}

		/// <summary>
		/// Unsubscribe menu instance.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public void Unsubscribe(IMenuSubscriber menu)
		{
			Subscribers.Remove(menu);
		}

		/// <summary>
		/// Send ResetItems event to the subscribers.
		/// </summary>
		protected void SubscribersReset()
		{
			foreach (var s in Subscribers)
			{
				ResetItems(s);
			}
		}

		/// <summary>
		/// Send ResetItems event to the subscriber.
		/// </summary>
		/// <param name="subscriber">Subscriber.</param>
		protected static void ResetItems(IMenuSubscriber subscriber)
		{
			subscriber.ResetItems();
		}

		/// <summary>
		/// Send UpdateItems event to the subscribers.
		/// </summary>
		protected void SubscribersUpdate()
		{
			foreach (var s in Subscribers)
			{
				UpdateItems(s);
			}
		}

		/// <summary>
		/// Send UpdateItems event to the subscriber.
		/// </summary>
		/// <param name="subscriber">Subscriber.</param>
		protected static void UpdateItems(IMenuSubscriber subscriber)
		{
			subscriber.UpdateItems();
		}

		/// <summary>
		/// Return menu to the cache.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public void Return(ContextMenuView menu)
		{
			if (menu == null)
			{
				return;
			}

			menu.IsPointerOver = false;
			menu.transform.SetParent(RectTransform, false);
			menu.gameObject.SetActive(false);

			Cache.Add(menu);
		}

		/// <summary>
		/// Return MenuItem view to the cache.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <param name="view">Instance.</param>
		public void Return(MenuItem item, MenuItemView view)
		{
			GetItemTemplate(item).Return(view);
		}

		/// <summary>
		/// Get menu.
		/// </summary>
		/// <returns>Menu.</returns>
		public virtual ContextMenuView Instance()
		{
			if (EmptyTemplate == null)
			{
				CreateEmptyTemplate();
			}

			ContextMenuView menu;
			if (Cache.Count > 0)
			{
				menu = Cache[Cache.Count - 1];
				Cache.RemoveAt(Cache.Count - 1);
			}
			else
			{
				menu = Instantiate(EmptyTemplate);
				Utilities.FixInstantiated(EmptyTemplate, menu);
				menu.Init();
			}

			menu.IsPointerOver = false;
			menu.gameObject.SetActive(true);

			return menu;
		}

		/// <summary>
		/// Create empty template.
		/// </summary>
		protected virtual void CreateEmptyTemplate()
		{
			EmptyTemplate = Instantiate(this);
			Utilities.FixInstantiated(this, EmptyTemplate);

			EmptyTemplate.gameObject.SetActive(false);
			EmptyTemplate.name = string.Format("{0}EmptyTemplate", name);

			// destroy nested defaultItem
			if (EmptyTemplate.defaultItem.RectTransform.IsChildOf(EmptyTemplate.RectTransform))
			{
				if (EmptyTemplate.defaultItemTemplate != null)
				{
					EmptyTemplate.defaultItemTemplate.Clear();
				}

				EmptyTemplate.defaultItem.transform.SetParent(null, false);
				Destroy(EmptyTemplate.defaultItem.gameObject);
			}

			// destroy nested specialItems
			for (int i = 0; i < EmptyTemplate.specialItems.Count; i++)
			{
				var item = EmptyTemplate.specialItems[i];
				if (item.Template.RectTransform.IsChildOf(EmptyTemplate.RectTransform))
				{
					item.Clear();

					item.Template.transform.SetParent(null, false);
					Destroy(item.Template.gameObject);
				}
			}

			EmptyTemplate.Init();

			UpdateEmptyTemplateReferences();
		}

		/// <summary>
		/// Update empty template.
		/// </summary>
		protected virtual void UpdateEmptyTemplateReferences()
		{
			if (EmptyTemplate == null)
			{
				return;
			}

			EmptyTemplate.defaultItem = defaultItem;
			EmptyTemplate.DefaultItemTemplate = DefaultItemTemplate;
			EmptyTemplate.specialItems = specialItems;
		}

		/// <summary>
		/// Get item instance.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Instance.</returns>
		public MenuItemView GetItemView(MenuItem item)
		{
			return GetItemTemplate(item).Instance();
		}

		/// <summary>
		/// Get item template.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Template.</returns>
		protected MenuItemTemplate GetItemTemplate(MenuItem item)
		{
			for (int i = 0; i < SpecialItems.Count; i++)
			{
				if (SpecialItems[i].Key == item.Template)
				{
					return SpecialItems[i];
				}
			}

			return DefaultItemTemplate;
		}

		/// <summary>
		/// Process the pointer enter event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerEnter(PointerEventData eventData)
		{
			IsPointerOver = true;
		}

		/// <summary>
		/// Process the pointer exit event.
		/// </summary>
		/// <param name="eventData">Event data.</param>
		public void OnPointerExit(PointerEventData eventData)
		{
			IsPointerOver = false;
		}

		#region IStyleable

		/// <summary>
		/// Name of the delimiter template.
		/// </summary>
		protected static readonly string DelimiterName = "-";

		/// <inheritdoc/>
		public bool SetStyle(Style style)
		{
			style.ContextMenu.MainBackground.ApplyTo(this);

			for (int i = 0; i < SpecialItems.Count; i++)
			{
				if (UtilitiesCompare.StartsWith(SpecialItems[i].Key, DelimiterName))
				{
					SpecialItems[i].Template.SetDelimiterStyle(style.ContextMenu.DelimiterImage);
				}
				else
				{
					SpecialItems[i].Template.SetStyle(style);
				}
			}

			DefaultItem.SetStyle(style);

			return true;
		}

		/// <inheritdoc/>
		public bool GetStyle(Style style)
		{
			style.ContextMenu.MainBackground.GetFrom(this);

			for (int i = 0; i < SpecialItems.Count; i++)
			{
				if (UtilitiesCompare.StartsWith(SpecialItems[i].Key, DelimiterName))
				{
					SpecialItems[i].Template.GetDelimiterStyle(style.ContextMenu.DelimiterImage);
				}
				else
				{
					SpecialItems[i].Template.GetStyle(style);
				}
			}

			DefaultItem.GetStyle(style);

			return true;
		}
		#endregion
	}
}