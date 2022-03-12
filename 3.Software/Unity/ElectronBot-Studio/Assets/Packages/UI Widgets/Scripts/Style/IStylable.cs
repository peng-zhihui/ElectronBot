namespace UIWidgets.Styles
{
	/// <summary>
	/// IStylable interface.
	/// </summary>
	public interface IStylable
	{
		/// <summary>
		/// Set widget properties from specified style.
		/// </summary>
		/// <param name="style">Style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		bool SetStyle(Style style);

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="style">Style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		bool GetStyle(Style style);
	}

	/// <summary>
	/// IStylable interface.
	/// </summary>
	/// <typeparam name="T">Type of component to apply style.</typeparam>
	public interface IStylable<T>
	{
		/// <summary>
		/// Set widget properties from specified style.
		/// </summary>
		/// <param name="styleTyped">Style for specified type.</param>
		/// <param name="style">Full style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		bool SetStyle(T styleTyped, Style style);

		/// <summary>
		/// Set style options from widget properties.
		/// </summary>
		/// <param name="styleTyped">Style for specified type.</param>
		/// <param name="style">Full style data.</param>
		/// <returns><c>true</c>, if children gameobjects was processed, <c>false</c> otherwise.</returns>
		bool GetStyle(T styleTyped, Style style);
	}
}