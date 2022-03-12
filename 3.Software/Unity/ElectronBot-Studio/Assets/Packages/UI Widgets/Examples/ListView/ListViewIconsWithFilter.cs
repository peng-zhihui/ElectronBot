namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.Serialization;

	/// <summary>
	/// Sample ListViewIcons with filter.
	/// </summary>
	public class ListViewIconsWithFilter : ListViewIcons
	{
		[SerializeField]
		[FormerlySerializedAs("m_items")]
		List<ListViewIconsItemDescription> listItems = new List<ListViewIconsItemDescription>();

		ObservableList<ListViewIconsItemDescription> originalItems;

		/// <summary>
		/// Get or sets items.
		/// </summary>
		public ObservableList<ListViewIconsItemDescription> OriginalItems
		{
			get
			{
				if (originalItems == null)
				{
					originalItems = new ObservableList<ListViewIconsItemDescription>(listItems);
					originalItems.OnChange += Filter;
				}

				return originalItems;
			}

			set
			{
				if (originalItems != null)
				{
					originalItems.OnChange -= Filter;
				}

				originalItems = value;

				if (originalItems != null)
				{
					originalItems.OnChange += Filter;
				}
			}
		}

		/// <summary>
		/// Search string.
		/// </summary>
		protected string Search = string.Empty;

		/// <summary>
		/// Filter data using specified search string.
		/// </summary>
		/// <param name="search">Search string.</param>
		public void Filter(string search)
		{
			Search = search;
			Filter();
		}

		/// <summary>
		/// Copy items from OriginalItems to DataSource if it's match specified string.
		/// </summary>
		protected void Filter()
		{
			DataSource.BeginUpdate();
			DataSource.Clear();

			if (string.IsNullOrEmpty(Search))
			{
				// if search string not specified add all items
				DataSource.AddRange(OriginalItems);
			}
			else
			{
				// else add items with name starts with specified string
				foreach (var item in OriginalItems)
				{
					if (item.Name.StartsWith(Search, System.StringComparison.InvariantCulture))
					{
						DataSource.Add(item);
					}
				}
			}

			DataSource.EndUpdate();
		}

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			base.Init();

			// call Filter() to set initial DataSource
			Filter();
		}

		/// <summary>
		/// Process the destroy event.
		/// </summary>
		protected override void OnDestroy()
		{
			if (originalItems != null)
			{
				originalItems.OnChange -= Filter;
			}

			base.OnDestroy();
		}
	}
}