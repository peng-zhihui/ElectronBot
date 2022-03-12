namespace UIWidgets
{
	using UIWidgets.l10n;

	/// <summary>
	/// Autocomplete for ListViewIcons.
	/// </summary>
	public class AutocompleteIcons : AutocompleteCustom<ListViewIconsItemDescription, ListViewIconsItemComponent, ListViewIcons>
	{
		/// <summary>
		/// Determines whether the beginning of value matches the Query.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Query; otherwise, false.</returns>
		public override bool Startswith(ListViewIconsItemDescription value)
		{
			var name = Localization.GetTranslation(value.Name);
			return UtilitiesCompare.StartsWith(name, Query, CaseSensitive)
				|| (value.LocalizedName != null && UtilitiesCompare.StartsWith(value.LocalizedName, Query, CaseSensitive));
		}

		/// <summary>
		/// Returns a value indicating whether Query occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Query occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(ListViewIconsItemDescription value)
		{
			var name = Localization.GetTranslation(value.Name);
			return UtilitiesCompare.Contains(name, Query, CaseSensitive)
				|| (value.LocalizedName != null && UtilitiesCompare.Contains(value.LocalizedName, Query, CaseSensitive));
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(ListViewIconsItemDescription value)
		{
			return value.LocalizedName ?? Localization.GetTranslation(value.Name);
		}
	}
}