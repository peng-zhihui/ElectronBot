namespace UIWidgets
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Comparer using specified comparison for comparisons between items.
	/// </summary>
	/// <typeparam name="T">The type of the objects to compare.</typeparam>
	public class ComparisonComparer<T> : IComparer<T>
	{
		readonly Comparison<T> comparison;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComparisonComparer{T}"/> class.
		/// </summary>
		/// <param name="newComparison">The comparison.</param>
		public ComparisonComparer(Comparison<T> newComparison)
		{
			if (newComparison == null)
			{
				throw new ArgumentNullException("newComparison");
			}

			comparison = newComparison;
		}

		/// <summary>
		/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
		/// </summary>
		/// <param name="x">The first object to compare.</param>
		/// <param name="y">The second object to compare.</param>
		/// <returns>A signed integer that indicates the relative values of x and y.</returns>
		public int Compare(T x, T y)
		{
			return comparison(x, y);
		}
	}
}