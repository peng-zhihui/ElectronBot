namespace UIWidgets
{
	/// <summary>
	/// Enumerator mode.
	/// </summary>
	public enum PoolEnumeratorMode
	{
		/// <summary>
		/// Active instances only.
		/// </summary>
		Active = 0,

		/// <summary>
		/// Template and cached instances only.
		/// </summary>
		Cache = 1,

		/// <summary>
		/// Template and all instances.
		/// </summary>
		All = 2,
	}
}