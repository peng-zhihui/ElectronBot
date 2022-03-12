namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// ProgressbarIndetermitate test.
	/// </summary>
	public class TestProgressbarIndeterminate : MonoBehaviour
	{
		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		public ProgressbarIndeterminate Bar;

		/// <summary>
		/// Toggle progressbar.
		/// </summary>
		public void Toggle()
		{
			if (Bar.IsAnimationRunning)
			{
				Bar.Stop();
			}
			else
			{
				Bar.Animate();
			}
		}
	}
}