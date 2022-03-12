namespace UIWidgets
{
	using UIWidgets.l10n;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ListViewIcons item component.
	/// </summary>
	public class ListViewIconsItemComponent : ListViewItem, IViewData<ListViewIconsItemDescription>, IViewData<TreeViewItem>
	{
		GameObject[] objectsToResize;

		/// <summary>
		/// Gets the objects to resize.
		/// </summary>
		/// <value>The objects to resize.</value>
		public GameObject[] ObjectsToResize
		{
			get
			{
				if (objectsToResize == null)
				{
					objectsToResize = (TextAdapter == null)
						 ? new GameObject[] { Icon.transform.parent.gameObject }
						 : new GameObject[] { Icon.transform.parent.gameObject, TextAdapter.gameObject, };
				}

				return objectsToResize;
			}
		}

		/// <summary>
		/// The icon.
		/// </summary>
		[SerializeField]
		public Image Icon;

		/// <summary>
		/// The text.
		/// </summary>
		[SerializeField]
		[HideInInspector]
		[System.Obsolete("Replaced with TextAdapter.")]
		public Text Text;

		/// <summary>
		/// The text adapter.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// Set icon native size.
		/// </summary>
		public bool SetNativeSize = true;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public ListViewIconsItemDescription Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Init graphics foreground.
		/// </summary>
		protected override void GraphicsForegroundInit()
		{
			if (GraphicsForegroundVersion == 0)
			{
				Foreground = new Graphic[] { UtilitiesUI.GetGraphic(TextAdapter), };
				GraphicsForegroundVersion = 1;
			}
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(ListViewIconsItemDescription item)
		{
			Item = item;

			#if UNITY_EDITOR
			name = item == null ? "DefaultItem " + Index.ToString() : item.Name;
			#endif

			if (Item == null)
			{
				if (Icon != null)
				{
					Icon.sprite = null;
				}

				if (TextAdapter != null)
				{
					TextAdapter.text = string.Empty;
				}
			}
			else
			{
				if (Icon != null)
				{
					Icon.sprite = Item.Icon;
				}

				if (TextAdapter != null)
				{
					UpdateName();
				}
			}

			if (Icon != null)
			{
				if (SetNativeSize)
				{
					Icon.SetNativeSize();
				}

				// set transparent color if no icon
				Icon.color = (Icon.sprite == null) ? Color.clear : Color.white;
			}
		}

		/// <summary>
		/// Update display name.
		/// </summary>
		protected void UpdateName()
		{
			if (Item == null)
			{
				return;
			}

			if (TextAdapter != null)
			{
				var name = Item.Name.Replace("\\n", "\n");
				TextAdapter.text = Item.LocalizedName ?? Localization.GetTranslation(name);
			}
		}

		/// <inheritdoc/>
		public override void LocaleChanged()
		{
			UpdateName();
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(TreeViewItem item)
		{
			SetData(new ListViewIconsItemDescription()
			{
				Name = item.Name,
				LocalizedName = item.LocalizedName,
				Icon = item.Icon,
				Value = item.Value,
			});
		}

		/// <summary>
		/// Called when item moved to cache, you can use it free used resources.
		/// </summary>
		public override void MovedToCache()
		{
			if (Icon != null)
			{
				Icon.sprite = null;
			}
		}

		/// <summary>
		/// Upgrade serialized data to the latest version.
		/// </summary>
		public override void Upgrade()
		{
			base.Upgrade();

#pragma warning disable 0618
			Utilities.GetOrAddComponent(Text, ref TextAdapter);
#pragma warning restore 0618
		}
	}
}