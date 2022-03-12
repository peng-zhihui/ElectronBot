namespace UIWidgets.Examples
{
	using UIWidgets;
	using UnityEngine;

	/// <summary>
	/// Progressbar sample.
	/// </summary>
	[RequireComponent(typeof(ProgressbarDeterminateBase))]
	public class SampleProgressbar : MonoBehaviour
	{
		/// <summary>
		/// Set custom test progress display.
		/// </summary>
		protected virtual void Start()
		{
			var bar = GetComponent<ProgressbarDeterminateBase>();
			bar.TextFunc = x => string.Format("Exp to next level: {0} / {1}", x.Value.ToString(), x.Max.ToString());
		}
	}
}