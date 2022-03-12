namespace UIWidgets
{
	/// <summary>
	/// ComboboxIcons item component.
	/// </summary>
	public class ComboboxIconsItemComponent : ListViewIconsItemComponent
	{
		/// <summary>
		/// ComboboxIcons.
		/// </summary>
		public ComboboxIcons ComboboxIcons;

		/// <summary>
		/// Deselect this instance.
		/// </summary>
		[System.Obsolete("Renamed to DeselectItem()")]
		public void Remove()
		{
			DeselectItem();
		}
	}
}