namespace UIWidgets
{
	/// <summary>
	/// AutoComboboxIcons.
	/// </summary>
	public partial class AutoComboboxString : AutoCombobox<string, ListViewString, ListViewStringItemComponent, AutocompleteString, ComboboxString>
	{
		/// <inheritdoc/>
		protected override string GetStringValue(string item)
		{
			return item;
		}

		/// <inheritdoc/>
		protected override string Input2Item(string input)
		{
			return input;
		}
	}
}