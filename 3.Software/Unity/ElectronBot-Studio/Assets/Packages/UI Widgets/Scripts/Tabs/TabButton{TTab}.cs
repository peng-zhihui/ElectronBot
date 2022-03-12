namespace UIWidgets
{
	/// <summary>
	/// Tab button.
	/// </summary>
	/// <typeparam name="TTab">Tab type.</typeparam>
	public abstract class TabButton<TTab> : TabButtonBase
		where TTab : Tab
	{
		/// <summary>
		/// Set data.
		/// </summary>
		/// <param name="tab">Tab.</param>
		public abstract void SetData(TTab tab);
	}
}