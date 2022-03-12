namespace UIWidgets.Examples
{
	using System.Collections.Generic;
	using UIWidgets;

	/// <summary>
	/// KeyValueAutocomplete.
	/// </summary>
	public class KeyValueAutocomplete : AutocompleteCustom<KeyValuePair<string, string>, KeyValueListViewItem, KeyValueListView>
	{
		/// <summary>
		/// Determines whether the beginning of value matches the Query.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if beginning of value matches the Query; otherwise, false.</returns>
		public override bool Startswith(KeyValuePair<string, string> value)
		{
			var str = GetStringValue(value);
			return UtilitiesCompare.StartsWith(str, Query, CaseSensitive);
		}

		/// <summary>
		/// Returns a value indicating whether Query occurs within specified value.
		/// </summary>
		/// <param name="value">Value.</param>
		/// <returns>true if the Query occurs within value parameter; otherwise, false.</returns>
		public override bool Contains(KeyValuePair<string, string> value)
		{
			var str = GetStringValue(value);
			return UtilitiesCompare.Contains(str, Query, CaseSensitive);
		}

		/// <summary>
		/// Convert value to string.
		/// </summary>
		/// <returns>The string value.</returns>
		/// <param name="value">Value.</param>
		protected override string GetStringValue(KeyValuePair<string, string> value)
		{
			return string.Format("0;{1}", value.Key, value.Value);
		}
	}
}