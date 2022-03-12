#if UIWIDGETS_TMPRO_SUPPORT && (UNITY_5_2 || UNITY_5_3 || UNITY_5_3_OR_NEWER)
namespace UIWidgets.TMProSupport
{
	/// <summary>
	/// Autocomplete for ListViewIcons.
	/// </summary>
	public class AutocompleteIconsTMPro : AutocompleteCustomTMPro<ListViewIconsItemDescription, ListViewIconsItemComponent, ListViewIcons>
	{
		/// <summary>
		/// Determines whether the beginning of value matches the Query.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Query; otherwise, false.</returns>
		public override bool Startswith(ListViewIconsItemDescription value)
		{
			return UtilitiesCompare.StartsWith(value.Name, Query, CaseSensitive)
				|| (value.LocalizedName != null && UtilitiesCompare.StartsWith(value.LocalizedName, Query, CaseSensitive));
		}

		/// <summary>
		/// Returns a value indicating whether Query occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Query occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(ListViewIconsItemDescription value)
		{
			return UtilitiesCompare.Contains(value.Name, Query, CaseSensitive)
				|| (value.LocalizedName != null && UtilitiesCompare.Contains(value.LocalizedName, Query, CaseSensitive));
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(ListViewIconsItemDescription value)
		{
			return value.LocalizedName ?? value.Name;
		}
	}
}
#endif