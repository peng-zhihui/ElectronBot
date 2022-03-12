namespace UIWidgets
{
	/// <summary>
	/// Paginator page positions.
	/// </summary>
	public enum PaginatorPagePosition
	{
		/// <summary>
		/// Do not auto scroll.
		/// </summary>
		None = 0,

		/// <summary>
		/// Scroll to start of the page after user scroll or drag.
		/// </summary>
		OnStart = 1,

		/// <summary>
		/// Scroll to center of the page after user scroll or drag.
		/// </summary>
		OnCenter = 2,

		/// <summary>
		/// Scroll to end of the page after user scroll or drag.
		/// </summary>
		OnEnd = 3,
	}
}