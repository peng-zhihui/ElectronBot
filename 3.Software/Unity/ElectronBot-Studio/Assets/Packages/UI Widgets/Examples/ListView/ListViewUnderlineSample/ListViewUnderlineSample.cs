namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// ListViewUnderline sample.
	/// </summary>
	public class ListViewUnderlineSample : ListViewCustom<ListViewUnderlineSampleComponent, ListViewUnderlineSampleItemDescription>
	{
		readonly Comparison<ListViewUnderlineSampleItemDescription> itemsComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

		/// <summary>
		/// Set items comparison.
		/// </summary>
		public override void Init()
		{
			base.Init();
			DataSource.Comparison = itemsComparison;
		}
	}
}