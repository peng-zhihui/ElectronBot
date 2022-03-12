namespace UIWidgets
{
	using UIWidgets.Styles;
	using UnityEngine;

	/// <summary>
	/// Picker for the ListViewCustom.
	/// </summary>
	/// <typeparam name="TListView">Type of the ListView.</typeparam>
	/// <typeparam name="TListViewComponent">Type of the ListView component.</typeparam>
	/// <typeparam name="TValue">Type of the value.</typeparam>
	/// <typeparam name="TPicker">Type of the this picker.</typeparam>
	public class PickerListViewCustom<TListView, TListViewComponent, TValue, TPicker> : Picker<TValue, TPicker>
		where TListView : ListViewCustom<TListViewComponent, TValue>
		where TListViewComponent : ListViewItem
		where TPicker : Picker<TValue, TPicker>
	{
		/// <summary>
		/// ListView.
		/// </summary>
		[SerializeField]
		public TListView ListView;

		/// <summary>
		/// Prepare picker to open.
		/// </summary>
		/// <param name="defaultValue">Default value.</param>
		public override void BeforeOpen(TValue defaultValue)
		{
			ListView.SelectedIndex = ListView.DataSource.IndexOf(defaultValue);

			ListView.OnSelectObject.AddListener(ListViewCallback);
		}

		void ListViewCallback(int index)
		{
			Selected(ListView.DataSource[index]);
		}

		/// <summary>
		/// Prepare picker to close.
		/// </summary>
		public override void BeforeClose()
		{
			ListView.OnSelectObject.RemoveListener(ListViewCallback);
		}

		#region IStylable implementation

		/// <inheritdoc/>
		public override bool SetStyle(Style style)
		{
			base.SetStyle(style);

			ListView.SetStyle(style);

			style.Dialog.Button.ApplyTo(transform.Find("Buttons/Cancel"));

			return true;
		}

		/// <inheritdoc/>
		public override bool GetStyle(Style style)
		{
			base.GetStyle(style);

			ListView.GetStyle(style);

			style.Dialog.Button.GetFrom(transform.Find("Buttons/Cancel"));

			return true;
		}
		#endregion
	}
}