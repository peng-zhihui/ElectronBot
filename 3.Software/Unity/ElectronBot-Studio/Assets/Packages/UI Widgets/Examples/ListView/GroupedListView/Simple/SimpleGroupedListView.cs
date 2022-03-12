namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// SimpleGroupedListView
	/// </summary>
	public class SimpleGroupedListView : ListViewCustom<SimpleGroupedComponent, SimpleGroupedItem>
	{
		/// <summary>
		/// Grouped data.
		/// </summary>
		public SimpleGroupedList GroupedData = new SimpleGroupedList();

		bool isGroupedListViewInited;

		/// <summary>
		/// Init this instance.
		/// </summary>
		public override void Init()
		{
			if (isGroupedListViewInited)
			{
				return;
			}

			isGroupedListViewInited = true;

			base.Init();

			// set groups sort by name
			GroupedData.GroupComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

			// set data source
			GroupedData.Data = DataSource;

			// allow select only ordinary items, not the group items
			CanSelect = IsItem;
		}

		bool IsItem(int index)
		{
			return !DataSource[index].IsGroup;
		}
	}
}