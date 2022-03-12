namespace UIWidgets
{
	using System.Collections.Generic;
	using System.ComponentModel;

	/// <summary>
	/// IObservableList.
	/// </summary>
	/// <typeparam name="T">Type of item.</typeparam>
	public interface IObservableList<T> : IList<T>, INotifyPropertyChanged, ICollectionChanged, ICollectionItemChanged
	{
		/// <summary>
		/// Maintains performance while items are added/removed/changed by preventing the widgets from drawing until the EndUpdate method is called.
		/// </summary>
		void BeginUpdate();

		/// <summary>
		/// Ends the update and raise OnChange if something was changed.
		/// </summary>
		void EndUpdate();
	}
}