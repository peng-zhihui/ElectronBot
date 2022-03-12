#if UIWIDGETS_TMPRO_SUPPORT
namespace UIWidgets.TMProSupport
{
	using UIWidgets;

	/// <summary>
	/// ComboboxIcons item component.
	/// Demonstrate how to remove selected item - add Remove() call on Button.OnClick().
	/// </summary>
	public class ComboboxIconsItemComponentTMPro : ListViewIconsItemComponentTMPro
	{
		/// <summary>
		/// ComboboxIcons.
		/// </summary>
		public ComboboxIcons ComboboxIcons;

		/// <summary>
		/// Remove this instance.
		/// </summary>
		public void Remove()
		{
			ComboboxIcons.ListView.Deselect(Index);
		}
	}
}
#endif