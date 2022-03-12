namespace UIWidgets.Examples
{
	using System.ComponentModel;
	using UIWidgets;

	/// <summary>
	/// TreeViewSampleItem interface.
	/// </summary>
	public interface ITreeViewSampleItem : IObservable, INotifyPropertyChanged
	{
		/// <summary>
		/// Display item data using specified component.
		/// </summary>
		/// <param name="component">Component.</param>
		void Display(TreeViewSampleComponent component);
	}
}