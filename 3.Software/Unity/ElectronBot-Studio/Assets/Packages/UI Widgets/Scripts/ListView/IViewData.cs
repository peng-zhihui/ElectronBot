namespace UIWidgets
{
	/// <summary>
	/// interface for displaying item data.
	/// </summary>
	/// <typeparam name="T">Item type.</typeparam>
	public interface IViewData<T>
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="item">Item.</param>
		void SetData(T item);
	}
}