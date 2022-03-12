namespace UIWidgets.Examples
{
	using UIWidgets;

	/// <summary>
	/// TreeViewSample component continent.
	/// </summary>
	public class TreeViewSampleComponentContinent : TreeViewSampleComponent
	{
		/// <inheritdoc/>
		protected override void UpdateView()
		{
			var item = Item as TreeViewSampleItemContinent;
			if (item == null)
			{
				Icon.sprite = null;
				TextAdapter.text = string.Empty;
			}
			else
			{
				TextAdapter.text = string.Format("{0} (Countries: {1})", item.Name, item.Countries.ToString());
			}
		}
	}
}