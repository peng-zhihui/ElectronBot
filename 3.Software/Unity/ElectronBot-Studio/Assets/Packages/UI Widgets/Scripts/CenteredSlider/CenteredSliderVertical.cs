namespace UIWidgets
{
	/// <summary>
	/// Centered vertical slider (zero at center, positive and negative parts have different scales).
	/// </summary>
	public class CenteredSliderVertical : CenteredSlider
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