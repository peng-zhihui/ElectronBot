namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// GroupedListViewComponent.
	/// </summary>
	public class GroupedListViewComponent : ListViewItem, IViewData<IGroupedListItem>
	{
		/// <summary>
		/// Item.
		/// </summary>
		protected IGroupedListItem Item;

		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		public void SetData(IGroupedListItem item)
		{
			Item = item;

			UpdateView();
		}

		/// <summary>
		/// Update view.
		/// </summary>
		public virtual void UpdateView()
		{
		}
	}
}