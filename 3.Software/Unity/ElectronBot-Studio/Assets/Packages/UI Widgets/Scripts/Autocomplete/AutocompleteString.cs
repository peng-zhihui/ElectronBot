namespace UIWidgets
{
	/// <summary>
	/// Autocomplete for ListViewString.
	/// </summary>
	public class AutocompleteString : AutocompleteCustom<string, ListViewStringItemComponent, ListViewString>
	{
		/// <summary>
		/// Determines whether the beginning of value matches the Input.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Input; otherwise, false.</returns>
		public override bool Startswith(string value)
		{
			return UtilitiesCompare.StartsWith(value, Query, CaseSensitive);
		}

		/// <summary>
		/// Returns a value indicating whether Input occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Input occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(string value)
		{
			return UtilitiesCompare.Contains(value, Query, CaseSensitive);
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(string value)
		{
			return value;
		}
	}
}