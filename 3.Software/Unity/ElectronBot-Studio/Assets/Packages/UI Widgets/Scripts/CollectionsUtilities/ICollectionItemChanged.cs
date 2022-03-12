namespace UIWidgets
{
	/// <summary>
	/// IObservable.
	/// </summary>
	public interface ICollectionItemChanged
	{
		/// <summary>
		/// Occurs when changed data of item in collection.
		/// </summary>
		event OnChange OnCollectionItemChange;
	}
}