namespace UIWidgets
{
	/// <summary>
	/// OnChange.
	/// </summary>
	public delegate void OnChange();

	/// <summary>
	/// IObservable.
	/// </summary>
	public interface IObservable
	{
		/// <summary>
		/// Occurs when data changed.
		/// </summary>
		event OnChange OnChange;
	}
}