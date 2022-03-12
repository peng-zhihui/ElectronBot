namespace UIWidgets
{
	using System;
	using System.Collections.Generic;

	/// <summary>
	/// Layout with items sticked to the top line.
	/// </summary>
	/// <typeparam name="TData">Type of the data.</typeparam>
	/// <typeparam name="TPoint">Type of the points.</typeparam>
	public class TrackLayoutTopLine<TData, TPoint> : TrackLayout<TData, TPoint>
		where TData : class, ITrackData<TPoint>
		where TPoint : IComparable<TPoint>
	{
		/// <summary>
		/// Set order for the specified items.
		/// </summary>
		/// <param name="items">Items.</param>
		/// <param name="temp">Temp list.</param>
		/// <param name="used">Temp list for the used items.</param>
		protected override void Layout(List<TData> items, List<TData> temp, List<TData> used)
		{
			for (int i = 0; i < items.Count; i++)
			{
				items[i].Order = i;
			}
		}
	}
}