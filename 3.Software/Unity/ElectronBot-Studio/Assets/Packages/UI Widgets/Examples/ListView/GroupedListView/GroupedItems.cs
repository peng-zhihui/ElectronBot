namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// GroupedItems sample.
	/// Items grouped by keys.
	/// </summary>
	public class GroupedItems : GroupedList<IGroupedListItem>
	{
		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected override IGroupedListItem GetGroup(IGroupedListItem item)
		{
			var name = item.Name.Length > 0 ? item.Name[0].ToString() : string.Empty;

			foreach (var key in GroupsWithItems.Keys)
			{
				if (key.Name == name)
				{
					return key;
				}
			}

			return new GroupedListGroup() { Name = name };
		}
	}
}