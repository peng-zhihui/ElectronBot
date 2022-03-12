namespace UIWidgets.Menu
{
	using System;
	using System.Collections.Generic;
	using UIWidgets.Styles;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ContextMenu template.
	/// </summary>
	[RequireComponent(typeof(LayoutGroup))]
	[RequireComponent(typeof(RectTransform))]
	public partial class ContextMenuView : MonoBehaviour, IStylable
	{
		/// <summary>
		/// MenuItem template.
		/// </summary>
		[Serializable]
		public class MenuItemTemplate
		{
			[SerializeField]
			string key;

			/// <summary>
			/// Key.
			/// </summary>
			public string Key
			{
				get
				{
					return key;
				}

				private set
				{
					key = value;
				}
			}

			[SerializeField]
			MenuItemView template;

			/// <summary>
			/// Template.
			/// </summary>
			public MenuItemView Template
			{
				get
				{
					return template;
				}

				private set
				{
					template = value;
				}
			}

			[SerializeField]
			[HideInInspector]
			List<MenuItemView> Cache = new List<MenuItemView>();

			/// <summary>
			/// Initializes a new instance of the <see cref="MenuItemTemplate"/> class.
			/// </summary>
			/// <param name="key">Key.</param>
			/// <param name="template">Template.</param>
			public MenuItemTemplate(string key, MenuItemView template)
			{
				if (template == null)
				{
					throw new ArgumentNullException("template");
				}

				Key = key;
				Template = template;
			}

			/// <summary>
			/// Get item instance.
			/// </summary>
			/// <returns>Instance.</returns>
			public MenuItemView Instance()
			{
				MenuItemView item;

				if (Cache.Count > 0)
				{
					item = Cache[Cache.Count - 1];
					Cache.RemoveAt(Cache.Count - 1);
				}
				else
				{
					item = Instantiate(Template);
					Utilities.FixInstantiated(Template, item);
					item.RectTransform.localPosition = Vector3.zero;
				}

				item.gameObject.SetActive(true);

				return item;
			}

			/// <summary>
			/// Return item instance to the cache.
			/// </summary>
			/// <param name="instance">Item.</param>
			public void Return(MenuItemView instance)
			{
				instance.transform.SetParent(Template.RectTransform.parent, false);
				instance.Index = -1;
				instance.Item = null;

				instance.gameObject.SetActive(false);

				Cache.Add(instance);
			}

			/// <summary>
			/// Clear cache.
			/// </summary>
			public void Clear()
			{
				for (int i = 0; i < Cache.Count; i++)
				{
					Destroy(Cache[i].gameObject);
				}

				Cache.Clear();
			}
		}
	}
}