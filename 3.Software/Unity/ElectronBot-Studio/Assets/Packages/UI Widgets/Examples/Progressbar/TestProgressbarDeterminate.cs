namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// ProgressbarDeterminate test.
	/// </summary>
	public class TestProgressbarDeterminate : MonoBehaviour
	{
		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		protected ProgressbarDeterminateBase Progressbar;

		/// <summary>
		/// Spinner.
		/// </summary>
		[SerializeField]
		protected Spinner Spinner;

		/// <summary>
		/// Toggle progressbar.
		/// </summary>
		public void Toggle()
		{
			if (Progressbar.IsAnimationRunning)
			{
				Progressbar.Stop();
			}
			else
			{
				if (Progressbar.Value == 0)
				{
					Progressbar.Animate(Progressbar.Max);
				}
				else
				{
					Progressbar.Animate(0);
				}
			}
		}

		/// <summary>
		/// Set progressbar value.
		/// </summary>
		/// <param name="value">Value.</param>
		public void SetValue(int value)
		{
			Progressbar.Animate(value);
		}

		/// <summary>
		/// Set value from spinner.
		/// </summary>
		public void SetFromSpinner()
		{
			SetValue(Spinner.Value);
		}
	}
}