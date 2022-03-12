namespace UIWidgets
{
	using System.Collections.Generic;
	using UnityEngine;

	/// <summary>
	/// Snap grid support.
	/// </summary>
	public interface ISnapGridSupport
	{
		/// <summary>
		/// Snap grids.
		/// </summary>
		List<SnapGridBase> SnapGrids
		{
			get;
			set;
		}

		/// <summary>
		/// Snap distance.
		/// </summary>
		Vector2 SnapDistance
		{
			get;
			set;
		}
	}
}