namespace UIWidgets.Examples
{
	using UIWidgets;
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Nested ListView component.
	/// </summary>
	public class NestedListViewComponent : ListViewItem, IViewData<NestedListViewItem>
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
						 ? new GameObject[] { ListView.transform.parent.gameObject }
						 : new GameObject[] { ListView.transform.parent.gameObject, TextAdapter.gameObject, };
				}

				return objectsToResize;
			}
		}

		/// <summary>
		/// TextAdapter.
		/// </summary>
		[SerializeField]
		public TextAdapter TextAdapter;

		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public ListViewIcons ListView;

		/// <summary>
		/// Gets the current item.
		/// </summary>
		public NestedListViewItem Item
		{
			get;
			protected set;
		}

		/// <summary>
		/// Sets component data with specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		public virtual void SetData(NestedListViewItem item)
		{
			Item = item;

			if (Item == null)
			{
				if (ListView != null)
				{
					ListView.DataSource = new ObservableList<ListViewIconsItemDescription>();
				}

				if (TextAdapter != null)
				{
					TextAdapter.text = string.Empty;
				}
			}
			else
			{
				if (ListView != null)
				{
					ListView.DataSource = item.Items;
				}

				if (TextAdapter != null)
				{
					TextAdapter.text = Item.Name;
				}
			}
		}

		/// <inheritdoc/>
		public override void SetStyle(StyleImage styleBackground, StyleText styleText, Style style)
		{
			base.SetStyle(styleBackground, styleText, style);

			if (ListView != null)
			{
				ListView.SetStyle(style);
			}
		}
	}
}