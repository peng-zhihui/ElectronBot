namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;
	using UnityEngine.UI;

	/// <summary>
	/// ProgressbarIndetermitate test.
	/// </summary>
	[RequireComponent(typeof(Button))]
	public class SampleProgressBar1 : MonoBehaviour
	{
		/// <summary>
		/// Progressbar.
		/// </summary>
		[SerializeField]
		public ProgressbarIndeterminate Bar;

		Button button;

		/// <summary>
		/// Adds listener.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void Start()
		{
			button = GetComponent<Button>();
			if (button != null)
			{
				button.onClick.AddListener(Click);
			}
		}

		/// <summary>
		/// Toggle progressbar animation.
		/// </summary>
		protected virtual void Click()
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

		/// <summary>
		/// Remove listener.
		/// </summary>
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0603:Delegate allocation from a method group", Justification = "Required")]
		protected virtual void OnDestroy()
		{
			if (button != null)
			{
				button.onClick.RemoveListener(Click);
			}
		}
	}
}