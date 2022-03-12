namespace UIWidgets.Examples
{
	using System;
	using UIWidgets;

	/// <summary>
	/// ListViewCustom sample.
	/// </summary>
	public class ListViewCustomSample : ListViewCustom<ListViewCustomSampleComponent, ListViewCustomSampleItemDescription>
	{
		readonly Comparison<ListViewCustomSampleItemDescription> itemsComparison = (x, y) => UtilitiesCompare.Compare(x.Name, y.Name);

		bool isListViewCustomSampleInited;

		/// <summary>
		/// Set items comparison.
		/// </summary>
		public override void Init()
		{
			if (isListViewCustomSampleInited)
			{
				return;
			}

			isListViewCustomSampleInited = true;

			base.Init();
			DataSource.Comparison = itemsComparison;
		}
	}
}