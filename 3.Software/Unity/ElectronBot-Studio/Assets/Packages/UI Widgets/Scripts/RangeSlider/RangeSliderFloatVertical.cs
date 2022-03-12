namespace UIWidgets
{
	using UnityEngine;

	/// <summary>
	/// Vertical range slider.
	/// </summary>
	public class RangeSliderFloatVertical : RangeSliderFloat
	{
		/// <summary>
		/// Determines whether this instance is horizontal.
		/// </summary>
		/// <returns><c>true</c> if this instance is horizontal; otherwise, <c>false</c>.</returns>
		public override bool IsHorizontal()
		{
			return false;
		}
	}
}