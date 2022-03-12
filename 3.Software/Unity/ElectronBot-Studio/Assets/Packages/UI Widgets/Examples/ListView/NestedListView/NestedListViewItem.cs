namespace UIWidgets.Examples
{
	using System;
	using System.Collections.Generic;
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// NestedListView item.
	/// </summary>
	[Serializable]
	public class NestedListViewItem
	{
		/// <summary>
		/// Name.
		/// </summary>
		[SerializeField]
		public string Name;

		[SerializeField]
		List<ListViewIconsItemDescription> items = new List<ListViewIconsItemDescription>();

		ObservableList<ListViewIconsItemDescription> oitems;

		/// <summary>
		/// Items.
		/// </summary>
		public ObservableList<ListViewIconsItemDescription> Items
		{
			get
			{
				if (oitems == null)
				{
					oitems = new ObservableList<ListViewIconsItemDescription>(items);
				}

				return oitems;
			}

			set
			{
				oitems = value;
			}
		}
	}
}