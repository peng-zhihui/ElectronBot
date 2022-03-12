namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// GroupedPhotos example.
	/// Items grouped by keys.
	/// </summary>
	public class GroupedPhotos : GroupedList<Photo>
	{
		/// <summary>
		/// Get group for specified item.
		/// </summary>
		/// <param name="item">Item.</param>
		/// <returns>Group for specified item.</returns>
		protected override Photo GetGroup(Photo item)
		{
			var date = item.Created.Date;

			foreach (var key in GroupsWithItems.Keys)
			{
				if (key.Created == date)
				{
					return key;
				}
			}

			return new Photo() { Created = item.Created.Date, IsGroup = true };
		}
	}
}